// Tipos que reflejan los DTOs de la API de C#

export interface Phone {
  id: number
  brand: string
  model: string
  price: number
  stock: number
  isActive: boolean
}

export interface Customer {
  id: number
  name: string
  email: string
  role: string
  createdAt: string
  isActive: boolean
}

export interface Purchase {
  id: number
  customerId: number
  customerName: string
  phoneId: number
  phoneBrand: string
  phoneModel: string
  quantity: number
  totalPrice: number
  purchaseDate: string
  status: string
  isActive: boolean
}

export interface AuthResponse {
  token: string
  name: string
  email: string
  role: string
  customerId: number
}

export interface CreatePhoneDTO {
  brand: string
  model: string
  price: number
  stock: number
}

export interface CreatePurchaseDTO {
  customerId: number
  phoneId: number
  quantity: number
}
