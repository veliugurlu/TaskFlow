# TaskFlow API

Takım görev yönetimi için geliştirdiğim bir ASP.NET Core Web API projesi.
Kullanıcılar proje oluşturur, projelere görev ekler, görevleri durum ve önceliğe göre yönetir.

## Özellikler

- JWT ile kayıt ve giriş, BCrypt ile şifre hash'leme
- Rol tabanlı (Admin / Member) ve sahiplik tabanlı yetkilendirme
- Projeler ve görevler için CRUD
- Sayfalama, filtreleme (durum, öncelik, arama) ve sıralama
- FluentValidation ile istek doğrulama
- Global exception middleware ve Serilog ile dosyaya loglama
- xUnit ve Moq ile unit testler

## Teknolojiler

C#, .NET 10, ASP.NET Core Web API, Entity Framework Core, SQL Server,
FluentValidation, Serilog, JWT Bearer, BCrypt.Net, xUnit, Moq, Scalar

## Mimari

Clean Architecture'a uygun 4 katman. Bağımlılık içe doğru akar.

| Katman | Sorumluluk |
|---|---|
| Domain | Entity'ler ve enum'lar (hiçbir şeye bağlı değil) |
| Application | DTO'lar, validator'lar, interface'ler, servisler |
| Infrastructure | EF Core, repository'ler, JWT ve BCrypt gerçekleştirmeleri |
| Api | Controller'lar, middleware, yapılandırma |

## Kurulum

1. Repoyu klonla
2. `TaskFlow.Api/appsettings.json` içindeki `DefaultConnection` değerini kendi SQL Server'ına göre düzenle
3. JWT gizli anahtarını User Secrets ile ekle (en az 32 karakter):
4. Veritabanını oluştur (Package Manager Console, Default project: `TaskFlow.Infrastructure`):
5. Projeyi çalıştır, API dokümantasyonu `https://localhost:7251/scalar/v1` adresinde açılır

## Ana endpoint'ler

| Method | Adres | Açıklama |
|---|---|---|
| POST | /api/Auth/register | Kayıt ol |
| POST | /api/Auth/login | Giriş yap, token al |
| GET/POST | /api/Projects | Projeleri listele / oluştur |
| PUT/DELETE | /api/Projects/{id} | Güncelle / sil (sahip ya da Admin) |
| GET | /api/Tasks/by-project/{projectId} | Görevleri listele (sayfalama, filtre, sıralama) |
| POST | /api/Tasks | Görev oluştur |
| PUT/DELETE | /api/Tasks/{id} | Güncelle / sil |

## Testler

`TaskFlow.Tests` projesi içinde. Visual Studio'da **Test → Run All Tests**.

## Yol haritası

- Yorumlar ve proje üyeliği endpoint'leri
- Refresh token
- Docker desteği