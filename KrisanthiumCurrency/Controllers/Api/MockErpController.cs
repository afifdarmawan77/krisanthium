using KrisanthiumCurrency.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace KrisanthiumCurrency.Controllers.Api {
    /// <summary>
    /// Simulasi ERP API perusahaan, karena kandidat tidak punya akses ke ERP asli (sesuai soal poin 8).
    /// </summary>
    [ApiController]
    [Route("api/mock-erp")]
    public class MockErpController : ControllerBase {
        private static readonly Random _random = new();

        /// <summary>POST /api/mock-erp/exchange-rate</summary>
        [HttpPost("exchange-rate")]
        public IActionResult SyncExchangeRate([FromBody] MockErpRequest request) {
            // Simulasi: ~80% berhasil, ~20% gagal (agar alur FAILED & retry bisa diuji)
            var isSuccess = _random.Next(1, 101) <= 80;

            if (isSuccess) {
                return Ok(new MockErpResponse {
                    Success = true,
                    Message = "Rate synchronized successfully"
                });
            }

            return Ok(new MockErpResponse {
                Success = false,
                Message = "ERP connection failed"
            });
        }
    }

}
