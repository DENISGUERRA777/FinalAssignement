# QA & Testing Guide

## Quick Start for QA

### Prerequisites
- Node.js v22+ installed
- .NET API running on `http://localhost:5128`
- Google Maps API key configured
- Modern web browser (Chrome, Firefox, Safari, Edge)

### Starting the Application

```bash
cd frontend/quasar-project
npm install
npm run dev
```

The app will open at `http://localhost:8080`

---

## Automated Frontend Tests

The frontend includes a Vitest setup for component-level checks.

### Run the current test file

```bash
cd frontend/quasar-project
npm run test:unit -- src/pages/__tests__/OrderFormPage.spec.js
```

### Current coverage
- Order form validation state for customer, employee, and product selection
- Google Maps embed URL generation for validated addresses and text-search fallback

---

## Test Cases by Feature

## 1. Order Management

### Test 1.1: Create New Order
**Objective**: Verify ability to create a complete new order

**Steps**:
1. Navigate to `/orders`
2. Verify form is empty (new order state)
3. Select a customer from dropdown
4. Select an employee from dropdown
5. Add a product line item
6. Enter quantity and unit price
7. Click "Save" button

**Expected Result**:
- ✓ Success dialog appears with new order ID
- ✓ Form resets or shows newly created order
- ✓ Order appears in Shipments page shipments list
- ✓ Order ID is auto-generated (not 0 or null)

**Data Validation**:
- Customer field: Required, must be from dropdown
- Employee field: Required, must be from dropdown  
- Product line: At least one product required
- Order Date: Defaults to today's date
- Frequencies: Can be saved with nullable fields (dates, shipping info optional)

---

### Test 1.2: Edit Existing Order
**Objective**: Verify ability to update an existing order

**Steps**:
1. Navigate to `/shipments`
2. Click navigation arrow on any order row
3. Verify OrderFormPage loads with order data populated
4. Modify one field (e.g., freight amount)
5. Click "Save" button

**Expected Result**:
- ✓ Order data loads into form (URL query param `?orderId=XXXX`)
- ✓ All fields are correctly pre-filled
- ✓ Modify one field (e.g., change freight from 10 to 20)
- ✓ Success dialog confirms update
- ✓ ShipmentsPage reflects the change when navigating back

**Edge Cases to Test**:
- Order with null required date → Form shows empty field (not error)
- Order with null shipVia → Form shows empty select
- Order with null freight → Form shows 0
- Order ID in URL that doesn't exist → Form shows empty (new order)

---

### Test 1.3: Delete Order
**Objective**: Verify order deletion functionality

**Steps**:
1. Load an existing order (via Shipments page navigation)
2. Click "Delete" button
3. Confirm deletion in the browser's confirmation dialog
4. Wait for success message

**Expected Result**:
- ✓ Confirmation dialog appears
- ✓ After confirmation, success message shown
- ✓ Order no longer appears in Shipments list
- ✓ Form resets to empty state

**Error Scenario**:
- Click Delete and cancel → Order remains unchanged

---

### Test 1.4: Order Validation Rules
**Objective**: Verify form validation prevents invalid submissions

**Test Cases**:

| Field | Condition | Expected Behavior |
|-------|-----------|-------------------|
| Customer | Not selected | "Save" button disabled, warning: "Customer is required" |
| Employee | Not selected | "Save" button disabled, warning: "Employee is required" |
| Product Lines | No products | "Save" button disabled, warning: "Add at least one product" |
| Multiple Products | Valid | Calculate total: sum(qty × price × (1 - discount)) |
| Quantity | 0 or negative | Auto-correct to 1 |
| Freight | Non-numeric | Safe conversion to 0 |

---

### Test 1.5: Order Line Items
**Objective**: Verify line item management

**Steps**:
1. Create new order with 1 product line
2. Click "Add Product" button
3. Select different products
4. Enter different quantities and prices
5. Try to remove all line items (should fail with warning)
6. Save the order

**Expected Result**:
- ✓ Can add multiple line items
- ✓ Line item counter shows correct count
- ✓ Total order amount updates automatically
- ✓ Cannot remove last line item (keep at least 1)
- ✓ Each line item can be removed individually
- ✓ Auto-fill unit price from product master

---

### Test 1.6: Order Total Calculation
**Objective**: Verify correct calculation of order totals

