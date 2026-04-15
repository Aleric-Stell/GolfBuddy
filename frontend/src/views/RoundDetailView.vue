<template>
  <section class="detail-grid">
    <div class="card span-two">
      <div class="section-heading">
        <div>
          <p class="eyebrow">Round Scorecard</p>
          <h2>{{ round?.courseName || 'Round' }}</h2>
          <p class="muted">{{ round ? formatDate(round.datePlayed) : '' }}</p>
        </div>
        <router-link class="ghost-button link-button" :to="{ name: 'rounds' }">Back to rounds</router-link>
      </div>

      <div v-if="loading" class="muted">Loading round...</div>
      <div v-else-if="round && scorecard" class="scoreboard">
        <article class="score-stat">
          <span>Total strokes</span>
          <strong>{{ scorecard.totalStrokes }}</strong>
        </article>
        <article class="score-stat">
          <span>Total par</span>
          <strong>{{ scorecard.totalPar }}</strong>
        </article>
        <article class="score-stat">
          <span>Score</span>
          <strong :class="scoreClass(scorecard.scoreToPar)">
            {{ scoreLabel(scorecard.scoreToPar) }}
          </strong>
        </article>
      </div>

      <div v-if="scorecard" class="scorecard-table">
        <div class="scorecard-row scorecard-head">
          <span>Hole</span>
          <span>Par</span>
          <span>Yards</span>
          <span>Strokes</span>
          <span>Score</span>
        </div>

        <div v-for="hole in scorecard.holes" :key="hole.holeId" class="scorecard-row">
          <span>{{ hole.holeNumber }}</span>
          <span>{{ hole.par }}</span>
          <span>{{ hole.yardage }}</span>
          <span>{{ hole.strokes }}</span>
          <span :class="scoreClass(hole.scoreToPar)">{{ scoreLabel(hole.scoreToPar) }}</span>
        </div>
      </div>
    </div>

    <div class="card">
      <p class="eyebrow">Shot Entry</p>
      <h2>Add a shot</h2>

      <form class="stack-form" @submit.prevent="addShot">
        <label>
          Hole
          <select v-model.number="shotForm.holeId" required>
            <option v-for="hole in round?.holes || []" :key="hole.holeId" :value="hole.holeId">
              Hole {{ hole.holeNumber }} | Par {{ hole.par }} | {{ hole.yardage }} yds
            </option>
          </select>
        </label>

        <label>
          Shot number
          <input v-model.number="shotForm.shotNumber" min="1" type="number" required />
        </label>

        <label>
          Club
          <input v-model="shotForm.club" placeholder="7 iron, driver, putter..." required />
        </label>

        <label>
          Distance in yards
          <input v-model.number="shotForm.distanceYards" min="0" type="number" required />
        </label>

        <label>
          Result
          <input v-model="shotForm.result" placeholder="Fairway, green, bunker, holed putt..." />
        </label>

        <p v-if="errorMessage" class="error-text">{{ errorMessage }}</p>
        <button class="primary-button" :disabled="savingShot" type="submit">
          {{ savingShot ? 'Saving Shot...' : 'Save Shot' }}
        </button>
      </form>
    </div>

    <div class="card span-two">
      <div class="section-heading">
        <div>
          <p class="eyebrow">Shot Log</p>
          <h2>Every shot in this round</h2>
        </div>
        <button class="ghost-button" type="button" @click="refreshRound">Refresh</button>
      </div>

      <div v-if="!round?.shots?.length" class="muted">
        No shots yet. Add one from the form to start filling out the scorecard.
      </div>

      <div v-else class="shots-grid">
        <article v-for="shot in round.shots" :key="shot.id" class="shot-card">
          <div class="shot-card__header">
            <strong>Hole {{ shot.holeNumber }}</strong>
            <span>Shot {{ shot.shotNumber }}</span>
          </div>
          <p>{{ shot.club }} | {{ shot.distanceYards }} yds</p>
          <p class="muted">{{ shot.result || 'No result note' }}</p>
          <button class="danger-button" type="button" @click="deleteShot(shot.id)">Delete</button>
        </article>
      </div>
    </div>
  </section>
</template>

<script setup>
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import api from '../api/api'

const route = useRoute()
const round = ref(null)
const scorecard = ref(null)
const loading = ref(false)
const savingShot = ref(false)
const errorMessage = ref('')

const shotForm = reactive({
  holeId: 0,
  shotNumber: 1,
  club: '',
  distanceYards: 0,
  result: ''
})

const selectedHoleShotCount = computed(() => {
  if (!round.value || !shotForm.holeId) {
    return 0
  }

  return round.value.shots.filter((shot) => shot.holeId === shotForm.holeId).length
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

async function refreshRound() {
  loading.value = true

  try {
    const [roundResponse, scorecardResponse] = await Promise.all([
      api.get(`/api/Rounds/${route.params.id}`),
      api.get(`/api/Rounds/${route.params.id}/scorecard`)
    ])

    round.value = roundResponse.data
    scorecard.value = scorecardResponse.data

    if (!shotForm.holeId && round.value.holes.length > 0) {
      shotForm.holeId = round.value.holes[0].holeId
    }
  } finally {
    loading.value = false
  }
}

async function addShot() {
  savingShot.value = true
  errorMessage.value = ''

  try {
    await api.post('/api/Shots', {
      roundId: Number(route.params.id),
      holeId: shotForm.holeId,
      shotNumber: shotForm.shotNumber,
      club: shotForm.club,
      distanceYards: shotForm.distanceYards,
      result: shotForm.result || null
    })

    shotForm.club = ''
    shotForm.distanceYards = 0
    shotForm.result = ''

    await refreshRound()
    shotForm.shotNumber = selectedHoleShotCount.value + 1
  } catch (error) {
    errorMessage.value = error?.response?.data || 'Unable to save the shot.'
  } finally {
    savingShot.value = false
  }
}

async function deleteShot(shotId) {
  await api.delete(`/api/Shots/${shotId}`)
  await refreshRound()
  shotForm.shotNumber = selectedHoleShotCount.value + 1
}

watch(
  () => shotForm.holeId,
  () => {
    shotForm.shotNumber = selectedHoleShotCount.value + 1
  }
)

onMounted(refreshRound)
</script>
