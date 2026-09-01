import { Badge } from '../../shared/ui/Badge'
import { Card } from '../../shared/ui/Card'
import type { CourseListItem } from '../../shared/api/types'
import './CourseCard.css'

interface CourseCardProps {
  course: CourseListItem
}

const priceFormatter = new Intl.NumberFormat('tr-TR', {
  style: 'currency',
  currency: 'TRY',
  maximumFractionDigits: 2,
})

export function CourseCard({ course }: CourseCardProps) {
  return (
    <Card className="course-card">
      <div className="course-card__media">
        {/* Başlık kartta metin olarak da göründüğü için görsel dekoratif kabul edilir. */}
        <img src={course.coverImageUrl} alt="" loading="lazy" />
      </div>
      <div className="course-card__body">
        <Badge className="course-card__category">{course.categoryName}</Badge>
        <h3 className="course-card__title">{course.title}</h3>
        <p className="course-card__price">{priceFormatter.format(course.listPrice)}</p>
      </div>
    </Card>
  )
}