**Scenario**:
```
Product 1: Qty 2, Price 10.00, Discount 0.1 = 2 × 10 × 0.9 = 18.00
Product 2: Qty 3, Price 5.00, Discount 0.0 = 3 × 5 × 1.0 = 15.00
Freight: 2.50
Total: 18 + 15 + 2.50 = 35.50
```

**Expected Result**:
- ✓ Calculation displays correctly
- ✓ Discounts applied correctly (percentage model)
- ✓ Freight added to total
- ✓ Real-time update as values change

---

## 2. Address Validation & Mapping

### Test 2.1: Valid Address Validation
**Objective**: Verify address validation for supported regions

**Steps**:
1. Navigate to `/orders`
2. Fill in shipping address: "123 Main Street, New York, NY 10001"
3. Click "Validate Address" button
4. Wait for response

**Expected Result**:
- ✓ Success message: "Address validated successfully"
- ✓ Formatted address displays (e.g., "123 Main Street, New York, NY 10001, USA")
- ✓ Latitude field shows valid decimal (e.g., 40.753583)
- ✓ Longitude field shows valid decimal (e.g., -73.984090)
- ✓ Google Map embed appears showing the location pin

**Validation Fields Updated**:
- Formatted Address: Full validated address
- Latitude: Precise coordinate
- Longitude: Precise coordinate
- Verdict: "VALIDATED" or similar

---

### Test 2.2: Unsupported Region Fallback
**Objective**: Verify graceful handling of unsupported regions

**Steps**:
1. Navigate to `/orders`
2. Fill in Venezuelan address: "Avenida Principal, Caracas, Venezuela"
3. Enter "Venezuela" or "VE" in the Country field
4. Click "Validate Address" button
5. Wait for response

**Expected Result**:
- ✓ **Initial validation fails** (400 Unsupported region code)
- ✓ **Automatic fallback to Geocoding API** (transparent to user)
- ✓ Success message displayed
- ✓ Approximate latitude/longitude returned
- ✓ Google Map shows location (text-based search, not precise pin)

**User Experience**:
- No error shown to user
- Warning message: "Address validation API does not support region"
- Map still renders with best-effort geocoding

---

### Test 2.3: Map Display
**Objective**: Verify Google Maps embed functionality

**Steps**:
1. Validate any address (supported or unsupported)
2. Observe map preview below address fields
3. Verify map shows correct location
4. Modify an address field (e.g., change city)
5. Verify map clears (resets to empty state)

**Expected Result**:
- ✓ Map loads after successful validation
- ✓ Map shows correct pin at validated coordinates
- ✓ Map zoom level appropriate (around 15x for coordinates)
- ✓ Map resets when user modifies address fields
- ✓ Map shows full address via text search if coordinates unavailable

---

### Test 2.4: Address Field Auto-fill
**Objective**: Verify that API response updates form fields

**Steps**:
1. Enter intentionally misspelled address: "123 Main Sreet, New York"
2. Click "Validate Address"
3. Observe form fields

**Expected Result**:
- ✓ Address corrected to "123 Main Street"
- ✓ City field auto-populated from response
- ✓ Postal code from response fills in
- ✓ All fields update without losing other data

---

## 3. API Integration Tests

### Test 3.1: Backend Connectivity
**Objective**: Verify frontend can communicate with backend

**Steps**:
1. Navigate to `/` (home page)
2. Observe "Backend connection test" card
3. Check the status badge
4. Click "Reload" button

**Expected Result**:
- ✓ Status badge shows "Connected" or "Success"
- ✓ Orders loaded count > 0
- ✓ Total orders count displayed
- ✓ Orders table shows data from backend
- ✓ "Reload" button refreshes data

**Error Scenario**:
- If backend not running:
  - ✓ Status badge shows "Disconnected"
  - ✓ Error message in red banner
  - ✓ Zero loaded orders

---

### Test 3.2: API Pagina​tion
**Objective**: Verify correct handling of paginated responses

**Expected Behavior**:
- Initial load: `page=1, pageSize=400` → 400 orders loaded
- Orders list: All 400 cached and available
- Search: Can find orders beyond pagination (fallback API call)

**Test**:
1. Load `/shipments`
2. Look for an order with ID > 10400 (beyond first 400)
3. Click navigation arrow to open that order
4. Verify order loads correctly

