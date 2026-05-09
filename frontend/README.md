# Northwind Traders Frontend Application

## Overview

**Northwind Traders App** is a comprehensive Vue 3 + Quasar Framework web application designed to manage orders, shipments, inventory, and analytics for a trading company. The application provides a modern, responsive UI with real-time data visualization and seamless integration with a .NET API backend.

### Key Features

- 📦 **Order Management**: Create, read, update, and delete orders with detailed line items
- 🚚 **Shipments Tracking**: Visualize order fulfillment trends and shipment data
- 📊 **Inventory Management**: Browse and manage product inventory
- 📈 **Analytics Dashboard**: Real-time metrics and visual charts
- 🗺️ **Address Validation**: Google Maps integration for address validation and geographic visualization
- 🔄 **API Integration**: Seamless backend communication via REST endpoints
- 📱 **Responsive Design**: Fully responsive UI with mobile support
- 🎨 **Modern UI**: Built with Quasar components and Material Design

---

## Project Structure

```
frontend/
├── quasar-project/
│   ├── src/
│   │   ├── App.vue                    # Root Vue component
│   │   ├── pages/                     # Page components
│   │   │   ├── IndexPage.vue          # Home/Dashboard
│   │   │   ├── OrderFormPage.vue      # Order management
│   │   │   ├── ShipmentsPage.vue      # Shipment tracking
│   │   │   ├── InventoryPage.vue      # Product inventory
│   │   │   ├── AnalyticsPage.vue      # Analytics dashboard
│   │   │   ├── FleetPage.vue          # Fleet tracking (placeholder)
│   │   │   └── ErrorNotFound.vue      # 404 error page
│   │   ├── components/                # Reusable Vue components
│   │   │   └── EssentialMenu.vue      # Navigation menu
│   │   ├── layouts/                   # Layout wrappers
│   │   │   └── MainLayout.vue         # Main application layout
│   │   ├── services/                  # API service modules
│   │   │   ├── ordersApi.js           # Orders API endpoints
│   │   │   ├── CustomersApi.js        # Customers API endpoints
│   │   │   ├── ProductsApi.js         # Products API endpoints
│   │   │   └── GoogleValidationApi.js # Google Maps API integration
│   │   ├── router/                    # Vue Router configuration
│   │   │   ├── index.js               # Router initialization
│   │   │   └── routes.js              # Route definitions
│   │   ├── css/                       # Global stylesheets
│   │   ├── assets/                    # Static assets (images, etc.)
│   │   └── boot/                      # Application boot files
│   ├── quasar.config.js              # Quasar framework configuration
│   ├── vite.config.js                # Vite build configuration
│   ├── package.json                  # Project dependencies
│   ├── .env                          # Environment variables
│   └── eslint.config.js              # ESLint configuration
├── README.md                          # This file
└── pnpm-workspace.yaml               # PNPM workspace configuration
```

---

## Installation & Setup

### Prerequisites

- **Node.js**: v22.12 or higher (v24, v26, or v28 recommended)
- **Package Manager**: npm (v6.13.4+), yarn (v1.21.1+), or pnpm (v10.0.0+)
- **Git**: For version control

### Quick Start

1. **Navigate to the frontend directory**
   ```bash
   cd frontend/quasar-project
   ```

2. **Install dependencies**
   ```bash
   npm install
   # or
   pnpm install
   # or
   yarn install
   ```

