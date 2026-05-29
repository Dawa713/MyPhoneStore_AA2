<template>
  <div>
    <h2>Gestión de Clientes</h2>
    <ConfirmDialog />

    <DataTable
      :value="customers"
      :loading="loading"
      paginator
      :rows="10"
      stripedRows
      tableStyle="min-width: 40rem"
    >
      <Column field="id" header="ID" sortable style="width:5%" />
      <Column field="name" header="Nombre" sortable />
      <Column field="email" header="Email" sortable />
      <Column field="role" header="Rol">
        <template #body="{ data }">
          <Tag :severity="data.role === 'ADMIN' ? 'warning' : 'info'" :value="data.role" />
        </template>
      </Column>
      <Column field="createdAt" header="Registro">
        <template #body="{ data }">{{ formatDate(data.createdAt) }}</template>
      </Column>
      <Column field="isActive" header="Estado">
        <template #body="{ data }">
          <Tag :severity="data.isActive ? 'success' : 'danger'" :value="data.isActive ? 'Activo' : 'Inactivo'" />
        </template>
      </Column>
      <Column header="Acciones">
        <template #body="{ data }">
          <Button icon="pi pi-trash" size="small" severity="danger" @click="remove(data.id)" :disabled="data.role === 'ADMIN'" />
        </template>
      </Column>
    </DataTable>

    <Toast />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Tag from 'primevue/tag'
import Button from 'primevue/button'
import Toast from 'primevue/toast'
import { useToast } from 'primevue/usetoast'
import ConfirmDialog from 'primevue/confirmdialog'
import { useConfirm } from 'primevue/useconfirm'
import { getCustomers } from '@/services/api'
import type { Customer } from '@/types'
import axios from 'axios'

const customers = ref<Customer[]>([])
const loading = ref(false)
const toast = useToast()
const confirm = useConfirm()

async function load() {
  loading.value = true
  try {
    const res = await getCustomers()
    customers.value = res.data
  } catch {
    toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar los clientes', life: 4000 })
  } finally {
    loading.value = false
  }
}

function remove(id: number) {
  confirm.require({
    message: '¿Seguro que quieres desactivar este cliente?',
    header: 'Confirmar',
    icon: 'pi pi-exclamation-triangle',
    rejectLabel: 'Cancelar',
    acceptLabel: 'Desactivar',
    acceptClass: 'p-button-danger',
    accept: async () => {
      try {
        const token = localStorage.getItem('token')
        await axios.delete(`${import.meta.env.VITE_API_URL}/customers/${id}`, {
          headers: { Authorization: `Bearer ${token}` }
        })
        customers.value = customers.value.filter((c) => c.id !== id)
        toast.add({ severity: 'info', summary: 'Desactivado', life: 3000 })
      } catch (e: any) {
        toast.add({ severity: 'error', summary: 'Error', detail: e.response?.data?.message ?? 'Error', life: 4000 })
      }
    }
  })
}

function formatDate(d: string) {
  return new Date(d).toLocaleDateString('es-ES')
}

onMounted(load)
</script>

<style scoped>
h2 { margin-bottom: 1.5rem; }
</style>
