using KrisanthiumCurrency.Data;
using KrisanthiumCurrency.Models;
using Microsoft.EntityFrameworkCore;

namespace KrisanthiumCurrency.Services {
    public class ExchangeRateAppService {
        private static readonly string[] SupportedCurrencies = { "USD", "EUR", "SGD" };
        private const string BaseCurrency = "IDR";

        private readonly AppDbContext _db;
        private readonly IExchangeRateProvider _provider;
        private readonly IMockErpClient _erpClient;

        public ExchangeRateAppService(AppDbContext db, IExchangeRateProvider provider, IMockErpClient erpClient) {
            _db = db;
            _provider = provider;
            _erpClient = erpClient;
        }

        /// <summary>1. Ambil kurs dari API eksternal, 2. Simpan/update ke database (upsert per hari).</summary>
        public async Task<List<ExchangeRate>> FetchAndSaveLatestAsync(CancellationToken ct = default) {
            var rates = await _provider.GetLatestRatesAsync(SupportedCurrencies, BaseCurrency, ct);
            var today = DateOnly.FromDateTime(DateTime.Now);
            var saved = new List<ExchangeRate>();

            foreach (var (currency, rate) in rates) {
                var existing = await _db.ExchangeRates
                    .FirstOrDefaultAsync(e => e.Currency == currency && e.RateDate == today, ct);

                if (existing is not null) {
                    // Requirement 2: 1 currency hanya 1 baris per tanggal -> update, bukan insert baru
                    existing.Rate = rate;
                    existing.Source = _provider.SourceName;
                    existing.UpdatedAt = DateTime.UtcNow;
                    saved.Add(existing);
                }
                else {
                    var entity = new ExchangeRate {
                        Currency = currency,
                        Rate = rate,
                        RateDate = today,
                        Source = _provider.SourceName,
                        ErpStatus = ErpStatuses.NotSynced,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _db.ExchangeRates.Add(entity);
                    saved.Add(entity);
                }
            }

            await _db.SaveChangesAsync(ct);
            return saved.OrderBy(e => e.Currency).ToList();
        }

        /// <summary>3. Menampilkan kurs terbaru (baris dengan rate_date terbesar per currency).</summary>
        public async Task<List<ExchangeRate>> GetLatestAsync(CancellationToken ct = default) {
            var latestDatesPerCurrency = await _db.ExchangeRates
                .GroupBy(e => e.Currency)
                .Select(g => new { Currency = g.Key, MaxDate = g.Max(x => x.RateDate) })
                .ToListAsync(ct);

            var result = new List<ExchangeRate>();
            foreach (var item in latestDatesPerCurrency) {
                var rate = await _db.ExchangeRates
                    .FirstAsync(e => e.Currency == item.Currency && e.RateDate == item.MaxDate, ct);
                result.Add(rate);
            }

            return result.OrderBy(r => r.Currency).ToList();
        }

        /// <summary>4. Menampilkan history kurs, dengan filter opsional currency & tanggal.</summary>
        public async Task<List<ExchangeRate>> GetHistoryAsync(string? currency, DateOnly? date, CancellationToken ct = default) {
            var query = _db.ExchangeRates.AsQueryable();

            if (!string.IsNullOrWhiteSpace(currency))
                query = query.Where(e => e.Currency == currency.ToUpperInvariant());

            if (date.HasValue)
                query = query.Where(e => e.RateDate == date.Value);

            return await query
                .OrderByDescending(e => e.RateDate)
                .ThenBy(e => e.Currency)
                .ToListAsync(ct);
        }

        /// <summary>5. & 7. Sinkronisasi satu baris kurs ke Mock ERP API dan update status.</summary>
        public async Task<ExchangeRate> SyncToErpAsync(int id, CancellationToken ct = default) {
            var entity = await _db.ExchangeRates.FindAsync(new object[] { id }, ct)
                ?? throw new KeyNotFoundException("Exchange rate not found.");

            var response = await _erpClient.SyncAsync(entity.Currency, entity.Rate, entity.RateDate, ct);

            // Requirement 3 & 4: update status sesuai hasil sync
            entity.ErpStatus = response.Success ? ErpStatuses.Synced : ErpStatuses.Failed;
            entity.SyncedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            return entity;
        }
    }
}
