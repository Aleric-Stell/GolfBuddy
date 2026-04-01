// src/store/index.js
import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    token: null
  }),
  actions: {
    login(userData, jwtToken) {
      this.user = userData
      this.token = jwtToken
    },
    logout() {
      this.user = null
      this.token = null
    }
  }
})