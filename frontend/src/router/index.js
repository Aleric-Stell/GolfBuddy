import { createRouter, createWebHistory } from 'vue-router'
import CoursesView from "../views/CoursesView.vue";

const routes = [
  { path: '/', redirect: '/courses' },
  { path: '/courses', component: CoursesView }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router