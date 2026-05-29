<template>
  <div class="page">
    <h1>Catálogo de Teléfonos</h1>

    <!-- Barra de búsqueda con componentes PrimeVue -->
    <div class="search-bar">
      <IconField>
        <InputIcon class="pi pi-search" />
        <InputText v-model="searchBrand" placeholder="Buscar por marca..." @input="onSearchBrand" />
      </IconField>
      <div class="price-filter">
        <InputNumber v-model="minPrice" placeholder="Precio mín." :min="0" prefix="€ " />
        <InputNumber v-model="maxPrice" placeholder="Precio máx." :min="0" prefix="€ " />
        <Button label="Filtrar" icon="pi pi-filter" @click="onSearchPrice" />
        <Button label="Limpiar" icon="pi pi-times" severity="secondary" @click="store.fetchAll()" />
      </div>
    </div>

    <ProgressSpinner v-if="store.loading" style="width:50px;height:50px;display:block;margin:2rem auto" />

    <Message v-if="store.error" severity="error" :closable="false">{{ store.error }}</Message>

    <!-- Grid: el v-for está aquí, pero el pintado lo hace PhoneCard -->
    <div class="phones-grid">
      <PhoneCard
        v-for="phone in store.phones"
        :key="phone.id"
        :phone="phone"
        :is-logged-in="auth.isLoggedIn"
        :buying="buyingId === phone.id"
        @buy="handleBuy"
      />
    </div>

    <p v-if="!store.loading && store.phones.length === 0" class="empty">
      No se encontraron teléfonos.
    </p>

    <Toast />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import InputText from 'primevue/inputtext'
import InputNumber from 'primevue/inputnumber'
import IconField from 'primevue/iconfield'
import InputIcon from 'primevue/inputicon'
import Button from 'primevue/button'
import ProgressSpinner from 'primevue/progressspinner'
import Message from 'primevue/message'
import Toast from 'primevue/toast'
import { useToast } from 'primevue/usetoast'

import PhoneCard from '@/components/PhoneCard.vue'
import { usePhonesStore } from '@/stores/phones'
import { usePurchasesStore } from '@/stores/purchases'
import { useAuthStore } from '@/stores/auth'
import type { Phone } from '@/types'

const store = usePhonesStore()
const purchasesStore = usePurchasesStore()
const auth = useAuthStore()
const toast = useToast()

const searchBrand = ref('')
const minPrice = ref<number | null>(null)
const maxPrice = ref<number | null>(null)
const buyingId = ref<number | null>(null)

async function onSearchBrand() {
  if (!searchBrand.value.trim()) return store.fetchAll()
  await store.fetchByBrand(searchBrand.value)
}

async function onSearchPrice() {
  await store.fetchByPrice(minPrice.value ?? 0, maxPrice.value ?? 999999)
}

async function handleBuy(phone: Phone, quantity: number) {
  if (!auth.customerId) return
  buyingId.value = phone.id
  try {
    await purchasesStore.add({ customerId: auth.customerId, phoneId: phone.id, quantity })
    phone.stock -= quantity
    toast.add({ severity: 'success', summary: 'Compra realizada', detail: `${phone.brand} ${phone.model} x${quantity}`, life: 3000 })
  } catch (e: any) {
    toast.add({ severity: 'error', summary: 'Error', detail: e.response?.data?.message ?? 'Error al comprar', life: 4000 })
  } finally {
    buyingId.value = null
  }
}

onMounted(() => store.fetchAll())
</script>

<style scoped>
.page { max-width: 1200px; margin: 2rem auto; padding: 0 1.5rem; }
h1 { margin-bottom: 1.5rem; font-size: 1.8rem; }

.search-bar { display: flex; flex-wrap: wrap; gap: 1rem; margin-bottom: 1.5rem; align-items: center; }
.price-filter { display: flex; gap: 0.6rem; align-items: center; flex-wrap: wrap; }

.phones-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(260px, 1fr)); gap: 1.2rem; }

.empty { text-align: center; color: #888; padding: 3rem; font-size: 1.1rem; }
</style>
