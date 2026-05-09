<template>
  <!-- Top summary cards: trend placeholder and regional shipment overview -->
  <div class="row q-col-gutter-sm q-mb-sm">
    <!-- Orders over time panel with year selector and API-backed summary -->
    <div class="col-12 col-md-8">
      <q-card flat bordered class="full-height">
        <q-card-section class="q-pa-sm">
          <div class="row items-center q-mb-sm">
            <div class="text-subtitle1 text-weight-medium col">Orders Over Time</div>
            <q-select
              dense
              outlined
              v-model="year"
              :options="yearOptions"
              style="width: 120px"
            />
          </div>
          <div class="chart-shell q-pa-sm">
            <div class="row items-center justify-between q-mb-xs">
              <div>
                <div class="text-caption chart-label">Orders vs shipped orders</div>
                <div class="text-body2 text-weight-medium">Year {{ year }}</div>
              </div>
              <div class="row items-center q-gutter-sm chart-legend">
                <div class="row items-center q-gutter-xs">
                  <span class="legend-swatch legend-primary"></span>
                  <span>Orders</span>
                </div>
                <div class="row items-center q-gutter-xs">
                  <span class="legend-swatch legend-secondary"></span>
                  <span>Shipped</span>
                </div>
              </div>
            </div>

            <div class="chart-frame">
              <svg :viewBox="`0 0 ${chartWidth} ${chartHeight}`" class="chart-svg" preserveAspectRatio="none" aria-label="Orders over time line chart" role="img">
                <defs>
                  <linearGradient id="ordersFill" x1="0" x2="0" y1="0" y2="1">
                    <stop offset="0%" stop-color="rgba(25, 118, 210, 0.28)" />
                    <stop offset="100%" stop-color="rgba(25, 118, 210, 0)" />
                  </linearGradient>
                  <linearGradient id="shippedFill" x1="0" x2="0" y1="0" y2="1">
                    <stop offset="0%" stop-color="rgba(156, 39, 176, 0.22)" />
                    <stop offset="100%" stop-color="rgba(156, 39, 176, 0)" />
                  </linearGradient>
                </defs>

                <g v-for="gridLine in chartGridLines" :key="gridLine" class="chart-grid">
                  <line x1="48" :y1="gridLine" x2="700" :y2="gridLine" />
                </g>

                <path :d="ordersAreaPath" fill="url(#ordersFill)" />
                <path :d="shippedAreaPath" fill="url(#shippedFill)" />

                <path
                  :d="ordersLinePath"
                  class="chart-line chart-line-orders"
                  fill="none"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                />
                <path
                  :d="shippedLinePath"
                  class="chart-line chart-line-shipped"
                  fill="none"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-dasharray="6 6"
                />

                <g v-for="point in chartPoints" :key="point.month" class="chart-point-group">
                  <circle :cx="point.x" :cy="point.ordersY" r="3.5" class="chart-point chart-point-orders" />
                  <circle :cx="point.x" :cy="point.shippedY" r="3.5" class="chart-point chart-point-shipped" />
                </g>

                <g v-for="label in chartAxisLabels" :key="label.month" class="chart-axis-label">
                  <text :x="label.x" y="242" text-anchor="middle">{{ label.month }}</text>
                </g>
              </svg>
            </div>

            <div class="row q-col-gutter-sm q-mt-xs chart-summary">
              <div class="col-12 col-md-4">
                <div class="chart-metric">
                  <div class="text-caption">Total orders</div>
                  <div class="text-h6 text-weight-bold">{{ yearTotals.orders }}</div>
                </div>
              </div>
              <div class="col-12 col-md-4">
                <div class="chart-metric">
                  <div class="text-caption">Shipped orders</div>
                  <div class="text-h6 text-weight-bold">{{ yearTotals.shipped }}</div>
                </div>
              </div>
              <div class="col-12 col-md-4">
                <div class="chart-metric">
                  <div class="text-caption">Delayed orders</div>
                  <div class="text-h6 text-weight-bold">{{ yearTotals.delayed }}</div>
                </div>
              </div>
            </div>
          </div>
        </q-card-section>
      </q-card>
    </div>

    <!-- Shipment distribution panel built from the loaded orders -->
    <div class="col-12 col-md-4">
      <q-card flat bordered class="full-height">
        <q-card-section class="q-pa-sm">
          <div class="text-subtitle1 text-weight-medium q-mb-sm">Shipments by Region</div>
          <div style="height: 160px" class="flex flex-center">
            <div class="pie-shell">
              <svg viewBox="0 0 160 160" width="120" height="120" aria-hidden="true">
                <g transform="translate(80,80)">
                  <template v-for="r in regionsData" :key="r.name">
                    <path :d="r.path" :fill="r.color" stroke="#0f172a" stroke-width="0.5" />
                  </template>
                  <circle r="36" fill="white" />
                </g>
              </svg>
              
            </div>
          </div>
          <q-list dense class="q-mt-sm">
            <q-item v-for="region in regions" :key="region.name">
              <q-item-section avatar>
                <q-badge rounded class="region-swatch" :style="{ backgroundColor: region.color }" />
              </q-item-section>
              <q-item-section>{{ region.name }}</q-item-section>
              <q-item-section side class="text-caption">{{ region.percent }}%</q-item-section>
            </q-item>
          </q-list>
        </q-card-section>
      </q-card>
    </div>
  </div>

  <!-- Detailed orders table with filters and row formatting -->
  <q-card flat bordered>
    <q-table
      :rows="rows"
      :columns="columns"
      row-key="orderId"
      flat
      :loading="loading"
    >
      <!-- Table header actions: filter toggle and quick actions -->
      <template v-slot:top>
        <div class="text-h6">Order Details</div>
        <q-space />
        <q-btn-toggle
          v-model="filter"
          flat color="grey-6"
          toggle-color="primary"
          :options="[{label: 'All Orders', value: 'all'}, {label: 'Delayed', value: 'delay'}]"
        />
        <q-btn-toggle
          v-model="timeRange"
          flat color="grey-6"
          toggle-color="primary"
          class="q-ml-md"
          :options="[
            { label: 'This Month', value: 'thisMonth' },
            { label: 'This Year', value: 'thisYear' },
            { label: 'All Time', value: 'allTime' }
          ]"
        />
        <q-btn-dropdown outline icon="file_download" label="Export" class="q-ml-sm">
          <q-list dense>
            <q-item clickable v-close-popup @click="handleExport('excel')">
              <q-item-section>Excel</q-item-section>
            </q-item>
            <q-item clickable v-close-popup @click="handleExport('pdf')">
              <q-item-section>PDF</q-item-section>
            </q-item>
          </q-list>
        </q-btn-dropdown>
      </template>

      <!-- Customer column with initials badge and full customer name -->
      <template v-slot:body-cell-customer="props">
        <q-td :props="props">
          <div class="row items-center">
            <q-avatar size="sm" color="blue-1" text-color="blue-9" class="q-mr-sm">
              {{ getCustomerInitials(props.row.customer) }}
            </q-avatar>
            {{ props.row.customer }}
          </div>
        </q-td>
      </template>

      <!-- Products column showing the order details returned by the API -->
      <template v-slot:body-cell-products="props">
        <q-td :props="props">
          <div class="column q-gutter-xs">
            <div class="text-weight-medium">{{ props.row.productsSummary }}</div>
            <div class="row q-gutter-xs">
              <q-badge
                v-for="product in props.row.productBadges"
                :key="product"
                outline
                color="primary"
                text-color="primary"
              >
                {{ product }}
              </q-badge>
            </div>
          </div>
        </q-td>
      </template>

      <!-- Region column derived from the shipping information in each order -->
      <template v-slot:body-cell-region="props">
        <q-td :props="props">
          <q-badge outline color="teal" text-color="teal">
            {{ props.value }}
          </q-badge>
        </q-td>
      </template>

      <!-- Totals column calculated from the order detail lines -->
      <template v-slot:body-cell-totals="props">
        <q-td :props="props" class="text-right">
          <div class="text-weight-bold">{{ formatCurrency(safeNumber(props.row.totals)) }}</div>
          <div class="text-caption text-grey-7">{{ props.row.productCount }} items</div>
        </q-td>
      </template>

      <!-- Status column with color-coded badges based on shipment state -->
      <template v-slot:body-cell-status="props">
        <q-td :props="props">
          <q-badge
            :color="props.value === 'DELIVERED' ? 'green-1' : props.value === 'DELAYED' ? 'orange-1' : 'grey-3'"
            :text-color="props.value === 'DELIVERED' ? 'green-9' : props.value === 'DELAYED' ? 'orange-9' : 'grey-9'"
            class="text-weight-bold q-pa-sm"
          >
            {{ props.value }}
          </q-badge>
        </q-td>
      </template>

      <!-- Row action column with a navigation arrow to the future details page -->
      <template v-slot:body-cell-details="props">
        <q-td :props="props" class="text-right">
          <q-btn
            flat
            round
            dense
            icon="chevron_right"
            color="primary"
            @click="goToOrderDetails(props.row.orderId)"
          />
        </q-td>
      </template>
    </q-table>
  </q-card>

