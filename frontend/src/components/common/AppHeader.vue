<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import { useAuth } from '@/composables/useAuth'

const authStore = useAuthStore()
const { logout, isLoading } = useAuth()
</script>

<template>
  <header class="bg-white shadow-sm border-b border-gray-200">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between items-center h-16">
        <div class="flex items-center">
          <h1 class="text-xl font-bold text-primary-600">Techbodia Notes</h1>
        </div>

        <div v-if="authStore.isAuthenticated" class="flex items-center space-x-4">
          <span class="text-sm text-gray-600">
            {{ authStore.user?.username }}
          </span>
          <button
            @click="logout"
            :disabled="isLoading"
            class="btn-secondary text-sm px-3 py-1.5"
          >
            {{ isLoading ? 'Logging out...' : 'Logout' }}
          </button>
        </div>
      </div>
    </div>
  </header>
</template>
