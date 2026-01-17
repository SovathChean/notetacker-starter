<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import type { Note, CreateNoteRequest, UpdateNoteRequest } from '@/types'

interface Props {
  isOpen: boolean
  note?: Note | null
  isLoading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  note: null,
  isLoading: false
})

const emit = defineEmits<{
  close: []
  save: [data: CreateNoteRequest | UpdateNoteRequest]
}>()

const form = ref({
  title: '',
  content: ''
})

const validationErrors = ref<Record<string, string>>({})

const isEditing = computed(() => !!props.note)

const modalTitle = computed(() => isEditing.value ? 'Edit Note' : 'Create New Note')

watch(() => props.note, (newNote) => {
  if (newNote) {
    form.value = {
      title: newNote.title,
      content: newNote.content
    }
  } else {
    form.value = { title: '', content: '' }
  }
  validationErrors.value = {}
}, { immediate: true })

watch(() => props.isOpen, (isOpen) => {
  if (!isOpen) {
    form.value = { title: '', content: '' }
    validationErrors.value = {}
  }
})

const validateForm = (): boolean => {
  validationErrors.value = {}

  if (!form.value.title.trim()) {
    validationErrors.value.title = 'Title is required'
  } else if (form.value.title.length > 200) {
    validationErrors.value.title = 'Title must be at most 200 characters'
  }

  if (!form.value.content.trim()) {
    validationErrors.value.content = 'Content is required'
  }

  return Object.keys(validationErrors.value).length === 0
}

const isFormValid = computed(() => {
  return form.value.title.trim() && form.value.content.trim()
})

const handleSubmit = () => {
  if (!validateForm()) return

  emit('save', {
    title: form.value.title.trim(),
    content: form.value.content.trim()
  })
}

const handleClose = () => {
  emit('close')
}
</script>

<template>
  <Teleport to="body">
    <div v-if="isOpen" class="fixed inset-0 z-50 overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
      <div class="flex items-center justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
        <!-- Background overlay -->
        <div
          class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity"
          aria-hidden="true"
          @click="handleClose"
        ></div>

        <!-- Center modal -->
        <span class="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true">&#8203;</span>

        <!-- Modal panel -->
        <div class="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
          <form @submit.prevent="handleSubmit">
            <div class="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
              <h3 id="modal-title" class="text-lg leading-6 font-medium text-gray-900 mb-4">
                {{ modalTitle }}
              </h3>

              <div class="space-y-4">
                <div>
                  <label for="note-title" class="label">Title</label>
                  <input
                    id="note-title"
                    v-model="form.title"
                    type="text"
                    class="input"
                    :class="{ 'input-error': validationErrors.title }"
                    placeholder="Enter note title"
                    maxlength="200"
                  />
                  <p v-if="validationErrors.title" class="error-message">{{ validationErrors.title }}</p>
                </div>

                <div>
                  <label for="note-content" class="label">Content</label>
                  <textarea
                    id="note-content"
                    v-model="form.content"
                    rows="6"
                    class="input resize-none"
                    :class="{ 'input-error': validationErrors.content }"
                    placeholder="Write your note here..."
                  ></textarea>
                  <p v-if="validationErrors.content" class="error-message">{{ validationErrors.content }}</p>
                </div>
              </div>
            </div>

            <div class="bg-gray-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse gap-2">
              <button
                type="submit"
                :disabled="isLoading || !isFormValid"
                class="btn-primary w-full sm:w-auto"
              >
                <span v-if="isLoading" class="flex items-center justify-center">
                  <svg class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Saving...
                </span>
                <span v-else>{{ isEditing ? 'Update' : 'Create' }}</span>
              </button>
              <button
                type="button"
                @click="handleClose"
                :disabled="isLoading"
                class="btn-secondary w-full sm:w-auto mt-2 sm:mt-0"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </Teleport>
</template>
