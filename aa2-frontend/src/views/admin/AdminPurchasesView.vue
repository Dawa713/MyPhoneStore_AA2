<template>
  <div>
    <h2>Todas las Compras</h2>

    <DataTable :value="store.purchases" :loading="store.loading" paginator :rows="15" stripedRows tableStyle="min-width: 50rem">
      <Column field="id" header="ID" sortable style="width:5%" />
      <Column field="customerName" header="Cliente" sortable />
      <Column field="phoneBrand" header="Teléfono" sortable>
        <template #body="{ data }">{{ data.phoneBrand }} {{ data.phoneModel }}</template>
      </Column>
      <Column field="quantity" header="Cant." sortable />
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
    </DataTable>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Tag from 'primevue/tag'
import { usePurchasesStore } from '@/stores/purchases'

const store = usePurchasesStore()
onMounted(() => store.fetchAll())
</script>

<style scoped>
h2 { margin-bottom: 1.5rem; }
</style>
