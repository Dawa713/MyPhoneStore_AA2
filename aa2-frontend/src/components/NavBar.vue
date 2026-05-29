<template>
  <nav class="navbar">
    <div class="navbar-brand">
      <RouterLink to="/">📱 PhoneStore</RouterLink>
    </div>

    <div class="navbar-links">
      <RouterLink to="/">Catálogo</RouterLink>

      <!-- Solo si está logueado -->
      <RouterLink v-if="auth.isLoggedIn" to="/my-purchases">Mis compras</RouterLink>

      <!-- Solo si es ADMIN -->
      <RouterLink v-if="auth.isAdmin" to="/admin">Panel Admin</RouterLink>

      <!-- No logueado: mostrar Login y Registro -->
      <template v-if="!auth.isLoggedIn">
        <RouterLink to="/login">Iniciar sesión</RouterLink>
        <RouterLink to="/register" class="btn-primary">Registrarse</RouterLink>
      </template>

      <!-- Logueado: mostrar nombre y botón salir -->
      <template v-else>
        <span class="user-info">{{ auth.userName }} ({{ auth.userRole }})</span>
        <button @click="handleLogout" class="btn-logout">Cerrar sesión</button>
      </template>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'

const auth = useAuthStore()
const router = useRouter()

function handleLogout() {
  auth.logout()
  router.push('/login')
}
</script>

<style scoped>
.navbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.8rem 2rem;
  background: #1a1a2e;
  color: white;
  box-shadow: 0 2px 8px rgba(0,0,0,0.3);
}

.navbar-brand a {
  font-size: 1.3rem;
  font-weight: bold;
  color: white;
  text-decoration: none;
}

.navbar-links {
  display: flex;
  align-items: center;
  gap: 1.2rem;
}

.navbar-links a {
  color: #ccc;
  text-decoration: none;
  transition: color 0.2s;
}

.navbar-links a:hover,
.navbar-links a.router-link-active {
  color: white;
}

.btn-primary {
  background: #4f8ef7;
  color: white !important;
  padding: 0.4rem 1rem;
  border-radius: 6px;
}

.user-info {
  color: #aaa;
  font-size: 0.9rem;
}

.btn-logout {
  background: none;
  border: 1px solid #666;
  color: #ccc;
  padding: 0.3rem 0.8rem;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-logout:hover {
  border-color: #f66;
  color: #f66;
}
</style>
