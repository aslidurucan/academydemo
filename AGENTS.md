# AGENTS.md

İşaret levhası. Kural burada **kopyalanmaz**, yalnızca nerede olduğu gösterilir — tek gerçek kaynak (single source of truth) her zaman ilgili `docs/*.md` dosyasıdır.

## Proje Özeti

Videolu eğitim ürünleri satan bir e-ticaret platformu (Udemy benzeri). **Modüler monolith**: tek .NET minimal API uygulaması + EF Core, tek veritabanı ama modül başına ayrı şema. Arayüz React + Vite (`frontend/`). Modüller birbirini yalnızca `Contracts` katmanından görür — detaylar için [docs/architecture.md](docs/architecture.md).

## 7 Altın Kural

1. **Spec'siz feature/branch yok.** → [specs/TEMPLATE.md](specs/TEMPLATE.md), [docs/git.md](docs/git.md)
2. **Modül sınırı yalnızca `Contracts`'tan geçilir.** Başka modülün DbContext/entity/internal tipine çapraz erişim yasak. → [docs/architecture.md](docs/architecture.md) §Yasak Liste
3. **Para ve yüzde alanları her zaman `decimal`.** → [docs/conventions.md](docs/conventions.md)
4. **Endpoint'ler minimal API ile yazılır**, controller yok. → [docs/conventions.md](docs/conventions.md)
5. **Branch/commit/PR, `docs/git.md`'deki formata birebir uyar.** main'e doğrudan commit, force push, history silme yasak. → [docs/git.md](docs/git.md)
6. **Her acceptance criteria bir testtir**; frontend'de her AC görsel kanıtla kapatılır. → [docs/testing.md](docs/testing.md)
7. **Öneri Kuralı:** Soru, seçenek, bulgu veya açık konu getiren — kendi önerisini ve gerekçesini de getirir. Karar Takım Yöneticisi'nde; öneri onaysız uygulanmaz.

## Neyi Nerede Bulursun

| Konu | Dosya |
|---|---|
| Modül haritası, iletişim kuralları, yasaklar, V1 kapsam dışı | [docs/architecture.md](docs/architecture.md) |
| Terimler (ubiquitous language), iş kuralları (sipariş/kupon/iade) | [docs/domain.md](docs/domain.md) |
| Adlandırma, katmanlar, hata yönetimi, `decimal` kuralı | [docs/conventions.md](docs/conventions.md) |
| Branch stratejisi, commit formatı, PR şartları | [docs/git.md](docs/git.md) |
| Test stratejisi (backend + frontend), CI kapıları | [docs/testing.md](docs/testing.md) |
| Frontend standardı (dil/yapı, API, tasarım sistemi, a11y, kalite kapıları) | [docs/frontend.md](docs/frontend.md) |
| Mimari kararların gerekçesi (ADR) | [docs/decisions/](docs/decisions/) |
| Roller | [docs/roles/](docs/roles/) — oturum açılışında rolünü üstlen. |
| Sorun protokolleri | [docs/ap.md](docs/ap.md) — AP kodlarının çözümü. |
| Yeni feature başlatma | [specs/TEMPLATE.md](specs/TEMPLATE.md) |
| Bitmiş feature'lar | [specs/done/](specs/done/) |

## Çalışma Disiplini

- Başlamadan önce ilgili `specs/*.md` dosyasını oku. Karşılık gelen spec yoksa önce onu yaz/onaylat, koda geçme.
- Emin değilsen durma noktası: soruyu **öneri + gerekçeyle birlikte** sor (Altın Kural 7), sessizce varsayım yapıp ilerleme.
- Modül sınırını "hız için" bile ihlal eden kısayol üretme — bkz. [docs/architecture.md](docs/architecture.md) §Yasak Liste.
- Commit/PR açarken [docs/git.md](docs/git.md) formatını birebir uygula; kendi başına format basitleştirme.
- Testsiz / görsel kanıtsız PR açma — bkz. [docs/testing.md](docs/testing.md).
- Bir spec tamamlandığında `specs/done/`'a taşı (bkz. [specs/TEMPLATE.md](specs/TEMPLATE.md) Definition of Done).
- Her frontend işi [docs/frontend.md](docs/frontend.md) standardına göre denetlenir — kişiye değil dosyaya göre.
