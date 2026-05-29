<template>
  <div class="auth-card">
    <h2>📱 PhoneStore</h2>
    <p class="subtitle">Crea tu cuenta</p>

    <form @submit="onSubmit">
      <div class="field">
        <label>Nombre</label>
        <InputText v-model="name" placeholder="Tu nombre" :class="{ 'p-invalid': errors.name }" fluid />
        <small class="p-error">{{ errors.name }}</small>
      </div>

      <div class="field">
        <label>Email</label>
        <InputText v-model="email" type="email" placeholder="tu@email.com" :class="{ 'p-invalid': errors.email }" fluid />
        <small class="p-error">{{ errors.email }}</small>
      </div>

      <div class="field">
        <label>Contraseña</label>
        <Password v-model="password" placeholder="Mínimo 6 caracteres" :feedback="true" :class="{ 'p-invalid': errors.password }" fluid />
        <small class="p-error">{{ errors.password }}</small>
      </div>

      <Message v-if="apiError" severity="error" :closable="false" style="margin-bottom:1rem">
        {{ apiError }}
      </Message>

      <Button type="submit" label="Crear cuenta" icon="pi pi-user-plus" :loading="loading" fluid />
    </form>

    <Divider />
    <p class="login-link">
      ¿Ya tienes cuenta? <RouterLink to="/login">Inicia sesión</RouterLink>
    </p>
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

const schema = yup.object({
  name: yup.string().required('El nombre es obligatorio').min(2, 'Mínimo 2 caracteres'),
  email: yup.string().required('El email es obligatorio').email('Email inválido'),
  password: yup.string().required('La contraseña es obligatoria').min(6, 'Mínimo 6 caracteres'),
})

const { defineField, handleSubmit, errors } = useForm({ validationSchema: schema })
const [name] = defineField('name')
const [email] = defineField('email')
const [password] = defineField('password')

const onSubmit = handleSubmit(async (values) => {
  loading.value = true
  apiError.value = ''
  try {
    await auth.register(values.name, values.email, values.password)
    router.push('/')
  } catch (e: any) {
    apiError.value = e.response?.data?.message ?? 'Error al crear la cuenta'
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.auth-card { background: white; border-radius: 16px; padding: 2.5rem; width: 100%; max-width: 420px; box-shadow: 0 20px 60px rgba(0,0,0,0.3); }
h2 { text-align: center; font-size: 1.5rem; margin-bottom: 0.3rem; }
.subtitle { text-align: center; color: #888; margin-bottom: 1.5rem; }
.field { margin-bottom: 1.2rem; }
.field label { display: block; font-size: 0.9rem; font-weight: 500; margin-bottom: 0.4rem; }
.login-link { text-align: center; font-size: 0.9rem; color: #666; }
.login-link a { color: #4f8ef7; }
</style>
