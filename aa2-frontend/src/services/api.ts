import axios from 'axios'
import type { Phone, Customer, Purchase, AuthResponse, CreatePhoneDTO, CreatePurchaseDTO } from '@/types'

// URL base de la API de C# - ajusta el puerto si cambia
const BASE_URL = 'http://localhost:5149/api'

// Instancia de axios con la URL base
const http = axios.create({ baseURL: BASE_URL })

// Interceptor: añade automáticamente el token JWT a todas las peticiones
// Si el usuario está logueado, cada llamada a la API lleva su token
http.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// ─── AUTH ────────────────────────────────────────────────────────────────────

export const login = (email: string, password: string) =>
  http.post<AuthResponse>('/auth/login', { email, password })

export const register = (name: string, email: string, password: string) =>
  http.post<AuthResponse>('/auth/register', { name, email, password })

// ─── PHONES ──────────────────────────────────────────────────────────────────

export const getPhones = () =>
  http.get<Phone[]>('/phones')

export const getPhoneById = (id: number) =>
  http.get<Phone>(`/phones/${id}`)

export const searchPhonesByBrand = (brand: string) =>
  http.get<Phone[]>(`/phones/search/byBrand?brand=${brand}`)

export const searchPhonesByPrice = (min: number, max: number) =>
  http.get<Phone[]>(`/phones/search/byPrice?minPrice=${min}&maxPrice=${max}`)

export const createPhone = (data: CreatePhoneDTO) =>
  http.post<Phone>('/phones', data)

export const updatePhone = (id: number, data: CreatePhoneDTO) =>
  http.put<Phone>(`/phones/${id}`, data)

export const deletePhone = (id: number) =>
  http.delete(`/phones/${id}`)

// ─── PURCHASES ───────────────────────────────────────────────────────────────

export const createPurchase = (data: CreatePurchaseDTO) =>
  http.post<Purchase>('/purchases', data)

export const getMyPurchases = (customerId: number) =>
  http.get<Purchase[]>(`/purchases/customer/${customerId}`)

export const getAllPurchases = () =>
  http.get<Purchase[]>('/purchases')

export const cancelPurchase = (id: number) =>
  http.put(`/purchases/${id}/cancel`)

// ─── CUSTOMERS (admin) ───────────────────────────────────────────────────────

export const getCustomers = () =>
  http.get<Customer[]>('/customers')
