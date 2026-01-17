# /vue - Vue 3 Component Skill

Generate Vue 3 components with TypeScript and TailwindCSS for techbodia-note.

## Usage

```
/vue component NoteCard       # Create component
/vue page NotesPage           # Create page
/vue store notes              # Create Pinia store
/vue composable useNotes      # Create composable
/vue service notesApi         # Create API service
/vue form NoteForm            # Create form with validation
```

## Arguments

| Arg | Description |
|-----|-------------|
| `component [name]` | Standard component in `components/` |
| `page [name]` | Page component in `pages/` |
| `store [name]` | Pinia store in `stores/` |
| `composable [name]` | Composable in `composables/` |
| `service [name]` | API service in `services/` |
| `form [name]` | Form component with validation |
| `modal [name]` | Modal/dialog component |
| `list [name]` | List component with empty state |

## Output Locations

```
frontend/src/
├── components/
│   ├── notes/         # Note-related components
│   ├── auth/          # Auth components
│   └── common/        # Shared components
├── pages/             # Route pages
├── stores/            # Pinia stores
├── composables/       # Composition functions
├── services/          # API services
└── types/             # TypeScript types
```

---

## Templates

### Component Template

```vue
<!-- frontend/src/components/notes/NoteCard.vue -->
<script setup lang="ts">
import { computed } from 'vue'
import type { Note } from '@/types/note'

// Props
const props = defineProps<{
  note: Note
}>()

// Emits
const emit = defineEmits<{
  (e: 'click', note: Note): void
  (e: 'delete', id: number): void
}>()

// Computed
const formattedDate = computed(() => {
  return new Date(props.note.createdAt).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
})

// Methods
const handleClick = () => {
  emit('click', props.note)
}

const handleDelete = (e: Event) => {
  e.stopPropagation()
  emit('delete', props.note.id)
}
</script>

<template>
  <article
    class="p-4 bg-white rounded-lg shadow hover:shadow-md transition-shadow cursor-pointer"
    @click="handleClick"
  >
    <div class="flex justify-between items-start">
      <h3 class="text-lg font-semibold text-gray-900 truncate">
        {{ note.title }}
      </h3>
      <button
        class="text-gray-400 hover:text-red-500 transition-colors"
        @click="handleDelete"
        aria-label="Delete note"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>
    </div>
    <p class="mt-2 text-gray-600 text-sm line-clamp-2">
      {{ note.content || 'No content' }}
    </p>
    <time class="mt-3 text-xs text-gray-400 block">
      {{ formattedDate }}
    </time>
  </article>
</template>
```

### Page Template

```vue
<!-- frontend/src/pages/NotesPage.vue -->
<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useNotesStore } from '@/stores/notesStore'
import NoteList from '@/components/notes/NoteList.vue'
import NoteForm from '@/components/notes/NoteForm.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import ConfirmDialog from '@/components/common/ConfirmDialog.vue'

const router = useRouter()
const notesStore = useNotesStore()

// State
const showCreateForm = ref(false)
const deleteNoteId = ref<number | null>(null)

// Lifecycle
onMounted(() => {
  notesStore.fetchNotes()
})

// Methods
const handleNoteClick = (noteId: number) => {
  router.push(`/notes/${noteId}`)
}

const handleCreateSuccess = () => {
  showCreateForm.value = false
}

const handleDeleteConfirm = async () => {
  if (deleteNoteId.value) {
    await notesStore.deleteNote(deleteNoteId.value)
    deleteNoteId.value = null
  }
}
</script>

<template>
  <main class="container mx-auto px-4 py-8 max-w-4xl">
    <!-- Header -->
    <div class="flex justify-between items-center mb-8">
      <h1 class="text-2xl font-bold text-gray-900">My Notes</h1>
      <button
        class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors"
        @click="showCreateForm = true"
      >
        Create Note
      </button>
    </div>

    <!-- Loading -->
    <LoadingSpinner v-if="notesStore.isLoading" />

    <!-- Error -->
    <div v-else-if="notesStore.error" class="p-4 bg-red-50 text-red-700 rounded-md">
      {{ notesStore.error }}
    </div>

    <!-- Notes List -->
    <NoteList
      v-else
      :notes="notesStore.sortedNotes"
      @click="handleNoteClick"
      @delete="(id) => deleteNoteId = id"
    />

    <!-- Create Form Modal -->
    <NoteForm
      v-if="showCreateForm"
      @submit="handleCreateSuccess"
      @cancel="showCreateForm = false"
    />

    <!-- Delete Confirmation -->
    <ConfirmDialog
      v-if="deleteNoteId"
      title="Delete Note"
      message="Are you sure you want to delete this note? This action cannot be undone."
      confirm-text="Delete"
      @confirm="handleDeleteConfirm"
      @cancel="deleteNoteId = null"
    />
  </main>
</template>
```

