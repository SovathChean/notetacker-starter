<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { storeToRefs } from 'pinia'
import { useNotesStore } from '@/stores/notes'
import { useNotes } from '@/composables/useNotes'
import AppHeader from '@/components/common/AppHeader.vue'
import NoteCard from '@/components/notes/NoteCard.vue'
import NoteModal from '@/components/notes/NoteModal.vue'
import DeleteConfirmModal from '@/components/notes/DeleteConfirmModal.vue'
import NotesFilter from '@/components/notes/NotesFilter.vue'
import type { Note, CreateNoteRequest, UpdateNoteRequest } from '@/types'

const notesStore = useNotesStore()
const { notes, totalNotes, currentPage, totalPages, queryParams } = storeToRefs(notesStore)
const { fetchNotes, createNote, updateNote, deleteNote, isLoading, error } = useNotes()

// Modal state
const isNoteModalOpen = ref(false)
const isDeleteModalOpen = ref(false)
const selectedNote = ref<Note | null>(null)
const isSaving = ref(false)
const isDeleting = ref(false)

// Filter state
const search = ref(queryParams.value.search || '')
const sortBy = ref(queryParams.value.sortBy || 'createdAt')
const sortOrder = ref(queryParams.value.sortOrder || 'desc')

// Computed
const hasNotes = computed(() => notes.value.length > 0)
const showPagination = computed(() => totalPages.value > 1)

// Fetch notes on mount
onMounted(() => {
  fetchNotes()
})

// Modal handlers
const openCreateModal = () => {
  selectedNote.value = null
  isNoteModalOpen.value = true
}

const openEditModal = (note: Note) => {
  selectedNote.value = note
  isNoteModalOpen.value = true
}

const openDeleteModal = (note: Note) => {
  selectedNote.value = note
  isDeleteModalOpen.value = true
}

const closeNoteModal = () => {
  isNoteModalOpen.value = false
  selectedNote.value = null
}

const closeDeleteModal = () => {
  isDeleteModalOpen.value = false
  selectedNote.value = null
}

// CRUD handlers
const handleSaveNote = async (data: CreateNoteRequest | UpdateNoteRequest) => {
  isSaving.value = true
  try {
    if (selectedNote.value) {
      await updateNote(selectedNote.value.id, data)
    } else {
      await createNote(data)
    }
    closeNoteModal()
  } catch {
    // Error handled in composable
  } finally {
    isSaving.value = false
  }
}

const handleDeleteNote = async () => {
  if (!selectedNote.value) return

  isDeleting.value = true
  try {
    await deleteNote(selectedNote.value.id)
    closeDeleteModal()
  } catch {
    // Error handled in composable
  } finally {
    isDeleting.value = false
  }
}

// Filter/Sort handlers
const handleSearch = () => {
  fetchNotes({
    search: search.value,
    sortBy: sortBy.value as 'createdAt' | 'updatedAt' | 'title',
    sortOrder: sortOrder.value as 'asc' | 'desc',
    page: 1
  })
}

// Pagination handlers
const goToPage = (page: number) => {
  fetchNotes({ page })
}

const previousPage = () => {
  if (currentPage.value > 1) {
    goToPage(currentPage.value - 1)
  }
}

const nextPage = () => {
  if (currentPage.value < totalPages.value) {
    goToPage(currentPage.value + 1)
  }
}
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <AppHeader />

    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Header -->
      <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6">
        <div>
          <h1 class="text-2xl font-bold text-gray-900">My Notes</h1>
          <p class="text-gray-500 mt-1">{{ totalNotes }} note{{ totalNotes !== 1 ? 's' : '' }}</p>
        </div>
        <button @click="openCreateModal" class="btn-primary">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 -ml-1" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clip-rule="evenodd" />
          </svg>
          Create Note
        </button>
      </div>

      <!-- Error message -->
      <div v-if="error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg mb-6">
        {{ error }}
      </div>

      <!-- Filters -->
      <NotesFilter
        v-model:search="search"
        v-model:sortBy="sortBy"
        v-model:sortOrder="sortOrder"
        @search="handleSearch"
      />

      <!-- Loading state -->
      <div v-if="isLoading" class="flex justify-center items-center py-12">
        <svg class="animate-spin h-8 w-8 text-primary-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
      </div>

      <!-- Empty state -->
      <div v-else-if="!hasNotes" class="text-center py-12">
        <svg xmlns="http://www.w3.org/2000/svg" class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
        </svg>
        <h3 class="mt-2 text-sm font-medium text-gray-900">No notes</h3>
        <p class="mt-1 text-sm text-gray-500">
          {{ search ? 'No notes match your search.' : 'Get started by creating a new note.' }}
        </p>
        <div v-if="!search" class="mt-6">
          <button @click="openCreateModal" class="btn-primary">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 -ml-1" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clip-rule="evenodd" />
            </svg>
            Create Note
          </button>
        </div>
      </div>

      <!-- Notes grid -->
      <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <NoteCard
          v-for="note in notes"
          :key="note.id"
          :note="note"
          @edit="openEditModal"
          @delete="openDeleteModal"
        />
      </div>

      <!-- Pagination -->
      <div v-if="showPagination" class="mt-8 flex justify-center items-center space-x-4">
        <button
          @click="previousPage"
          :disabled="currentPage <= 1"
          class="btn-secondary px-3 py-2"
          :class="{ 'opacity-50 cursor-not-allowed': currentPage <= 1 }"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M12.707 5.293a1 1 0 010 1.414L9.414 10l3.293 3.293a1 1 0 01-1.414 1.414l-4-4a1 1 0 010-1.414l4-4a1 1 0 011.414 0z" clip-rule="evenodd" />
          </svg>
        </button>

        <span class="text-sm text-gray-700">
          Page {{ currentPage }} of {{ totalPages }}
        </span>

        <button
          @click="nextPage"
          :disabled="currentPage >= totalPages"
          class="btn-secondary px-3 py-2"
          :class="{ 'opacity-50 cursor-not-allowed': currentPage >= totalPages }"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd" />
          </svg>
        </button>
      </div>
    </main>

    <!-- Modals -->
    <NoteModal
      :is-open="isNoteModalOpen"
      :note="selectedNote"
      :is-loading="isSaving"
      @close="closeNoteModal"
      @save="handleSaveNote"
    />

    <DeleteConfirmModal
      :is-open="isDeleteModalOpen"
      :note="selectedNote"
      :is-loading="isDeleting"
      @close="closeDeleteModal"
      @confirm="handleDeleteNote"
    />
  </div>
</template>
