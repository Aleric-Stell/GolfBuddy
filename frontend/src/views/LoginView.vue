<template>
  <section class="auth-page">
    <div class="auth-card card">
      <p class="eyebrow">Sign In</p>
      <h2>Use the seeded test account</h2>
      <p class="muted">
        Admin: <code>admin@golfbuddy.local</code> / <code>GolfBuddy123!</code>
      </p>
      <p class="muted">
        Player: <code>player@golfbuddy.local</code> / <code>GolfBuddy123!</code>
      </p>

      <form class="stack-form" @submit.prevent="submit">
        <label>
          Email
          <input v-model="form.email" type="email" required />
        </label>

        <label>
          Password
          <input v-model="form.password" type="password" required />
        </label>

        <p v-if="errorMessage" class="error-text">{{ errorMessage }}</p>
        <button class="primary-button" :disabled="loading" type="submit">
          {{ loading ? 'Signing In...' : 'Sign In' }}
        </button>
      </form>
    </div>
  </section>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const auth = useAuthStore()
const loading = ref(false)
const errorMessage = ref('')

const form = reactive({
  email: 'admin@golfbuddy.local',
  password: 'GolfBuddy123!'
})

async function submit() {
  loading.value = true
  errorMessage.value = ''

  try {
    await auth.login(form)
    router.push({ name: 'rounds' })
  } catch (error) {
    errorMessage.value = error?.response?.data || 'Sign in failed.'
  } finally {
    loading.value = false
  }
}
</script>
