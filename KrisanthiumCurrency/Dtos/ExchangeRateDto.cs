using System.Text.Json.Serialization;

namespace KrisanthiumCurrency.Dtos {
    public class ExchangeRateDto {
        public int Id { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public string RateDate { get; set; } = string.Empty; // yyyy-MM-dd
        public string Source { get; set; } = string.Empty;
        public string ErpStatus { get; set; } = string.Empty;
        public DateTime? SyncedAt { get; set; }
    }

    /// <summary>Payload yang dikirim aplikasi ini ke Mock ERP API</summary>
    public class MockErpRequest {
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty; // yyyy-MM-dd
    }

    /// <summary>Response dari Mock ERP API</summary>
    public class MockErpResponse {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}
