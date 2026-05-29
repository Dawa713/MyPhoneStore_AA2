import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { login as apiLogin, register as apiRegister } from '@/services/api'

// Store de autenticación: guarda el token y los datos del usuario
// Los datos persisten en localStorage para que no se pierdan al recargar
export const useAuthStore = defineStore('auth', () => {
  // Estado: leemos de localStorage al iniciar
  const token = ref<string | null>(localStorage.getItem('token'))
  const userName = ref<string | null>(localStorage.getItem('userName'))
  const userEmail = ref<string | null>(localStorage.getItem('userEmail'))
  const userRole = ref<string | null>(localStorage.getItem('userRole'))
  const customerId = ref<number | null>(
    localStorage.getItem('customerId') ? Number(localStorage.getItem('customerId')) : null
  )

  // Computed: booleanos derivados del estado
  const isLoggedIn = computed(() => !!token.value)
  const isAdmin = computed(() => userRole.value === 'ADMIN')

  // Guarda los datos en localStorage y en el estado reactivo
  function saveSession(data: {
    token: string
    name: string
    email: string
    role: string
    customerId: number
  }) {
    token.value = data.token
    userName.value = data.name
    userEmail.value = data.email
    userRole.value = data.role
    customerId.value = data.customerId

    localStorage.setItem('token', data.token)
    localStorage.setItem('userName', data.name)
    localStorage.setItem('userEmail', data.email)
    localStorage.setItem('userRole', data.role)
    localStorage.setItem('customerId', String(data.customerId))
  }

  async function login(email: string, password: string) {
    const res = await apiLogin(email, password)
    saveSession(res.data)
  }

  async function register(name: string, email: string, password: string) {
    const res = await apiRegister(name, email, password)
    saveSession(res.data)
  }

  function logout() {
    token.value = null
    userName.value = null
    userEmail.value = null
    userRole.value = null
    customerId.value = null

    localStorage.removeItem('token')
    localStorage.removeItem('userName')
    localStorage.removeItem('userEmail')
    localStorage.removeItem('userRole')
    localStorage.removeItem('customerId')
  }

  return { token, userName, userEmail, userRole, customerId, isLoggedIn, isAdmin, login, register, logout }
})
