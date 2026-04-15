<template>
  <div class="app-shell">
    <header class="topbar">
      <div>
        <p class="eyebrow">Golf Buddy</p>
        <h1>Track rounds, shots, and scorecards</h1>
      </div>

      <nav v-if="auth.initialized" class="nav-links">
        <router-link v-if="auth.isAuthenticated" to="/rounds">Rounds</router-link>
        <router-link v-if="auth.isAdmin" to="/courses">Courses</router-link>
        <span v-if="auth.user" class="user-pill">
          {{ auth.user.userName }} | {{ auth.user.roles.join(', ') }}
        </span>
        <button v-if="auth.isAuthenticated" class="ghost-button" @click="logout">Log Out</button>
      </nav>
    </header>

    <main class="page-shell">
      <router-view />
    </main>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useAuthStore } from './stores/auth'

const router = useRouter()
const auth = useAuthStore()

function logout() {
  auth.logout()
  router.push({ name: 'login' })
}
</script>
