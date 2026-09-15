using System.Text.Json.Serialization;

namespace KrisanthiumCurrency.Services {
    public class ExchangeRateApiProvider : IExchangeRateProvider {
        private readonly HttpClient _httpClient;

        public string SourceName => "open.er-api.com";

        public ExchangeRateApiProvider(HttpClient httpClient) {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        public async Task<Dictionary<string, decimal>> GetLatestRatesAsync(
            IEnumerable<string> currencies,
            string baseCurrency,
            CancellationToken ct = default) {
            var result = new Dictionary<string, decimal>();

            foreach (var currency in currencies) {
                try {
                    var url = $"https://open.er-api.com/v6/latest/{currency}";
                    using var response = await _httpClient.GetAsync(url, ct);

                    if (!response.IsSuccessStatusCode)
                        throw new ExternalApiException("Failed to retrieve exchange rate. Please try again.");

                    var payload = await response.Content.ReadFromJsonAsync<ErApiResponse>(cancellationToken: ct);

                    if (payload is null
                        || !string.Equals(payload.Result, "success", StringComparison.OrdinalIgnoreCase)
                        || payload.Rates is null
                        || !payload.Rates.TryGetValue(baseCurrency, out var rate)) {
                        throw new ExternalApiException("Failed to retrieve exchange rate. Please try again.");
                    }

                    result[currency] = rate;
                }
                catch (Exception ex) when (ex is not ExternalApiException) {
                    throw new ExternalApiException("Failed to retrieve exchange rate. Please try again.");
                }
            }

            return result;
        }

        private class ErApiResponse {
            [JsonPropertyName("result")]
            public string? Result { get; set; }

            [JsonPropertyName("base_code")]
            public string? BaseCode { get; set; }

            [JsonPropertyName("rates")]
            public Dictionary<string, decimal>? Rates { get; set; }
        }
    }
}