**Expected Result**:
- ✓ Order loads even though beyond initial pagination
- ✓ No "Order not found" error
- ✓ Form populates with fetched data

---

### Test 3.3: Error Responses
**Objective**: Verify graceful error handling

**Test Cases**:

| Scenario | Expected Behavior |
|----------|-------------------|
| Backend returns 400 (Bad Request) | Show error dialog with status code |
| Backend returns 404 (Not Found) | Handle gracefully, show appropriate message |
| Backend returns 500 (Server Error) | Show error notification to user |
| Network timeout | Show "Connection failed" message |
| Malformed JSON response | Show "Invalid response format" |

---

## 4. Navigation & Routing

### Test 4.1: Navigation Menu
**Objective**: Verify all navigation links work correctly

**Links to Test**:
- [ ] Home (/) → IndexPage loads
- [ ] Orders (/orders) → OrderFormPage loads
- [ ] Shipments (/shipments) → ShipmentsPage loads
- [ ] Inventory (/inventory) → InventoryPage loads
- [ ] Analytics (/analytics) → AnalyticsPage loads
- [ ] Fleet (/fleet) → FleetPage loads

**Expected Result**:
- ✓ All links navigate to correct pages
- ✓ URL updates correctly
- ✓ Back browser button works
- ✓ Forward browser button works

---

### Test 4.2: Deeplink Navigation (Orders via Query Param)
**Objective**: Verify direct linking to specific orders

**Steps**:
1. Copy URL from Shipments page when clicking order: `/orders?orderId=10248`
2. Paste URL directly in browser address bar and press Enter
3. Verify OrderFormPage loads with order data

**Expected Result**:
- ✓ Form loads with order pre-populated
- ✓ Order ID, customer, products all display
- ✓ No "Order not found" error
- ✓ Works even on fresh page load (no cache needed)

---

### Test 4.3: Invalid Routes
**Objective**: Verify 404 error page for invalid routes

**Steps**:
1. Navigate to `/invalid-page`
2. Navigate to `/orders/edit/12345` (non-existent route)
3. Navigate to `/admin` (not implemented)

**Expected Result**:
- ✓ ErrorNotFound.vue displays
- ✓ Quasar logo and "Go to home" button shown
- ✓ Button navigation to home works

---

## 5. Data Validation & Edge Cases

### Test 5.1: Null/Empty Field Handling
**Objective**: Verify form handles optional null fields

**Test Data**: Load an order with these characteristics:
```javascript
{
  orderId: 10248,
  customerId: "ALFKI",
  employeeId: 2,
  shippedDate: null,           // Optional date
  shipVia: null,               // Optional number
  requiredDate: null,          // Optional date
  shipRegion: null,            // Optional text
  shipPostalCode: ""           // Empty string
}
```

**Expected Result**:
- ✓ Null fields show empty (not "null" or "undefined" strings)
- ✓ Empty strings display as empty inputs
- ✓ Null numbers display as 0
- ✓ Null dates display as empty date input
- ✓ Form can be saved without filling optional fields

---

### Test 5.2: Special Characters in Text Fields
**Objective**: Verify form handles special characters safely

**Test Data**:
- Customer Name: "Empresa S.A. & Co."
- Address: "123 O'Reilly St., (Suite #5)"
- City: "São Paulo"

**Expected Result**:
- ✓ Special characters saved correctly
- ✓ No encoding errors
- ✓ Display correctly on form reload
- ✓ API communication works with special chars

---

### Test 5.3: Large Decimal Values
**Objective**: Verify numeric precision

**Test Data**:
```
Quantity: 999999
Unit Price: 99999.99
Discount: 0.99
Freight: 10000.50
```

**Expected Result**:
- ✓ All values saved correctly
- ✓ Total calculated accurately
- ✓ No precision loss or rounding errors
- ✓ Database stores correct values

---

### Test 5.4: Very Long Text Fields
**Objective**: Verify text field length limits

**Test Data**:
- Address: 500+ character string
- City: 100+ character string

**Expected Result**:
- ✓ Fields accept long text
- ✓ No truncation on display
- ✓ Saved to backend correctly
- ✓ Retrieval shows full text

---

## 6. Performance & Stress Tests

### Test 6.1: Form with Many Line Items
**Objective**: Verify performance with large number of products

**Steps**:
1. Create order with 50+ product line items
2. Add more items (click "Add" repeatedly)
3. Modify quantities and prices
4. Try to save

