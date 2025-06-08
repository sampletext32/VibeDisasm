import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

export interface Program {
  id: string;
  name: string;
  filePath: string;
}

// PE Data interfaces
export interface PeInfo {
  is64Bit: boolean;
  isDll: boolean;
  isExecutable: boolean;
  imageBase: number;
  entryPoint: number;
  sectionAlignment: number;
  fileAlignment: number;
  characteristics: number;
  subsystem: number;
  timestamp: number;
  machineType: number;
  numberOfSections: number;
}

export interface SectionInfo {
  name: string;
  virtualAddress: number;
  virtualSize: number;
  rawDataAddress: number;
  rawDataSize: number;
  characteristics: number;
  isExecutable: boolean;
  isReadable: boolean;
}

export interface EntryPointInfo {
  fileOffset: number;
  rva: number;
  source: string;
  description: string;
  computedView: string;
}

export interface ImportInfo {
  libraries: ImportLibrary[];
}

export interface ImportLibrary {
  name: string;
  functions: ImportFunction[];
}

export interface ImportFunction {
  name: string;
  ordinal: number;
  hint: number;
  address: number;
}

export interface ExportInfo {
  name: string;
  functions: ExportFunction[];
}

export interface ExportFunction {
  name: string;
  ordinal: number;
  address: number;
}

export interface ResourceInfo {
  entries: ResourceEntry[];
}

export interface ResourceEntry {
  id: number;
  name?: string;
  type: number;
  typeName?: string;
  language: number;
  offset: number;
  size: number;
  children?: ResourceEntry[];
}

export interface DelayImportInfo {
  libraries: DelayImportLibrary[];
}

export interface DelayImportLibrary {
  name: string;
  functions: DelayImportFunction[];
}

export interface DelayImportFunction {
  name: string;
  address: number;
}

export interface ExceptionInfo {
  entries: ExceptionEntry[];
}

export interface ExceptionEntry {
  address: number;
  size: number;
}

export interface TlsInfo {
  startAddressOfRawData: number;
  endAddressOfRawData: number;
  addressOfIndex: number;
  addressOfCallbacks: number;
  sizeOfZeroFill: number;
  characteristics: number;
}

export interface VersionInfo {
  key: string;
  value: string;
}

export interface StringTableInfo {
  id: number;
  value: string;
}

export interface Project {
  id: string;
  title: string;
  createdAt: string;
}

export interface RecentMetadata {
  projectId: string,
  path: string,
  lastOpened: string,
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  createProject(title: string): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/projects/create`, { title });
  }

  saveProject(projectId: string): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/projects/save/${projectId}`, {});
  }

  recentProjects(): Observable<RecentMetadata[]> {
    return this.http.get<RecentMetadata[]>(`${this.baseUrl}/projects/recent`);
  }

  openRecent(projectId: string): Observable<boolean> {
    return this.http.post<boolean>(`${this.baseUrl}/projects/open-recent/${projectId}`, {});
  }

  importProgram(projectId: string): Observable<string> {
    const params = new HttpParams().set('projectId', projectId);
    return this.http.post<string>(`${this.baseUrl}/programs/import`, {}, { params });
  }

  listPrograms(projectId: string): Observable<Program[]> {
    const params = new HttpParams().set('projectId', projectId);
    return this.http.get<Program[]>(`${this.baseUrl}/programs/byproject`, { params });
  }

  getPeInfo(programId: string): Observable<PeInfo> {
    return this.http.get<PeInfo>(`${this.baseUrl}/program/${programId}/pe/info`);
  }

  getSections(programId: string): Observable<SectionInfo[]> {
    return this.http.get<SectionInfo[]>(`${this.baseUrl}/program/${programId}/pe/sections`);
  }

  getTls(programId: string): Observable<TlsInfo | null> {
    return this.http.get<TlsInfo | null>(`${this.baseUrl}/program/${programId}/pe/tls`);
  }

  getEntryPoint(programId: string): Observable<EntryPointInfo[]> {
    return this.http.get<EntryPointInfo[]>(`${this.baseUrl}/program/${programId}/pe/entrypoint`);
  }

  getImports(programId: string): Observable<ImportInfo | null> {
    return this.http.get<ImportInfo | null>(`${this.baseUrl}/program/${programId}/pe/imports`);
  }

  getExports(programId: string): Observable<ExportInfo | null> {
    return this.http.get<ExportInfo | null>(`${this.baseUrl}/program/${programId}/pe/exports`);
  }

  getResources(programId: string): Observable<ResourceInfo | null> {
    return this.http.get<ResourceInfo | null>(`${this.baseUrl}/program/${programId}/pe/resources`);
  }

  getDelayImports(programId: string): Observable<DelayImportInfo | null> {
    return this.http.get<DelayImportInfo | null>(`${this.baseUrl}/program/${programId}/pe/delayimports`);
  }

  getExceptions(programId: string): Observable<ExceptionInfo | null> {
    return this.http.get<ExceptionInfo | null>(`${this.baseUrl}/program/${programId}/pe/exceptions`);
  }

  getVersionInfo(programId: string): Observable<VersionInfo[]> {
    return this.http.get<VersionInfo[]>(`${this.baseUrl}/program/${programId}/pe/version`);
  }

  getStringTable(programId: string): Observable<StringTableInfo[]> {
    return this.http.get<StringTableInfo[]>(`${this.baseUrl}/program/${programId}/pe/stringtable`);
  }
}
