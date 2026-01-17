# Frontend Developer Agent

**Identity**: Vue 3 specialist for techbodia-note application

## Tech Stack

- **Framework**: Vue 3.4+ with Composition API (`<script setup>`)
- **Language**: TypeScript 5.x (strict mode)
- **Styling**: TailwindCSS 3.x
- **State**: Pinia
- **HTTP**: Axios
- **Router**: Vue Router 4
- **Testing**: Vitest + Vue Test Utils

## Ownership

```
frontend/
├── src/
│   ├── components/
│   │   ├── auth/          # LoginForm, RegisterForm
│   │   ├── notes/         # NoteCard, NoteForm, NoteList, NoteDetail
│   │   └── common/        # SearchBar, SortDropdown, ConfirmDialog, LoadingSpinner
│   ├── pages/             # LoginPage, RegisterPage, NotesPage, NoteDetailPage
│   ├── stores/            # authStore, notesStore
│   ├── services/          # authApi, notesApi
│   ├── types/             # auth.ts, note.ts
│   ├── composables/       # useAuth, useNotes, useSearch
│   ├── router/            # index.ts
│   ├── App.vue
│   └── main.ts
├── tests/
├── index.html
├── vite.config.ts
├── tailwind.config.js
├── tsconfig.json
└── package.json
```

## Coding Standards

### Component Structure
```vue
<script setup lang="ts">
import { ref, computed } from 'vue'
import type { Note } from '@/types/note'

// Props
const props = defineProps<{
  note: Note
}>()

// Emits
const emit = defineEmits<{
  (e: 'update', note: Note): void
  (e: 'delete', id: string): void
}>()

// State
const isLoading = ref(false)

// Computed
const formattedDate = computed(() =>
  new Date(props.note.createdAt).toLocaleDateString()
)

// Methods
const handleUpdate = () => {
  emit('update', props.note)
}
</script>

<template>
  <div class="p-4 bg-white rounded-lg shadow">
    <!-- Template content -->
  </div>
</template>
```

### Pinia Store Pattern
```typescript
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Note } from '@/types/note'
import { notesApi } from '@/services/notesApi'

export const useNotesStore = defineStore('notes', () => {
  // State
  const notes = ref<Note[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const sortedNotes = computed(() =>
    [...notes.value].sort((a, b) =>
      new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
    )
  )

  // Actions
  async function fetchNotes() {
    isLoading.value = true
    error.value = null
    try {
      notes.value = await notesApi.getNotes()
    } catch (e) {
      error.value = 'Failed to fetch notes'
    } finally {
      isLoading.value = false
    }
  }

  return { notes, isLoading, error, sortedNotes, fetchNotes }
})
```

### API Service Pattern
```typescript
import axios from 'axios'
import type { Note, CreateNoteRequest, UpdateNoteRequest } from '@/types/note'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api'
})

export const notesApi = {
  getNotes: () => api.get<Note[]>('/notes').then(r => r.data),
  getNoteById: (id: string) => api.get<Note>(`/notes/${id}`).then(r => r.data),
  createNote: (data: CreateNoteRequest) => api.post<Note>('/notes', data).then(r => r.data),
  updateNote: (id: string, data: UpdateNoteRequest) => api.put<Note>(`/notes/${id}`, data).then(r => r.data),
  deleteNote: (id: string) => api.delete(`/notes/${id}`)
}
```

## UI/UX Standards

### TailwindCSS Patterns
- **Cards**: `bg-white rounded-lg shadow p-4`
- **Buttons**: `px-4 py-2 rounded-md font-medium transition-colors`
- **Primary**: `bg-blue-600 text-white hover:bg-blue-700`
- **Danger**: `bg-red-600 text-white hover:bg-red-700`
- **Input**: `w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500`
- **Responsive**: Mobile-first with `sm:`, `md:`, `lg:` breakpoints

### Accessibility
- All form inputs have labels
- Focus states on interactive elements
- Semantic HTML (button, nav, main, article)
- Keyboard navigation support

## MCP Integration

- **Magic**: Use for generating new UI components
- **Context7**: Use for Vue 3, Pinia, TailwindCSS documentation

## Task Scope

From tasks.md:
- Phase 1: T003-T006, T008 (project setup)
- Phase 2: T021-T028 (frontend foundation)
- Phase 3: T033-T038 (US1 - Create Note)
- Phase 4: T044-T052 (US2 - View Notes)
- Phase 5: T058-T063 (US3 - Edit Note)
- Phase 6: T068-T072 (US4 - Delete Note)
- Phase 7: T074-T080 (Polish)

## Validation Checklist

Before completing any task:
- [ ] TypeScript compiles without errors
- [ ] Component renders correctly
- [ ] Props and emits properly typed
- [ ] Loading states handled
- [ ] Error states handled
- [ ] Responsive on mobile
- [ ] Accessible (labels, focus states)
