# Frontend Konvansiyonları (İskelet)

> Bu belge şu an **iskelet** halinde. 1.8 numaralı dilimde (slice) profesyonel standarda genişletilecek: tasarım token'ları, bileşen envanteri, kalite kapıları. Şimdilik aşağıdaki asgari kurallar geçerli — bkz. [AGENTS.md](../AGENTS.md), [docs/testing.md](testing.md).

## Stack

- React + Vite + TypeScript, `frontend/` klasöründe.
- Frontend, backend modülleriyle bire bir eşlenmez; ekran/akış bazlı organize olur — ama hangi API'yi çağırdığı her zaman bellidir (aşağıya bkz.).

## API Erişimi

- Tüm API çağrıları **tek bir client dosyasından** geçer (ör. `frontend/src/api/client.ts`).
- Bileşenler `fetch`/`axios` çağrısını doğrudan yapmaz; client dosyasındaki fonksiyonları çağırır.
- Bu kural sayesinde backend endpoint'i değiştiğinde (route, response şekli) tek bir yer güncellenir.

## Zorunlu Ekran Durumları

Veri getiren her ekran/bileşen üç durumu da ele almak **zorunda**:

- **Yükleniyor** — iskelet/spinner.
- **Boş** — veri yok durumu; kullanıcıya ne yapması gerektiğini söyler.
- **Hata** — kullanıcıya anlaşılır, iş diliyle yazılmış mesaj.

## Hata Mesajları

- Kullanıcıya **teknik hata metni sızmaz** — stack trace, exception mesajı, HTTP status kodu ekranda asla gösterilmez.
- Teknik detay yalnızca konsola/log'a yazılır; arayüz her zaman "ne oldu, ne yapabilirsin" diliyle konuşur.

## Dilimler (Slices)

- Her frontend dilimi bir mini-spec'le koşar (bkz. [specs/TEMPLATE.md](../specs/TEMPLATE.md)); spec'siz frontend değişikliği yapılmaz.
- Her dilimin kabul kriterleri görsel kanıtla kapatılır (bkz. [docs/testing.md](testing.md)).

## Sırada — 1.8'de Genişletilecek

- Tasarım token'ları (renk, tipografi, boşluk ölçeği)
- Bileşen envanteri (ortak buton/form/kart bileşenleri)
- Kalite kapıları (erişilebilirlik, performans bütçesi, görsel regresyon)