</template>

<script setup>
// Load orders from the API and derive the displayed shipment metrics from them.
import { computed, onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { getOrders } from 'src/services/ordersApi'
import * as XLSX from 'xlsx'
import { jsPDF } from 'jspdf'
import { autoTable } from 'jspdf-autotable'

// Reactive state for the loaded orders and page filters.
const orders = ref([])
const loading = ref(false)
const filter = ref('all')
const year = ref(String(new Date().getFullYear()))
const timeRange = ref('allTime')
const router = useRouter()

// Table columns shown in the detailed orders grid.
const columns = [
  { name: 'customer', label: 'Customer', field: 'customer', align: 'left', sortable: true },
  { name: 'products', label: 'Products', field: 'productsSummary', align: 'left' },
  { name: 'region', label: 'Region', field: 'region', align: 'left', sortable: true },
  { name: 'orderDate', label: 'Order Date', field: 'orderDate', align: 'left', sortable: true, format: formatDate },
  { name: 'status', label: 'Status', field: 'status', align: 'left', sortable: true },
  { name: 'totals', label: 'Totals', field: 'totals', align: 'right', sortable: true, format: formatCurrency },
  { name: 'details', label: '', field: 'details', align: 'right' },
]

// Available years are inferred from the loaded order dates.
const yearOptions = computed(() => {
  const years = new Set(
    orders.value
      .map((order) => getYear(order.orderDate))
      .filter(Boolean),
  )

  if (!years.size) {
    years.add(String(new Date().getFullYear()))
  }

  return Array.from(years).sort((a, b) => Number(b) - Number(a))
})

// Normalize API orders into the shape needed by the table and summary widgets.
const normalizedRows = computed(() => {
  return orders.value.map((order) => ({
    orderId: order.orderId,
    customer: order.shipName || order.customerId || 'Unknown',
    orderDate: order.orderDate,
    status: getShipmentStatus(order),
    productsSummary: getProductsSummary(order.orderDetails),
    productBadges: getProductBadges(order.orderDetails),
    productCount: Array.isArray(order.orderDetails) ? order.orderDetails.length : 0,
    region: order.shipRegion || order.shipCountry || 'Unknown',
    totals: calculateOrderTotal(order.orderDetails, order.freight),
    shipCountry: order.shipCountry,
    shipRegion: order.shipRegion,
    shippedDate: order.shippedDate,
  }))
})

// Apply the selected year and filter toggle to the visible rows.
const filteredOrders = computed(() => {
  return normalizedRows.value.filter((order) => {
    const matchesFilter = filter.value === 'all' || (filter.value === 'delay' && order.status === 'DELAYED')

    if (timeRange.value === 'thisMonth') {
      if (!order.orderDate) return false
      const d = new Date(order.orderDate)
      const now = new Date()
      const sameMonth = d.getMonth() === now.getMonth() && d.getFullYear() === now.getFullYear()

      return sameMonth && matchesFilter
    }

    if (timeRange.value === 'thisYear') {
      if (!order.orderDate) return false
      const d = new Date(order.orderDate)
      const now = new Date()
      const sameYear = d.getFullYear() === now.getFullYear()

      return sameYear && matchesFilter
    }

    return matchesFilter
  })
})

// Rows rendered by the table.
const rows = computed(() => filteredOrders.value)


// Aggregate monthly counts to draw the time-series chart.
const chartData = computed(() => {
  const monthLabels = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
  const buckets = monthLabels.map((month) => ({ month, orders: 0, shipped: 0 }))

  for (const order of normalizedRows.value) {
    if (getYear(order.orderDate) !== year.value) {
      continue
    }

    const monthIndex = new Date(order.orderDate).getMonth()
    const bucket = buckets[monthIndex]

    if (!bucket) {
      continue
    }

    bucket.orders += 1

    if (order.status === 'DELIVERED') {
      bucket.shipped += 1
    }
  }

  return buckets
})

// The chart is rendered in a fixed SVG grid so the page can stay dependency-free.
const chartWidth = 720
const chartHeight = 260
const chartPadding = 48
const chartInnerWidth = chartWidth - chartPadding * 2
const chartMaxValue = computed(() => Math.max(1, ...chartData.value.flatMap((item) => [item.orders, item.shipped])))

const chartPoints = computed(() => {
  return chartData.value.map((item, index) => {
    const x = chartPadding + (chartInnerWidth / 11) * index
    const ordersY = scaleChartValue(item.orders)
    const shippedY = scaleChartValue(item.shipped)

    return {
      month: item.month,
      x,
      ordersY,
      shippedY,
    }
  })
})

const chartAxisLabels = computed(() => {
  return chartPoints.value.map((point) => ({
    month: point.month,
    x: point.x,
  }))
})

const chartGridLines = computed(() => {
  return [40, 88, 136, 184, 232]
})

const ordersLinePath = computed(() => buildLinePath(chartPoints.value.map((point) => ({ x: point.x, y: point.ordersY }))))
const shippedLinePath = computed(() => buildLinePath(chartPoints.value.map((point) => ({ x: point.x, y: point.shippedY }))))
const ordersAreaPath = computed(() => buildAreaPath(chartPoints.value.map((point) => ({ x: point.x, y: point.ordersY }))))
const shippedAreaPath = computed(() => buildAreaPath(chartPoints.value.map((point) => ({ x: point.x, y: point.shippedY }))))

const yearTotals = computed(() => ({
  orders: chartData.value.reduce((sum, item) => sum + item.orders, 0),
  shipped: chartData.value.reduce((sum, item) => sum + item.shipped, 0),
  delayed: rows.value.filter((order) => order.status === 'DELAYED').length,
}))

// Use app's primary/secondary/accent colors to stay consistent with global theme
const regionPalette = ['#1976d2', '#26a69a', '#9c27b0', '#f2c037', '#fb7185', '#94a3b8']

// Build the region buckets once and reuse them for the pie chart and legend.
const regionEntries = computed(() => {
  const sourceRows = rows.value.length ? rows.value : normalizedRows.value
  const counts = new Map()

  for (const order of sourceRows) {
    const regionName = order.shipRegion || order.shipCountry || 'Unknown'
    counts.set(regionName, (counts.get(regionName) ?? 0) + 1)
  }

  const sortedEntries = Array.from(counts.entries()).sort((a, b) => b[1] - a[1])
  const topEntries = sortedEntries.slice(0, 5)
  const restCount = sortedEntries.slice(5).reduce((sum, [, count]) => sum + count, 0)

  if (restCount > 0) {
    topEntries.push(['Others', restCount])
  }

  return topEntries
})

const regions = computed(() => {
  const total = regionEntries.value.reduce((sum, [, count]) => sum + count, 0)

  return regionEntries.value.map(([name, count], index) => ({
    name,
    percent: total ? Math.round((count / total) * 100) : 0,
    color: regionPalette[index % regionPalette.length],
  }))
})

// Build region data (with paths) for the pie chart and make sure the full 100% is represented.
const regionsData = computed(() => {
  const total = regionEntries.value.reduce((sum, [, count]) => sum + count, 0) || 1

  let startAngle = -90

  return regionEntries.value.map(([name, count], index) => {
    const angle = (count / total) * 360
    const endAngle = startAngle + angle

    const path = describeArcPath(0, 0, 64, startAngle, endAngle)
    startAngle = endAngle

    return {
      name,
      count,
      percent: Math.round((count / total) * 100),
      color: regionPalette[index % regionPalette.length],
      path,
    }
  })
})

// Keep the selected year valid when the available dataset changes.
watch(yearOptions, (options) => {
  if (!options.includes(year.value)) {
    year.value = options[0]
  }
}, { immediate: true })

function polarToCartesian(cx, cy, radius, angleInDegrees) {
  const angleInRadians = (angleInDegrees - 90) * Math.PI / 180.0
  return {
    x: cx + radius * Math.cos(angleInRadians),
    y: cy + radius * Math.sin(angleInRadians),
  }
}

function describeArcPath(cx, cy, radius, startAngle, endAngle) {
  const start = polarToCartesian(cx, cy, radius, endAngle)
  const end = polarToCartesian(cx, cy, radius, startAngle)
  const largeArcFlag = endAngle - startAngle <= 180 ? '0' : '1'

  return [
    `M ${cx} ${cy}`,
    `L ${start.x.toFixed(3)} ${start.y.toFixed(3)}`,
    `A ${radius} ${radius} 0 ${largeArcFlag} 0 ${end.x.toFixed(3)} ${end.y.toFixed(3)}`,
    'Z',
  ].join(' ')
}

// Fetch the orders once and populate the page state.
async function loadShipments() {
  loading.value = true

  try {
    const response = await getOrders({ page: 1, pageSize: 1000 })
    orders.value = Array.isArray(response?.items) ? response.items : []
  } finally {
    loading.value = false
  }
}

// Derive a shipment status from order dates so the UI can stay data-driven.
function getShipmentStatus(order) {
  if (!order.shippedDate) {
    return 'PENDING'
  }

  if (order.requiredDate && new Date(order.shippedDate) > new Date(order.requiredDate)) {
    return 'DELAYED'
  }

  return 'DELIVERED'
}

// Extract the year from an ISO date string.
function getYear(value) {
  if (!value) {
    return ''
  }

  return new Date(value).getFullYear().toString()
}

// Generate initials for the avatar shown in the customer column.
function getCustomerInitials(value) {
  if (!value) {
    return '--'
  }

  return value
    .split(/\s+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0]?.toUpperCase() ?? '')
    .join('')
    .slice(0, 2)
}