### Pinia Store Template

```typescript
// frontend/src/stores/notesStore.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Note, CreateNoteRequest, UpdateNoteRequest } from '@/types/note'
import { notesApi } from '@/services/notesApi'

export const useNotesStore = defineStore('notes', () => {
  // State
  const notes = ref<Note[]>([])
  const currentNote = ref<Note | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const sortedNotes = computed(() =>
    [...notes.value].sort((a, b) =>
      new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
    )
  )

  const noteCount = computed(() => notes.value.length)

  // Actions
  async function fetchNotes() {
    isLoading.value = true
    error.value = null
    try {
      notes.value = await notesApi.getNotes()
    } catch (e) {
      error.value = 'Failed to fetch notes'
      console.error(e)
    } finally {
      isLoading.value = false
    }
  }

  async function fetchNoteById(id: number) {
    isLoading.value = true
    error.value = null
    try {
      currentNote.value = await notesApi.getNoteById(id)
    } catch (e) {
      error.value = 'Failed to fetch note'
      console.error(e)
    } finally {
      isLoading.value = false
    }
  }

  async function createNote(data: CreateNoteRequest) {
    isLoading.value = true
    error.value = null
    try {
      const newNote = await notesApi.createNote(data)
      notes.value.unshift(newNote)
      return newNote
    } catch (e) {
      error.value = 'Failed to create note'
      throw e
    } finally {
      isLoading.value = false
    }
  }

  async function updateNote(id: number, data: UpdateNoteRequest) {
    isLoading.value = true
    error.value = null
    try {
      const updated = await notesApi.updateNote(id, data)
      const index = notes.value.findIndex(n => n.id === id)
      if (index !== -1) {
        notes.value[index] = updated
      }
      if (currentNote.value?.id === id) {
        currentNote.value = updated
      }
      return updated
    } catch (e) {
      error.value = 'Failed to update note'
      throw e
    } finally {
      isLoading.value = false
    }
  }

  async function deleteNote(id: number) {
    isLoading.value = true
    error.value = null
    try {
      await notesApi.deleteNote(id)
      notes.value = notes.value.filter(n => n.id !== id)
      if (currentNote.value?.id === id) {
        currentNote.value = null
      }
    } catch (e) {
      error.value = 'Failed to delete note'
      throw e
    } finally {
      isLoading.value = false
    }
  }

  function clearError() {
    error.value = null
  }

  return {
    // State
    notes,
    currentNote,
    isLoading,
    error,
    // Getters
    sortedNotes,
    noteCount,
    // Actions
    fetchNotes,
    fetchNoteById,
    createNote,
    updateNote,
    deleteNote,
    clearError
  }
})
```

### API Service Template

```typescript
// frontend/src/services/notesApi.ts
import axios from 'axios'
import type {
  Note,
  CreateNoteRequest,
  UpdateNoteRequest,
  NotesListResponse
} from '@/types/note'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api',
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor for auth token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('access_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Response interceptor for token refresh
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      // Handle token refresh or redirect to login
      localStorage.removeItem('access_token')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export const notesApi = {
  getNotes: async (): Promise<Note[]> => {
    const response = await api.get<Note[]>('/notes')
    return response.data
  },

  getNoteById: async (id: number): Promise<Note> => {
    const response = await api.get<Note>(`/notes/${id}`)
    return response.data
  },

  createNote: async (data: CreateNoteRequest): Promise<Note> => {
    const response = await api.post<Note>('/notes', data)
    return response.data
  },

  updateNote: async (id: number, data: UpdateNoteRequest): Promise<Note> => {
    const response = await api.put<Note>(`/notes/${id}`, data)
    return response.data
  },

  deleteNote: async (id: number): Promise<void> => {
    await api.delete(`/notes/${id}`)
  },

  searchNotes: async (query: string): Promise<Note[]> => {
    const response = await api.get<Note[]>('/notes', {
      params: { search: query }
    })
    return response.data
  }
}
```

