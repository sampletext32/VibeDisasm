import {Injectable} from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StateService {
  private saving: boolean = false;

  constructor() {
  }

  public setProjectId(projectId: string | undefined): void {
    if (projectId === undefined) {
      localStorage.removeItem('projectId');
      return;
    }
    localStorage.setItem('projectId', projectId);
  }

  public getProjectId(): string {
    return localStorage.getItem('projectId') || '';
  }

  public setSaving(saving: boolean): void {
    this.saving = saving;
  }

  public getSaving(): boolean {
    return this.saving;
  }
}
