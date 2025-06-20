export interface UserProgramDatabaseEntry {
  $type: string;
  size: number;
  address: number;
  type: UserProgramDatabaseEntryType;
}

export interface UserProgramDatabaseEntryType {
  size: number;
  readOnly: boolean;
  semantic: string;
}
