<template>
  <section class="page-grid">
    <div class="card">
      <div class="section-heading">
        <div>
          <p class="eyebrow">Admin</p>
          <h2>Add a course with its holes</h2>
        </div>
        <button class="ghost-button" type="button" @click="resetHoles">Reset 18 Holes</button>
      </div>

      <form class="stack-form" @submit.prevent="createCourse">
        <label>
          Course name
          <input v-model="courseForm.name" required />
        </label>

        <label>
          Location
          <input v-model="courseForm.location" placeholder="City, State" />
        </label>

        <div class="holes-grid">
          <article v-for="hole in courseForm.holes" :key="hole.holeNumber" class="hole-editor">
            <h3>Hole {{ hole.holeNumber }}</h3>
            <label>
              Par
              <input v-model.number="hole.par" min="3" max="6" type="number" required />
            </label>
            <label>
              Yardage
              <input v-model.number="hole.yardage" min="0" type="number" required />
            </label>
          </article>
        </div>

        <p v-if="errorMessage" class="error-text">{{ errorMessage }}</p>
        <button class="primary-button" :disabled="saving" type="submit">
          {{ saving ? 'Saving Course...' : 'Save Course' }}
        </button>
      </form>
    </div>

    <div class="card">
      <div class="section-heading">
        <div>
          <p class="eyebrow">Library</p>
          <h2>Available courses</h2>
        </div>
        <button class="ghost-button" type="button" @click="fetchCourses">Refresh</button>
      </div>

      <div v-if="loading" class="muted">Loading courses...</div>
      <div v-else class="course-stack">
        <article v-for="course in courses" :key="course.id" class="course-card">
          <div class="course-card__header">
            <div>
              <h3>{{ course.name }}</h3>
              <p class="muted">{{ course.location || 'Location not set' }}</p>
            </div>
            <button class="danger-button" type="button" @click="deleteCourse(course.id)">Delete</button>
          </div>
          <p class="stat-line">
            {{ course.holeCount }} holes | Par {{ course.totalPar }}
          </p>
        </article>
      </div>
    </div>
  </section>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import api from '../api/api'

const courses = ref([])
const loading = ref(false)
const saving = ref(false)
const errorMessage = ref('')

const courseForm = reactive({
  name: '',
  location: '',
  holes: buildDefaultHoles()
})

function buildDefaultHoles() {
  return Array.from({ length: 18 }, (_, index) => ({
    holeNumber: index + 1,
    par: [4, 4, 3, 5][index % 4],
    yardage: 150 + index * 12
  }))
}

function resetHoles() {
  courseForm.holes = buildDefaultHoles()
}

async function fetchCourses() {
  loading.value = true

  try {
    const { data } = await api.get('/api/Courses')
    courses.value = data
  } finally {
    loading.value = false
  }
}

async function createCourse() {
  saving.value = true
  errorMessage.value = ''

  try {
    await api.post('/api/Courses', {
      name: courseForm.name,
      location: courseForm.location || null,
      holes: courseForm.holes
    })

    courseForm.name = ''
    courseForm.location = ''
    courseForm.holes = buildDefaultHoles()
    await fetchCourses()
  } catch (error) {
    errorMessage.value = error?.response?.data || 'Unable to create course.'
  } finally {
    saving.value = false
  }
}

async function deleteCourse(courseId) {
  await api.delete(`/api/Courses/${courseId}`)
  courses.value = courses.value.filter((course) => course.id !== courseId)
}

onMounted(fetchCourses)
</script>
