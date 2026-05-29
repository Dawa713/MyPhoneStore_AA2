<template>
  <footer class="footer">
    <div class="footer-content">
      <div class="footer-brand">
        <span>📱 PhoneStore</span>
        <p>Tu tienda de confianza</p>
      </div>
      <div class="footer-links">
        <RouterLink to="/">Catálogo</RouterLink>
        <RouterLink v-if="auth.isLoggedIn" to="/my-purchases">Mis compras</RouterLink>
        <RouterLink v-if="!auth.isLoggedIn" to="/login">Iniciar sesión</RouterLink>
        <RouterLink v-if="!auth.isLoggedIn" to="/register">Registrarse</RouterLink>
      </div>
      <div class="footer-copy">
        © {{ year }} PhoneStore — Proyecto DAW 2º
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
const auth = useAuthStore()
const year = new Date().getFullYear()
</script>

<style scoped>
.footer {
  background: #1a1a2e;
  color: #aaa;
  padding: 2rem;
  margin-top: auto;
}
.footer-content {
  max-width: 1100px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  flex-wrap: wrap;
  gap: 1.5rem;
}
.footer-brand span { color: white; font-weight: bold; font-size: 1.1rem; }
.footer-brand p { font-size: 0.85rem; margin-top: 0.3rem; }
.footer-links { display: flex; flex-direction: column; gap: 0.4rem; }
.footer-links a { color: #aaa; font-size: 0.9rem; text-decoration: none; }
.footer-links a:hover { color: white; }
.footer-copy { font-size: 0.8rem; align-self: flex-end; }
</style>
