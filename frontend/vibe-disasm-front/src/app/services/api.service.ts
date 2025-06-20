import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {environment} from 'src/environments/environment';
import {RecentMetadata} from "../dtos/recentMetadata";
import {Program} from "../dtos/program";

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

  deleteRecent(projectId: string): Observable<boolean> {
    return this.http.post<boolean>(`${this.baseUrl}/projects/delete-recent/${projectId}`, {});
  }

  importProgram(projectId: string): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/program/${projectId}/import`, {});
  }
  listPrograms(projectId: string): Observable<Program[]> {
    return this.http.get<Program[]>(`${this.baseUrl}/program/${projectId}/list`);
  }
  listingAt(projectId: string, programId: string, address: number): Observable<{}> {
    return this.http.get<{ }>(`${this.baseUrl}/listing/${projectId}/${programId}/ataddress/${address}`);
  }
}
