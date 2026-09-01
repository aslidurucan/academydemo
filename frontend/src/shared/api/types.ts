/**
 * Catalog API sözleşmesinden türetilen DTO tipleri (docs/frontend.md §API Disiplini).
 * Kaynak: Academy.Catalog.Api — CourseListResponse / CourseListItemResponse
 * (src/Modules/Catalog/Catalog.Api, spec 0001, branch feature/0001-urun-listeleme).
 * ASP.NET Core minimal API varsayılan System.Text.Json politikası camelCase'e serileştirir.
 *
 * Not (bilinçli isim tercihi): backend'in ubiquitous language'ı (docs/domain.md) "Kurs/Course"
 * kullanır, "Ürün/Product" değil — eş anlamlıların karışması docs/domain.md'de açıkça yasak.
 * specs/frontend-001-urun-listesi.md metninde "ürün" ifadesi geçse de, kod ve ekran metninde
 * domain terimiyle tutarlı kalmak için "Kurs" kullanıldı.
 */

export interface CourseListItem {
  id: string
  title: string
  listPrice: number
  categoryName: string
  instructorName: string
  coverImageUrl: string
}

export interface CourseListResponse {
  items: CourseListItem[]
  totalCount: number
  page: number
  pageSize: number
}
