# 🔌 NorthwindTradersApp - API Endpoints Documentation

## Base URL
```
http://localhost:5000/api
```

---

## 📑 Tabla de Contenidos

1. [Health Check](#health-check)
2. [Orders Endpoints](#orders-endpoints)
3. [Products Endpoints](#products-endpoints)
4. [Customers Endpoints](#customers-endpoints)
5. [Response Formats](#response-formats)
6. [Error Handling](#error-handling)

---

## Health Check

### **GET /health**
Verifica que la API esté activa y funcional.

**Response:**
```
200 OK
```

---

## Orders Endpoints

### **1. GET /api/orders**
Obtiene lista de todos los pedidos con paginación.

**Query Parameters:**
```
pageNumber: int (default: 1) - Número de página
pageSize: int (default: 20) - Cantidad de registros por página
```

**Example Request:**
```http
GET /api/orders?pageNumber=1&pageSize=20
```

**Response (200 OK):**
```json
{
  "data": [
    {
      "orderId": 10248,
      "customerId": "VINET",
      "employeeId": 5,
      "orderDate": "2023-07-04T00:00:00",
      "freight": 32.38,
      "shipVia": 3,
      "orderDetails": [
        {
          "orderId": 10248,
          "productId": 11,
          "unitPrice": 14.00,
          "quantity": 12,
          "discount": 0.0
        },
        {
          "orderId": 10248,
          "productId": 42,
          "unitPrice": 9.80,
          "quantity": 10,
          "discount": 0.0
        }
      ]
    }
  ],
  "totalCount": 830,
  "pageNumber": 1,
  "pageSize": 20
}
```

---

### **2. GET /api/orders/search**
Busca pedidos por ID de pedido y/o nombre de cliente.

**Query Parameters:**
```
orderId: int (optional) - ID del pedido a buscar
customerName: string (optional) - Nombre (completo o parcial) del cliente
pageNumber: int (default: 1) - Número de página
pageSize: int (default: 20) - Cantidad de registros por página
```

**Example Requests:**
```http
GET /api/orders/search?orderId=10248
GET /api/orders/search?customerName=VINET
GET /api/orders/search?orderId=10248&customerName=VINET
```

**Response (200 OK):**
```json
{
  "data": [
    {
      "orderId": 10248,
      "customerId": "VINET",
      "employeeId": 5,
      "orderDate": "2023-07-04T00:00:00",
      "freight": 32.38,
      "shipVia": 3,
      "orderDetails": [...]
    }
  ],
  "totalCount": 1,
  "pageNumber": 1,
  "pageSize": 20
}
```

**Response (404 Not Found):**
```json
{
  "error": "Order 99999 not found"
}
```

---

### **3. POST /api/orders**
Crea un nuevo pedido con detalles de líneas.

**Request Body:**
```json
{
  "customerId": "ALFKI",
  "employeeId": 1,
  "orderDate": "2024-05-08T10:30:00",
  "freight": 50.00,
  "shipVia": 1,
  "orderDetails": [
    {
      "productId": 1,
      "unitPrice": 18.00,
      "quantity": 10,
      "discount": 0.0
    },
    {
      "productId": 2,
      "unitPrice": 19.00,
      "quantity": 5,
      "discount": 0.1
    }
  ]
}
```

**Response (201 Created):**
```json
{
  "orderId": 11078,
  "customerId": "ALFKI",
  "employeeId": 1,
  "orderDate": "2024-05-08T10:30:00",
  "freight": 50.00,
  "shipVia": 1,
  "orderDetails": [
    {
      "orderId": 11078,
      "productId": 1,
      "unitPrice": 18.00,
      "quantity": 10,
      "discount": 0.0
    },
    {
      "orderId": 11078,
      "productId": 2,
      "unitPrice": 19.00,
      "quantity": 5,
      "discount": 0.1
    }
  ]
}
```

**Headers:**
```
Location: /api/orders/11078
```

---

### **4. PUT /api/orders/{orderId}**
Actualiza un pedido existente y sus detalles.

**Path Parameter:**
```
orderId: int - ID del pedido a actualizar
```

**Request Body:**
```json
{
  "customerId": "ALFKI",
  "employeeId": 2,
  "orderDate": "2024-05-08T14:00:00",
  "freight": 60.00,
  "shipVia": 2,
  "orderDetails": [
    {
      "productId": 1,
      "unitPrice": 18.00,
      "quantity": 20,
      "discount": 0.05
    }
  ]
}
```

**Response (200 OK):**
```json
{
  "orderId": 10248,
  "customerId": "ALFKI",
  "employeeId": 2,
  "orderDate": "2024-05-08T14:00:00",
  "freight": 60.00,
  "shipVia": 2,
  "orderDetails": [
    {
      "orderId": 10248,
      "productId": 1,
      "unitPrice": 18.00,
      "quantity": 20,
      "discount": 0.05
    }
  ]
}
```

**Response (404 Not Found):**
```json
{
  "error": "Order 99999 not found"
}
```

---

### **5. DELETE /api/orders/{orderId}**
Elimina un pedido y todos sus detalles asociados.

**Path Parameter:**
```
orderId: int - ID del pedido a eliminar
```

**Example Request:**
```http
DELETE /api/orders/10248
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Order deleted successfully"
}
```

**Response (404 Not Found):**
```json
{
  "error": "Order 99999 not found"
}
```

---

## Products Endpoints

### **1. GET /api/products**
Obtiene lista de todos los productos con paginación.

**Query Parameters:**
```
pageNumber: int (default: 1) - Número de página
pageSize: int (default: 20) - Cantidad de registros por página
```

**Example Request:**
```http
GET /api/products?pageNumber=1&pageSize=20
```

**Response (200 OK):**
```json
{
  "data": [
    {
      "productId": 1,
      "productName": "Chai",
      "unitPrice": 18.00,
      "unitsInStock": 39,
      "categoryId": 1,
      "categoryName": "Beverages",
      "supplierId": 1
    },
    {
      "productId": 2,
      "productName": "Chang",
      "unitPrice": 19.00,
      "unitsInStock": 17,
      "categoryId": 1,
      "categoryName": "Beverages",
      "supplierId": 1
    }
  ],
  "totalCount": 77,
  "pageNumber": 1,
  "pageSize": 20
}
```

---

### **2. GET /api/products/search**
Busca productos por nombre.

**Query Parameters:**
```
productName: string - Nombre (completo o parcial) del producto
pageNumber: int (default: 1) - Número de página
pageSize: int (default: 20) - Cantidad de registros por página
```

**Example Request:**
```http
GET /api/products/search?productName=chai&pageNumber=1&pageSize=10
```

**Response (200 OK):**
```json
{
  "data": [
    {
      "productId": 1,
      "productName": "Chai",
      "unitPrice": 18.00,
      "unitsInStock": 39,
      "categoryId": 1,
      "categoryName": "Beverages",
      "supplierId": 1
    }
  ],
  "totalCount": 1,
  "pageNumber": 1,
  "pageSize": 10
}
```

---

### **3. POST /api/products**
Crea un nuevo producto.

**Request Body:**
```json
{
  "productName": "New Product",
  "unitPrice": 25.50,
  "unitsInStock": 100,
  "categoryId": 1,
  "supplierId": 1
}
```

**Response (201 Created):**
```json
{
  "productId": 78,
  "productName": "New Product",
  "unitPrice": 25.50,
  "unitsInStock": 100,
  "categoryId": 1,
  "categoryName": "Beverages",
  "supplierId": 1
}
```

**Headers:**
```
Location: /api/products/78
```

---

### **4. PUT /api/products/{productId}**
Actualiza un producto existente.

**Path Parameter:**
```
productId: int - ID del producto a actualizar
```

**Request Body:**
```json
{
  "productName": "Updated Product Name",
  "unitPrice": 30.00,
  "unitsInStock": 150,
  "categoryId": 2,
  "supplierId": 2
}
```

**Response (200 OK):**
```json
{
  "productId": 1,
  "productName": "Updated Product Name",
  "unitPrice": 30.00,
  "unitsInStock": 150,
  "categoryId": 2,
  "categoryName": "Condiments",
  "supplierId": 2
}
```

---

### **5. DELETE /api/products/{productId}**
Elimina un producto.

**Path Parameter:**
```
productId: int - ID del producto a eliminar
```

**Example Request:**
```http
DELETE /api/products/78
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Product deleted successfully"
}
```

---

## Customers Endpoints

### **1. GET /api/customers**
Obtiene lista de todos los clientes con paginación.

**Query Parameters:**
```
pageNumber: int (default: 1) - Número de página
pageSize: int (default: 20) - Cantidad de registros por página
```

**Example Request:**
```http
GET /api/customers?pageNumber=1&pageSize=20
```

**Response (200 OK):**
```json
{
  "data": [
    {
      "customerId": "ALFKI",
      "companyName": "Alfreds Futterkiste",
      "contactName": "Maria Anders",
      "contactTitle": "Sales Representative",
      "address": "Obere Str. 57",
      "city": "Berlin",
      "region": null,
      "postalCode": "12209",
      "country": "Germany",
      "phone": "030-0074321",
      "fax": "030-0076545"
    }
  ],
  "totalCount": 91,
  "pageNumber": 1,
  "pageSize": 20
}
```

---

### **2. GET /api/customers/search**
Busca clientes por nombre de compañía o contacto.

**Query Parameters:**
```
customerName: string - Nombre de compañía o contacto (parcial)
pageNumber: int (default: 1) - Número de página
pageSize: int (default: 20) - Cantidad de registros por página
```

**Example Request:**
```http
GET /api/customers/search?customerName=Alfreds&pageNumber=1&pageSize=10
```

**Response (200 OK):**
```json
{
  "data": [
    {
      "customerId": "ALFKI",
      "companyName": "Alfreds Futterkiste",
      "contactName": "Maria Anders",
      "contactTitle": "Sales Representative",
      "address": "Obere Str. 57",
      "city": "Berlin",
      "region": null,
      "postalCode": "12209",
      "country": "Germany",
      "phone": "030-0074321",
      "fax": "030-0076545"
    }
  ],
  "totalCount": 1,
  "pageNumber": 1,
  "pageSize": 10
}
```

---

### **3. POST /api/customers**
Crea un nuevo cliente.

**Request Body:**
```json
{
  "customerId": "NEWCO",
  "companyName": "New Company Ltd",
  "contactName": "John Doe",
  "contactTitle": "Manager",
  "address": "123 Main Street",
  "city": "New York",
  "region": "NY",
  "postalCode": "10001",
  "country": "USA",
  "phone": "212-555-1234",
  "fax": "212-555-5678"
}
```

**Response (201 Created):**
```json
{
  "customerId": "NEWCO",
  "companyName": "New Company Ltd",
  "contactName": "John Doe",
  "contactTitle": "Manager",
  "address": "123 Main Street",
  "city": "New York",
  "region": "NY",
  "postalCode": "10001",
  "country": "USA",
  "phone": "212-555-1234",
  "fax": "212-555-5678"
}
```

**Headers:**
```
Location: /api/customers/NEWCO
```

---

### **4. PUT /api/customers/{customerId}**
Actualiza un cliente existente.

**Path Parameter:**
```
customerId: string - ID del cliente a actualizar
```

**Request Body:**
```json
{
  "companyName": "Updated Company Name",
  "contactName": "Jane Smith",
  "contactTitle": "Director",
  "address": "456 Oak Avenue",
  "city": "Los Angeles",
  "region": "CA",
  "postalCode": "90001",
  "country": "USA",
  "phone": "213-555-9999",
  "fax": "213-555-8888"
}
```

**Response (200 OK):**
```json
{
  "customerId": "ALFKI",
  "companyName": "Updated Company Name",
  "contactName": "Jane Smith",
  "contactTitle": "Director",
  "address": "456 Oak Avenue",
  "city": "Los Angeles",
  "region": "CA",
  "postalCode": "90001",
  "country": "USA",
  "phone": "213-555-9999",
  "fax": "213-555-8888"
}
```

---

### **5. DELETE /api/customers/{customerId}**
Elimina un cliente.

**Path Parameter:**
```
customerId: string - ID del cliente a eliminar
```

**Example Request:**
```http
DELETE /api/customers/NEWCO
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Customer deleted successfully"
}
```

---

## Response Formats

### **Success Response (200 OK)**
```json
{
  "data": [...],
  "totalCount": 830,
  "pageNumber": 1,
  "pageSize": 20
}
```

### **Created Response (201 Created)**
```json
{
  "orderId": 11078,
  "customerId": "ALFKI",
  ...
}
```

### **Deleted Response (200 OK)**
```json
{
  "success": true,
  "message": "Order deleted successfully"
}
```

---

## Error Handling

### **404 Not Found**
```json
{
  "error": "Order 99999 not found",
  "timestamp": "2024-05-08T15:30:00Z",
  "statusCode": 404
}
```

### **400 Bad Request**
```json
{
  "error": "Invalid input parameters",
  "details": {
    "pageNumber": "Page number must be greater than 0",
    "productName": "Product name is required"
  },
  "timestamp": "2024-05-08T15:30:00Z",
  "statusCode": 400
}
```

### **500 Internal Server Error**
```json
{
  "error": "An unexpected error occurred",
  "timestamp": "2024-05-08T15:30:00Z",
  "statusCode": 500
}
```

---

## Paginación

Todos los endpoints GET que retornan colecciones soportan paginación con estos parámetros:

| Parámetro | Tipo | Por Defecto | Descripción |
|-----------|------|-------------|-------------|
| `pageNumber` | int | 1 | Número de página (comienza en 1) |
| `pageSize` | int | 20 | Cantidad de registros por página |

**Ejemplo:**
```http
GET /api/orders?pageNumber=2&pageSize=50
```

---

## Rate Limiting

- **No hay limite de rate** configurado actualmente
- Se recomienda implementar rate limiting en producción

---

## Autenticación

- **No requiere autenticación** actualmente
- Se recomienda implementar Bearer Token o API Key en producción

---

## CORS

- **CORS habilitado** para desarrollo local
- Configuración en `Program.cs`

---

## Swagger UI

Accede a la documentación interactiva en:
```
http://localhost:5000/swagger/index.html
```
