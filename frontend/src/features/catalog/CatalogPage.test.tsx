import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { fetchPublishedCourses } from '../../shared/api/client'
import { CatalogPage } from './CatalogPage'

vi.mock('../../shared/api/client', () => ({
  fetchPublishedCourses: vi.fn(),
}))

const mockedFetch = vi.mocked(fetchPublishedCourses)

const sampleCourse = {
  id: '11111111-1111-1111-1111-111111111111',
  title: 'ASP.NET Core ile API Geliştirme',
  listPrice: 249.9,
  categoryName: 'Programlama',
  instructorName: 'Ayşe Yılmaz',
  coverImageUrl: 'https://example.test/cover.jpg',
}

// Kritik akış: kullanıcı katalog ekranını açar ve üç durumu (yükleme/boş/hata) + başarı
// durumunu görür (bkz. specs/frontend-001-urun-listesi.md Acceptance Criteria).
describe('CatalogPage', () => {
  beforeEach(() => {
    mockedFetch.mockReset()
  })

  it('shows skeleton while loading, then renders course cards on success', async () => {
    let resolveFetch: (value: Awaited<ReturnType<typeof fetchPublishedCourses>>) => void = () => {}
    mockedFetch.mockReturnValue(
      new Promise((resolve) => {
        resolveFetch = resolve
      }),
    )

    render(<CatalogPage />)

    expect(screen.getByTestId('catalog-skeleton')).toBeInTheDocument()

    resolveFetch({ items: [sampleCourse], totalCount: 1, page: 1, pageSize: 20 })

    await waitFor(() => expect(screen.getByText(sampleCourse.title)).toBeInTheDocument())
    expect(screen.queryByTestId('catalog-skeleton')).not.toBeInTheDocument()
  })

  it('shows empty state when the catalog has no courses', async () => {
    mockedFetch.mockResolvedValue({ items: [], totalCount: 0, page: 1, pageSize: 20 })

    render(<CatalogPage />)

    expect(await screen.findByText('Henüz kurs yok')).toBeInTheDocument()
  })

  it('shows error state without leaking technical details, and retries on demand', async () => {
    mockedFetch.mockRejectedValueOnce(new Error('ECONNREFUSED at 10.0.0.1:5432'))
    mockedFetch.mockResolvedValueOnce({
      items: [sampleCourse],
      totalCount: 1,
      page: 1,
      pageSize: 20,
    })

    render(<CatalogPage />)

    expect(await screen.findByRole('alert')).toBeInTheDocument()
    expect(screen.queryByText(/ECONNREFUSED/)).not.toBeInTheDocument()

    await userEvent.click(screen.getByRole('button', { name: 'Tekrar dene' }))

    expect(await screen.findByText(sampleCourse.title)).toBeInTheDocument()
  })

  it('navigates to the next page and requests it from the API', async () => {
    mockedFetch.mockResolvedValue({
      items: [sampleCourse],
      totalCount: 40,
      page: 1,
      pageSize: 20,
    })

    render(<CatalogPage />)

    expect(await screen.findByText('Sayfa 1 / 2')).toBeInTheDocument()

    mockedFetch.mockResolvedValueOnce({
      items: [sampleCourse],
      totalCount: 40,
      page: 2,
      pageSize: 20,
    })

    await userEvent.click(screen.getByRole('button', { name: 'Sonraki' }))

    await waitFor(() => expect(mockedFetch).toHaveBeenLastCalledWith({ page: 2 }))
  })
})
