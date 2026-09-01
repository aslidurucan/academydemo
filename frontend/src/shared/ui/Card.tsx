import type { HTMLAttributes } from 'react'
import './Card.css'

/** Bileşen Envanteri — Card (bkz. docs/frontend.md §Tasarım Sistemi). */
export function Card({ className, ...rest }: HTMLAttributes<HTMLDivElement>) {
  const classes = ['card', className].filter(Boolean).join(' ')
  return <div className={classes} {...rest} />
}
