# Roller

Bu projede üç rolle çalışılır: **Analist**, **Developer**, **QA**. Bir oturum tek bir role bürünür. Rol, [AGENTS.md](../../AGENTS.md)'nin üstüne biner — onu değiştirmez, sadece o oturumda hangi yetki/yasak setinin geçerli olduğunu daraltır.

## Rol Nasıl Üstlenilir

Oturum başında tek satırlık üstlenme kalıbı:

```
Rolüm: <Analist|Developer|QA>
```

Bu satır, oturumun geri kalanında hangi kartın (`docs/roles/*.md`) geçerli olduğunu sabitler.

## Şapka Değişimi

Bir oturum içinde rol değişecekse — ör. bir Developer oturumunda Analist'ten Developer'a geçiş gerekiyorsa — bu **İLAN EDİLİR**, sessizce geçilmez:

```
Şapka değişti: Analist → Developer
```

İlan edilmeden yapılan iş, hangi rolün yetkisiyle yapıldığı belirsiz kaldığı için geçersiz sayılır.

## Ortak Kurallar (tüm roller)

- [AGENTS.md](../../AGENTS.md) okunur — roller onun üstüne biner, onu geçersiz kılmaz.
- **Kanıtsız iddia yok:** "çalışıyor", "düzeltildi", "tamamlandı" gibi ifadeler kanıt (test çıktısı, ekran görüntüsü, repro) olmadan yazılmaz.
- Belirsizlikte **AP-10** işletilir — bkz. [docs/ap.md](../ap.md).
- **Öneri Kuralı:** soru, bulgu veya seçenek önerisiz sunulmaz — her biri kendi önerisi ve gerekçesiyle gelir. Karar Takım Yöneticisi'nde; öneri onaysız uygulanmaz.

## Rol Kartları

| Rol | Omurga | Kart |
|---|---|---|
| Analist | INTENT · CLARIFY · SPEC | [analist.md](analist.md) |
| Developer | PLAN · BUILD · düzeltme | [developer.md](developer.md) |
| QA | REVIEW · TEST · VERIFY · repro | [qa.md](qa.md) |
