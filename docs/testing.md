# Test Stratejisi

> Bkz. [AGENTS.md](../AGENTS.md) altın kural 6. Frontend detayları için [docs/frontend.md](frontend.md).

## Backend (xUnit)

- **Kural: her Acceptance Criteria = en az bir test.** Spec'teki (bkz. [specs/TEMPLATE.md](../specs/TEMPLATE.md)) her AC satırı, karşılık gelen bir test metoduyla eşlenir.
- Adlandırma deseni: `MethodName_Senaryo_BeklenenSonuç` — örn. `ApplyCoupon_ExpiredCoupon_ReturnsRejected`, `GrantAccess_AlreadyEnrolled_IsIdempotent`.
- Test projesi modül başına: `tests/Modules/<Module>.Tests`. Bir modülün testi başka modülün internal'ine erişmez — modül sınırı testler için de geçerli (bkz. [docs/architecture.md](architecture.md) §Yasak Liste).
- Domain event contract testleri de test kapsamına girer: event doğru koşulda yayınlanıyor mu, payload doğru mu.
- Cross-module senkron çağrılar (bkz. [docs/architecture.md](architecture.md) §İletişim Kuralları) `Contracts` arayüzü üzerinden mock'lanır; gerçek DB'ye çapraz erişimle test yazılmaz.

## Frontend

- Her mini-spec'in her Acceptance Criteria satırı için **görsel kanıt** (ilgili ekranın ekran görüntüsü) PR'a eklenir.
- Kritik akışlar (satın alma, giriş, kurs izleme başlatma) için en az bir **smoke test** — uçtan uca, kritik yolun çökmediğini doğrular; ayrıntılı senaryo testi değildir.
- Detaylı frontend konvansiyonları: [docs/frontend.md](frontend.md).

## CI Kapıları

| Taraf | Zorunlu |
|---|---|
| Backend PR | build + xUnit yeşil |
| Frontend PR | lint + build yeşil + PR açıklamasında ekran görüntüsü |

Her iki taraf da spec'in Definition of Done'ını karşılamadan merge edilmez (bkz. [docs/git.md](git.md) §PR Şartları).
