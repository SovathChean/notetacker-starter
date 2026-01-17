<script setup lang="ts">
import { ref, watch } from 'vue'

interface Props {
  search?: string
  sortBy?: string
  sortOrder?: string
}

const props = withDefaults(defineProps<Props>(), {
  search: '',
  sortBy: 'createdAt',
  sortOrder: 'desc'
})

const emit = defineEmits<{
  'update:search': [value: string]
  'update:sortBy': [value: string]
  'update:sortOrder': [value: string]
  search: []
}>()

const localSearch = ref(props.search)
const localSortBy = ref(props.sortBy)
const localSortOrder = ref(props.sortOrder)

watch(() => props.search, (val) => { localSearch.value = val })
watch(() => props.sortBy, (val) => { localSortBy.value = val })
watch(() => props.sortOrder, (val) => { localSortOrder.value = val })

const handleSearch = () => {
  emit('update:search', localSearch.value)
  emit('search')
}

const handleSortChange = () => {
  emit('update:sortBy', localSortBy.value)
  emit('update:sortOrder', localSortOrder.value)
  emit('search')
}

const handleKeypress = (event: KeyboardEvent) => {
  if (event.key === 'Enter') {
    handleSearch()
  }
}
</script>

<template>
  <div class="flex flex-col sm:flex-row gap-4 mb-6">
    <!-- Search -->
    <div class="flex-1">
      <div class="relative">
        <input
          v-model="localSearch"
          type="text"
          class="input pl-10"
          placeholder="Search notes..."
          @keypress="handleKeypress"
        />
        <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
          <svg class="h-5 w-5 text-gray-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M8 4a4 4 0 100 8 4 4 0 000-8zM2 8a6 6 0 1110.89 3.476l4.817 4.817a1 1 0 01-1.414 1.414l-4.816-4.816A6 6 0 012 8z" clip-rule="evenodd" />
          </svg>
        </div>
        <button
          v-if="localSearch"
          @click="localSearch = ''; handleSearch()"
          class="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-400 hover:text-gray-600"
        >
          <svg class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
          </svg>
        </button>
      </div>
    </div>

    <!-- Sort -->
    <div class="flex gap-2">
      <select
        v-model="localSortBy"
        @change="handleSortChange"
        class="input w-auto"
      >
        <option value="createdAt">Created Date</option>
        <option value="updatedAt">Updated Date</option>
        <option value="title">Title</option>
      </select>

      <select
        v-model="localSortOrder"
        @change="handleSortChange"
        class="input w-auto"
      >
        <option value="desc">Newest First</option>
        <option value="asc">Oldest First</option>
      </select>
    </div>
  </div>
</template>
