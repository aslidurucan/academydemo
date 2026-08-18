# Konvansiyonlar

> Modül sınırlarının *neden*i için [docs/architecture.md](architecture.md). Bu belge *nasıl* yazıldığını sabitler: klasör/isimlendirme, katmanlar, hata yönetimi, para kuralı.

## Klasör Yapısı

```
src/
  Modules/
    <Module>/
      <Module>.Api             minimal API endpoint tanımları + DI extension'ı
      <Module>.Domain          entity, value object, domain event tanımları
      <Module>.Infrastructure  EF Core DbContext (modülün kendi şeması), migration, dış entegrasyon
      <Module>.Contracts       başka modüllerin gördüğü TEK yer: public arayüzler + DTO
  Host/                        composition root — Program.cs, her modülün Api'sini map eder
  Shared/                      teknik yardımcılar (Result<T>, event bus arayüzü) — iş kuralı YOK
tests/
  Modules/
    <Module>.Tests
frontend/                      bkz. docs/frontend.md
specs/                         bkz. specs/TEMPLATE.md
```

Bağımlılık yönü: `Api` ve `Infrastructure` → `Domain`; `Contracts` diğer modüllerin tek görebildiği yüzey. Bir modül başka bir modülün `Api`/`Domain`/`Infrastructure` projesine asla referans vermez — yalnızca `Contracts`'ına.

## Adlandırma

- Namespace: `Academy.<Module>.<Layer>` (ör. `Academy.Catalog.Api`, `Academy.Catalog.Contracts`).
- Tip/metot: `PascalCase`. Parametre/local değişken: `camelCase`.
- Entity adı tekil (`Course`, `Order` — `Courses`, `Orders` değil); `DbSet<T>` özelliği çoğul.
- Endpoint route: kebab-case, çoğul kaynak — `/api/courses`, `/api/courses/{id}/sections`.
- DTO: `Request` / `Response` soneki — `CreateCourseRequest`, `CourseResponse`.
- Domain event: geçmiş zaman, iş dilinde — `PaymentCompleted`, `AccessGranted` (bkz. [docs/domain.md](domain.md) ortak dil tablosu).

## Minimal API

- Controller kullanılmaz. Her modülün `Api` projesinde bir `MapXEndpoints(this IEndpointRouteBuilder app)` extension metodu olur; `Program.cs` yalnızca bunları çağırır.
- Endpoint metotları ince kalır: request doğrulama + Domain/Contracts çağrısı + response mapping. İş kuralı endpoint içine yazılmaz, `Domain`'e gider.

## Katmanlar

| Katman | İçerir | İçermez |
|---|---|---|
| `Domain` | Entity, value object, invariant, domain event tanımı | EF Core, HTTP, dış servis referansı |
| `Infrastructure` | `DbContext`, migration, repository implementasyonu, dış entegrasyon (ödeme sağlayıcı vb.) | İş kuralı |
| `Api` | Endpoint tanımı, request/response mapping, DI kaydı | Doğrudan SQL/EF sorgusu (Infrastructure üzerinden gider) |
| `Contracts` | Başka modüllerin kullandığı arayüz + DTO | Implementasyon (implementasyon `Infrastructure`/`Api`'de) |

## Hata Yönetimi

- **Beklenen iş kuralı ihlalleri** (validasyon, iş kuralı reddi — ör. "süresi geçmiş kupon") exception fırlatmaz; `Result<T>` döner, endpoint bunu açık bir 4xx + `ProblemDetails`'e çevirir.
- **Beklenmeyen/teknik hatalar** (DB bağlantısı, dış servis çöküşü) exception olarak fırlatılır; global bir exception-handling middleware bunları `ProblemDetails` + 500'e çevirir.
- Kullanıcıya hiçbir zaman stack trace / exception mesajı / iç hata kodu sızmaz — frontend tarafı için bkz. [docs/frontend.md](frontend.md).

## Para ve Yüzde

- Para ve yüzde alanları **her zaman `decimal`** — `double`/`float` yasak (bkz. [AGENTS.md](../AGENTS.md) altın kural 3).
- EF Core kolon tipi: para alanları `decimal(18,2)`, yüzde/oran alanları `decimal(5,4)`.
- Sipariş kalemlerinde fiyat `PriceSnapshot` olarak saklanır, sonradan değişmez (bkz. [docs/domain.md](domain.md) §Sipariş).
- V1'de tek para birimi (TRY); kod alanı tutulur ama dönüşüm yapılmaz (bkz. [docs/architecture.md](architecture.md) §Kapsam Dışı).

## Şema Sabitleme

Her modülün `Infrastructure` projesindeki `DbContext`, `OnModelCreating` içinde kendi şemasını sabitler:

```csharp
modelBuilder.HasDefaultSchema("catalog");
```

Şema adları [docs/architecture.md](architecture.md) modül tablosundaki `Şema` sütunuyla birebir eşleşir.
