<template>
  <section class="page-grid">
    <div class="card">
      <p class="eyebrow">New Round</p>
      <h2>Start a round from an existing course</h2>

      <form class="stack-form" @submit.prevent="createRound">
        <label>
          Course
          <select v-model.number="roundForm.courseId" required>
            <option disabled value="0">Select a course</option>
            <option v-for="course in courses" :key="course.id" :value="course.id">
              {{ course.name }} | {{ course.holeCount }} holes | Par {{ course.totalPar }}
            </option>
          </select>
        </label>

        <label>
          Date played
          <input v-model="roundForm.datePlayed" type="date" required />
        </label>

        <p v-if="errorMessage" class="error-text">{{ errorMessage }}</p>
        <button class="primary-button" :disabled="creating" type="submit">
          {{ creating ? 'Creating Round...' : 'Create Round' }}
        </button>
      </form>
    </div>

    <div class="card">
      <div class="section-heading">
        <div>
          <p class="eyebrow">Your Rounds</p>
          <h2>Recent scorecards</h2>
        </div>
        <button class="ghost-button" type="button" @click="loadData">Refresh</button>
      </div>

      <div v-if="loading" class="muted">Loading rounds...</div>
      <div v-else-if="rounds.length === 0" class="muted">
        No rounds yet. Create one to start tracking shots hole by hole.
      </div>
      <div v-else class="rounds-stack">
        <router-link
          v-for="round in rounds"
          :key="round.id"
          :to="{ name: 'round-detail', params: { id: round.id } }"
          class="round-card"
        >
          <div>
            <h3>{{ round.courseName }}</h3>
            <p class="muted">{{ formatDate(round.datePlayed) }}</p>
          </div>

          <div class="round-card__stats">
            <span>Strokes {{ round.totalStrokes }}</span>
            <span>Par {{ round.totalPar }}</span>
            <span :class="scoreClass(round.scoreToPar)">{{ scoreLabel(round.scoreToPar) }}</span>
          </div>
        </router-link>
      </div>
    </div>
  </section>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../api/api'

const router = useRouter()
const courses = ref([])
const rounds = ref([])
const loading = ref(false)
const creating = ref(false)
const errorMessage = ref('')

const roundForm = reactive({
  courseId: 0,
  datePlayed: new Date().toISOString().slice(0, 10)
})

function formatDate(value) {
  return new Date(value).toLocaleDateString()
}

function scoreLabel(scoreToPar) {
  if (scoreToPar === 0) return 'E'
  return scoreToPar > 0 ? `+${scoreToPar}` : `${scoreToPar}`
}

function scoreClass(scoreToPar) {
  if (scoreToPar < 0) return 'score-good'
  if (scoreToPar > 0) return 'score-bad'
  return 'score-even'
}

async function loadData() {
  loading.value = true

  try {
    const [{ data: courseData }, { data: roundData }] = await Promise.all([
      api.get('/api/Courses'),
      api.get('/api/Rounds')
    ])

    courses.value = courseData
    rounds.value = roundData
  } finally {
    loading.value = false
  }
}

async function createRound() {
  creating.value = true
  errorMessage.value = ''

  try {
    const { data } = await api.post('/api/Rounds', {
      courseId: roundForm.courseId,
      datePlayed: new Date(roundForm.datePlayed).toISOString()
    })

    await router.push({ name: 'round-detail', params: { id: data.id } })
  } catch (error) {
    errorMessage.value = error?.response?.data || 'Unable to create the round.'
  } finally {
    creating.value = false
  }
}

onMounted(loadData)
</script>
