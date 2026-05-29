<template>
  <div class="layout-admin">
    <!-- Header específico del admin -->
    <header class="admin-header">
      <div class="admin-header-left">
        <button @click="ui.toggleSidebar()" class="sidebar-toggle">
          <i class="pi pi-bars" />
        </button>
        <span class="admin-title">⚙️ Panel de Administración</span>
      </div>
      <div class="admin-header-right">
        <span class="admin-user">{{ auth.userName }}</span>
        <RouterLink to="/" class="btn-back">← Volver a la tienda</RouterLink>
        <button @click="handleLogout" class="btn-logout">Cerrar sesión</button>
      </div>
    </header>

    <div class="admin-body">
      <!-- Sidebar de navegación del admin -->
      <aside :class="['admin-sidebar', { open: ui.sidebarOpen }]">
        <nav>
          <RouterLink to="/admin" active-class="active">
            <i class="pi pi-mobile" /> Teléfonos
          </RouterLink>
          <RouterLink to="/admin/customers" active-class="active">
            <i class="pi pi-users" /> Clientes
          </RouterLink>
          <RouterLink to="/admin/purchases" active-class="active">
            <i class="pi pi-shopping-cart" /> Compras
          </RouterLink>
        </nav>
      </aside>

      <main class="admin-content">
        <slot />
      </main>
    </div>

    <!-- Footer específico del admin -->
    <footer class="admin-footer">
      PhoneStore Admin Panel — {{ new Date().getFullYear() }}
    </footer>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import { useUiStore } from '@/stores/ui'
import { useRouter } from 'vue-router'

const auth = useAuthStore()
const ui = useUiStore()
const router = useRouter()

function handleLogout() {
  auth.logout()
  router.push('/login')
}
</script>

<style scoped>
.layout-admin { display: flex; flex-direction: column; min-height: 100vh; }

.admin-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.8rem 1.5rem;
  background: #2d3748;
  color: white;
  position: sticky;
  top: 0;
  z-index: 100;
}

.admin-header-left { display: flex; align-items: center; gap: 1rem; }
.admin-header-right { display: flex; align-items: center; gap: 1rem; }

.sidebar-toggle {
  background: none;
  border: none;
  color: white;
  font-size: 1.2rem;
  cursor: pointer;
  padding: 0.3rem;
}

.admin-title { font-weight: 600; font-size: 1rem; }
.admin-user { color: #a0aec0; font-size: 0.9rem; }

.btn-back { color: #90cdf4; font-size: 0.85rem; text-decoration: none; }
.btn-back:hover { color: white; }

.btn-logout {
  background: none;
  border: 1px solid #666;
  color: #ccc;
  padding: 0.3rem 0.8rem;
  border-radius: 6px;
  cursor: pointer;
  font-size: 0.85rem;
}
.btn-logout:hover { border-color: #f66; color: #f66; }

.admin-body { display: flex; flex: 1; }

.admin-sidebar {
  width: 220px;
  background: #1a202c;
  padding: 1.5rem 0;
  flex-shrink: 0;
  transition: width 0.3s;
}

.admin-sidebar.open { width: 220px; }

.admin-sidebar nav { display: flex; flex-direction: column; }

.admin-sidebar nav a {
  display: flex;
  align-items: center;
  gap: 0.7rem;
  padding: 0.8rem 1.5rem;
  color: #a0aec0;
  text-decoration: none;
  font-size: 0.95rem;
  transition: all 0.2s;
}

.admin-sidebar nav a:hover { background: #2d3748; color: white; }
.admin-sidebar nav a.active { background: #4f8ef7; color: white; }

.admin-content { flex: 1; padding: 2rem; background: #f4f6f9; overflow-y: auto; }

.admin-footer { padding: 0.8rem; text-align: center; background: #2d3748; color: #718096; font-size: 0.8rem; }
</style>
