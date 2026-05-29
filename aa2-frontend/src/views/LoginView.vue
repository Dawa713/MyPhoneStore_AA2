<template>
  <div class="auth-card">
    <h2>📱 PhoneStore</h2>
    <p class="subtitle">Inicia sesión en tu cuenta</p>

    <!-- useForm de VeeValidate gestiona el estado del formulario -->
    <form @submit="onSubmit">
      <div class="field">
        <label>Email</label>
        <InputText
          v-model="email"
          type="email"
          placeholder="tu@email.com"
          :class="{ 'p-invalid': errors.email }"
          fluid
        />
        <small class="p-error">{{ errors.email }}</small>
      </div>

      <div class="field">
        <label>Contraseña</label>
        <Password
          v-model="password"
          placeholder="••••••••"
          :feedback="false"
          :class="{ 'p-invalid': errors.password }"
          fluid
        />
        <small class="p-error">{{ errors.password }}</small>
      </div>

      <Message v-if="apiError" severity="error" :closable="false" style="margin-bottom:1rem">
        {{ apiError }}
      </Message>

      <Button type="submit" label="Entrar" icon="pi pi-sign-in" :loading="loading" fluid />
    </form>

    <Divider />

    <p class="register-link">
      ¿No tienes cuenta? <RouterLink to="/register">Regístrate</RouterLink>
    </p>

    <div class="demo-section">
      <p>Cuentas de prueba:</p>
      <div class="demo-buttons">
        <Button label="Admin" icon="pi pi-shield" size="small" severity="secondary"
          @click="fillDemo('juan@email.com', 'password123')" />
        <Button label="Cliente" icon="pi pi-user" size="small" severity="secondary"
          @click="fillDemo('maria@email.com', 'pass1234')" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useForm } from 'vee-validate'
import * as yup from 'yup'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import Button from 'primevue/button'
import Message from 'primevue/message'
import Divider from 'primevue/divider'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const router = useRouter()
const apiError = ref('')
const loading = ref(false)

// Esquema de validación con Yup
const schema = yup.object({
  email: yup.string().required('El email es obligatorio').email('Email inválido'),
  password: yup.string().required('La contraseña es obligatoria').min(4, 'Mínimo 4 caracteres'),
})

const { defineField, handleSubmit, errors } = useForm({ validationSchema: schema })
const [email] = defineField('email')
const [password] = defineField('password')

const onSubmit = handleSubmit(async (values) => {
  loading.value = true
  apiError.value = ''
  try {
    await auth.login(values.email, values.password)
    router.push('/')
  } catch (e: any) {
    apiError.value = e.response?.data?.message ?? 'Email o contraseña incorrectos'
  } finally {
    loading.value = false
  }
})

function fillDemo(e: string, p: string) {
  email.value = e
  password.value = p
}
</script>

<style scoped>
.auth-card {
  background: white;
  border-radius: 16px;
  padding: 2.5rem;
  width: 100%;
  max-width: 420px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
}

h2 { text-align: center; font-size: 1.5rem; margin-bottom: 0.3rem; }
.subtitle { text-align: center; color: #888; margin-bottom: 1.5rem; }

.field { margin-bottom: 1.2rem; }
.field label { display: block; font-size: 0.9rem; font-weight: 500; margin-bottom: 0.4rem; }

.register-link { text-align: center; font-size: 0.9rem; color: #666; }
.register-link a { color: #4f8ef7; }

.demo-section { margin-top: 1rem; padding-top: 1rem; border-top: 1px solid #eee; }
.demo-section p { font-size: 0.8rem; color: #aaa; margin-bottom: 0.5rem; text-align: center; }
.demo-buttons { display: flex; gap: 0.5rem; justify-content: center; }
</style>
