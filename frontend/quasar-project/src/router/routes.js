const routes = [
  //Main page
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [{ path: '', component: () => import('pages/IndexPage.vue') }],
  },

  //Shipments page
  {
    path: '/shipments',
    component: () => import('layouts/MainLayout.vue'),
    children: [{ path: '', component: () => import('pages/ShipmentsPage.vue') }],
  },

  //Inventory page
  {
    path: '/inventory',
    component: () => import('layouts/MainLayout.vue'),
    children: [{ path: '', component: () => import('pages/InventoryPage.vue') }],
  },

//Analytics page
  {
    path: '/analytics',
    component: () => import('layouts/MainLayout.vue'),
    children: [{ path: '', component: () => import('pages/AnalyticsPage.vue') }],
  },

  //Fleet page
  {
    path: '/fleet',
    component: () => import('layouts/MainLayout.vue'),
    children: [{ path: '', component: () => import('pages/FleetPage.vue') }],
  },

  //Orders page
  {
    path: '/orders',
    component: () => import('layouts/MainLayout.vue'),
    children: [{ path: '', component: () => import('pages/OrderFormPage.vue') }],
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue'),
  },
]

export default routes
