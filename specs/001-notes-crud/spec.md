# Feature Specification: Notes CRUD Operations

**Feature Branch**: `001-notes-crud`
**Created**: 2026-01-17
**Status**: Draft
**Input**: User description: "Notes CRUD - Create, Read, Update, Delete notes with auto timestamps"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Create a New Note (Priority: P1)

As a user, I want to create a new note with a title and optional content so that I can
capture my thoughts and information quickly.

**Why this priority**: Creating notes is the foundational action. Without the ability to
create notes, no other functionality is useful. This is the core value proposition.

**Independent Test**: Can be fully tested by creating a note with just a title, verifying
it appears in the notes list with auto-generated timestamps.

**Acceptance Scenarios**:

1. **Given** I am on the notes page, **When** I click "Create Note" and enter a title
   "Meeting Notes", **Then** the note is created and appears in my notes list with the
   current date as Created At timestamp.

2. **Given** I am creating a new note, **When** I enter a title "Shopping List" and
   content "Milk, Bread, Eggs", **Then** both title and content are saved and visible
   when I view the note.

3. **Given** I am creating a new note, **When** I try to save without entering a title,
   **Then** I see a validation error indicating title is required.

4. **Given** I am creating a new note, **When** I enter only a title without content,
   **Then** the note is created successfully with empty content.

---

### User Story 2 - View Notes List and Details (Priority: P2)

As a user, I want to view a list of all my notes showing titles and creation dates,
and click on any note to see its full content.

**Why this priority**: After creating notes, users need to find and read them. This
enables the core "read" functionality and makes created notes useful.

**Independent Test**: Can be tested by navigating to the notes list, verifying notes
display with title and date, then clicking a note to view full details.

**Acceptance Scenarios**:

1. **Given** I have created multiple notes, **When** I navigate to the notes list,
   **Then** I see all my notes displayed with their titles and creation dates.

2. **Given** I am viewing the notes list, **When** I click on a note titled
   "Meeting Notes", **Then** I see the full note details including title, content,
   Created At, and Updated At timestamps.

3. **Given** I have no notes, **When** I view the notes list, **Then** I see an
   empty state message indicating no notes exist yet.

---

### User Story 3 - Edit an Existing Note (Priority: P3)

As a user, I want to edit the title and content of my existing notes so that I can
update information as it changes.

**Why this priority**: Editing allows users to keep their notes current. This is
essential for notes that contain evolving information like project plans or lists.

**Independent Test**: Can be tested by opening an existing note, modifying its title
or content, saving, and verifying the changes persist with an updated timestamp.

**Acceptance Scenarios**:

1. **Given** I am viewing a note, **When** I click "Edit", change the title from
   "Meeting Notes" to "Team Meeting Notes", and save, **Then** the title is updated
   and the Updated At timestamp reflects the current date/time.

2. **Given** I am editing a note, **When** I modify the content and save, **Then**
   the content is updated and the Updated At timestamp is refreshed.

3. **Given** I am editing a note, **When** I clear the title field and try to save,
   **Then** I see a validation error indicating title is required.

4. **Given** I am editing a note, **When** I click "Cancel", **Then** no changes
   are saved and I return to the note view with original content.

---

### User Story 4 - Delete a Note (Priority: P4)

As a user, I want to delete notes I no longer need so that I can keep my notes
list organized and clutter-free.

**Why this priority**: Deletion completes the CRUD cycle and helps users manage
their notes over time. Lower priority because users can function without it initially.

**Independent Test**: Can be tested by selecting a note, confirming deletion, and
verifying it no longer appears in the notes list.

**Acceptance Scenarios**:

1. **Given** I am viewing a note, **When** I click "Delete" and confirm the action,
   **Then** the note is permanently removed from my notes list.

2. **Given** I click "Delete" on a note, **When** I am asked to confirm, **Then**
   I can cancel the deletion and the note remains unchanged.

3. **Given** I delete a note, **When** I return to the notes list, **Then** the
   deleted note is no longer visible.

---

### User Story 5 - User Registration (Priority: P5)

As a new user, I want to register an account with my email and password so that
I can securely store my notes.

**Why this priority**: Registration is required for new users to access the system.
Users must have an account before they can create or manage notes.

**Independent Test**: Can be tested by filling out the registration form with valid
email and password, submitting, and verifying account creation and automatic login.

**Acceptance Scenarios**:

1. **Given** I am on the registration page, **When** I enter a valid email and
   password (min 8 characters), **Then** my account is created and I am logged in
   with access and refresh tokens.

2. **Given** I am registering, **When** I enter an email that already exists,
   **Then** I see an error message indicating the email is already registered.

3. **Given** I am registering, **When** I enter a password shorter than 8 characters,
   **Then** I see a validation error indicating password requirements.

4. **Given** I am registering, **When** I enter an invalid email format,
   **Then** I see a validation error indicating email format is invalid.

---

### User Story 6 - User Login (Priority: P6)

As a registered user, I want to log in with my email and password so that I can
access my notes securely.

**Why this priority**: Login is required for returning users to access their notes.
Without login, users cannot access their previously created notes.

**Independent Test**: Can be tested by entering valid credentials on the login page,
submitting, and verifying successful authentication with token storage.

**Acceptance Scenarios**:

1. **Given** I am on the login page, **When** I enter valid email and password,
   **Then** I receive access_token and refresh_token and am redirected to my notes.

2. **Given** I am logging in, **When** I enter incorrect password,
   **Then** I see an error message indicating invalid credentials.

3. **Given** I am logging in, **When** I enter a non-existent email,
   **Then** I see an error message indicating invalid credentials (same as wrong password).

4. **Given** my access_token expires, **When** I make an API request,
   **Then** the system automatically uses refresh_token to obtain a new access_token.

