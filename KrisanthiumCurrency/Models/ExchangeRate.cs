namespace KrisanthiumCurrency.Models {
    public class ExchangeRate {
        public int Id { get; set; }

        /// <summary>Kode mata uang, mis. USD / EUR / SGD</summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>Nilai kurs dalam IDR untuk 1 unit mata uang</summary>
        public decimal Rate { get; set; }

        /// <summary>Tanggal kurs</summary>
        public DateOnly RateDate { get; set; }

        /// <summary>Sumber API pengambilan kurs</summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>NOT_SYNCED / SYNCED / FAILED</summary>
        public string ErpStatus { get; set; } = ErpStatuses.NotSynced;

        public DateTime? SyncedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public static class ErpStatuses {
        public const string NotSynced = "NOT_SYNCED";
        public const string Synced = "SYNCED";
        public const string Failed = "FAILED";
    }
}
