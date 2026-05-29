<template>
  <div>
    <h2>Gestión de Teléfonos</h2>

    <!-- Formulario con VeeValidate -->
    <Card style="margin-bottom:2rem">
      <template #title>{{ editingId ? 'Editar teléfono' : 'Añadir teléfono' }}</template>
      <template #content>
        <form @submit="onSubmit" class="form-grid">
          <div class="field">
            <label>Marca *</label>
            <InputText v-model="brand" :class="{ 'p-invalid': errors.brand }" fluid />
            <small class="p-error">{{ errors.brand }}</small>
          </div>
          <div class="field">
            <label>Modelo *</label>
            <InputText v-model="model" :class="{ 'p-invalid': errors.model }" fluid />
            <small class="p-error">{{ errors.model }}</small>
          </div>
          <div class="field">
            <label>Precio (€) *</label>
            <InputNumber v-model="price" :min="0" :minFractionDigits="2" fluid />
            <small class="p-error">{{ errors.price }}</small>
          </div>
          <div class="field">
            <label>Stock *</label>
            <InputNumber v-model="stock" :min="0" fluid />
            <small class="p-error">{{ errors.stock }}</small>
          </div>
          <div class="form-actions">
            <Button type="submit" :label="editingId ? 'Actualizar' : 'Crear'" icon="pi pi-save" :loading="saving" />
            <Button v-if="editingId" label="Cancelar" icon="pi pi-times" severity="secondary" type="button" @click="resetForm" />
          </div>
        </form>
      </template>
    </Card>

    <!-- Tabla de teléfonos con DataTable de PrimeVue -->
    <DataTable
      :value="store.phones"
      :loading="store.loading"
      paginator
      :rows="10"
      stripedRows
      tableStyle="min-width: 50rem"
    >
      <Column field="id" header="ID" sortable style="width:5%" />
      <Column field="brand" header="Marca" sortable />
      <Column field="model" header="Modelo" sortable />
      <Column field="price" header="Precio" sortable>
        <template #body="{ data }">{{ data.price.toFixed(2) }} €</template>
      </Column>
      <Column field="stock" header="Stock" sortable>
        <template #body="{ data }">
          <Tag :severity="data.stock > 0 ? 'success' : 'danger'" :value="String(data.stock)" />
        </template>
      </Column>
      <Column header="Acciones">
        <template #body="{ data }">
          <div style="display:flex;gap:0.5rem">
            <Button icon="pi pi-pencil" size="small" severity="warn" @click="startEdit(data)" />
            <Button icon="pi pi-trash" size="small" severity="danger" @click="remove(data.id)" />
          </div>
        </template>
      </Column>
    </DataTable>

    <Toast />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useForm } from 'vee-validate'
import * as yup from 'yup'
import Card from 'primevue/card'
import InputText from 'primevue/inputtext'
import InputNumber from 'primevue/inputnumber'
import Button from 'primevue/button'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Tag from 'primevue/tag'
import Toast from 'primevue/toast'
import { useToast } from 'primevue/usetoast'
import { usePhonesStore } from '@/stores/phones'
import type { Phone } from '@/types'

const store = usePhonesStore()
const toast = useToast()
const saving = ref(false)
const editingId = ref<number | null>(null)

const schema = yup.object({
  brand: yup.string().required('La marca es obligatoria'),
  model: yup.string().required('El modelo es obligatorio'),
  price: yup.number().required().min(0, 'El precio no puede ser negativo'),
  stock: yup.number().required().min(0, 'El stock no puede ser negativo').integer(),
})

const { defineField, handleSubmit, errors, resetForm: resetVee, setValues } = useForm({ validationSchema: schema })
const [brand] = defineField('brand')
const [model] = defineField('model')
const [price] = defineField('price')
const [stock] = defineField('stock')

const onSubmit = handleSubmit(async (values) => {
  saving.value = true
  try {
    if (editingId.value) {
      await store.edit(editingId.value, values as any)
      toast.add({ severity: 'success', summary: 'Actualizado', detail: 'Teléfono actualizado', life: 3000 })
    } else {
      await store.add(values as any)
      toast.add({ severity: 'success', summary: 'Creado', detail: 'Teléfono añadido', life: 3000 })
    }
    resetForm()
  } catch (e: any) {
    toast.add({ severity: 'error', summary: 'Error', detail: e.response?.data?.message ?? 'Error al guardar', life: 4000 })
  } finally {
    saving.value = false
  }
})

function startEdit(phone: Phone) {
  editingId.value = phone.id
  setValues({ brand: phone.brand, model: phone.model, price: phone.price, stock: phone.stock })
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

function resetForm() {
  editingId.value = null
  resetVee()
}

async function remove(id: number) {
  if (!confirm('¿Eliminar este teléfono?')) return
  try {
    await store.remove(id)
    toast.add({ severity: 'info', summary: 'Eliminado', life: 3000 })
  } catch (e: any) {
    toast.add({ severity: 'error', summary: 'Error', detail: e.response?.data?.message ?? 'Error', life: 4000 })
  }
}

onMounted(() => store.fetchAll())
</script>

<style scoped>
h2 { margin-bottom: 1.5rem; }
.form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; }
.field { display: flex; flex-direction: column; gap: 0.3rem; }
.field label { font-size: 0.9rem; font-weight: 500; }
.form-actions { grid-column: span 2; display: flex; gap: 0.8rem; }
</style>