5. **Given** my refresh_token expires, **When** I try to access the app,
   **Then** I am redirected to the login page.

---

### User Story 7 - User Logout (Priority: P7)

As a logged-in user, I want to log out of my account so that I can secure my
notes when using a shared device.

**Why this priority**: Logout provides security for users on shared devices.

**Independent Test**: Can be tested by clicking logout, verifying tokens are cleared,
and attempting to access notes (should redirect to login).

**Acceptance Scenarios**:

1. **Given** I am logged in, **When** I click "Logout",
   **Then** my tokens are cleared and I am redirected to the login page.

2. **Given** I have logged out, **When** I try to access the notes page directly,
   **Then** I am redirected to the login page.

---

### Edge Cases

- What happens when a user tries to create a note with an extremely long title?
  System MUST accept titles up to 200 characters and display an error for longer titles.

- What happens when a user tries to save note content that is very large?
  System MUST accept content up to 50,000 characters (approximately 10,000 words).

- How does the system handle concurrent edits to the same note (if applicable)?
  System MUST use optimistic locking - if the note was modified since loading,
  user sees a conflict message and can reload or overwrite.

- What happens when the notes list is very long?
  System MUST paginate or use infinite scroll for lists exceeding 50 notes.

- How does the system handle special characters in title or content?
  System MUST properly handle and display special characters, emojis, and
  multi-language text without corruption.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow users to create notes with a mandatory title field.
- **FR-002**: System MUST allow users to optionally add content when creating a note.
- **FR-003**: System MUST auto-generate a Created At timestamp when a note is created.
- **FR-004**: System MUST auto-generate an Updated At timestamp when a note is modified.
- **FR-005**: System MUST display a list of notes showing title and creation date.
- **FR-006**: System MUST allow users to view the full details of any note.
- **FR-007**: System MUST allow users to edit the title and content of existing notes.
- **FR-008**: System MUST allow users to delete notes with confirmation.
- **FR-009**: System MUST validate that title is not empty before saving.
- **FR-010**: System MUST enforce title maximum length of 200 characters.
- **FR-011**: System MUST enforce content maximum length of 50,000 characters.
- **FR-012**: System MUST only show users their own notes (per constitution principle I).
- **FR-013**: System MUST persist all note data reliably across sessions.
- **FR-014**: System MUST provide visual feedback during save and delete operations.
- **FR-015**: System MUST handle empty states gracefully with helpful messaging.
- **FR-016**: System MUST allow users to register with email and password.
- **FR-017**: System MUST validate email format and uniqueness during registration.
- **FR-018**: System MUST enforce minimum password length of 8 characters.
- **FR-019**: System MUST hash passwords securely before storing (bcrypt/Argon2).
- **FR-020**: System MUST authenticate users with email and password.
- **FR-021**: System MUST issue JWT access_token (short-lived, 15 minutes) on successful login.
- **FR-022**: System MUST issue refresh_token (long-lived, 7 days) on successful login.
- **FR-023**: System MUST provide token refresh endpoint to exchange refresh_token for new access_token.
- **FR-024**: System MUST invalidate refresh_token on logout.
- **FR-025**: System MUST protect all note endpoints with JWT authentication.
- **FR-026**: System MUST return 401 Unauthorized for invalid or expired access_token.
- **FR-027**: System MUST securely store tokens in frontend (httpOnly cookies or secure storage).

### Key Entities

- **Note**: Represents a single user note. Key attributes: unique identifier, title
  (required, max 200 chars), content (optional, max 50,000 chars), Created At
  (auto-generated datetime), Updated At (auto-generated datetime on modification),
  owner reference (links note to its creator).

- **User**: The owner of notes. Key attributes: unique identifier, email (unique),
  password hash (bcrypt/Argon2), Created At. Relationship: One user owns many notes.
  Notes are scoped to their owner and not accessible by other users.

- **RefreshToken**: Stores refresh tokens for token renewal. Key attributes: unique
  identifier, token value (hashed), user reference, expiration date, revoked flag,
  Created At. Relationship: One user can have multiple refresh tokens (multiple devices).

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can create a new note in under 30 seconds from clicking "Create"
  to seeing the note in their list.

- **SC-002**: Users can find and open any note from their list in under 10 seconds.

- **SC-003**: Users can edit and save changes to a note in under 30 seconds.

- **SC-004**: Users can delete a note in under 10 seconds including confirmation.

- **SC-005**: 95% of users successfully complete their first note creation without
  encountering errors or confusion.

- **SC-006**: System displays the updated timestamp immediately after any edit is saved.

- **SC-007**: Notes list loads and displays within 2 seconds for users with up to
  500 notes.

- **SC-008**: All form validation errors are clearly communicated within 1 second
  of the user's invalid action.

## Assumptions

The following reasonable defaults have been assumed based on the feature description
and industry standards:

1. **Single user per session**: Each user session operates independently; no real-time
   collaboration features are required for this phase.

2. **Soft delete not required**: Deleted notes are permanently removed; no trash/restore
   functionality is needed initially.

3. **No note categorization**: Notes are stored in a flat list without folders, tags,
   or categories (can be added in future features).

4. **Chronological default sort**: Notes list defaults to showing newest first by
   creation date.

5. **No rich text editing**: Content is plain text only; no formatting, images, or
   attachments in this phase.

6. **No offline support**: Application requires network connectivity to function.

7. **JWT Authentication**: Access tokens expire after 15 minutes; refresh tokens
   expire after 7 days. Token refresh is handled automatically by the frontend.

8. **Password Security**: Passwords are hashed using bcrypt with appropriate cost factor.
   Plain text passwords are never stored or logged.
