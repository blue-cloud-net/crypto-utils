<script setup lang="ts">
import { ref } from 'vue'

defineProps<{
  modelValue: string
  placeholder?: string
  rows?: number
}>()

const emit = defineEmits<{ (e: 'update:modelValue', value: string): void }>()

const dragging = ref(false)

function onInput(value: string): void {
  emit('update:modelValue', value)
}

function onDrop(event: DragEvent): void {
  dragging.value = false
  const file = event.dataTransfer?.files?.[0]
  if (!file) {
    return
  }
  const reader = new FileReader()
  reader.onload = () => {
    emit('update:modelValue', String(reader.result ?? ''))
  }
  reader.readAsText(file)
}
</script>

<template>
  <div
    class="key-text-area"
    :class="{ dragging }"
    @dragover.prevent="dragging = true"
    @dragleave.prevent="dragging = false"
    @drop.prevent="onDrop"
  >
    <el-input
      type="textarea"
      :model-value="modelValue"
      :placeholder="placeholder"
      :rows="rows ?? 8"
      resize="vertical"
      @update:model-value="onInput"
    />
    <div class="drop-hint" :class="{ visible: dragging }">Drop file to load</div>
  </div>
</template>

<style scoped>
.key-text-area {
  position: relative;
}
.drop-hint {
  position: absolute;
  inset: 0;
  display: none;
  align-items: center;
  justify-content: center;
  background: var(--el-color-primary-light-9);
  border: 2px dashed var(--el-color-primary);
  border-radius: 4px;
  color: var(--el-color-primary);
  font-size: 14px;
  pointer-events: none;
}
.drop-hint.visible {
  display: flex;
}
</style>
