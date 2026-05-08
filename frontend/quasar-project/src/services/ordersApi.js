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

export async function getOrders({ page = 1, pageSize = 10 } = {}) {
  return requestJson(`${API_BASE_URL}/orders?page=${page}&pageSize=${pageSize}`, {}, 'No se pudo cargar orders')
}

export async function searchOrders({ orderId, customerName } = {}) {
  const params = new URLSearchParams()

  if (orderId != null && orderId !== '') {
    params.set('orderId', orderId)
  }

  if (customerName) {
    params.set('customerName', customerName)
  }

  const queryString = params.toString()
  const url = queryString ? `${API_BASE_URL}/orders/search?${queryString}` : `${API_BASE_URL}/orders/search`

  return requestJson(url, {}, 'No se pudo buscar orders')
}

export async function createOrder(orderDto) {
  return requestJson(
    `${API_BASE_URL}/orders`,
    {
      method: 'POST',
      body: JSON.stringify(orderDto),
    },
    'No se pudo crear la order',
  )
}

export async function updateOrder(orderId, orderDto) {
  return requestJson(
    `${API_BASE_URL}/orders/${orderId}`,
    {
      method: 'PUT',
      body: JSON.stringify(orderDto),
    },
    'No se pudo actualizar la order',
  )
}

export async function deleteOrder(orderId) {
  return requestJson(
    `${API_BASE_URL}/orders/${orderId}`,
    {
      method: 'DELETE',
    },
    'No se pudo eliminar la order',
  )
}
