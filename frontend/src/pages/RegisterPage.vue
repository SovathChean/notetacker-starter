<script setup lang="ts">
import { ref, computed } from 'vue'
import { RouterLink } from 'vue-router'
import { useAuth } from '@/composables/useAuth'

const { register, isLoading, error } = useAuth()

const form = ref({
  email: '',
  username: '',
  password: '',
  confirmPassword: ''
})

const validationErrors = ref<Record<string, string>>({})

const validateForm = (): boolean => {
  validationErrors.value = {}

  // Email validation
  if (!form.value.email) {
    validationErrors.value.email = 'Email is required'
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.value.email)) {
    validationErrors.value.email = 'Please enter a valid email'
  }

  // Username validation
  if (!form.value.username) {
    validationErrors.value.username = 'Username is required'
  } else if (form.value.username.length < 3) {
    validationErrors.value.username = 'Username must be at least 3 characters'
  } else if (!/^[a-zA-Z0-9_]+$/.test(form.value.username)) {
    validationErrors.value.username = 'Username can only contain letters, numbers, and underscores'
  }

  // Password validation
  if (!form.value.password) {
    validationErrors.value.password = 'Password is required'
  } else if (form.value.password.length < 8) {
    validationErrors.value.password = 'Password must be at least 8 characters'
  }

  // Confirm password validation
  if (!form.value.confirmPassword) {
    validationErrors.value.confirmPassword = 'Please confirm your password'
  } else if (form.value.password !== form.value.confirmPassword) {
    validationErrors.value.confirmPassword = 'Passwords do not match'
  }

  return Object.keys(validationErrors.value).length === 0
}

const isFormValid = computed(() => {
  return (
    form.value.email &&
    form.value.username.length >= 3 &&
    form.value.password.length >= 8 &&
    form.value.password === form.value.confirmPassword
  )
})

const handleSubmit = async () => {
  if (!validateForm()) return

  try {
    await register({
      email: form.value.email,
      username: form.value.username,
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
        <h2 class="mt-6 text-center text-2xl font-semibold text-gray-700">Create your account</h2>
        <p class="mt-2 text-center text-sm text-gray-600">
          Already have an account?
          <RouterLink to="/login" class="font-medium text-primary-600 hover:text-primary-500">
            Sign in
          </RouterLink>
        </p>
      </div>

      <form class="mt-8 space-y-6" @submit.prevent="handleSubmit">
        <div v-if="error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg" role="alert">
          {{ error }}
        </div>

        <div class="space-y-4">
          <div>
            <label for="email" class="label">Email address</label>
            <input
              id="email"
              v-model="form.email"
              type="email"
              autocomplete="email"
              required
              class="input"
              :class="{ 'input-error': validationErrors.email }"
              placeholder="you@example.com"
            />
            <p v-if="validationErrors.email" class="error-message">{{ validationErrors.email }}</p>
          </div>

          <div>
            <label for="username" class="label">Username</label>
            <input
              id="username"
              v-model="form.username"
              type="text"
              autocomplete="username"
              required
              class="input"
              :class="{ 'input-error': validationErrors.username }"
              placeholder="johndoe"
            />
            <p v-if="validationErrors.username" class="error-message">{{ validationErrors.username }}</p>
          </div>

          <div>
            <label for="password" class="label">Password</label>
            <input
              id="password"
              v-model="form.password"
              type="password"
              autocomplete="new-password"
              required
              class="input"
              :class="{ 'input-error': validationErrors.password }"
              placeholder="At least 8 characters"
            />
            <p v-if="validationErrors.password" class="error-message">{{ validationErrors.password }}</p>
          </div>

          <div>
            <label for="confirmPassword" class="label">Confirm Password</label>
            <input
              id="confirmPassword"
              v-model="form.confirmPassword"
              type="password"
              autocomplete="new-password"
              required
              class="input"
              :class="{ 'input-error': validationErrors.confirmPassword }"
              placeholder="Confirm your password"
            />
            <p v-if="validationErrors.confirmPassword" class="error-message">{{ validationErrors.confirmPassword }}</p>
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
              Creating account...
            </span>
            <span v-else>Create account</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
