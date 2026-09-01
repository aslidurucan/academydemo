import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App'
import './shared/ui/tokens.css'
import './index.css'

const rootElement = document.getElementById('root')
if (!rootElement) {
  throw new Error('#root elemanı bulunamadı.')
}

createRoot(rootElement).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
