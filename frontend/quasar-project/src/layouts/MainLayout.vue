<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated>
      <q-toolbar>
        <q-btn flat dense round icon="menu" aria-label="Menu" @click="toggleLeftDrawer" />

        <q-toolbar-title class="cursor-pointer my-title" @click="$router.push('/')"> Northwind Traders </q-toolbar-title>

        <div>Quasar v{{ $q.version }}</div>
      </q-toolbar>
    </q-header>

    <q-drawer v-model="leftDrawerOpen" show-if-above bordered>
      <q-list>
        <q-item-label class="q-py-lg" header> Essential Menu </q-item-label>

        <EssentialMenu v-for="link in linksList" :key="link.title" v-bind="link" />

      </q-list>
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script setup>
import EssentialMenu from 'src/components/EssentialMenu.vue';
import { ref } from 'vue'

const linksList = [
  {
    title: 'Home',
    caption: 'Dashboard',
    icon: 'home',
    link: '/',
  },
  {
    title: 'Shipments',
    caption: 'Shipments management',
    icon: 'local_shipping',
    link: '/shipments',
  },
  {
    title: 'Inventory',
    caption: 'Inventory management',
    icon: 'inventory_2',
    link: '/inventory',
  },
  {
    title: 'Analytics',
    caption: 'Business intelligence',
    icon: 'bar_chart',
    link: '/analytics',
  },
  {
    title: 'Fleet',
    caption: 'Fleet management',
    icon: 'local_shipping',
    link: '/fleet',
  }
]

const leftDrawerOpen = ref(false)

function toggleLeftDrawer() {
  leftDrawerOpen.value = !leftDrawerOpen.value
}
</script>
