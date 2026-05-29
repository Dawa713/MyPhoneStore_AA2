<template>
  <div class="page">
    <h1>Mis Compras</h1>

    <DataTable
      :value="store.purchases"
      :loading="store.loading"
      paginator
      :rows="10"
      stripedRows
      tableStyle="min-width: 40rem"
    >
      <Column field="id" header="#" sortable style="width:5%" />
      <Column header="Teléfono">
        <template #body="{ data }">{{ data.phoneBrand }} {{ data.phoneModel }}</template>
      </Column>
      <Column field="quantity" header="Cantidad" sortable />
      <Column field="totalPrice" header="Total" sortable>
        <template #body="{ data }">{{ data.totalPrice.toFixed(2) }} €</template>
      </Column>
      <Column field="purchaseDate" header="Fecha" sortable>
        <template #body="{ data }">{{ new Date(data.purchaseDate).toLocaleDateString('es-ES') }}</template>
      </Column>
      <Column field="status" header="Estado">
        <template #body="{ data }">
          <Tag :severity="data.status === 'COMPLETED' ? 'success' : 'danger'" :value="data.status" />
        </template>
      </Column>
      <Column header="Acción">
        <template #body="{ data }">
          <Button
            v-if="data.status === 'COMPLETED'"
            label="Cancelar"
            icon="pi pi-times"
            severity="danger"
            size="small"
            outlined
            @click="cancel(data.id)"
          />
        </template>
      </Column>
    </DataTable>

    <div v-if="!store.loading && store.purchases.length === 0" class="empty">
      No tienes compras todavía. <RouterLink to="/">Ver catálogo</RouterLink>
    </div>

    <Toast />
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Tag from 'primevue/tag'
import Button from 'primevue/button'
import Toast from 'primevue/toast'
import { useToast } from 'primevue/usetoast'
import { usePurchasesStore } from '@/stores/purchases'
import { useAuthStore } from '@/stores/auth'

const store = usePurchasesStore()
const auth = useAuthStore()
const toast = useToast()

async function cancel(id: number) {
  try {
    await store.cancel(id)
    toast.add({ severity: 'info', summary: 'Cancelada', detail: 'Compra cancelada y stock devuelto', life: 3000 })
  } catch (e: any) {
    toast.add({ severity: 'error', summary: 'Error', detail: e.response?.data?.message ?? 'Error', life: 4000 })
  }
}

onMounted(() => {
  if (auth.customerId) store.fetchMine(auth.customerId)
})
</script>

<style scoped>
.page { max-width: 1000px; margin: 2rem auto; padding: 0 1.5rem; }
h1 { margin-bottom: 1.5rem; }
.empty { text-align: center; padding: 3rem; color: #888; }
.empty a { color: #4f8ef7; }
</style>
