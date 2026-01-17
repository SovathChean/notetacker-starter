import api from './api'
import type { LoginRequest, RegisterRequest, AuthResponse, RefreshTokenRequest } from '@/types'

export const authService = {
  async login(credentials: LoginRequest): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>('/auth/login', credentials)
    return response.data
  },

  async register(data: RegisterRequest): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>('/auth/register', data)
    return response.data
  },

  async logout(): Promise<void> {
    const refreshToken = localStorage.getItem('refresh_token')
    if (refreshToken) {
      await api.post('/auth/logout', { refresh_token: refreshToken })
    }
  },

  async refreshToken(data: RefreshTokenRequest): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>('/auth/refresh', data)
    return response.data
  },

  async getCurrentUser(): Promise<AuthResponse['user']> {
    const response = await api.get<AuthResponse['user']>('/auth/me')
    return response.data
  }
}
