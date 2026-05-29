import { defineStore } from 'pinia'
import { ref } from 'vue'
import { getMyPurchases, getAllPurchases, createPurchase, cancelPurchase } from '@/services/api'
import type { Purchase, CreatePurchaseDTO } from '@/types'

// Store de compras: gestiona el historial de compras del usuario o todas (admin).
export const usePurchasesStore = defineStore('purchases', () => {
  const purchases = ref<Purchase[]>([])
  const loading = ref(false)
  const error = ref('')

  async function fetchMine(customerId: number) {
    loading.value = true
    error.value = ''
    try {
      const res = await getMyPurchases(customerId)
      purchases.value = res.data
    } catch {
      error.value = 'Error al cargar compras'
    } finally {
      loading.value = false
    }
  }

  async function fetchAll() {
    loading.value = true
    error.value = ''
    try {
      const res = await getAllPurchases()
      purchases.value = res.data
    } catch {
      error.value = 'Error al cargar todas las compras'
    } finally {
      loading.value = false
    }
  }

  async function add(data: CreatePurchaseDTO) {
    const res = await createPurchase(data)
    purchases.value.unshift(res.data)
    return res.data
  }

  async function cancel(id: number) {
    await cancelPurchase(id)
    const p = purchases.value.find((x) => x.id === id)
    if (p) p.status = 'CANCELLED'
  }

  return { purchases, loading, error, fetchMine, fetchAll, add, cancel }
})
