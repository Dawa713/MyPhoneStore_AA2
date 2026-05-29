<template>
  <div class="page">
    <h1>Panel de Administración</h1>

    <!-- Formulario crear / editar -->
    <div class="form-card">
      <h2>{{ editingId ? 'Editar teléfono' : 'Añadir teléfono' }}</h2>
      <form @submit.prevent="save">
        <div class="form-row">
          <div class="field">
            <label>Marca</label>
            <input v-model="form.brand" required placeholder="Apple" />
          </div>
          <div class="field">
            <label>Modelo</label>
            <input v-model="form.model" required placeholder="iPhone 16" />
          </div>
        </div>
        <div class="form-row">
          <div class="field">
            <label>Precio (€)</label>
            <input v-model.number="form.price" type="number" step="0.01" required min="0" />
          </div>
          <div class="field">
            <label>Stock</label>
            <input v-model.number="form.stock" type="number" required min="0" />
          </div>
        </div>

        <p v-if="formError" class="error">{{ formError }}</p>

        <div class="form-actions">
          <button type="submit" :disabled="saving">
            {{ saving ? 'Guardando...' : editingId ? 'Actualizar' : 'Crear' }}
          </button>
          <button v-if="editingId" type="button" @click="cancelEdit" class="btn-secondary">
            Cancelar
          </button>
        </div>
      </form>
    </div>

    <!-- Listado de teléfonos -->
    <h2 style="margin-top: 2rem">Teléfonos</h2>
    <p v-if="loading" class="info">Cargando...</p>

    <table v-if="phones.length > 0" class="table">
      <thead>
        <tr>
          <th>ID</th>
          <th>Marca</th>
          <th>Modelo</th>
          <th>Precio</th>
          <th>Stock</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="phone in phones" :key="phone.id">
          <td>{{ phone.id }}</td>
          <td>{{ phone.brand }}</td>
          <td>{{ phone.model }}</td>
          <td>{{ phone.price.toFixed(2) }} €</td>
          <td>{{ phone.stock }}</td>
          <td class="actions">
            <button @click="startEdit(phone)" class="btn-edit">Editar</button>
            <button @click="remove(phone.id)" class="btn-delete">Eliminar</button>
          </td>
        </tr>
      </tbody>
    </table>

    <div v-if="successMsg" class="toast">{{ successMsg }}</div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { getPhones, createPhone, updatePhone, deletePhone } from '@/services/api'
import type { Phone, CreatePhoneDTO } from '@/types'

const phones = ref<Phone[]>([])
const loading = ref(false)
const saving = ref(false)
const formError = ref('')
const successMsg = ref('')
const editingId = ref<number | null>(null)

const form = reactive<CreatePhoneDTO>({ brand: '', model: '', price: 0, stock: 0 })

async function load() {
  loading.value = true
  try {
    const res = await getPhones()
    phones.value = res.data
  } finally {
    loading.value = false
  }
}

async function save() {
  saving.value = true
  formError.value = ''
  try {
    if (editingId.value) {
      await updatePhone(editingId.value, { ...form })
      showSuccess('Teléfono actualizado')
    } else {
      await createPhone({ ...form })
      showSuccess('Teléfono creado')
    }
    resetForm()
    await load()
  } catch (e: any) {
    formError.value = e.response?.data?.message ?? 'Error al guardar'
  } finally {
    saving.value = false
  }
}

async function remove(id: number) {
  if (!confirm('¿Eliminar este teléfono?')) return
  try {
    await deletePhone(id)
    phones.value = phones.value.filter((p) => p.id !== id)
    showSuccess('Teléfono eliminado')
  } catch (e: any) {
    alert(e.response?.data?.message ?? 'Error al eliminar')
  }
}

function startEdit(phone: Phone) {
  editingId.value = phone.id
  form.brand = phone.brand
  form.model = phone.model
  form.price = phone.price
  form.stock = phone.stock
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

function cancelEdit() {
  editingId.value = null
  resetForm()
}

function resetForm() {
  editingId.value = null
  form.brand = ''
  form.model = ''
  form.price = 0
  form.stock = 0
}

function showSuccess(msg: string) {
  successMsg.value = msg
  setTimeout(() => (successMsg.value = ''), 3000)
}

onMounted(load)
</script>

<style scoped>
.page { max-width: 1000px; margin: 2rem auto; padding: 0 1rem; }
h1 { margin-bottom: 1.5rem; }

.form-card { background: white; border-radius: 10px; padding: 1.5rem; box-shadow: 0 2px 8px rgba(0,0,0,0.08); }
.form-card h2 { margin: 0 0 1rem; font-size: 1.1rem; }

.form-row { display: flex; gap: 1rem; }
.field { flex: 1; margin-bottom: 1rem; }
.field label { display: block; font-size: 0.9rem; font-weight: 500; margin-bottom: 0.3rem; }
.field input { width: 100%; padding: 0.6rem 0.8rem; border: 1px solid #ccc; border-radius: 6px; font-size: 0.95rem; box-sizing: border-box; }
.field input:focus { outline: none; border-color: #4f8ef7; }

.form-actions { display: flex; gap: 0.8rem; }
button { padding: 0.6rem 1.2rem; border: none; border-radius: 6px; cursor: pointer; background: #4f8ef7; color: white; font-size: 0.9rem; }
button:hover:not(:disabled) { opacity: 0.85; }
button:disabled { background: #aaa; cursor: not-allowed; }
.btn-secondary { background: #888; }

.table { width: 100%; border-collapse: collapse; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.08); margin-top: 1rem; }
.table th { background: #f0f4ff; padding: 0.8rem 1rem; text-align: left; font-size: 0.9rem; color: #555; }
.table td { padding: 0.8rem 1rem; border-top: 1px solid #f0f0f0; font-size: 0.9rem; }
.table tr:hover td { background: #fafafa; }

.actions { display: flex; gap: 0.5rem; }
.btn-edit { background: #ffc107; color: #333; padding: 0.3rem 0.7rem; font-size: 0.85rem; }
.btn-delete { background: #dc3545; padding: 0.3rem 0.7rem; font-size: 0.85rem; }

.info { color: #666; padding: 1rem; }
.error { color: #dc3545; background: #f8d7da; padding: 0.6rem; border-radius: 6px; font-size: 0.9rem; margin-bottom: 0.5rem; }

.toast { position: fixed; bottom: 2rem; right: 2rem; background: #28a745; color: white; padding: 1rem 1.5rem; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.2); }
</style>
