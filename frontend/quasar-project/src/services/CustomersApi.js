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

export async function getCustomers({ page = 1, pageSize = 10 } = {}) {
  return requestJson(`${API_BASE_URL}/customers?page=${page}&pageSize=${pageSize}`, {}, 'No se pudo cargar customers')
}

export async function searchCustomers({ name } = {}) {
  const params = new URLSearchParams()

  if (name) {
    params.set('name', name)
  }

  const queryString = params.toString()
  const url = queryString ? `${API_BASE_URL}/customers/search?${queryString}` : `${API_BASE_URL}/customers/search`

  return requestJson(url, {}, 'No se pudo buscar customers')
}

export async function createCustomer(customerDto) {
  return requestJson(
    `${API_BASE_URL}/customers`,
    {
      method: 'POST',
      body: JSON.stringify(customerDto),
    },
    'No se pudo crear el customer',
  )
}

export async function updateCustomer(customerId, customerDto) {
  return requestJson(
    `${API_BASE_URL}/customers/${customerId}`,
    {
      method: 'PUT',
      body: JSON.stringify(customerDto),
    },
    'No se pudo actualizar el customer',
  )
}

export async function deleteCustomer(customerId) {
  return requestJson(
    `${API_BASE_URL}/customers/${customerId}`,
    {
      method: 'DELETE',
    },
    'No se pudo eliminar el customer',
  )
}