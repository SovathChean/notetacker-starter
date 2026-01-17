import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Note, NotesQueryParams } from '@/types'

export const useNotesStore = defineStore('notes', () => {
  // State
  const notes = ref<Note[]>([])
  const currentNote = ref<Note | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const totalNotes = ref(0)
  const currentPage = ref(1)
  const totalPages = ref(1)
  const queryParams = ref<NotesQueryParams>({
    page: 1,
    limit: 10,
    sort_by: 'created_at',
    sort_order: 'desc'
  })

  // Getters
  const hasNotes = computed(() => notes.value.length > 0)
  const sortedNotes = computed(() => notes.value)

  // Actions
  function setNotes(newNotes: Note[]) {
    notes.value = newNotes
  }

  function addNote(note: Note) {
    notes.value.unshift(note)
    totalNotes.value++
  }

  function updateNote(updatedNote: Note) {
    const index = notes.value.findIndex(n => n.id === updatedNote.id)
    if (index !== -1) {
      notes.value[index] = updatedNote
    }
  }

  function removeNote(noteId: string) {
    notes.value = notes.value.filter(n => n.id !== noteId)
    totalNotes.value--
  }

  function setCurrentNote(note: Note | null) {
    currentNote.value = note
  }

  function setQueryParams(params: Partial<NotesQueryParams>) {
    queryParams.value = { ...queryParams.value, ...params }
  }

  function setPagination(total: number, page: number, pages: number) {
    totalNotes.value = total
    currentPage.value = page
    totalPages.value = pages
  }

  function setLoading(loading: boolean) {
    isLoading.value = loading
  }

  function setError(message: string | null) {
    error.value = message
  }

  function clearNotes() {
    notes.value = []
    currentNote.value = null
    totalNotes.value = 0
    currentPage.value = 1
    totalPages.value = 1
  }

  return {
    // State
    notes,
    currentNote,
    isLoading,
    error,
    totalNotes,
    currentPage,
    totalPages,
    queryParams,
    // Getters
    hasNotes,
    sortedNotes,
    // Actions
    setNotes,
    addNote,
    updateNote,
    removeNote,
    setCurrentNote,
    setQueryParams,
    setPagination,
    setLoading,
    setError,
    clearNotes
  }
})
