// User types
export interface User {
  id: string
  email: string
  username: string
  createdAt: string
  updatedAt: string
}

// Note types
export interface Note {
  id: string
  userId: string
  title: string
  content: string
  createdAt: string
  updatedAt: string
}

export interface CreateNoteRequest {
  title: string
  content: string
}

export interface UpdateNoteRequest {
  title: string
  content: string
}

// Auth types
export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  email: string
  username: string
  password: string
}

export interface AuthResponse {
  accessToken: string
  refreshToken: string
  tokenType: string
  expiresIn: number
  user: User
}

export interface RefreshTokenRequest {
  refreshToken: string
}

// API Response types
export interface ApiResponse<T> {
  data: T
  message?: string
}

export interface ApiError {
  message: string
  errors?: Record<string, string[]>
}

export interface PaginatedResponse<T> {
  data: T[]
  total: number
  page: number
  limit: number
  totalPages: number
}

// Query params
export interface NotesQueryParams {
  page?: number
  limit?: number
  search?: string
  sortBy?: 'createdAt' | 'updatedAt' | 'title'
  sortOrder?: 'asc' | 'desc'
}
