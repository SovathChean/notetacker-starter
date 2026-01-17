import api from './api'
import type { Note, CreateNoteRequest, UpdateNoteRequest, PaginatedResponse, NotesQueryParams } from '@/types'

export const notesService = {
  async getNotes(params: NotesQueryParams = {}): Promise<PaginatedResponse<Note>> {
    const response = await api.get<PaginatedResponse<Note>>('/notes', { params })
    return response.data
  },

  async getNote(id: string): Promise<Note> {
    const response = await api.get<Note>(`/notes/${id}`)
    return response.data
  },

  async createNote(data: CreateNoteRequest): Promise<Note> {
    const response = await api.post<Note>('/notes', data)
    return response.data
  },

  async updateNote(id: string, data: UpdateNoteRequest): Promise<Note> {
    const response = await api.put<Note>(`/notes/${id}`, data)
    return response.data
  },

  async deleteNote(id: string): Promise<void> {
    await api.delete(`/notes/${id}`)
  }
}
