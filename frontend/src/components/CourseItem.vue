<!-- src/components/CourseItem.vue -->
<template>
  <div class="course-item">
    <h3>{{ course.name }}</h3>
    <p v-if="course.location">Location: {{ course.location }}</p>
    <button @click="$emit('delete-course', course.id)">Delete</button>
  </div>
</template>

<script>
export default {
  name: 'CourseItem',
  props: {
    course: { type: Object, required: true }
  }
}
</script>

<style scoped>
.course-item {
  border: 1px solid #ccc;
  padding: 8px;
  margin-bottom: 8px;
  border-radius: 4px;
}
button {
  background-color: #ff4d4d;
  color: white;
  border: none;
  padding: 4px 8px;
  cursor: pointer;
  border-radius: 4px;
}
button:hover { background-color: #ff0000; }
</style>