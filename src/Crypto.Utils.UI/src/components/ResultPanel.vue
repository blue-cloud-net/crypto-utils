<script setup lang="ts">
defineProps<{
  error?: string | null
}>()
</script>

<template>
  <el-alert
    v-if="error"
    type="error"
    :title="error"
    :closable="false"
    show-icon
    class="result-panel"
  />
</template>

<style scoped>
.result-panel {
  margin-top: 16px;
}
</style>
