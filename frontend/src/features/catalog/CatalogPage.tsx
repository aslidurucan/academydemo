import { EmptyState } from '../../shared/ui/EmptyState'
import { ErrorState } from '../../shared/ui/ErrorState'
import { Skeleton } from '../../shared/ui/Skeleton'
import { CourseCard } from './CourseCard'
import { Pagination } from './Pagination'
import { useCourseCatalog } from './useCourseCatalog'
import './CatalogPage.css'

const SKELETON_CARD_COUNT = 8

/**
 * Ürün Listesi Ekranı (specs/frontend-001-urun-listesi.md) — kart grid'i + sayfalama.
 * Üç durum kuralı burada tam olarak uygulanır: yükleme (skeleton), boş (EmptyState),
 * hata (ErrorState) — bkz. docs/frontend.md §Üç Durum Kuralı.
 */
export function CatalogPage() {
  const { state, page, totalPages, goToNextPage, goToPreviousPage, retry } = useCourseCatalog()

  return (
    <section className="catalog-page" aria-label="Kurs listesi">
      {state.status === 'loading' && (
        <div className="catalog-page__grid" data-testid="catalog-skeleton">
          {Array.from({ length: SKELETON_CARD_COUNT }).map((_, index) => (
            <Skeleton key={index} height="280px" />
          ))}
        </div>
      )}

      {state.status === 'error' && <ErrorState onRetry={retry} />}

      {state.status === 'success' && state.data.items.length === 0 && (
        <EmptyState
          title="Henüz kurs yok"
          description="Katalogda görüntülenecek bir kurs bulunamadı. Daha sonra tekrar kontrol edin."
        />
      )}

      {state.status === 'success' && state.data.items.length > 0 && (
        <>
          <div className="catalog-page__grid">
            {state.data.items.map((course) => (
              <CourseCard key={course.id} course={course} />
            ))}
          </div>
          <Pagination
            page={page}
            totalPages={totalPages}
            onPrevious={goToPreviousPage}
            onNext={goToNextPage}
          />
        </>
      )}
    </section>
  )
}