// Build a short readable products label from the order detail lines.
function getProductsSummary(orderDetails) {
  if (!Array.isArray(orderDetails) || !orderDetails.length) {
    return 'No products'
  }

  return `${orderDetails.length} product${orderDetails.length === 1 ? '' : 's'}`
}

// Keep the product badges compact so the table stays readable.
function getProductBadges(orderDetails) {
  if (!Array.isArray(orderDetails) || !orderDetails.length) {
    return []
  }

  return orderDetails.slice(0, 3).map((item) => `#${item.productId}`)
}

// Calculate the order total from the detail lines when available.
function calculateOrderTotal(orderDetails, fallbackValue = 0) {
  if (!Array.isArray(orderDetails) || !orderDetails.length) {
    return safeNumber(fallbackValue)
  }

  const total = orderDetails.reduce((sum, item) => {
    const lineTotal = safeNumber(item.unitPrice) * safeNumber(item.quantity) * (1 - safeNumber(item.discount))
    return sum + lineTotal
  }, 0)

  return safeNumber(total)
}

// Convert any incoming value into a safe numeric value.
function safeNumber(value) {
  const numericValue = Number(value)
  return Number.isFinite(numericValue) ? numericValue : 0
}

// Scale a data value into the SVG coordinate system.
function scaleChartValue(value) {
  const chartBottom = chartHeight - 34
  const chartTop = 24
  const availableHeight = chartBottom - chartTop

  return chartBottom - (value / chartMaxValue.value) * availableHeight
}

