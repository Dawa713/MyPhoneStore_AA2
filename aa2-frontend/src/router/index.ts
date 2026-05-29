import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    // ── Rutas públicas (DefaultLayout: con NavBar y Footer) ──
    {
      path: '/',
      name: 'home',
      component: () => import('@/views/HomeView.vue'),
      meta: { layout: 'default' },
    },
    {
      path: '/my-purchases',
      name: 'my-purchases',
      component: () => import('@/views/MyPurchasesView.vue'),
      meta: { layout: 'default', requiresAuth: true },
    },

    // ── Rutas de autenticación (AuthLayout: sin header ni footer) ──
    {
      path: '/login',
      name: 'login',
      component: () => import('@/views/LoginView.vue'),
      meta: { layout: 'auth' },
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/views/RegisterView.vue'),
      meta: { layout: 'auth' },
    },

    // ── Rutas de administración (AdminLayout: header/footer distintos + sidebar) ──
    {
      path: '/admin',
      redirect: '/admin/phones',
      meta: { layout: 'admin', requiresAuth: true, requiresAdmin: true },
    },
    {
      path: '/admin/phones',
      name: 'admin-phones',
      component: () => import('@/views/admin/AdminPhonesView.vue'),
      meta: { layout: 'admin', requiresAuth: true, requiresAdmin: true },
    },
    {
      path: '/admin/customers',
      name: 'admin-customers',
      component: () => import('@/views/admin/AdminCustomersView.vue'),
      meta: { layout: 'admin', requiresAuth: true, requiresAdmin: true },
    },
    {
      path: '/admin/purchases',
      name: 'admin-purchases',
      component: () => import('@/views/admin/AdminPurchasesView.vue'),
      meta: { layout: 'admin', requiresAuth: true, requiresAdmin: true },
    },
  ],
})

// Guard de navegación: redirige si no tienes acceso
router.beforeEach((to) => {
  const auth = useAuthStore()

  if (to.meta.requiresAuth && !auth.isLoggedIn) {
    return { name: 'login' }
  }
  if (to.meta.requiresAdmin && !auth.isAdmin) {
    return { name: 'home' }
  }
})

export default router
