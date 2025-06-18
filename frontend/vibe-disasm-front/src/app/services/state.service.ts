import {Injectable} from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StateService {
  private saving: boolean = false;

  constructor() {
  }

  public setProjectId(projectId: string): void {
    localStorage.setItem('projectId', projectId);
  }

  public getProjectId(): string {
    let projectId = localStorage.getItem('projectId') ?? '';
    return projectId ?? '';
  }

  public setSaving(saving: boolean): void {
    this.saving = saving;
  }

  public getSaving(): boolean {
    return this.saving;
  }
}
