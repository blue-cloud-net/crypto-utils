/**
 * 证书主体信息（SubjectInfo）
 * 与后端 SubjectInfo 对应，字段名保持后端大写形式
 */
export interface SubjectInfo {
  /** Common Name（必填） */
  CN: string
  /** Organization */
  O?: string | null
  /** Organizational Unit */
  OU?: string | null
  /** Country（2 位） */
  C?: string | null
  /** State/Province */
  ST?: string | null
  /** Locality */
  L?: string | null
  /** Email */
  E?: string | null
}

/** 构造一个空的 SubjectInfo */
export function createEmptySubject(): SubjectInfo {
  return { CN: '', O: null, OU: null, C: null, ST: null, L: null, E: null }
}

/** 将表单中的空字符串规范化为 null，避免后端收到多余空字段 */
export function normalizeSubject(subject: SubjectInfo): SubjectInfo {
  const result: SubjectInfo = { CN: subject.CN.trim() }
  ;(['O', 'OU', 'C', 'ST', 'L', 'E'] as const).forEach((field) => {
    const value = subject[field]
    if (value !== undefined && value !== null && value.trim() !== '') {
      result[field] = value.trim()
    }
  })
  return result
}