### TypeScript Types Template

```typescript
// frontend/src/types/note.ts
export interface Note {
  id: number
  title: string
  content: string | null
  createdAt: string
  updatedAt: string
}

export interface CreateNoteRequest {
  title: string
  content?: string
}

export interface UpdateNoteRequest {
  title: string
  content?: string
}

export interface NotesListResponse {
  notes: Note[]
  total: number
}
```

### Form Component Template

```vue
<!-- frontend/src/components/notes/NoteForm.vue -->
<script setup lang="ts">
import { ref, computed } from 'vue'
import { useNotesStore } from '@/stores/notesStore'
import type { Note } from '@/types/note'

// Props
const props = defineProps<{
  note?: Note  // If provided, edit mode
}>()

// Emits
const emit = defineEmits<{
  (e: 'submit', note: Note): void
  (e: 'cancel'): void
}>()

const notesStore = useNotesStore()

// Form state
const title = ref(props.note?.title ?? '')
const content = ref(props.note?.content ?? '')
const errors = ref<{ title?: string; content?: string }>({})

// Computed
const isEditMode = computed(() => !!props.note)
const isValid = computed(() => title.value.trim().length > 0)

// Validation
const validate = (): boolean => {
  errors.value = {}

  if (!title.value.trim()) {
    errors.value.title = 'Title is required'
  } else if (title.value.length > 200) {
    errors.value.title = 'Title must be 200 characters or less'
  }

  if (content.value.length > 50000) {
    errors.value.content = 'Content must be 50,000 characters or less'
  }

  return Object.keys(errors.value).length === 0
}

// Submit
const handleSubmit = async () => {
  if (!validate()) return

  try {
    const data = {
      title: title.value.trim(),
      content: content.value.trim() || undefined
    }

    let result: Note
    if (isEditMode.value && props.note) {
      result = await notesStore.updateNote(props.note.id, data)
    } else {
      result = await notesStore.createNote(data)
    }

    emit('submit', result)
  } catch (e) {
    // Error handled by store
  }
}
</script>

<template>
  <div class="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
    <div class="bg-white rounded-lg shadow-xl w-full max-w-lg">
      <form @submit.prevent="handleSubmit" class="p-6">
        <h2 class="text-xl font-semibold mb-4">
          {{ isEditMode ? 'Edit Note' : 'Create Note' }}
        </h2>

        <!-- Title -->
        <div class="mb-4">
          <label for="title" class="block text-sm font-medium text-gray-700 mb-1">
            Title <span class="text-red-500">*</span>
          </label>
          <input
            id="title"
            v-model="title"
            type="text"
            maxlength="200"
            class="w-full px-3 py-2 border rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            :class="{ 'border-red-500': errors.title }"
            placeholder="Enter note title"
          />
          <p v-if="errors.title" class="mt-1 text-sm text-red-500">
            {{ errors.title }}
          </p>
          <p class="mt-1 text-xs text-gray-400">
            {{ title.length }}/200
          </p>
        </div>

        <!-- Content -->
        <div class="mb-6">
          <label for="content" class="block text-sm font-medium text-gray-700 mb-1">
            Content
          </label>
          <textarea
            id="content"
            v-model="content"
            rows="6"
            maxlength="50000"
            class="w-full px-3 py-2 border rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500 resize-none"
            :class="{ 'border-red-500': errors.content }"
            placeholder="Enter note content (optional)"
          />
          <p v-if="errors.content" class="mt-1 text-sm text-red-500">
            {{ errors.content }}
          </p>
        </div>

        <!-- Actions -->
        <div class="flex justify-end gap-3">
          <button
            type="button"
            class="px-4 py-2 text-gray-700 bg-gray-100 rounded-md hover:bg-gray-200 transition-colors"
            @click="emit('cancel')"
          >
            Cancel
          </button>
          <button
            type="submit"
            class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors disabled:opacity-50"
            :disabled="!isValid || notesStore.isLoading"
          >
            {{ notesStore.isLoading ? 'Saving...' : (isEditMode ? 'Update' : 'Create') }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
```

---

## Workflow

1. Parse component type and name from arguments
2. Determine output location based on type
3. Generate component using appropriate template
4. Include TypeScript types
5. Follow TailwindCSS patterns
6. Add accessibility attributes
7. Register in appropriate index file if needed
