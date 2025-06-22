/**
 * Represents a type archive list item from the API
 */
export interface TypeArchiveListItem {
  namespace: string;
  absoluteFilePath: string | null;
  typeCount: number;
}
