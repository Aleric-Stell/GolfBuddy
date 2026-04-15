import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import LoginView from '../views/LoginView.vue'
import CoursesView from '../views/CoursesView.vue'
import RoundsView from '../views/RoundsView.vue'
import RoundDetailView from '../views/RoundDetailView.vue'

const routes = [
  { path: '/', redirect: '/rounds' },
  { path: '/login', name: 'login', component: LoginView, meta: { guestOnly: true } },
  { path: '/courses', name: 'courses', component: CoursesView, meta: { requiresAuth: true, requiresAdmin: true } },
  { path: '/rounds', name: 'rounds', component: RoundsView, meta: { requiresAuth: true } },
  { path: '/rounds/:id', name: 'round-detail', component: RoundDetailView, meta: { requiresAuth: true } }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to) => {
  const authStore = useAuthStore()

  if (!authStore.initialized) {
    return true
  }

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return { name: 'login' }
  }

  if (to.meta.guestOnly && authStore.isAuthenticated) {
    return { name: 'rounds' }
  }

  if (to.meta.requiresAdmin && !authStore.isAdmin) {
    return { name: 'rounds' }
  }

  return true
})

export default router