// Build a smooth-looking line path from a list of points.
function buildLinePath(points) {
  if (!points.length) {
    return ''
  }

  return points
    .map((point, index) => `${index === 0 ? 'M' : 'L'} ${point.x.toFixed(1)} ${point.y.toFixed(1)}`)
    .join(' ')
}

// Build a closed path so the chart can show a subtle filled area.
function buildAreaPath(points) {
  if (!points.length) {
    return ''
  }

  const baseline = chartHeight - 34
  const startPoint = points[0]
  const endPoint = points[points.length - 1]

  return [
    `M ${startPoint.x.toFixed(1)} ${baseline}`,
    `L ${startPoint.x.toFixed(1)} ${startPoint.y.toFixed(1)}`,
    ...points.slice(1).map((point) => `L ${point.x.toFixed(1)} ${point.y.toFixed(1)}`),
    `L ${endPoint.x.toFixed(1)} ${baseline}`,
    'Z',
  ].join(' ')
}

// Format dates for table display.
function formatDate(value) {
  if (!value) return '-'

  return new Intl.DateTimeFormat('en-US', { dateStyle: 'medium' }).format(new Date(value))
}

// Format numeric amounts as currency.
function formatCurrency(value) {
  if (!Number.isFinite(Number(value))) return '-'

  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
  }).format(value)
}

