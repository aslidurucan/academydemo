import type { CourseListResponse } from './types'

/**
 * Catalog API'sine bağlanan TEK client (docs/frontend.md §API Disiplini).
 * Bileşenler bu dosyanın dışında `fetch`/`axios` çağırmaz, endpoint URL'i yazmaz —
 * bkz. eslint.config.js (no-restricted-syntax: 'fetch').
 */

const API_BASE = '/api'

export class ApiError extends Error {
  constructor(
    message: string,
    public readonly status: number,
  ) {
    super(message)
    this.name = 'ApiError'
  }
}

export interface CourseListQuery {
  page?: number
  pageSize?: number
  categoryId?: string
}

export async function fetchPublishedCourses(
  query: CourseListQuery = {},
  signal?: AbortSignal,
): Promise<CourseListResponse> {
  const params = new URLSearchParams()
  if (query.page != null) params.set('page', String(query.page))
  if (query.pageSize != null) params.set('pageSize', String(query.pageSize))
  if (query.categoryId) params.set('categoryId', query.categoryId)

  const queryString = params.toString()
  const url = `${API_BASE}/courses${queryString ? `?${queryString}` : ''}`

  const response = await fetch(url, { signal })

  if (!response.ok) {
    // Teknik detay (status kodu) yalnızca burada/console'da kalır — ekrana asla yansımaz,
    // arayüz tarafı (ör. ErrorState) sabit, iş diliyle yazılmış bir mesaj gösterir.
    throw new ApiError(`Kurs listesi isteği başarısız: HTTP ${response.status}`, response.status)
  }

  return (await response.json()) as CourseListResponse
}
