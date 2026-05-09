# Quick Reference Guide

A quick cheat sheet for developers and testers working with the Northwind Traders Frontend Application.

---

## 🚀 Quick Start (2 minutes)

```bash
# 1. Navigate to frontend
cd frontend/quasar-project

# 2. Install dependencies
npm install

# 3. Set environment variable (.env file)
echo "VITE_GOOGLE_API_KEY=your_key_here" > .env

# 4. Start dev server
npm run dev

# 5. Open browser
# http://localhost:8080
```

---

## 📍 Important Ports & URLs

| Service | URL | Purpose |
|---------|-----|---------|
| Frontend Dev | `http://localhost:8080` | Vue app |
| Backend API | `http://localhost:5128` | .NET Core API |
| API Proxy | `/api` | Frontend → Backend via proxy |
| Backend Swagger | `http://localhost:5128/swagger` | API documentation |

---

## 🗺️ Project Routes

```
/              → Home/Dashboard (API connectivity test)
/orders        → Order Management (CRUD operations)
/shipments     → Shipment Analytics & Tracking
/inventory     → Product Inventory
/analytics     → Business Analytics
/fleet         → Fleet Management (placeholder)
/*             → 404 Error Page
```

---

## 📁 Key Files Quick Lookup

| File | Purpose |
|------|---------|
| `src/pages/OrderFormPage.vue` | Main order management component |
| `src/services/ordersApi.js` | Orders API endpoints |
| `src/router/routes.js` | Route definitions |
| `quasar.config.js` | Framework configuration |
| `.env` | Environment variables (API keys) |
| `package.json` | Dependencies and scripts |

---

## 🛠️ Common Commands

```bash
# Development
npm run dev              # Start dev server with HMR

# Building
npm run build            # Production build

# Code Quality
npm run lint             # Check code with ESLint
npm run format           # Auto-format code with Prettier

# Testing
npm test                 # Run tests (currently none configured)
```

---

## 🔑 Environment Variables

```bash
# .env file in frontend/quasar-project/

# Required
VITE_GOOGLE_API_KEY=sk-xxxxxxxxxxxxx

# Optional
VITE_API_BASE_URL=http://localhost:5128
```

**How to get Google API Key**:
1. Go to https://console.cloud.google.com/
2. Create new project
3. Enable APIs: Maps, Geocoding, Address Validation
4. Create API key under "Credentials"
5. Restrict to Browser + your development URL

---

## 📊 Order Form Field Mapping

```javascript
Order DTO Structure:
{
  orderId: number,           // Auto-generated on create
  customerId: string,        // Required, dropdown
  employeeId: number,        // Required, dropdown
  orderDate: string,         // ISO date, defaults to today
  requiredDate: string | null,
  shippedDate: string | null,
  shipVia: number | null,    // Shipper ID
  freight: number,           // Decimal amount
  shipName: string,          // Shipping name
  shipAddress: string,       // Full address or line 1
  shipCity: string,
  shipRegion: string | null, // State/Province
  shipPostalCode: string,
  shipCountry: string,       // Country code (2 chars)
  orderDetails: [            // Line items
    {
      productId: number,     // Required for each line
      quantity: number,      // Units
      unitPrice: number,     // Price per unit
      discount: number       // Decimal 0.0-1.0 (10% = 0.1)
    }
  ]
}
```

---

## 🔍 Common Debugging

### Issue: API returns 400 "Unsupported region code"
**Solution**: Address validation automatically falls back to Geocoding API
- No user action needed
- Map still displays
- Handled transparently

### Issue: Orders not loading
**Check**:
1. Is backend running? `curl http://localhost:5128/api/orders`
2. Is proxy configured? Check `quasar.config.js`
3. Are there CORS errors? Check browser console

### Issue: Map not showing
**Check**:
1. Is Google API key valid? Check `.env`
2. Is address validation successful? Check Network tab
3. Do coordinates exist? Check browser console: `validatedAddress.value`

### Issue: Form field not updating
**Check**:
1. Is the ref() properly initialized?
2. Is computed() recalculating? Check Vue DevTools
3. Is there a watch() clearing the value?

---

## 🧪 Testing Order Scenarios

### Scenario A: Create Order (Venezuela address)
```
Customer: CERC
Employee: 1
Product 1: ProductID 1, Qty 5, Price 10.00, Disc 0
Address: Avenida Principal, Caracas, Venezuela
Country: VE

Expected: Order saves, map shows via Geocoding fallback
```

### Scenario B: Edit Order with Null Fields
```
Load order with null shipVia, null shippedDate
Modify freight amount
Save

Expected: Null fields saved as null, not overwritten
```

### Scenario C: Large Order with Many Items
```
Add 20+ product line items
Modify quantities
Save

Expected: Calculation correct, no performance issues
```

---

## 🎨 Vue 3 Composition API Reference

```javascript
<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useQuasar } from 'quasar'

const $q = useQuasar()

// State
const form = ref({})
const loading = ref(false)

// Computed (derived state)
const isValid = computed(() => form.value.id > 0)

// Side effects
watch(
  () => form.value.address,
  () => clearValidation()
)

// Lifecycle
onMounted(async () => {
  await loadData()
})

// Methods
async function loadData() {
  loading.value = true
  try {
    // fetch...
  } finally {
    loading.value = false
  }
}
</script>
```

---

## 📡 API Service Pattern

```javascript
// services/exampleApi.js

const API_BASE_URL = '/api'

// Helper
async function requestJson(url, options = {}, errorMessage) {
  const response = await fetch(url, {
    headers: { 'Content-Type': 'application/json', ...options.headers },
    ...options
  })
  
  if (!response.ok) throw new Error(errorMessage)
  return response.json()
}

// Exported functions
export async function getItems() {
  return requestJson(`${API_BASE_URL}/items`)
}

export async function createItem(data) {
  return requestJson(`${API_BASE_URL}/items`, {
    method: 'POST',
    body: JSON.stringify(data)
  })
}
```