// Navigate to the future order details page for the selected order.
function goToOrderDetails(orderId) {
  router.push({ path: '/orders', query: { orderId } })
}

// Export the currently visible rows to Excel or PDF.
function handleExport(format) {
  const exportRows = rows.value.map((row) => ({
    OrderId: row.orderId,
    Customer: row.customer,
    Products: row.productsSummary,
    ProductBadges: row.productBadges.join(', '),
    Region: row.region,
    OrderDate: formatDate(row.orderDate),
    Status: row.status,
    Totals: safeNumber(row.totals),
    Items: row.productCount,
  }))

  if (format === 'excel') {
    exportToExcel(exportRows)
    return
  }

  if (format === 'pdf') {
    exportToPdf(exportRows)
  }
}

function setHeaderTime(timeRange) {
  if (timeRange === 'thisMonth') {
    const now = new Date()
    return `- ${now.toLocaleString('default', { month: 'long' })} ${now.getFullYear()}`
  }
  if (timeRange === 'thisYear') {
    return `- ${new Date().getFullYear()}`
  }
  return 'All Time'

}
function exportToExcel(exportRows) {
  const tableRows = exportRows.map((row) => ({
    ...row,
    Totals: formatCurrency(row.Totals),
  }))
  const worksheet = XLSX.utils.aoa_to_sheet([
    ['Northwind Trades'],
    [`Shipments Export - ${setHeaderTime(timeRange.value)}`],
    [],
    ['Order ID', 'Customer', 'Products', 'Badges', 'Region', 'Order Date', 'Status', 'Totals', 'Items'],
    ...tableRows.map((row) => [
      row.OrderId,
      row.Customer,
      row.Products,
      row.ProductBadges,
      row.Region,
      row.OrderDate,
      row.Status,
      row.Totals,
      row.Items,
    ]),
  ])
  const workbook = XLSX.utils.book_new()

  worksheet['!merges'] = [
    { s: { c: 0, r: 0 }, e: { c: 8, r: 0 } },
    { s: { c: 0, r: 1 }, e: { c: 8, r: 1 } },
  ]

  XLSX.utils.book_append_sheet(workbook, worksheet, 'Shipments')
  XLSX.writeFile(workbook, `shipments-${year.value}.xlsx`)
}

