const API_BASE_URL = '/api'

async function requestJson(url, options = {}, errorMessage = 'La solicitud falló') {
  const response = await fetch(url, {
    headers: {
      'Content-Type': 'application/json',
      ...(options.headers ?? {}),
    },
    ...options,
  })

  if (!response.ok) {
    throw new Error(`${errorMessage} (${response.status})`)
  }

  if (response.status === 204) {
    return null
  }

  return response.json()
}

export async function getProducts({ page = 1, pageSize = 20 } = {}) {
  return requestJson(`${API_BASE_URL}/products?page=${page}&pageSize=${pageSize}`, {}, 'No se pudo cargar products')
}

export async function searchProducts({ name } = {}) {
  const params = new URLSearchParams()

  if (name) {
    params.set('name', name)
  }

  const queryString = params.toString()
  const url = queryString ? `${API_BASE_URL}/products/search?${queryString}` : `${API_BASE_URL}/products/search`

  return requestJson(url, {}, 'No se pudo buscar products')
}

export async function createProduct(productDto) {
  return requestJson(
    `${API_BASE_URL}/products`,
    {
      method: 'POST',
      body: JSON.stringify(productDto),
    },
    'No se pudo crear el product',
  )
}

export async function updateProduct(productId, productDto) {
  return requestJson(
    `${API_BASE_URL}/products/${productId}`,
    {
      method: 'PUT',
      body: JSON.stringify(productDto),
    },
    'No se pudo actualizar el product',
  )
}

export async function deleteProduct(productId) {
  return requestJson(
    `${API_BASE_URL}/products/${productId}`,
    {
      method: 'DELETE',
    },
    'No se pudo eliminar el product',
  )
}