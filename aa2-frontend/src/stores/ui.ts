import { defineStore } from 'pinia'
import { ref } from 'vue'

// Store de UI: gestiona estados de interfaz como el sidebar del admin
// o notificaciones globales que no pertenecen a ninguna entidad concreta.
export const useUiStore = defineStore('ui', () => {
  const sidebarOpen = ref(false)
  const globalLoading = ref(false)

  function toggleSidebar() {
    sidebarOpen.value = !sidebarOpen.value
  }

  function setLoading(value: boolean) {
    globalLoading.value = value
  }

  return { sidebarOpen, globalLoading, toggleSidebar, setLoading }
})