function exportToPdf(exportRows) {
  const doc = new jsPDF({ orientation: 'landscape', unit: 'pt', format: 'a4' })
  const body = exportRows.map((row) => [
    row.OrderId,
    row.Customer,
    row.Products,
    row.ProductBadges,
    row.Region,
    row.OrderDate,
    row.Status,
    formatCurrency(row.Totals),
    row.Items,
  ])

  doc.setFontSize(18)
  doc.text('Northwind Trades', 40, 34)
  doc.setFontSize(12)
  doc.text(`Shipments Export - ${setHeaderTime(timeRange.value)}`, 40, 54)

  autoTable(doc, {
    startY: 72,
    head: [[
      'Order ID',
      'Customer',
      'Products',
      'Badges',
      'Region',
      'Order Date',
      'Status',
      'Totals',
      'Items',
    ]],
    body,
    styles: {
      fontSize: 8,
      cellPadding: 4,
    },
    headStyles: {
      fillColor: [15, 23, 42],
    },
    alternateRowStyles: {
      fillColor: [248, 250, 252],
    },
  })

  doc.save(`shipments-${year.value}.pdf`)
}

// Toggle the "This Month" filter applied to the table rows.
// Note: timeRange controls table time filtering: 'thisMonth' | 'thisYear' | 'allTime'

