# Frontend Development Guide - Technical Reference

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Vue 3 Composition API Patterns](#vue-3-composition-api-patterns)
3. [Service Layer Architecture](#service-layer-architecture)
4. [State Management Patterns](#state-management-patterns)
5. [Component Lifecycle](#component-lifecycle)
6. [Data Fetching Patterns](#data-fetching-patterns)
7. [Error Handling Best Practices](#error-handling-best-practices)
8. [Common Code Patterns](#common-code-patterns)
9. [Debugging Guide](#debugging-guide)
10. [Performance Tips](#performance-tips)

---

## Architecture Overview

### Three-Tier Architecture

```
┌─────────────────────────────────────┐
│         Pages (Vue Components)      │
│  - IndexPage, OrderFormPage, etc    │
└────────────┬────────────────────────┘
             │ uses
┌────────────▼────────────────────────┐
│       Services (API Layer)          │
│  - ordersApi, CustomersApi, etc     │
└────────────┬────────────────────────┘
             │ makes HTTP requests to
┌────────────▼────────────────────────┐
│    Backend API (.NET Core)          │
│  http://localhost:5128/api          │
└─────────────────────────────────────┘
```

### Request Flow Example

```
User clicks "Save Order" button
         ↓
OrderFormPage.saveOrder() called
         ↓
Validates form (customer, employee, products)
         ↓
Calls updateOrder(orderId, payload)
         ↓
ordersApi.js → updateOrder()
         ↓
HTTP PUT /api/orders/:id
         ↓
Backend processes and responds
         ↓
showResultDialog() displays response
         ↓
refreshOrders() reloads data
```

---

## Vue 3 Composition API Patterns

### 1. Basic Setup Pattern

```javascript
<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useQuasar } from 'quasar'

// Composition-API style: import composables first
const $q = useQuasar()

// 1. Reactive state with ref()
const data = ref(null)
const loading = ref(false)
const error = ref(null)

// 2. Computed properties with computed()
const isReady = computed(() => data.value && !loading.value)

// 3. Watchers for side effects
watch(
  () => data.value,
  (newVal) => {
    console.log('Data changed:', newVal)
  }
)

// 4. Lifecycle hooks
onMounted(async () => {
  await fetchData()
})

// 5. Methods
async function fetchData() {
  loading.value = true
  try {
    // API call
    data.value = await getOrders()
  } catch (err) {
    error.value = err.message
  } finally {
    loading.value = false
  }
}
</script>
```

### 2. Reactive Forms Pattern

```javascript
const form = ref({
  username: '',
  email: '',
  agreed: false
})

const errors = ref({})

const isFormValid = computed(() => {
  return form.value.username.trim() !== '' &&
         form.value.email.includes('@') &&
         form.value.agreed
})

async function submitForm() {
  errors.value = {}
  if (!isFormValid.value) {
    errors.value.form = 'Please fill all required fields'
    return
  }
  
  try {
    await updateUser(form.value)
    $q.notify({ type: 'positive', message: 'Success!' })
  } catch (err) {
    errors.value.form = err.message
  }
}
```

### 3. Computed Properties with Dependencies

```javascript
// Simple computed
const fullName = computed(() => `${user.value.firstName} ${user.value.lastName}`)

// Computed with dependency on other computed
const displayName = computed(() => {
  if (fullName.value) return fullName.value
  return user.value.email
})

// Computed with complex logic
const orderTotal = computed(() => {
  return form.value.orderDetails.reduce((sum, detail) => {
    const subtotal = detail.quantity * detail.unitPrice
    const discount = subtotal * detail.discount
    return sum + subtotal - discount
  }, 0) + safeNumber(form.value.freight)
})
```

---

## Service Layer Architecture

### API Service Structure

```javascript
// services/ordersApi.js

// 1. Configuration (top of file)
const API_BASE_URL = '/api'
const API_TIMEOUT = 5000

// 2. Helper function (shared logic)
async function requestJson(url, options = {}, errorMessage = 'Request failed') {
  const response = await fetch(url, {
    headers: {
      'Content-Type': 'application/json',
      // Add auth headers if needed
      ...(options.headers ?? {}),
    },
    ...options,
  })

  if (!response.ok) {
    const errorText = await response.text()
    console.error('API Error:', errorText)
    throw new Error(`${errorMessage} (${response.status}): ${errorText}`)
  }

  if (response.status === 204) return null
  return response.json()
}

// 3. Exported functions
export async function getOrders({ page = 1, pageSize = 10 } = {}) {
  return requestJson(
    `${API_BASE_URL}/orders?page=${page}&pageSize=${pageSize}`,
    {},
    'Failed to load orders'
  )
}

export async function createOrder(orderDto) {
  return requestJson(
    `${API_BASE_URL}/orders`,
    {
      method: 'POST',
      body: JSON.stringify(orderDto),
    },
    'Failed to create order'
  )
}
```

### Service Usage Pattern

```javascript
// In a Vue component
<script setup>
import { ref, onMounted } from 'vue'
import { getOrders, searchOrders, createOrder } from 'src/services/ordersApi'

const orders = ref([])
const loading = ref(false)
const error = ref(null)

// Fetch on mount
onMounted(async () => {
  await loadOrders()
})

async function loadOrders() {
  loading.value = true
  error.value = null
  try {
    orders.value = await getOrders({ page: 1, pageSize: 50 })
  } catch (err) {
    error.value = err.message
  } finally {
    loading.value = false
  }
}

// Search function
async function searchByCustomer(customerId) {
  try {
    const results = await searchOrders({ orderId: customerId })
    // Process results
  } catch (err) {
    console.error('Search failed:', err)
  }
}

// Create new order
async function handleCreateOrder(orderData) {
  try {
    const newOrder = await createOrder(orderData)
    orders.value.push(newOrder)
    return newOrder
  } catch (err) {
    throw err // Re-throw for caller to handle
  }
}
</script>
```

---

## State Management Patterns

### Local Component State

When to use: Single page/component-specific state

```javascript
const form = ref(buildEmptyForm())
const saving = ref(false)
const error = ref(null)
```

### Shared State Pattern

When multiple components need same data:

```javascript
// orderStore.js (simple shared state, no external library)
export const useOrderStore = () => {
  const orders = ref([])
  const selectedOrder = ref(null)
  const loading = ref(false)

  const getOrderById = (id) => {
    return orders.value.find(o => o.orderId === id)
  }

  async function loadOrders() {
    loading.value = true
    orders.value = await getOrders()
    loading.value = false
  }

  return { orders, selectedOrder, loading, getOrderById, loadOrders }
}

// In component
import { useOrderStore } from 'src/stores/orderStore'
const store = useOrderStore()
const orders = computed(() => store.orders.value)
```

---

## Component Lifecycle

### OrderFormPage.vue Lifecycle

```
1. Component Initializes
   └─ ref() variables created
   └─ computed() properties evaluated

2. onMounted() Hook Fires
   └─ loadInitialData() called
      ├─ Fetch orders, customers, products (parallel Promise.all)
      └─ Set ref values with results
   └─ syncOrderFromRoute() called
      ├─ Check route.query.orderId
      ├─ Find order in orders.value
      └─ If not found, search API (fallback)
      └─ loadOrderInForm(order) populates form

3. Component Renders
   └─ All form fields display data from form.value

4. User Interactions
   └─ Form field changes
      └─ watch() clears validated address if user modifies
      └─ computed() properties update in real-time
   └─ Button clicks
      └─ saveOrder(), validateAddress(), removeOrder() called

5. API Responses
   └─ Success: showResultDialog(), refreshOrders()
   └─ Error: notifyError() or showResultDialog('Error', ...)

6. Before Unmount
   └─ Cleanup happens automatically (refs garbage collected)
```

### Lifecycle Best Practices

```javascript
// ✅ DO: Use setup hooks
onMounted(async () => {
  await loadData()
})

watch(
  () => route.query.id,
  async (id) => {
    if (id) await loadData(id)
  }
)

// ❌ DON'T: Skip error handling
onMounted(async () => {
  await loadData() // Can throw!
})

// ✅ DO: Always handle errors
onMounted(async () => {
  try {
    await loadData()
  } catch (err) {
    error.value = err.message
  }
})
```

---

## Data Fetching Patterns

### Pattern 1: Fetch on Component Mount

```javascript
const data = ref(null)
const loading = ref(false)

onMounted(async () => {
  loading.value = true
  try {
    data.value = await fetchData()
  } finally {
    loading.value = false
  }
})
```

### Pattern 2: Reactive Data Fetching (when deps change)

```javascript
const selectedId = ref(null)
const data = ref(null)
const loading = ref(false)

watch(
  () => selectedId.value,
  async (id) => {
    if (!id) return
    loading.value = true
    try {
      data.value = await fetchDataById(id)
    } finally {
      loading.value = false
    }
  }
)
```

### Pattern 3: Parallel Requests

```javascript
const orders = ref([])
const customers = ref([])
const products = ref([])
const loading = ref(false)

onMounted(async () => {
  loading.value = true
  try {
    const [ordersData, customersData, productsData] = await Promise.all([
      getOrders({ page: 1, pageSize: 400 }),
      getCustomers({ page: 1, pageSize: 400 }),
      getProducts({ page: 1, pageSize: 400 }),
    ])
    
    orders.value = ordersData
    customers.value = customersData
    products.value = productsData
  } catch (err) {
    error.value = err.message
  } finally {
    loading.value = false
  }
})
```

### Pattern 4: Request Deduplication (Fallback Search)

```javascript
// From syncOrderFromRoute in OrderFormPage.vue
async function syncOrderFromRoute() {
  const routeOrderId = route.query.orderId
  if (!routeOrderId) return

  // Try to find in already-loaded orders first
  let selected = orders.value.find(o => String(o.orderId) === String(routeOrderId))
  
  if (!selected) {
    // If not found, search API (fallback for orders beyond pagination)
    try {
      const searchResult = await searchOrders({ orderId: routeOrderId })
      if (searchResult?.length > 0) {
        selected = searchResult[0]
      }
    } catch (err) {
      console.error('Could not fetch order from route', err)
    }
  }

  if (!selected) return
  
  selectedOrderId.value = selected.orderId
  loadOrderInForm(selected)
}
```

---

## Error Handling Best Practices

### Error Types & Handling

```javascript
// 1. Network Errors (fetch fails entirely)
try {
  await fetchData()
} catch (err) {
  if (err instanceof TypeError) {
    error.value = 'Network error - check connection'
  }
}

// 2. HTTP Errors (backend returns 4xx/5xx)
// Handled by requestJson() in service layer
// Throws Error with message including status code

// 3. Validation Errors
function validateForm() {
  const errors = {}
  if (!form.value.customer) errors.customer = 'Required'
  if (!form.value.employee) errors.employee = 'Required'
  return errors
}

// 4. Special Case Errors (e.g., API limitations)
if (error.message.includes('Unsupported region code')) {
  // Handle special case (address validation fallback)
  validatedAddress.value = { /* fallback data */ }
} else {
  notifyError(error, 'Request failed')
}
```

### Notification Patterns

```javascript
// Simple notification
$q.notify({ 
  type: 'positive', 
  message: 'Order saved successfully!' 
})

// Error notification
$q.notify({ 
  type: 'negative', 
  message: 'Failed to save order' 
})

// Warning notification
$q.notify({ 
  type: 'warning', 
  message: 'Address validation not available for this region' 
})

// Dialog for detailed errors
$q.dialog({
  title: 'Error',
  message: 'Detailed error message with <code>HTML support</code>',
  html: true,
  ok: {
    label: 'OK',
    color: 'negative'
  }
})
```

---

## Common Code Patterns

### Pattern: Safe Number Conversion

```javascript
function safeNumber(value) {
  const n = Number(value)
  return Number.isFinite(n) ? n : 0
}

// Usage
const quantity = safeNumber(form.value.quantity) // Returns 0 if invalid
```

### Pattern: Null Coalescing in Forms

```javascript
function toNullIfEmpty(value) {
  if (value === null || value === undefined || value === '') {
    return null
  }
  return value
}

// Usage
const payload = {
  shipVia: toNullIfEmpty(form.value.shipVia), // Null if empty, else original value
  freight: toNullIfEmpty(form.value.freight),
}
```

### Pattern: Date Formatting

```javascript
function toDateInput(value) {
  if (!value) return ''
  
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return ''
  
  return date.toISOString().slice(0, 10) // YYYY-MM-DD
}

// Usage
const orderDate = toDateInput(order.orderDate) // Formats for <input type="date">
```

### Pattern: Array Normalization

```javascript
function normalizeItems(response) {
  if (Array.isArray(response)) {
    return response
  }
  if (Array.isArray(response?.items)) {
    return response.items
  }
  return []
}

// Usage - handles different API response formats
const orders = normalizeItems(apiResponse)
```

### Pattern: Loading State Toggle

```javascript
async function executeAction() {
  actionLoading.value = true
  try {
    const result = await performAction()
    successMessage.value = 'Action completed!'
  } catch (err) {
    errorMessage.value = err.message
  } finally {
    actionLoading.value = false
  }
}

// In template: <q-btn :loading="actionLoading" @click="executeAction" />
```

---

## Debugging Guide

### Browser Developer Tools Setup

1. **Open DevTools**: F12 or Right-click → Inspect
2. **Vue DevTools**: Install Vue 3 DevTools browser extension
3. **Check Tabs**:
   - **Console**: JavaScript errors and logs
   - **Network**: HTTP requests and responses
   - **Vue**: Component hierarchy and state
   - **Application**: LocalStorage, IndexedDB, etc.

### Common Debugging Scenarios

#### Scenario 1: API Request Failed

**Check in Network tab:**
1. Find the failed request (red status code)
2. Click the request → Response tab
3. Look at response body for error details
4. Check Status code (400 = bad request, 404 = not found, 500 = server error)

#### Scenario 2: Form Data Not Saving

**Check in Vue DevTools:**
1. Open Vue DevTools (Vue tab)
2. Select the component in the tree
3. Look at "Inspected Component" section
4. Check `form.value` in the State section
5. Verify all required fields have values

#### Scenario 3: Map Not Displaying

**Check in Console:**
```javascript
// Manually test address validation
import { validateAddress } from 'src/services/GoogleValidationApi'

validateAddress({ address: '123 Main St, New York, NY' })
  .then(r => console.log('Success:', r))
  .catch(e => console.error('Error:', e))
```

**Look for:**
- API key errors
- CORS errors
- Network timeouts
- Invalid response format

#### Scenario 4: State Not Updating

**Check Reactivity Rules:**
```javascript
// ❌ WRONG - Object property mutation not reactive
orders.value.items[0].name = 'New Name'

// ✅ CORRECT - Reassign entire object
orders.value = {
  ...orders.value,
  items: orders.value.items.map((item, i) => 
    i === 0 ? { ...item, name: 'New Name' } : item
  )
}

// ✅ OR use splice for arrays
orders.value.items.splice(0, 1, { ...newItem })
```

### Logging Best Practices

```javascript
// Development logging
console.log('Component mounted:', { orders: orders.value })
console.error('API Error:', error)
console.warn('Deprecated method used')

// Conditional logging
if (process.env.NODE_ENV === 'development') {
  console.debug('Detailed debug info:', state)
}

// Remove before production
// Use proper logging library in real apps
```

---

## Performance Tips

### 1. Optimize Computed Properties

```javascript
// ❌ INEFFICIENT - Recalculates on every render
const sum = computed(() => {
  console.log('Recalculating...') // Logs on every dependency change!
  return orders.value.reduce((acc, o) => acc + o.total, 0)
})

// ✅ EFFICIENT - Only recalculates when orders changes
const sum = computed(() => {
  return orders.value.reduce((acc, o) => acc + o.total, 0)
})
```

### 2. Lazy Load Images and Data

```javascript
// Lazy load routes
const OrderFormPage = () => import('pages/OrderFormPage.vue')

// Pagination instead of loading everything
const getOrders = ({ page = 1, pageSize = 50 }) => {
  return fetchOrders({ page, pageSize })
}
```

### 3. Avoid Unnecessary Watches

```javascript
// ❌ INEFFICIENT - Fires on any form change
watch(
  () => form.value,
  () => validateAddress(),
  { deep: true }
)

// ✅ EFFICIENT - Only validate when address fields change
watch(
  () => [form.value.shipAddress, form.value.shipCity],
  () => clearValidatedAddress()
)
```

### 4. Use v-show for Frequent Toggling

```javascript
// ❌ LESS EFFICIENT - Component unmounted/remounted
<div v-if="showDetails">Details</div>

// ✅ MORE EFFICIENT - Just CSS display toggle
<div v-show="showDetails">Details</div>
```

---

## Testing Strategies

### Automated Unit Tests
- Use Vitest for component-level checks in `src/pages/__tests__/`
- Mock Quasar composables and API services so tests stay isolated from the backend
- Focus on derived state, validation gates, and URL generation logic
- Run a single spec with `npm run test:unit -- src/pages/__tests__/OrderFormPage.spec.js`

### Manual Testing Checklist

#### Order Form Testing
- [ ] Create new order with all required fields
- [ ] Update existing order
- [ ] Delete order
- [ ] Validate address (success and fallback cases)
- [ ] Add/remove line items
- [ ] Verify order total calculation
- [ ] Test form validation (disable save with invalid data)
- [ ] Run Vitest coverage for order validation and map URL generation

#### Navigation Testing
- [ ] Navigate to order from shipments page
- [ ] Check form pre-populates correctly
- [ ] Test back navigation
- [ ] Test invalid routes (404)

#### API Testing
- [ ] Verify backend is running before frontend
- [ ] Test with network disabled (error handling)
- [ ] Test with slow network (loading states)
- [ ] Check API response structure matches expected

---

## Code Review Checklist

When reviewing code changes:

- [ ] No console errors or warnings
- [ ] Error handling present for all async operations
- [ ] Loading states shown to user
- [ ] API responses validated before use
- [ ] Form validation before submission
- [ ] No hardcoded values or credentials
- [ ] Component structure follows Vue 3 Composition API
- [ ] Computed properties used instead of methods for derived data
- [ ] Proper TypeScript types (if using TypeScript)
- [ ] Accessibility considerations (labels, ARIA, etc.)

---

## Resources for Learning

- **Vue 3 Composition API**: https://vuejs.org/guide/extras/composition-api-faq.html
- **Quasar Components**: https://quasar.dev/vue-components/
- **Async/Await**: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Statements/async_function
- **Fetch API**: https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API
- **Vue Router**: https://router.vuejs.org/

---

**Version**: 1.0.0  
**Last Updated**: May 8, 2026
