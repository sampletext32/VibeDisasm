import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

export interface Program {
  id: string;
  name: string;
  filePath: string;
}

export interface Project {
  id: string;
  title: string;
  createdAt: string;
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

  listProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(`${this.baseUrl}/projects/list`);
  }

  openProject(projectId: string): Observable<boolean> {
    return this.http.post<boolean>(`${this.baseUrl}/projects/open/${projectId}`, {});
  }

  importProgram(projectId: string): Observable<string> {
    const params = new HttpParams().set('projectId', projectId);
    return this.http.post<string>(`${this.baseUrl}/programs/import`, {}, { params });
  }

  listPrograms(projectId: string): Observable<Program[]> {
    const params = new HttpParams().set('projectId', projectId);
    return this.http.get<Program[]>(`${this.baseUrl}/programs/byproject`, { params });
  }
}