**Expected Result**:
- ✓ Form remains responsive (no lag/freezing)
- ✓ Total calculates quickly
- ✓ Save completes without timeout
- ✓ No console errors

---

### Test 6.2: Rapid API Requests
**Objective**: Verify handling of rapid successive requests

**Steps**:
1. In Orders page, rapidly click "Validate Address" button 5 times
2. Rapidly click "Save" button 3 times
3. Quickly navigate to different pages

**Expected Result**:
- ✓ Only latest request processed
- ✓ No duplicate orders created
- ✓ Loading states show correctly
- ✓ Error messages clear and relevant
- ✓ No race conditions

---

### Test 6.3: Large Dataset Handling
**Objective**: Verify performance with 400 loaded orders

**Steps**:
1. Navigate to `/shipments`
2. Observe chart rendering with all orders
3. Try to scroll through orders list
4. Search for specific order

**Expected Result**:
- ✓ Chart renders smoothly
- ✓ No UI freezing
- ✓ Scrolling is smooth
- ✓ Search completes quickly

---

## 7. Browser & Device Compatibility

### Desktop Browsers
- [ ] Chrome (latest) - Full functionality
- [ ] Firefox (latest) - Full functionality
- [ ] Safari (latest) - Full functionality
- [ ] Edge (latest) - Full functionality

### Mobile/Tablet
- [ ] iPhone Safari - Responsive, all features work
- [ ] Android Chrome - Responsive, all features work
- [ ] iPad Safari - Responsive, all features work
- [ ] Android tablet - Responsive, all features work

### Test on Mobile:
1. Touch all buttons → Should respond
2. Scroll forms → Smooth scrolling
3. Validate address → Mobile keyboard appears/disappears
4. Map displays → Responsive embed

---

## 8. Accessibility Tests

### Keyboard Navigation
- [ ] Tab key navigates through form fields in logical order
- [ ] Enter key submits forms (where appropriate)
- [ ] Escape key closes dialogs
- [ ] All buttons accessible via keyboard

### Screen Readers
- [ ] Form labels associated with inputs
- [ ] Buttons have descriptive text
- [ ] Error messages announced
- [ ] Table headers semantic
- [ ] Icons have alt text

### Visual Accessibility
- [ ] Sufficient color contrast (WCAG AA minimum)
- [ ] Text is readable (not too small)
- [ ] No color-only information conveyance
- [ ] Focus indicators visible

---

## 9. User Acceptance Testing (UAT)

### UAT Scenario 1: Order Processing Workflow
**Objective**: End-to-end order lifecycle

**Workflow**:
1. Manager creates new order for customer ALFKI
2. Adds 3 product line items
3. Validates shipping address (New York)
4. Reviews auto-calculated total
5. Saves order
6. Verifies order appears in Shipments
7. Later updates order to mark as shipped
8. Verifies changes persist

**Success Criteria**:
- ✓ Order created successfully
- ✓ Address validates and maps
- ✓ Total calculation correct
- ✓ Changes persist after reload
- ✓ No data loss or errors

### UAT Scenario 2: International Order
**Objective**: Order with non-US address

**Workflow**:
1. Create order for European customer
2. Enter Spanish address: "Paseo de la Castellana 123, Madrid, Spain"
3. Validate address
4. Save order
5. Update and verify persistence

**Success Criteria**:
- ✓ Address accepts international format
- ✓ Validation either succeeds or gracefully degrades
- ✓ Map shows location
- ✓ Data saves correctly
- ✓ Can edit order later

---

## Bug Report Template

When submitting bugs, use this template:

```markdown
**Title**: [Component] Brief description

**Severity**: 
- [ ] Critical (app crash, data loss)
- [ ] High (feature broken)
- [ ] Medium (feature degraded)
- [ ] Low (cosmetic, UX issue)

**Steps to Reproduce**:
1. ...
2. ...
3. ...

**Expected Result**:
...

**Actual Result**:
...

**Screenshots/Attachments**:
[Include if visual issue]

**Environment**:
- Browser: [e.g., Chrome 120]
- OS: [e.g., Windows 11]
- Backend Status: Running / Not Running
- Error Console: [Any JavaScript errors?]

**Additional Context**:
...
```

---

**Testing Version**: 1.0.0  
**Last Updated**: May 8, 2026