3. **Configure environment variables**
   
   Create a `.env` file in the `frontend/quasar-project` directory:
   ```env
   VITE_GOOGLE_API_KEY=your_google_maps_api_key_here
   ```

   **Getting a Google API Key:**
   - Go to [Google Cloud Console](https://console.cloud.google.com/)
   - Create a new project
   - Enable these APIs:
     - Maps JavaScript API
     - Geocoding API
     - Address Validation API
   - Create an API key (Restrict to Browser applications)
   - Add your development URL (e.g., `localhost:8080`) to the API key restrictions

4. **Start the development server**
   ```bash
   npm run dev
   ```

   The application will open at `http://localhost:8080` in your browser.

---

## Available Commands

All commands are run from the root directory of the project using a package manager:

| Command        | Action                                                  |
| :------------- | :------------------------------------------------------ |
| `npm install`  | Install dependencies                                    |
| `npm run dev`  | Start the Quasar development server                     |
| `npm run build`| Build for production                                    |
| `npm run lint` | Run ESLint code linter                                  |
| `npm run format` | Format code with Prettier                             |
| `npm test`     | Run Vitest unit tests                                   |
| `npm run test:unit` | Run a focused Vitest unit test file                |

---

## Pages & Routes

### 1. **Home / Dashboard** (`/`)
- **Path**: IndexPage.vue
- **Description**: Backend connection test page showing API connectivity status
- **Features**:
  - Display connection status to backend
  - Show loaded orders count and total count
  - Test endpoint: GET `/api/orders?page=1&pageSize=10`
  - Quick access to Swagger API documentation

### 2. **Orders** (`/orders`)
- **Path**: OrderFormPage.vue
- **Description**: Comprehensive order management interface
- **Features**:
  - Create new orders with complete order details
  - Update existing orders (when navigated from Shipments page)
  - Add/remove order line items (products)
  - Address validation with Google Maps integration
  - Map preview showing validated shipping addresses
  - Real-time address geocoding with fallback to text-based search
  - Order validation (customer, employee, products required)
  - Total order calculation including freight charges
  - Save, delete, and refresh order functionality

**Form Fields:**
- Order ID (auto-generated for new orders)
- Customer (required dropdown)
- Employee (required dropdown)
- Order Date, Required Date, Shipped Date (date inputs)
- Ship Via, Freight
- Shipping Address (with address line, city, region, postal code, country)
- Order Details (line items with product, quantity, unit price, discount)

### 3. **Shipments** (`/shipments`)
- **Path**: ShipmentsPage.vue
- **Description**: Visual analytics and tracking of shipments
- **Features**:
  - Orders over time chart (by year)
  - Regional shipment overview
  - Interactive navigation to view/edit specific orders
  - Trend analysis and shipment status visualization

### 4. **Inventory** (`/inventory`)
- **Path**: InventoryPage.vue
- **Description**: Product inventory management (placeholder)
- **Features**: Expandable for future product management features

### 5. **Analytics** (`/analytics`)
- **Path**: AnalyticsPage.vue
- **Description**: Comprehensive analytics dashboard
- **Features**:
  - Custom analytics visualizations
  - Business metrics and KPIs
  - Data-driven insights

### 6. **Fleet** (`/fleet`)
- **Path**: FleetPage.vue
- **Description**: Fleet management and tracking (placeholder)
- **Features**: Expandable for future fleet tracking features

### 7. **Error Page** (`*`)
- **Path**: ErrorNotFound.vue
- **Description**: 404 Not Found error page for invalid routes

---

## API Services

The application communicates with the backend through dedicated service modules. All services use a common `requestJson` helper function that handles authentication headers, error handling, and JSON serialization.

### Base Configuration

- **API Base URL**: `/api` (proxied to `http://localhost:5128` during development)
- **Content-Type**: `application/json`
- **Method**: RESTful HTTP

### Service Modules

#### **ordersApi.js**
Manages all order-related API communications.

**Exported Functions:**

- `getOrders({ page = 1, pageSize = 10 })` - Retrieve paginated orders list
- `searchOrders({ orderId, customerName })` - Search orders by ID or customer name
- `createOrder(orderDto)` - Create a new order
- `updateOrder(orderId, orderDto)` - Update an existing order
- `deleteOrder(orderId)` - Delete an order

**Example Usage:**
```javascript
import { getOrders, createOrder, updateOrder, deleteOrder, searchOrders } from 'src/services/ordersApi'

// Fetch orders
const response = await getOrders({ page: 1, pageSize: 400 })

// Search specific order
const found = await searchOrders({ orderId: 10248 })

// Create new order
const newOrder = await createOrder({
  customerId: 'ALFKI',
  employeeId: 1,
  orderDate: '2024-01-15',
  // ... more fields
})

// Update existing order
await updateOrder(10248, updatedOrderData)

// Delete order
await deleteOrder(10248)
```

#### **CustomersApi.js**
Manages customer-related API communications.

**Exported Functions:**

- `getCustomers({ page = 1, pageSize = 10 })` - Retrieve paginated customers list
- `searchCustomers({ name })` - Search customers by name
- `createCustomer(customerDto)` - Create a new customer
- `updateCustomer(customerId, customerDto)` - Update customer

#### **ProductsApi.js**
Manages product-related API communications.

**Exported Functions:**

- `getProducts({ page = 1, pageSize = 20 })` - Retrieve paginated products list
- `searchProducts({ name })` - Search products by name
- `createProduct(productDto)` - Create a new product
- `updateProduct(productId, productDto)` - Update product

#### **GoogleValidationApi.js**
Integrates Google Maps APIs for address validation and geocoding.

**Exported Functions:**

- `validateAddress({ address, regionCode = '', languageCode = 'en' })` - Validate and geocode an address
  - **Features**:
    - Primary: Google Address Validation API (most accurate)
    - Fallback: Google Geocoding API (for unsupported regions)
    - Automatically handles API limitations gracefully
  - **Returns**: Object with `result.address.formattedAddress`, `result.geocode.location` (lat/lng)

- `suggestAddresses({ input, regionCode = '', languageCode = 'en' })` - Get address suggestions while typing

**Example Usage:**
```javascript
import { validateAddress } from 'src/services/GoogleValidationApi'

const result = await validateAddress({
  address: '123 Main St, New York, NY 10001',
  languageCode: 'en'
})

// Result structure:
// {
//   result: {
//     address: {
//       formattedAddress: "123 Main Street, New York, NY 10001, USA",
//       postalAddress: { ... }
//     },
//     geocode: {
//       location: { latitude: 40.1234, longitude: -74.5678 }
//     }
//   }
// }
```

---

## Key Components

### **MainLayout.vue**
The primary layout wrapper for all main pages. Includes:
- Navigation menu (top bar with links to all pages)
- Page content area (router-view)
- Quasar sidebar integration

### **EssentialMenu.vue**
Navigation menu component providing:
- Links to all application pages
- Active route highlighting
- Responsive mobile menu toggle

---

## Core Features in Detail

### Order Management (OrderFormPage.vue)

The Order Form is the most complex component in the application, providing full CRUD operations for orders:

#### **Form Fields & Validation**

**Required Fields:**
- Customer (must select from dropdown)
- Employee (must select from dropdown)
- At least one product line item

**Optional Fields:**
- All date fields (auto-defaults to today for Order Date)
- Shipping information (address, city, region, postal code, country)
- Ship Via, Freight charges

**Automatic Calculations:**
- Order Total: Sum of (quantity × unit price × (1 - discount)) + freight

#### **Address Validation & Mapping**

When the "Validate Address" button is clicked:

1. **Primary Flow** (Address Validation API):
   - Sends full shipping address to Google Address Validation API
   - Returns precise geocoding coordinates
   - Updates form fields with corrected address data

2. **Fallback Flow** (Geocoding API):
   - If Address Validation fails (e.g., unsupported regions like Venezuela)
   - Automatically falls back to Geocoding API
   - Still returns latitude/longitude for map display
   - Handles gracefully with user-friendly notifications

3. **Map Display**:
   - Once validated, embedded Google Maps iframe loads
   - Shows pin at the geocoded coordinates
   - Uses address text search if coordinates unavailable
   - Auto-clears when user modifies address fields

#### **Order Line Items**

- Add multiple products to a single order
- Each line item tracks: Product, Quantity, Unit Price, Discount
- Automatic unit price population when product selected
- Remove line item functionality (must keep at least one)
- Order total updates in real-time

#### **Data Flow & Hydration**

**Creating a New Order:**
1. User navigates to `/orders`
2. Form loads with empty state
3. User fills in details and saves
4. API returns new order with generated orderId

**Editing Existing Order:**
1. User clicks order in Shipments page
2. Router passes `orderId` via query parameter
3. `syncOrderFromRoute()` fetches order from API
4. Form populates with order data
5. User makes changes and saves

**Bug Fixes Applied:**
- Handles orders with null values in optional fields
- Fallback search to API if order not in initial 400-record pagination
- Prevents map clearing when form fields updated by API response

---

## Development Workflow

### Starting Development

```bash
npm run dev
```

This command:
1. Starts the Quasar dev server (typically on port 8080)
2. Launches your default browser
3. Enables hot module replacement (HMR) for instant code updates
4. Sets up API proxy to backend (`/api` → `http://localhost:5128`)

### Code Formatting & Linting

Format all code with Prettier:
```bash
npm run format
```

Run ESLint to check for code quality issues:
```bash
npm run lint
```

### Building for Production

```bash
npm run build
```

Creates optimized production build in `dist/` directory:
- Minified JavaScript and CSS
- Optimized images and assets
- Source maps (optional)
- Ready for deployment

---

## Configuration

### quasar.config.js

Key configurations in the Quasar config file:

```javascript
// CSS imports
css: ['app.scss']

// Framework config
framework: {
  plugins: ['Dialog', 'Notify']  // Quasar plugins
}

// Development server
devServer: {
  proxy: {
    '/api': {
      target: 'http://localhost:5128',  // Backend URL
      changeOrigin: true
    }
  }
}

// Build targets
build: {
  target: {
    browser: 'baseline-widely-available',
    node: 'node22'
  }
}
```

### Environment Variables

Create `.env` file for environment-specific configuration:

```env
# Google Maps API Key (required for address validation)
VITE_GOOGLE_API_KEY=your_key_here

# Backend API URL (optional, defaults to /api proxy)
# VITE_API_BASE_URL=http://localhost:5128
```

---

## State Management & Reactivity

The application uses **Vue 3 Composition API** with `ref()` and `computed()` for reactive state:

### OrderFormPage.vue State Example

```javascript
// Reactive state
const form = ref(buildEmptyForm())
const orders = ref([])
const customers = ref([])
const products = ref([])
const selectedOrderId = ref(null)
const saving = ref(false)
const validatingAddress = ref(false)

// Computed properties (derived state)
const canSaveOrder = computed(() => {
  return Boolean(form.value.customerId) && 
         Boolean(form.value.employeeId) && 
         form.value.orderDetails.some(d => d.productId)
})

const mapEmbedUrl = computed(() => {
  // Dynamically generate Google Maps embed URL
})
```

---

## Error Handling

The application implements comprehensive error handling:

### Error Notification Pattern

```javascript
// Quasar Notify for simple messages
$q.notify({ type: 'positive', message: 'Order saved successfully' })
$q.notify({ type: 'negative', message: 'Failed to save order' })

// Quasar Dialog for detailed errors
$q.dialog({
  title: 'Error',
  message: 'Detailed error message here',
  ok: { label: 'OK', color: 'negative' }
})
```

### Common Error Scenarios

1. **Address Validation Fails** → Fallback to Geocoding
2. **Order Update Fails** → Display error dialog with API message
3. **Network Error** → Show "Connection failed" notification
4. **Invalid Form Data** → Disable Save button, show warnings

---

## Performance Optimization

### Implemented Optimizations

1. **Lazy Loading**: Pages are lazy-loaded via dynamic imports
2. **Pagination**: Orders loaded with pagination (400 per page)
3. **Memoization**: Computed properties cache expensive calculations
4. **API Fallbacks**: Automatic fallback to Geocoding API avoids user frustration
5. **Debouncing**: Address field watchers don't clear map on every keystroke

### Best Practices

- Use `computed()` instead of methods for derived state
- Implement pagination for large datasets
- Batch API calls where possible with `Promise.all()`
- Use conditional rendering to reduce DOM nodes

---

## Browser Support

- **Modern Browsers**: Chrome, Firefox, Safari, Edge (latest versions)
- **Minimum**: ES2020 (configured in `baseline-widely-available` target)
- **Mobile**: Full iOS Safari and Chrome Android support

---

## Troubleshooting

### Issue: Map not displaying after address validation

**Solution:**
- Ensure `VITE_GOOGLE_API_KEY` is set correctly
- Check browser console for JavaScript errors
- Verify Google Maps API is enabled in Google Cloud Console
- Check API key restrictions and quotas

### Issue: Orders not loading from backend

**Solution:**
- Verify backend is running on `http://localhost:5128`
- Check network tab in browser DevTools
- Ensure API endpoints are correctly implemented
- Check CORS settings on backend

### Issue: "Unsupported region code" error

**Solution:**
- Application automatically handles this with Geocoding API fallback
- If still failing, ensure Google API key has Geocoding API enabled
- Check firewall/proxy settings aren't blocking external APIs

### Issue: ESLint errors during development

**Solution:**
```bash
npm run format  # Auto-fix formatting issues
npm run lint    # Show remaining issues
```

---

## API Response Formats

### Orders Response

```javascript
{
  items: [
    {
      orderId: 10248,
      customerId: "VINET",
      employeeId: 5,
      orderDate: "2024-01-15T00:00:00",
      requiredDate: "2024-02-12T00:00:00",
      shippedDate: null,
      shipVia: 3,
      freight: 32.38,
      shipName: "Vins et alcools Chevalier",
      shipAddress: "59 rue de l'Abbaye",
      shipCity: "Reims",
      shipRegion: null,
      shipPostalCode: "51100",
      shipCountry: "France",
      orderDetails: [
        {
          productId: 11,
          quantity: 12,
          unitPrice: 14.00,
          discount: 0
        }
      ]
    }
  ],
  totalCount: 830,
  pageCount: 3
}
```

### Google Maps Validation Response

```javascript
{
  result: {
    address: {
      formattedAddress: "123 Main Street, New York, NY 10001, USA",
      postalAddress: {
        addressLines: ["123 Main Street"],
        locality: "New York",
        administrativeArea: "NY",
        postalCode: "10001",
        regionCode: "US"
      }
    },
    geocode: {
      location: {
        latitude: 40.7128,
        longitude: -74.0060
      }
    },
    verdict: {
      possibleNextAction: "VALIDATED"
    }
  }
}
```

---

## Contributing

When working on this project:

1. **Code Style**: Follow ESLint rules (run `npm run lint`)
2. **Formatting**: Use Prettier (run `npm run format`)
3. **Vue 3 Best Practices**: Use Composition API
4. **Component Structure**: One component per file, organized by feature
5. **Naming Conventions**:
   - Components: PascalCase (PageName.vue)
   - Functions/Methods: camelCase (functionName)
   - Constants: UPPER_SNAKE_CASE (API_BASE_URL)
   - Computed props: camelCase (computedValue)

---

## Support & Resources

- **Quasar Documentation**: https://quasar.dev
- **Vue 3 Documentation**: https://vuejs.org
- **Vue Router Documentation**: https://router.vuejs.org
- **Google Maps API**: https://developers.google.com/maps
- **Vite Documentation**: https://vitejs.dev

---

## License

This project is part of the RSM Trainee Program Final Assignment. All rights reserved.

---

**Last Updated**: May 8, 2026  
**Version**: 1.0.0  
**Maintainer**: Denis Guerra (DENISGUERRA777)
