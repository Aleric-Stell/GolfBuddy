// src/main.js
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'     // imports router/index.js
import { createPinia } from 'pinia' // imports store/index.js

const app = createApp(App)
app.use(router)    // registers Vue Router globally
app.use(createPinia()) // registers Pinia store globally
app.mount('#app')  // mounts Vue app to <div id="app"> in index.html