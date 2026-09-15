using KrisanthiumCurrency.Dtos;

namespace KrisanthiumCurrency.Services {
    public interface IMockErpClient {
        Task<MockErpResponse> SyncAsync(string currency, decimal rate, DateOnly date, CancellationToken ct = default);
    }

    /// <summary>
    /// Client yang mengirim data kurs ke Mock ERP API (POST /api/mock-erp/exchange-rate)
    /// sesuai flow di soal: [Sync to ERP] -> Mock ERP API.
    /// </summary>
    public class MockErpClient : IMockErpClient {
        private readonly HttpClient _httpClient;

        public MockErpClient(HttpClient httpClient) {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<MockErpResponse> SyncAsync(string currency, decimal rate, DateOnly date, CancellationToken ct = default) {
            var payload = new MockErpRequest {
                Currency = currency,
                Rate = rate,
                Date = date.ToString("yyyy-MM-dd")
            };

            try {
                var response = await _httpClient.PostAsJsonAsync("/api/mock-erp/exchange-rate", payload, ct);
                var result = await response.Content.ReadFromJsonAsync<MockErpResponse>(cancellationToken: ct);
                return result ?? new MockErpResponse { Success = false, Message = "ERP connection failed" };
            }
            catch {
                return new MockErpResponse { Success = false, Message = "ERP connection failed" };
            }
        }
    }
}
