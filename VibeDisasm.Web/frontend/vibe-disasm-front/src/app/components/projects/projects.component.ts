import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { finalize } from 'rxjs/operators';
import { ApiService, Project } from '../../services/api.service';
import { NewProjectDialogComponent } from '../new-project-dialog/new-project-dialog.component';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.scss']
})
export class ProjectsComponent implements OnInit {
  projects: Project[] = [];
  loading = false;

  constructor(
    private apiService: ApiService,
    private router: Router,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.loading = true;
    this.apiService.listProjects()
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (projects) => {
          this.projects = projects;
        },
        error: (error) => {
          console.error('Error loading projects', error);
        }
      });
  }

  createProject(): void {
    const dialogRef = this.dialog.open(NewProjectDialogComponent, {
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // The dialog now returns the complete project object
        this.projects.push(result);
        // Refresh the project list to ensure we have the latest data
        this.loadProjects();
      }
    });
  }

  // Track loading state for each project
  openingProjectId: string | null = null;

  viewPrograms(project: Project): void {
    // Set loading state for this specific project
    this.openingProjectId = project.id;
    
    // Call the openProject endpoint
    this.apiService.openProject(project.id)
      .pipe(finalize(() => this.openingProjectId = null))
      .subscribe({
        next: () => {
          // Only navigate after successful response
          this.router.navigate(['/programs', project.id]);
        },
        error: (error) => {
          console.error('Error opening project', error);
          // Error handling is managed by the HTTP interceptor
        }
      });
  }
}
