<template>
  <!-- La clase column y full-height hacen que el contenido use todo el alto -->
  <q-page class="bg-grey-2 q-pa-md column no-wrap" style="height: 100%;">
    
    <!-- Sección Superior (aprox 45-50% del alto) -->
    <div class="col-6 row q-col-gutter-md q-pb-md" >

      <div class="col-12 col-md-4">
        <q-card class="full-height column">
          <q-card-section class="col">
            <div class="text-subtitle2">Total Sales</div>
            <div class="text-h3 text-weight-bolder q-py-md">$ 154,430</div>
            <div class="text-green text-weight-bold">13% ↑ <span class="text-grey-7">Week ratio</span></div>
          </q-card-section>
          <q-separator />
          <q-card-section>Day Sales $15,443</q-card-section>
        </q-card>
      </div>

      <!-- Aquí pones las otras 3 tarjetas pequeñas con col-md-3 -->
      <div class="col-12 col-md-4">
        <q-card class="full-height column">
          <q-card-section class="col">
            <div class="text-subtitle2">Total Sales</div>
            <div class="text-h3 text-weight-bolder q-py-md">$ 154,430</div>
            <div class="text-green text-weight-bold">13% ↑ <span class="text-grey-7">Week ratio</span></div>
          </q-card-section>
          <q-separator />
          <q-card-section>Day Sales $15,443</q-card-section>
        </q-card>
      </div>

      <!-- Aquí pones las otras 3 tarjetas pequeñas con col-md-3 -->
      <div class="col-12 col-md-4">
        <q-card class="full-height column">
          <q-card-section class="col">
            <div class="text-subtitle2">Total Sales</div>
            <div class="text-h3 text-weight-bolder q-py-md">$ 154,430</div>
            <div class="text-green text-weight-bold">13% ↑ <span class="text-grey-7">Week ratio</span></div>
          </q-card-section>
          <q-separator />
          <q-card-section>Day Sales $15,443</q-card-section>
        </q-card>
      </div>

    </div>

    <!-- Sección Inferior (aprox 45-50% del alto) -->
    <div class="col-6 row q-col-gutter-md">
      <!-- Gráfico Grande -->
      <div class="col-12 col-md-8">
        <q-card class="full-height column">
          <q-card-section class="col">
            <div class="text-h6">Store Sales Trend</div>
            <div class="flex flex-center bg-grey-1  border-dashed">
              [Espacio para Gráfico]
            </div>
          </q-card-section>
        </q-card>
      </div>

      <!-- Ranking -->
      <div class="col-12 col-md-4">
        <q-card class="full-height column">
          <q-card-section class="col scroll">
            <div class="text-h6">Sales ranking</div>
            <q-list separator>
              <q-item v-for="n in 7" :key="n" class="q-py-md">
                <q-item-section avatar>
                  <q-avatar color="primary" text-color="white">{{ n }}</q-avatar>
                </q-item-section>
                <q-item-section>No. {{ n }} Shop</q-item-section>
                <q-item-section side class="text-weight-bold">432,641</q-item-section>
              </q-item>
            </q-list>
          </q-card-section>
        </q-card>
      </div>
    </div>

  </q-page>
</template>

<style scoped>
.full-height {
  height: 100%;
}
.border-dashed {
  border: 2px dashed #e0e0e0;
  border-radius: 8px;
}
</style>