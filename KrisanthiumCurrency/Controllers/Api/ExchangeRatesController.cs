using KrisanthiumCurrency.Dtos;
using KrisanthiumCurrency.Models;
using KrisanthiumCurrency.Services;
using Microsoft.AspNetCore.Mvc;

namespace KrisanthiumCurrency.Controllers.Api {
    [ApiController]
    [Route("api/exchange-rates")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] Authorize dihilangkan karena tidak ada ketentuan di Soal
    public class ExchangeRatesController : ControllerBase {
        private readonly ExchangeRateAppService _service;

        public ExchangeRatesController(ExchangeRateAppService service) {
            _service = service;
        }

        /// <summary>GET /api/exchange-rates -> kurs terbaru per currency</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExchangeRateDto>>> GetLatest(CancellationToken ct) {
            var data = await _service.GetLatestAsync(ct);
            return Ok(data.Select(ToDto));
        }

        /// <summary>GET /api/exchange-rates/history?currency=USD&amp;date=2026-08-14</summary>
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<ExchangeRateDto>>> GetHistory(
            [FromQuery] string? currency,
            [FromQuery] DateOnly? date,
            CancellationToken ct) {
            var data = await _service.GetHistoryAsync(currency, date, ct);
            return Ok(data.Select(ToDto));
        }

        /// <summary>POST /api/exchange-rates/fetch -> ambil kurs terbaru dari API eksternal &amp; simpan</summary>
        [HttpPost("fetch")]
        public async Task<IActionResult> Fetch(CancellationToken ct) {
            try {
                var data = await _service.FetchAndSaveLatestAsync(ct);
                return Ok(data.Select(ToDto));
            }
            catch (ExternalApiException ex) {
                return StatusCode(502, new { message = ex.Message });
            }
        }

        /// <summary>POST /api/exchange-rates/{id}/sync -> sinkronisasi satu baris kurs ke Mock ERP</summary>
        [HttpPost("{id:int}/sync")]
        public async Task<IActionResult> Sync(int id, CancellationToken ct) {
            try {
                var data = await _service.SyncToErpAsync(id, ct);
                return Ok(ToDto(data));
            }
            catch (KeyNotFoundException) {
                return NotFound(new { message = "Exchange rate not found." });
            }
        }

        private static ExchangeRateDto ToDto(ExchangeRate e) => new() {
            Id = e.Id,
            Currency = e.Currency,
            Rate = e.Rate,
            RateDate = e.RateDate.ToString("yyyy-MM-dd"),
            Source = e.Source,
            ErpStatus = e.ErpStatus,
            SyncedAt = e.SyncedAt
        };
    }
}
