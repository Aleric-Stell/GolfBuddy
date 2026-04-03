<!-- src/views/CoursesView.vue -->
<template>
  <div>
    <h1>Courses</h1>
    <CourseForm @add-course="addCourse" />
    <CourseList
      :courses="courses"
      @delete-course="deleteCourse"
    />
  </div>
</template>

<script>
import CourseForm from '../components/CourseForm.vue'
import CourseList from '../components/CourseList.vue'
import axios from 'axios'

export default {
  name: 'CoursesView',
  components: { CourseForm, CourseList },
  data() {
    return {
      courses: []
    }
  },
  created() {
    this.fetchCourses()
  },
  methods: {
    async fetchCourses() {
      try {
        const res = await axios.get('http://localhost:5254/api/Courses')
        this.courses = res.data
      } catch (err) {
        console.error('Failed to fetch courses', err)
      }
    },
    async addCourse(newCourse) {
      try {
        const res = await axios.post('http://localhost:5254/api/Courses', newCourse)
        this.courses.push(res.data)
      } catch (err) {
        console.error('Failed to add course', err)
      }
    },
    async deleteCourse(courseId) {
      try {
        await axios.delete(`http://localhost:5254/api/Courses/${courseId}`)
        this.courses = this.courses.filter(c => c.id !== courseId)
      } catch (err) {
        console.error('Failed to delete course', err)
      }
    }
  }
}
</script>