# Frontend Standardı

> Bu belge artık iskelet değil — **profesyonel frontend standardımızdır.** Bundan sonra her frontend işi bu dosyaya göre denetlenir, kişiye göre değil. Süreç/git kuralları için [docs/git.md](git.md), test stratejisi için [docs/testing.md](testing.md), spec süreci için [specs/TEMPLATE.md](../specs/TEMPLATE.md).

## Dil ve Yapı

**Kural: TypeScript zorunlu.** `.js`/`.jsx` yok — yeni dosya her zaman `.ts`/`.tsx`.

Klasör yapısı:

```
frontend/src/
  features/<feature>/     ekranlar; o feature'a özel bileşen/hook/durum
  shared/
    ui/                   ortak, feature-bağımsız bileşenler — bkz. Bileşen Envanteri
    api/                  TEK API client — bkz. API Disiplini
```

Bir bileşen birden fazla feature'da kullanılacaksa `shared/ui`'a taşınır; tek feature'a özgü kalıyorsa `features/<feature>` içinde kalır.

## API Disiplini

**Kural: tüm istekler `shared/api` içindeki tek client'tan geçer.** Bileşen içine endpoint URL'i yazılMAZ, bileşenden doğrudan `fetch`/`axios` çağrısı yapılMAZ.

- DTO tipleri backend sözleşmesinden türetilir — response şekli `shared/api` içinde bir kez tanımlanır, bileşenler onu import eder, kendi tipini icat etmez.
- Backend endpoint'i (route, response şekli) değiştiğinde güncellenecek tek yer `shared/api` olur.

## Tasarım Sistemi

**Kural: gömülü sabit değer (hex/px) yasak.** Renk, aralık (spacing), tipografi, köşe yarıçapı — hepsi tek bir token dosyasından gelir; bileşenler doğrudan değer değil, token kullanır.

**Bileşen Envanteri:** `Button` · `Card` · `Badge` · `EmptyState` · `ErrorState` · `Skeleton`.

Her yeni ekran önce bu envantere bakar. Envanterde karşılığı varsa onu kullanır; yoksa yeni bileşeni **önerir** (bkz. [AGENTS.md](../AGENTS.md) Öneri Kuralı — öneri + gerekçe, karar Takım Yöneticisi'nde) — sessizce yeni bir varyant türetmez.

## Üç Durum Kuralı

**Kural: her ekran üç durumu da tasarlar — yükleme (skeleton), boş, hata.** Biri eksikse ekran eksik sayılır, PR'a hazır değildir.

- Hata durumunda kullanıcıya teknik metin sızmaz: stack trace, HTTP status kodu, exception mesajı ekranda asla görünmez.
- Hata mesajı **Türkçe** ve **eylem önerilidir** — ör. "Kurslar yüklenemedi. Tekrar dene." "Bir hata oluştu" gibi belirsiz mesaj yeterli değildir.

## Erişilebilirlik Tabanı

**Kural: semantik HTML — `div` yığını değil.**

- Form alanları her zaman etiketli (`<label htmlFor>` veya `aria-label`).
- Klavyeyle gezilebilirlik: tüm etkileşimli öğelere `Tab` ile ulaşılır, odak (focus) durumu görünürdür.
- Yeterli kontrast: metin/arka plan WCAG AA eşiğinin altına düşmez.

## Kalite Kapıları

**Kural: typecheck + ESLint + Prettier zorunlu; kritik akışa smoke test.**

| Kapı | Araç | Zorunluluk |
|---|---|---|
| Tip kontrolü | `tsc --noEmit` | Her PR |
| Lint | ESLint | Her PR |
| Format | Prettier (kontrol modu) | Her PR |
| Smoke test | Vitest + Testing Library | Kritik akışlar (satın alma, giriş, kurs izleme başlatma) |

Komutlar `frontend/package.json`'da tanımlı olur (`typecheck`, `lint`, `format:check`, `test`) — CI bunları çağırır, elle çalıştırmaya güvenilmez.

## Süreç

- Her frontend dilimi bir mini-spec'le koşar: `specs/frontend-XXX-<ad>.md` (bkz. [specs/TEMPLATE.md](../specs/TEMPLATE.md)).
- Görsel kanıt (ekran görüntüsü) **VERIFY'ın parçasıdır** — QA rolü diff'i spec'e karşı bu kanıtla doğrular (bkz. [docs/roles/qa.md](roles/qa.md)).
- Branch/commit/PR kuralları [docs/git.md](git.md) ile birebir aynıdır — frontend için ayrı bir istisna yoktur.
