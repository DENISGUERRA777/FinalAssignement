<template>
  <q-page class="flex flex-center">
    <img
      alt="Quasar logo"
      src="~assets/quasar-logo-vertical.svg"
      style="width: 200px; height: 200px"
    />
    <q-btn
      class="q-mt-xl"
      color="black"
      label="Go to home"
      @click="$router.push('/')"
    />
    
  </q-page>
</template>
