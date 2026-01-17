import { ref } from 'vue'
import { useNotesStore } from '@/stores/notes'
import { notesService } from '@/services/notes.service'
import type { CreateNoteRequest, UpdateNoteRequest, NotesQueryParams } from '@/types'

export function useNotes() {
  const notesStore = useNotesStore()
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const fetchNotes = async (params?: NotesQueryParams) => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = { ...notesStore.queryParams, ...params }
      const response = await notesService.getNotes(queryParams)
      notesStore.setNotes(response.data)
      notesStore.setPagination(response.total, response.page, response.total_pages)
      notesStore.setQueryParams(queryParams)
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to fetch notes'
    } finally {
      isLoading.value = false
    }
  }

  const fetchNote = async (id: string) => {
    isLoading.value = true
    error.value = null

    try {
      const note = await notesService.getNote(id)
      notesStore.setCurrentNote(note)
      return note
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to fetch note'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const createNote = async (data: CreateNoteRequest) => {
    isLoading.value = true
    error.value = null

    try {
      const note = await notesService.createNote(data)
      notesStore.addNote(note)
      return note
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to create note'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const updateNote = async (id: string, data: UpdateNoteRequest) => {
    isLoading.value = true
    error.value = null

    try {
      const note = await notesService.updateNote(id, data)
      notesStore.updateNote(note)
      return note
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to update note'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const deleteNote = async (id: string) => {
    isLoading.value = true
    error.value = null

    try {
      await notesService.deleteNote(id)
      notesStore.removeNote(id)
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to delete note'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const setSearch = (search: string) => {
    notesStore.setQueryParams({ search, page: 1 })
  }

  const setSorting = (sortBy: 'created_at' | 'updated_at' | 'title', sortOrder: 'asc' | 'desc') => {
    notesStore.setQueryParams({ sort_by: sortBy, sort_order: sortOrder, page: 1 })
  }

  const setPage = (page: number) => {
    notesStore.setQueryParams({ page })
  }

  return {
    isLoading,
    error,
    fetchNotes,
    fetchNote,
    createNote,
    updateNote,
    deleteNote,
    setSearch,
    setSorting,
    setPage
  }
}
