<template>

  <div class="row q-col-gutter-md q-mb-md">
    <!-- Gráfico de Órdenes en el Tiempo -->
    <div class="col-12 col-md-8">
      <q-card flat bordered class="full-height">
        <q-card-section>
          <div class="row items-center q-mb-md">
            <div class="text-h6 col">Orders Over Time</div>
            <q-select dense outlined v-model="year" :options="['2024', '2023']" style="width: 100px" />
          </div>
          <!-- Simulación de gráfico de barras -->
          <div style="height: 250px" class="bg-grey-1 flex flex-center border-dashed">
            [Gráfico de Barras ApexCharts]
          </div>
        </q-card-section>
      </q-card>
    </div>

    <!-- Gráfico de Envíos por Región -->
    <div class="col-12 col-md-4">
      <q-card flat bordered class="full-height">
        <q-card-section>
          <div class="text-h6 q-mb-md">Shipments by Region</div>
          <div style="height: 200px" class="flex flex-center">
            <!-- Aquí iría el gráfico circular -->
            <q-knob readonly v-model="totalValue" size="150px" thickness="0.15" color="primary" track-color="grey-3">
              <div class="text-center">
                <div class="text-h6">3,420</div>
                <div class="text-caption">TOTAL</div>
              </div>
            </q-knob>
          </div>
          <!-- Leyenda -->
          <q-list dense class="q-mt-md">
            <q-item v-for="region in regions" :key="region.name">
              <q-item-section avatar>
                <q-badge rounded :color="region.color" />
              </q-item-section>
              <q-item-section>{{ region.name }}</q-item-section>
              <q-item-section side class="text-caption">{{ region.percent }}%</q-item-section>
            </q-item>
          </q-list>
        </q-card-section>
      </q-card>
    </div>
  </div>

  <q-card flat bordered>
    <q-table
      :rows="rows"
      :columns="columns"
      row-key="name"
      flat
    >
      <!-- Slot para la cabecera personalizada -->
      <template v-slot:top>
        <div class="text-h6">Order Details</div>
        <q-space />
        <q-btn-toggle
          v-model="filter"
          flat color="grey-6"
          toggle-color="primary"
          :options="[{label: 'All Orders', value: 'all'}, {label: 'Delayed', value: 'delay'}]"
        />
        <q-btn flat icon="event" label="This Month" class="q-ml-md" />
        <q-btn outline icon="file_download" label="Export" class="q-ml-sm" />
      </template>

      <!-- Slot para la columna de Cliente -->
      <template v-slot:body-cell-customer="props">
        <q-td :props="props">
          <div class="row items-center">
            <q-avatar size="sm" color="blue-1" text-color="blue-9" class="q-mr-sm">
              {{ props.row.customer.substring(0,2) }}
            </q-avatar>
            {{ props.row.customer }}
          </div>
        </q-td>
      </template>

      <!-- Slot para el Status con Badges -->
      <template v-slot:body-cell-status="props">
        <q-td :props="props">
          <q-badge 
            :color="props.value === 'DELIVERED' ? 'green-1' : 'orange-1'" 
            :text-color="props.value === 'DELIVERED' ? 'green-9' : 'orange-9'"
            class="text-weight-bold q-pa-sm"
          >
            {{ props.value }}
          </q-badge>
        </q-td>
      </template>
    </q-table>
  </q-card>

</template>

<script setup>
  const props = defineProps({
    rows: {
      type: Array,
      default: () => ([
        { id: 1, customer: 'Acme Corp', orderDate: '2024-06-01', status: 'DELIVERED', amount: '$1,200' },
        { id: 2, customer: 'Globex Inc', orderDate: '2024-06-03', status: 'DELAYED', amount: '$850' },
        { id: 3, customer: 'Soylent Co', orderDate: '2024-06-05', status: 'DELIVERED', amount: '$2,500' },
        { id: 4, customer: 'Initech', orderDate: '2024-06-07', status: 'DELAYED', amount: '$1,750' },
        { id: 5, customer: 'Umbrella Corp', orderDate: '2024-06-10', status: 'DELIVERED', amount: '$3,000' },
      ])
    },

    columns: {
      type: Array,
      default: () => ([
        { name: 'customer', label: 'Customer', field: 'customer' },
        { name: 'orderDate', label: 'Order Date', field: 'orderDate' },
        { name: 'status', label: 'Status', field: 'status' },
        { name: 'amount', label: 'Amount', field: 'amount' },
      ])
    }
  })
</script>