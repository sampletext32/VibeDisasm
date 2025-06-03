import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Program {
  id: string;
  name: string;
  filePath: string;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = 'http://localhost:5201';

  constructor(private http: HttpClient) { }

  createProject(): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/create-project`, {});
  }

  listProjects(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/list-projects`);
  }

  importProgram(projectId: string): Observable<string> {
    const params = new HttpParams().set('projectId', projectId);
    return this.http.post<string>(`${this.baseUrl}/import-program`, {}, { params });
  }

  listPrograms(projectId: string): Observable<Program[]> {
    const params = new HttpParams().set('projectId', projectId);
    return this.http.get<Program[]>(`${this.baseUrl}/list-programs`, { params });
  }
}
