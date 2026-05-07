const API_BASE_URL = '/api'

export async function getOrders({ page = 1, pageSize = 10 } = {}) {
  const response = await fetch(`${API_BASE_URL}/orders?page=${page}&pageSize=${pageSize}`)

  if (!response.ok) {
    throw new Error(`No se pudo cargar orders (${response.status})`)
  }

  return response.json()
}
