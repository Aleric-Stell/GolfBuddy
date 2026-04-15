import { defineStore } from 'pinia'
import api from '../api/api'

const TOKEN_KEY = 'golfbuddy.token'
const USER_KEY = 'golfbuddy.user'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem(TOKEN_KEY) || '',
    user: JSON.parse(localStorage.getItem(USER_KEY) || 'null'),
    initialized: false
  }),
  getters: {
    isAuthenticated: (state) => Boolean(state.token),
    isAdmin: (state) => state.user?.roles?.includes('Admin') ?? false
  },
  actions: {
    async initialize() {
      if (!this.token) {
        this.initialized = true
        return
      }

      try {
        const { data } = await api.get('/api/Auth/me')
        this.user = data
        localStorage.setItem(USER_KEY, JSON.stringify(data))
      } catch {
        this.logout()
      } finally {
        this.initialized = true
      }
    },
    async login(credentials) {
      const { data } = await api.post('/api/Auth/login', credentials)
      this.token = data.token
      this.user = data.user
      localStorage.setItem(TOKEN_KEY, data.token)
      localStorage.setItem(USER_KEY, JSON.stringify(data.user))
    },
    logout() {
      this.token = ''
      this.user = null
      this.initialized = true
      localStorage.removeItem(TOKEN_KEY)
      localStorage.removeItem(USER_KEY)
    }
  }
})

export { TOKEN_KEY }
