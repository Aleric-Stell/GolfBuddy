<!-- src/components/CourseList.vue -->
<template>
  <div>
    <CourseItem
      v-for="course in courses"
      :key="course.id"
      :course="course"
      @delete-course="$emit('delete-course', $event)"
    />
  </div>
</template>

<script>
import CourseItem from './CourseItem.vue'

export default {
  name: 'CourseList',
  props: {
    courses: { type: Array, required: true }
  },
  components: { CourseItem }
}
</script>