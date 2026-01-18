<script setup lang="ts">
import { ref, computed } from 'vue'
import { RouterLink } from 'vue-router'
import { useAuth } from '@/composables/useAuth'

const { login, isLoading, error } = useAuth()

const form = ref({
  identifier: '',
  password: ''
})

const validationErrors = ref<Record<string, string>>({})

const validateForm = (): boolean => {
  validationErrors.value = {}

  if (!form.value.identifier) {
    validationErrors.value.identifier = 'Email or username is required'
  }

  if (!form.value.password) {
    validationErrors.value.password = 'Password is required'
  }

  return Object.keys(validationErrors.value).length === 0
}

const isFormValid = computed(() => {
  return form.value.identifier && form.value.password
})

const handleSubmit = async () => {
  if (!validateForm()) return

  try {
    await login({
      identifier: form.value.identifier,
      password: form.value.password
    })
  } catch {
    // Error is handled in useAuth
  }
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <div>
        <h1 class="text-center text-3xl font-bold text-gray-900">Techbodia Notes</h1>
        <h2 class="mt-6 text-center text-2xl font-semibold text-gray-700">Sign in to your account</h2>
        <p class="mt-2 text-center text-sm text-gray-600">
          Don't have an account?
          <RouterLink to="/register" class="font-medium text-primary-600 hover:text-primary-500">
            Sign up
          </RouterLink>
        </p>
      </div>

      <form class="mt-8 space-y-6" @submit.prevent="handleSubmit">
        <div v-if="error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg" role="alert">
          {{ error }}
        </div>

        <div class="space-y-4">
          <div>
            <label for="identifier" class="label">Email or Username</label>
            <input
              id="identifier"
              v-model="form.identifier"
              type="text"
              autocomplete="username"
              required
              class="input"
              :class="{ 'input-error': validationErrors.identifier }"
              placeholder="Email or username"
            />
            <p v-if="validationErrors.identifier" class="error-message">{{ validationErrors.identifier }}</p>
          </div>

          <div>
            <label for="password" class="label">Password</label>
            <input
              id="password"
              v-model="form.password"
              type="password"
              autocomplete="current-password"
              required
              class="input"
              :class="{ 'input-error': validationErrors.password }"
              placeholder="Enter your password"
            />
            <p v-if="validationErrors.password" class="error-message">{{ validationErrors.password }}</p>
          </div>
        </div>

        <div>
          <button
            type="submit"
            :disabled="isLoading || !isFormValid"
            class="btn-primary w-full flex justify-center py-3"
          >
            <span v-if="isLoading" class="flex items-center">
              <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              Signing in...
            </span>
            <span v-else>Sign in</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
