import { useCallback, useEffect, useRef, useState } from 'react'
import { fetchPublishedCourses } from '../../shared/api/client'
import type { CourseListResponse } from '../../shared/api/types'

type CatalogState =
  { status: 'loading' } | { status: 'error' } | { status: 'success'; data: CourseListResponse }

const DEFAULT_PAGE = 1

/**
 * Ürün listesi ekranının veri/durum makinesi: yükleme/hata/başarı + sayfalama.
 * Boş katalog ayrı bir fetch-durumu değil, başarı durumunun bir alt-render dalı
 * (data.items.length === 0) — bkz. features/catalog/CatalogPage.tsx.
 */
export function useCourseCatalog() {
  const [page, setPage] = useState(DEFAULT_PAGE)
  const [state, setState] = useState<CatalogState>({ status: 'loading' })
  // Yarış koşulunu (eski isteğin yeni isteği ezmesi) önlemek için: yalnızca en son isteğin
  // sonucu state'e yazılır.
  const latestRequestId = useRef(0)

  const load = useCallback((targetPage: number) => {
    const requestId = ++latestRequestId.current
    setState({ status: 'loading' })

    fetchPublishedCourses({ page: targetPage })
      .then((data) => {
        if (latestRequestId.current !== requestId) return
        setState({ status: 'success', data })
      })
      .catch((error: unknown) => {
        if (latestRequestId.current !== requestId) return
        // Teknik detay yalnızca konsola — ekrana hiçbir zaman sızmaz (docs/frontend.md §Üç Durum Kuralı).
        console.error('Kurs listesi alınamadı:', error)
        setState({ status: 'error' })
      })
  }, [])

  useEffect(() => {
    load(page)
  }, [page, load])

  const retry = useCallback(() => load(page), [load, page])

  const totalPages =
    state.status === 'success'
      ? Math.max(1, Math.ceil(state.data.totalCount / state.data.pageSize))
      : 1

  return {
    state,
    page,
    totalPages,
    goToNextPage: () => setPage((current) => Math.min(current + 1, totalPages)),
    goToPreviousPage: () => setPage((current) => Math.max(1, current - 1)),
    retry,
  }
}
