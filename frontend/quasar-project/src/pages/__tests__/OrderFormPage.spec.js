import { mount } from '@vue/test-utils'
import { nextTick } from 'vue'
import { describe, it, expect, vi } from 'vitest'

vi.mock('quasar', () => ({
  useQuasar: () => ({
    notify: vi.fn(),
    dialog: vi.fn(),
  }),
}))

vi.mock('vue-router', () => ({
  useRoute: () => ({ query: {} }),
}))

// Mock services used by the component to avoid network/api calls
vi.mock('src/services/ordersApi', () => ({
  getOrders: async () => [],
  createOrder: async () => ({}),
  updateOrder: async () => ({}),
  deleteOrder: async () => ({}),
  searchOrders: async () => [],
}))

vi.mock('src/services/CustomersApi', () => ({
  getCustomers: async () => [],
}))

vi.mock('src/services/ProductsApi', () => ({
  getProducts: async () => [],
}))

vi.mock('src/services/GoogleValidationApi', () => ({
  validateAddress: async () => ({ result: {} }),
}))

import OrderFormPage from '../OrderFormPage.vue'

const globalStubs = {
  'q-page': true,
  'q-select': true,
  'q-input': true,
  'q-btn': true,
  'q-card': true,
  'q-card-section': true,
}

describe('OrderFormPage - form validation and map interactions', () => {
  it('computes canSaveOrder correctly based on required fields', async () => {
    const wrapper = mount(OrderFormPage, { global: { stubs: globalStubs } })

    // Wait a tick for any initialisation
    await nextTick()

    // Initial form should not be saveable (no customer, no employee, no product)
    expect(wrapper.vm.canSaveOrder).toBe(false)

    // Fill required fields
    // The `form` is proxied; update values directly
    wrapper.vm.form.customerId = 'CUST1'
    wrapper.vm.form.employeeId = 1
    wrapper.vm.form.orderDetails[0].productId = 10

    await nextTick()

    expect(wrapper.vm.canSaveOrder).toBe(true)
  })

  it('shows map iframe when validatedAddress contains coordinates or formatted address', async () => {
    const wrapper = mount(OrderFormPage, { global: { stubs: globalStubs } })

    await nextTick()

    // Provide validated coordinates
    wrapper.vm.validatedAddress.formattedAddress = '123 Test St, Testville'
    wrapper.vm.validatedAddress.latitude = 12.34
    wrapper.vm.validatedAddress.longitude = 56.78
    wrapper.vm.validatedAddress.verdict = 'ok'

    await nextTick()

    expect(wrapper.vm.mapEmbedUrl).toBe(
      'https://maps.google.com/maps?q=12.34,56.78&z=15&output=embed',
    )

    wrapper.vm.validatedAddress.latitude = null
    wrapper.vm.validatedAddress.longitude = null
    await nextTick()

    expect(wrapper.vm.mapEmbedUrl).toBe(
      'https://maps.google.com/maps?q=123%20Test%20St%2C%20Testville&z=14&output=embed',
    )
  })
})