onMounted(loadShipments)
</script>

<style scoped>
.chart-shell {
  border-radius: 18px;
  background:
    radial-gradient(circle at top left, rgba(99, 102, 241, 0.18), transparent 42%),
    linear-gradient(180deg, #0f172a 0%, #111827 100%);
  color: #e5eefc;
  overflow: hidden;
}

.chart-label,
.chart-legend,
.chart-summary .text-caption {
  color: rgba(226, 232, 240, 0.72);
}

.chart-frame {
  border-radius: 16px;
  background:
    linear-gradient(180deg, rgba(15, 23, 42, 0.7), rgba(15, 23, 42, 0.42)),
    repeating-linear-gradient(
      to right,
      rgba(148, 163, 184, 0.08) 0,
      rgba(148, 163, 184, 0.08) 1px,
      transparent 1px,
      transparent 90px
    );
  border: 1px solid rgba(148, 163, 184, 0.14);
}

.chart-svg {
  display: block;
  width: 100%;
  height: 190px;
}

.chart-grid line {
  stroke: rgba(148, 163, 184, 0.14);
  stroke-width: 1;
}

.chart-line {
  stroke-width: 3.5;
  filter: drop-shadow(0 0 14px rgba(25, 118, 210, 0.2));
}

.chart-line-orders {
  stroke: #1976d2;
}

.chart-line-shipped {
  stroke: #9c27b0;
  filter: drop-shadow(0 0 14px rgba(156, 39, 176, 0.18));
}

.chart-point {
  stroke-width: 2.5;
}

.chart-point-orders {
  fill: #1976d2;
  stroke: #0f172a;
}

.chart-point-shipped {
  fill: #9c27b0;
  stroke: #0f172a;
}

.chart-axis-label text {
  fill: rgba(226, 232, 240, 0.62);
  font-size: 12px;
}

.legend-swatch {
  display: inline-block;
  width: 12px;
  height: 12px;
  border-radius: 999px;
}

.region-swatch {
  width: 12px;
  height: 12px;
  min-width: 12px;
  min-height: 12px;
  padding: 0;
}

.legend-primary {
  background: #1976d2;
}

.legend-secondary {
  background: #9c27b0;
}

.chart-metric {
  border-radius: 14px;
  background: rgba(15, 23, 42, 0.4);
  border: 1px solid rgba(148, 163, 184, 0.12);
  padding: 12px 14px;
}

.pie-shell {
  position: relative;
  width: 120px;
  height: 120px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.pie-shell svg {
  position: absolute;
  top: 0;
  left: 0;
}

.pie-center {
  position: absolute;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  z-index: 1;
  text-align: center;
}

@media (max-width: 700px) {
  .chart-svg {
    height: 170px;
  }

  .chart-metric {
    padding: 10px 12px;
  }
}
</style>