**Usage**:
```javascript
import { getItems, createItem } from 'src/services/exampleApi'

const items = await getItems()
const newItem = await createItem(itemData)
```

---

## ⚠️ Common Pitfalls

❌ **DON'T**:
```javascript
// Direct object mutation (not reactive)
order.value.customer = "New Name"

// Forgetting await on async
loadData()  // Missing await!

// Hardcoded API URLs
fetch('http://localhost:5128/api/...')

// No error handling
await saveOrder()  // Can throw!
```

✅ **DO**:
```javascript
// Immutable updates
order.value = { ...order.value, customer: "New Name" }

// Proper async/await
await loadData()

// Use service layer
import { getOrders } from 'src/services/ordersApi'

// Always handle errors
try {
  await saveOrder()
} catch (err) {
  error.value = err.message
}
```

---

## 🔗 Service Endpoints Quick Reference

### Orders
```javascript
getOrders(options)              // GET /api/orders?page=1&pageSize=10
searchOrders(filters)           // GET /api/orders/search?orderId=XX
createOrder(data)               // POST /api/orders
updateOrder(id, data)           // PUT /api/orders/:id
deleteOrder(id)                 // DELETE /api/orders/:id
```

### Customers
```javascript
getCustomers(options)           // GET /api/customers
searchCustomers(name)           // GET /api/customers/search?name=XX
createCustomer(data)            // POST /api/customers
updateCustomer(id, data)        // PUT /api/customers/:id
```

### Products
```javascript
getProducts(options)            // GET /api/products?pageSize=20
searchProducts(name)            // GET /api/products/search?name=XX
createProduct(data)             // POST /api/products
updateProduct(id, data)         // PUT /api/products/:id
```

### Address Validation (Google)
```javascript
validateAddress(input)          // Returns { latitude, longitude, address }
suggestAddresses(input)         // Returns address suggestions
```

---

## 🎯 Feature Checklist

### Order Management ✅
- [x] Create orders with line items
- [x] Edit existing orders
- [x] Delete orders
- [x] Form validation (customer, employee, products required)
- [x] Auto-calculate order total
- [x] Support nullable optional fields
- [x] Pagination (first 400 orders)
- [x] Search orders by ID (beyond pagination)

### Address Validation ✅
- [x] Validate addresses via Google API
- [x] Fallback to Geocoding for unsupported regions
- [x] Display coordinates (latitude, longitude)
- [x] Embed Google Maps
- [x] Auto-populate address fields
- [x] Clear map when user modifies address

### UI/UX ✅
- [x] Responsive design (desktop, tablet, mobile)
- [x] Loading states on buttons and forms
- [x] Error dialogs for failures
- [x] Success notifications
- [x] Form validation feedback
- [x] Keyboard navigation
- [x] Quasar components throughout

### Navigation ✅
- [x] Menu with all pages
- [x] Direct linking to orders via query param (`?orderId=XX`)
- [x] Fallback search if order not in pagination
- [x] 404 error page for invalid routes

---

## 📞 Troubleshooting Decision Tree

```
Issue: Something not working
└─ Frontend or Backend?
   ├─ Frontend
   │  ├─ Open DevTools (F12)
   │  ├─ Check Console for errors
   │  └─ Check Network tab for failed requests
   └─ Backend
      ├─ Is it running? (curl localhost:5128)
      ├─ Check backend logs
      └─ Verify API endpoint exists

Issue: Data not saving
└─ Check Backend API response
   ├─ Is 200 status returned?
   ├─ Check response body
   └─ Verify backend logic

Issue: UI not updating
└─ Vue DevTools
   ├─ Check component state (ref values)
   ├─ Check computed properties
   └─ Verify watchers firing

Issue: Slow performance
└─ Optimize
   ├─ Check for N+1 queries
   ├─ Use pagination
   ├─ Avoid deep watches
   └─ Check Network waterfall chart
```

---

## 📚 Documentation Files

- **README.md** - Overview and setup
- **DEVELOPMENT.md** - Technical deep-dive for developers
- **QA_TESTING.md** - Comprehensive testing guide
- **QUICK_REFERENCE.md** - This file (quick lookup)

---

## 🔗 External Resources

| Resource | URL |
|----------|-----|
| Vue 3 Docs | https://vuejs.org |
| Quasar Docs | https://quasar.dev |
| Vue Router | https://router.vuejs.org |
| Google Maps API | https://developers.google.com/maps |
| Vite | https://vitejs.dev |
| MDN - Fetch API | https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API |

---

## 💡 Pro Tips

1. **Use Vue DevTools** - Install browser extension for component inspection
2. **Check Network Tab** - 80% of issues are API-related
3. **Console.log Strategically** - Log before/after API calls
4. **Read Error Messages** - Backend returns detailed error messages in response
5. **Test in Production Mode** - `npm run build` then serve dist/ locally
6. **Clear Cache** - Hard refresh (Ctrl+Shift+R) for CSS/JS changes
7. **Proxy Debugging** - Open `http://localhost:8080/api/orders` directly in browser to see raw API response

---

## ⏱️ Estimated Task Times

| Task | Time |
|------|------|
| Setup & Install | 5 min |
| First Order Creation | 10 min |
| Address Validation | 15 min |
| Full Feature Test | 45 min |
| Bug Fix (typical) | 20 min |
| Feature Addition | 1-2 hours |

---

**Version**: 1.0.0  
**Last Updated**: May 8, 2026  
**Audience**: Developers, QA Engineers, Technical Leads
