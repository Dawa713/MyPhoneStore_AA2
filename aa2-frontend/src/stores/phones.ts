import { defineStore } from 'pinia'
import { ref } from 'vue'
import { getPhones, createPhone, updatePhone, deletePhone, searchPhonesByBrand, searchPhonesByPrice } from '@/services/api'
import type { Phone, CreatePhoneDTO } from '@/types'

// Store de teléfonos: gestiona el estado global de la lista de teléfonos.
// Todos los componentes que necesiten teléfonos usan este store,
// evitando duplicar llamadas a la API.
export const usePhonesStore = defineStore('phones', () => {
  const phones = ref<Phone[]>([])
  const loading = ref(false)
  const error = ref('')

  async function fetchAll() {
    loading.value = true
    error.value = ''
    try {
      const res = await getPhones()
      phones.value = res.data
    } catch {
      error.value = 'Error al cargar teléfonos'
    } finally {
      loading.value = false
    }
  }

  async function fetchByBrand(brand: string) {
    loading.value = true
    error.value = ''
    try {
      const res = await searchPhonesByBrand(brand)
      phones.value = res.data
    } catch {
      error.value = 'Error en búsqueda por marca'
    } finally {
      loading.value = false
    }
  }

  async function fetchByPrice(min: number, max: number) {
    loading.value = true
    error.value = ''
    try {
      const res = await searchPhonesByPrice(min, max)
      phones.value = res.data
    } catch {
      error.value = 'Error en búsqueda por precio'
    } finally {
      loading.value = false
    }
  }

  async function add(data: CreatePhoneDTO) {
    const res = await createPhone(data)
    phones.value.push(res.data)
  }

  async function edit(id: number, data: CreatePhoneDTO) {
    await updatePhone(id, data)
    const idx = phones.value.findIndex((p) => p.id === id)
    if (idx !== -1) phones.value[idx] = { ...phones.value[idx], ...data } as Phone
  }

  async function remove(id: number) {
    await deletePhone(id)
    phones.value = phones.value.filter((p) => p.id !== id)
  }

  return { phones, loading, error, fetchAll, fetchByBrand, fetchByPrice, add, edit, remove }
})
