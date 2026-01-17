import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { User, AuthResponse } from '@/types'

export const useAuthStore = defineStore('auth', () => {
  // State
  const user = ref<User | null>(null)
  const accessToken = ref<string | null>(localStorage.getItem('access_token'))
  const refreshToken = ref<string | null>(localStorage.getItem('refresh_token'))
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!accessToken.value)

  // Actions
  function setTokens(auth: AuthResponse) {
    accessToken.value = auth.access_token
    refreshToken.value = auth.refresh_token
    user.value = auth.user
    localStorage.setItem('access_token', auth.access_token)
    localStorage.setItem('refresh_token', auth.refresh_token)
  }

  function clearTokens() {
    accessToken.value = null
    refreshToken.value = null
    user.value = null
    localStorage.removeItem('access_token')
    localStorage.removeItem('refresh_token')
  }

  function setError(message: string | null) {
    error.value = message
  }

  function setLoading(loading: boolean) {
    isLoading.value = loading
  }

  return {
    // State
    user,
    accessToken,
    refreshToken,
    isLoading,
    error,
    // Getters
    isAuthenticated,
    // Actions
    setTokens,
    clearTokens,
    setError,
    setLoading
  }
})
