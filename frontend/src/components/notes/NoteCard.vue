<script setup lang="ts">
import type { Note } from '@/types'

interface Props {
  note: Note
}

const props = defineProps<Props>()

const emit = defineEmits<{
  edit: [note: Note]
  delete: [note: Note]
}>()

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const truncateContent = (content: string, maxLength: number = 150) => {
  if (content.length <= maxLength) return content
  return content.substring(0, maxLength) + '...'
}
</script>

<template>
  <div class="card hover:shadow-lg transition-shadow duration-200">
    <div class="flex justify-between items-start mb-2">
      <h3 class="text-lg font-semibold text-gray-900 line-clamp-1">{{ props.note.title }}</h3>
      <div class="flex space-x-2 flex-shrink-0 ml-2">
        <button
          @click="emit('edit', props.note)"
          class="text-primary-600 hover:text-primary-700 p-1"
          title="Edit note"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
            <path d="M13.586 3.586a2 2 0 112.828 2.828l-.793.793-2.828-2.828.793-.793zM11.379 5.793L3 14.172V17h2.828l8.38-8.379-2.83-2.828z" />
          </svg>
        </button>
        <button
          @click="emit('delete', props.note)"
          class="text-red-600 hover:text-red-700 p-1"
          title="Delete note"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M9 2a1 1 0 00-.894.553L7.382 4H4a1 1 0 000 2v10a2 2 0 002 2h8a2 2 0 002-2V6a1 1 0 100-2h-3.382l-.724-1.447A1 1 0 0011 2H9zM7 8a1 1 0 012 0v6a1 1 0 11-2 0V8zm5-1a1 1 0 00-1 1v6a1 1 0 102 0V8a1 1 0 00-1-1z" clip-rule="evenodd" />
          </svg>
        </button>
      </div>
    </div>

    <p class="text-gray-600 text-sm mb-4 line-clamp-3">
      {{ truncateContent(props.note.content) }}
    </p>

    <div class="text-xs text-gray-400">
      <span>Created: {{ formatDate(props.note.created_at) }}</span>
      <span v-if="props.note.updated_at !== props.note.created_at" class="ml-4">
        Updated: {{ formatDate(props.note.updated_at) }}
      </span>
    </div>
  </div>
</template>
