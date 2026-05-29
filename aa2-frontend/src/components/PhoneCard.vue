<!-- Componente que representa una tarjeta de teléfono.
     Separado del listado para que HomeView solo itere (v-for)
     y este componente se encargue del pintado. -->
<template>
  <div class="phone-card">
    <div class="card-header">
      <h3>{{ phone.brand }} {{ phone.model }}</h3>
      <Tag :severity="phone.stock > 0 ? 'success' : 'danger'" :value="phone.stock > 0 ? `Stock: ${phone.stock}` : 'Sin stock'" />
    </div>

    <p class="price">{{ phone.price.toFixed(2) }} €</p>

    <!-- Sección de compra: solo si el usuario está logueado y hay stock -->
    <div v-if="isLoggedIn && phone.stock > 0" class="buy-section">
      <InputNumber
        v-model="qty"
        :min="1"
        :max="phone.stock"
        showButtons
        buttonLayout="horizontal"
        style="width: 100%"
      />
      <Button
        label="Comprar"
        icon="pi pi-shopping-cart"
        :loading="buying"
        @click="emit('buy', phone, qty)"
        class="btn-buy"
        severity="success"
      />
    </div>

    <p v-else-if="!isLoggedIn" class="hint">
      <RouterLink to="/login">Inicia sesión</RouterLink> para comprar
    </p>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import Tag from 'primevue/tag'
import Button from 'primevue/button'
import InputNumber from 'primevue/inputnumber'
import type { Phone } from '@/types'

const props = defineProps<{
  phone: Phone
  isLoggedIn: boolean
  buying?: boolean
}>()

const emit = defineEmits<{
  buy: [phone: Phone, quantity: number]
}>()

const qty = ref(1)
</script>

<style scoped>
.phone-card {
  border: 1px solid #e0e0e0;
  border-radius: 10px;
  padding: 1.2rem;
  background: white;
  box-shadow: 0 2px 6px rgba(0,0,0,0.06);
  display: flex;
  flex-direction: column;
  gap: 0.6rem;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 0.5rem;
}

.card-header h3 { margin: 0; font-size: 1rem; }

.price { font-size: 1.3rem; font-weight: bold; color: #333; }

.buy-section { display: flex; flex-direction: column; gap: 0.5rem; margin-top: 0.3rem; }

.btn-buy { width: 100%; }

.hint { font-size: 0.85rem; color: #888; }
.hint a { color: #4f8ef7; }
</style>
