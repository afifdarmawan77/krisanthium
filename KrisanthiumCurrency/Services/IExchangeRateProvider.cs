namespace KrisanthiumCurrency.Services {
    public interface IExchangeRateProvider {
        /// <summary>
        /// Mengambil kurs terbaru untuk setiap currency terhadap baseCurrency.
        /// Return: dictionary currency -> rate (berapa baseCurrency untuk 1 unit currency).
        /// </summary>
        Task<Dictionary<string, decimal>> GetLatestRatesAsync(
            IEnumerable<string> currencies,
            string baseCurrency,
            CancellationToken ct = default);

        /// <summary>Nama sumber API, untuk disimpan di kolom "source".</summary>
        string SourceName { get; }
    }
}
