<!-- src/components/CourseForm.vue -->
<template>
  <form @submit.prevent="submitForm">
    <input
      v-model="name"
      placeholder="Course Name"
      required
    />
    <input
      v-model="location"
      placeholder="Location (optional)"
    />
    <button type="submit">Add Course</button>
  </form>
</template>

<script>
export default {
  name: 'CourseForm',
  data() {
    return {
      name: '',
      location: ''
    }
  },
  methods: {
    submitForm() {
      const newCourse = { name: this.name, location: this.location || null }
      this.$emit('add-course', newCourse)
      this.name = ''
      this.location = ''
    }
  }
}
</script>

<style scoped>
form {
  margin-bottom: 16px;
}
input {
  margin-right: 8px;
  padding: 4px;
}
button {
  padding: 4px 8px;
}
</style>