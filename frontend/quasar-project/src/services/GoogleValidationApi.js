const API_BASE_URL = 'https://addressvalidation.googleapis.com/v1'
const API_KEY = import.meta.env.VITE_GOOGLE_API_KEY

async function requestJson(url, options = {}, errorMessage = 'La solicitud falló') {
  const response = await fetch(url, {
    headers: {
      'Content-Type': 'application/json',
      ...(options.headers ?? {}),
    },
    ...options,
  })

  if (!response.ok) {
    const errorText = await response.text()
    console.error('Google API Error:', errorText)
    throw new Error(`${errorMessage} (${response.status}): ${errorText}`)
  }

  if (response.status === 204) {
    return null
  }

  return response.json()
}

/**
 * Validates an address using Google Address Validation API
 * @param {string} address - The address to validate
 * @param {string} regionCode - Optional region code (e.g., 'US', 'MX')
 * @param {string} languageCode - Optional language code (e.g., 'en', 'es')
 * @returns {Promise} Validation result with formatted address and geocoding info
 */
export async function validateAddress({ address, regionCode = '', languageCode = 'en' } = {}) {
  if (!API_KEY) {
    throw new Error('Google API Key is not configured. Set VITE_GOOGLE_API_KEY environment variable.')
  }

  if (!address) {
    throw new Error('Address is required for validation')
  }

  const requestBody = {
    address: {
      addressLines: [address],
      regionCode: regionCode || undefined,
      languageCode,
    },
    enableUspsCass: true,
  }

  // Remove undefined values
  Object.keys(requestBody.address).forEach((key) => {
    if (requestBody.address[key] === undefined) {
      delete requestBody.address[key]
    }
  })

  try {
    return await requestJson(
      `${API_BASE_URL}:validateAddress?key=${API_KEY}`,
      {
        method: 'POST',
        body: JSON.stringify(requestBody),
      },
      'No se pudo validar la dirección',
    )
  } catch (error) {
    // If validation fails for ANY reason (like Unsupported region code, or API block), 
    // fall back to Geocoding API as it's much more permissive across the globe.
    try {
      const geocodeResponse = await requestJson(`https://maps.googleapis.com/maps/api/geocode/json?address=${encodeURIComponent(address)}&key=${API_KEY}`, { method: 'GET' }, 'No se pudo hacer geocoding')
      
      if (geocodeResponse && geocodeResponse.status === 'OK' && geocodeResponse.results && geocodeResponse.results.length > 0) {
        const firstResult = geocodeResponse.results[0]
        return {
          result: {
            address: {
              formattedAddress: firstResult.formatted_address,
              postalAddress: {
                regionCode: regionCode || ''
              }
            },
            geocode: {
              location: firstResult.geometry.location
            },
            verdict: {
              possibleNextAction: 'GEOCODE_FALLBACK'
            }
          }
        }
      }
    } catch (fallbackError) {
      console.error('Geocoding fallback also failed:', fallbackError)
    }
    
    // If geocoding also fails, throw the original error
    throw error
  }
}

/**
 * Provides address suggestions based on partial input
 * @param {string} input - The partial address to get suggestions for
 * @param {string} regionCode - Optional region code (e.g., 'US', 'MX')
 * @param {string} languageCode - Optional language code (e.g., 'en', 'es')
 * @returns {Promise} List of address suggestions
 */
export async function suggestAddresses({ input, regionCode = '', languageCode = 'en' } = {}) {
  if (!API_KEY) {
    throw new Error('Google API Key is not configured. Set VITE_GOOGLE_API_KEY environment variable.')
  }

  if (!input) {
    throw new Error('Input is required for suggestions')
  }

  const requestBody = {
    input,
    regionCode: regionCode || undefined,
    languageCode,
  }

  // Remove undefined values
  Object.keys(requestBody).forEach((key) => {
    if (requestBody[key] === undefined) {
      delete requestBody[key]
    }
  })

  return requestJson(
    `${API_BASE_URL}:searchAndValidateAddress?key=${API_KEY}`,
    {
      method: 'POST',
      body: JSON.stringify(requestBody),
    },
    'No se pudieron obtener sugerencias de direcciones',
  )
}
