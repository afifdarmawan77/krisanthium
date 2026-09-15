## KrisanthiumCurrency

Sederhana — panduan singkat untuk instalasi, menjalankan, migrasi database, konfigurasi environment, API yang dipakai, dan testing.

### 1. Install (prerequisites)
- Direkomendasikan memakai Visual Studio
- .NET 8 SDK: https://dotnet.microsoft.com
- MySQL (port default 3306)

### 2. Cara menjalankan aplikasi
Dari direktori project (mis. `D:\KrisanthiumCurrency\`):

Buka terminal jalankan perintah berikut:
dotnet restore
dotnet build

## 3. Database migration
Project menggunakan Entity Framework Core dengan provider MySQL (Pomelo).

- Pastikan MySQL berjalan dan buat database (contoh menggunakan MySQL CLI):
mysql -u root -p -e "CREATE DATABASE IF NOT EXISTS exchange_rate CHARACTER SET utf8mb4;"

- Install/aktifkan `dotnet-ef` jika belum:
dotnet tool install --global dotnet-ef

- Terapkan migration yang sudah ada:
dotnet ef database update --project KrisanthiumCurrency --startup-project KrisanthiumCurrency

- Melihat daftar migration:
dotnet ef migrations list --project KrisanthiumCurrency


Catatan: `KrisanthiumCurrency` sudah menyertakan paket `Microsoft.EntityFrameworkCore.Design` dan `Microsoft.EntityFrameworkCore.Tools`.

## 4. Environment configuration
Konfigurasi utama ada di `appsettings.json` (contoh kunci penting):
- Connection string MySQL: `ConnectionStrings:DefaultConnection`
- Base URL untuk client internal: `AppBaseUrl`

Contoh `appsettings.json` (project):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=exchange_rate;uid=root;pwd=;CharSet=utf8mb4;"
  },
  "AppBaseUrl": "http://localhost:61095"
}
```

## 5. API yang digunakan
- External exchange-rate API: `https://open.er-api.com/v6/latest/{currency}`  
  Implementasi: `ExchangeRateApiProvider` (Service) — sumber bernama `open.er-api.com`.
- Mock ERP internal endpoint (disediakan oleh project):
  - `POST /api/mock-erp/exchange-rate` — menerima payload sinkronisasi kurs.
  - Client internal: `MockErpClient` mengirim POST ke `AppBaseUrl` + `/api/mock-erp/exchange-rate`.


Gunakan Swagger di `/swagger` untuk melihat API yang tersedia dan contoh request/response.

## 6. Cara menjalankan
Pada folder project, jalankan:
dotnet run --project KrisanthiumCurrency

atau jalankan lewat tombol Play/F5 di IDE (mis. Visual Studio, Rider, VS Code).

Setelah berjalan, akses antarmuka Razor Pages atau Swagger:
- Web UI: `http://localhost:61095`
- Swagger/OpenAPI: `http://localhost:61095/swagger`

Penting : Pastikan menggunakan port 61095, karena project sudah di Hard-Code menggunakan port tersebut.
Jika port salah atau sudah digunakan, API tidak akan bisa diakses.
Gunakan perintah `dotnet run --urls http://localhost:61095` untuk memastikan port yang benar.
