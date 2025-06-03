import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.scss']
})
export class ProjectsComponent implements OnInit {
  projects: string[] = [];
  loading = false;

  constructor(
    private apiService: ApiService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.loading = true;
    this.apiService.listProjects().subscribe({
      next: (projects) => {
        this.projects = projects;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading projects', error);
        this.loading = false;
      }
    });
  }

  createProject(): void {
    this.loading = true;
    this.apiService.createProject().subscribe({
      next: (projectId) => {
        this.projects.push(projectId);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error creating project', error);
        this.loading = false;
      }
    });
  }

  viewPrograms(projectId: string): void {
    this.router.navigate(['/programs', projectId]);
  }
}
