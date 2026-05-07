<template>
  <q-page class="bg-grey-2 q-pa-lg page-shell">
    <div class="row q-col-gutter-lg">
      <div class="col-12">
        <q-card class="hero-card text-white">
          <q-card-section class="hero-content">
            <div>
              <div class="text-overline text-blue-2">Backend connection test</div>
              <div class="text-h4 text-weight-bold">Northwind Traders API</div>
              <div class="text-body1 q-mt-sm hero-copy">
                This screen calls <span class="text-weight-bold">/api/orders</span> through the Quasar dev proxy.
              </div>
            </div>

            <div class="hero-actions">
              <q-badge :color="statusColor" class="q-px-md q-py-sm text-weight-medium">
                {{ statusLabel }}
              </q-badge>
              <q-btn color="white" text-color="primary" icon="refresh" label="Reload" :loading="loading" @click="loadOrders" />
            </div>
          </q-card-section>
        </q-card>
      </div>

      <div class="col-12 col-md-3">
        <q-card class="metric-card">
          <q-card-section>
            <div class="text-caption text-grey-7">Connection</div>
            <div class="text-h5 text-weight-bold">{{ statusLabel }}</div>
          </q-card-section>
        </q-card>
      </div>

      <div class="col-12 col-md-3">
        <q-card class="metric-card">
          <q-card-section>
            <div class="text-caption text-grey-7">Loaded rows</div>
            <div class="text-h5 text-weight-bold">{{ orders.length }}</div>
          </q-card-section>
        </q-card>
      </div>

      <div class="col-12 col-md-3">
        <q-card class="metric-card">
          <q-card-section>
            <div class="text-caption text-grey-7">Total count</div>
            <div class="text-h5 text-weight-bold">{{ totalCount }}</div>
          </q-card-section>
        </q-card>
      </div>

      <div class="col-12 col-md-3">
        <q-card class="metric-card">
          <q-card-section>
            <div class="text-caption text-grey-7">Backend URL</div>
            <div class="text-body2 text-weight-medium">http://localhost:5128</div>
          </q-card-section>
        </q-card>
      </div>

      <div class="col-12">
        <q-card class="table-card">
          <q-card-section class="row items-center justify-between q-gutter-md">
            <div>
              <div class="text-h6 text-weight-bold">Orders endpoint</div>
              <div class="text-caption text-grey-7">GET /api/orders?page=1&pageSize=10</div>
            </div>

            <q-btn flat icon="open_in_new" label="Open Swagger" type="a" href="http://localhost:5128/swagger" target="_blank" />
          </q-card-section>

          <q-separator />

          <q-card-section>
            <q-banner v-if="errorMessage" class="bg-red-1 text-red-9 q-mb-md" rounded>
              {{ errorMessage }}
            </q-banner>

            <q-table
              flat
              bordered
              :rows="orders"
              :columns="columns"
              row-key="orderId"
              :loading="loading"
              no-data-label="No orders loaded yet"
            />
          </q-card-section>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { getOrders } from 'src/services/ordersApi'

const orders = ref([])
const totalCount = ref(0)
const loading = ref(false)
const errorMessage = ref('')

const columns = [
  { name: 'orderId', label: 'Order ID', field: 'orderId', align: 'left', sortable: true },
  { name: 'shipName', label: 'Ship Name', field: 'shipName', align: 'left' },
  { name: 'shipCountry', label: 'Ship Country', field: 'shipCountry', align: 'left' },
  { name: 'orderDate', label: 'Order Date', field: 'orderDate', align: 'left', format: formatDate },
  { name: 'freight', label: 'Freight', field: 'freight', align: 'right', format: formatCurrency },
]

const statusLabel = computed(() => {
  if (loading.value) return 'Connecting'
  if (errorMessage.value) return 'Offline'
  return orders.value.length ? 'Connected' : 'Ready'
})

const statusColor = computed(() => {
  if (loading.value) return 'amber'
  if (errorMessage.value) return 'negative'
  return orders.value.length ? 'positive' : 'grey-7'
})

async function loadOrders() {
  loading.value = true
  errorMessage.value = ''

  try {
    const response = await getOrders({ page: 1, pageSize: 10 })
    orders.value = response.items ?? []
    totalCount.value = response.totalCount ?? orders.value.length
  } catch (error) {
    orders.value = []
    totalCount.value = 0
    errorMessage.value = error instanceof Error ? error.message : 'No se pudo conectar con el servidor'
  } finally {
    loading.value = false
  }
}

function formatDate(value) {
  if (!value) return '-'

  return new Intl.DateTimeFormat('es-ES', {
    dateStyle: 'medium',
  }).format(new Date(value))
}

function formatCurrency(value) {
  if (value == null) return '-'

  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
  }).format(value)
}

onMounted(loadOrders)
</script>

<style scoped>
.page-shell {
  min-height: 100vh;
  background:
    radial-gradient(circle at top left, rgba(25, 118, 210, 0.14), transparent 34%),
    linear-gradient(180deg, #f7f9fc 0%, #edf2f7 100%);
}

.hero-card {
  border-radius: 24px;
  background: linear-gradient(135deg, #0f2747 0%, #144a7b 50%, #1d72b8 100%);
  box-shadow: 0 18px 45px rgba(15, 39, 71, 0.18);
}

.hero-content {
  min-height: 190px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 24px;
}

.hero-copy {
  max-width: 720px;
  opacity: 0.92;
}

.hero-actions {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
}

.metric-card,
.table-card {
  border-radius: 20px;
  box-shadow: 0 10px 26px rgba(15, 39, 71, 0.08);
}

@media (max-width: 700px) {
  .hero-content {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>