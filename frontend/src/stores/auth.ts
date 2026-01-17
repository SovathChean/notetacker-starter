import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { User, AuthResponse } from '@/types'

export const useAuthStore = defineStore('auth', () => {
  // State
  const user = ref<User | null>(null)
  const accessToken = ref<string | null>(localStorage.getItem('accessToken'))
  const refreshToken = ref<string | null>(localStorage.getItem('refreshToken'))
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!accessToken.value)

  // Actions
  function setTokens(auth: AuthResponse) {
    accessToken.value = auth.accessToken
    refreshToken.value = auth.refreshToken
    user.value = auth.user
    localStorage.setItem('accessToken', auth.accessToken)
    localStorage.setItem('refreshToken', auth.refreshToken)
  }

  function clearTokens() {
    accessToken.value = null
    refreshToken.value = null
    user.value = null
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
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
