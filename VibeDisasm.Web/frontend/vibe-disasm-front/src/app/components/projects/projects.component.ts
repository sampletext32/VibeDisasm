import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {MatDialog} from '@angular/material/dialog';
import {finalize} from 'rxjs/operators';
import {ApiService, Project, RecentMetadata} from '../../services/api.service';
import {NewProjectDialogComponent} from '../new-project-dialog/new-project-dialog.component';
import {StateService} from "../../services/state.service";

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.scss'],
  standalone: false
})
export class ProjectsComponent implements OnInit {
  recents: RecentMetadata[] = [];
  loading = false;
  deleteProjectId: string | null = null;

  constructor(
    private apiService: ApiService,
    private router: Router,
    private dialog: MatDialog,
    private stateService: StateService
  ) {
  }

  ngOnInit(): void {
    this.loadRecents();
  }

  loadRecents(): void {
    this.loading = true;
    this.apiService.recentProjects()
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (recents) => {
          this.recents = recents;
        },
        error: (error) => {
          console.error('Error loading recent projects', error);
        }
      });
  }

  createProject(): void {
    const dialogRef = this.dialog.open(NewProjectDialogComponent, {
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {

        this.stateService.setProjectId(result);
        this.router.navigate(['/programs', result]);
      }
    });
  }

  // Track loading state for each project
  openingProjectId: string | null = null;

  openRecent(recent: RecentMetadata): void {
    // Set loading state for this specific project
    this.openingProjectId = recent.projectId;

    // Call the openProject endpoint
    this.apiService.openRecent(recent.projectId)
      .pipe(finalize(() => this.openingProjectId = null))
      .subscribe({
        next: () => {
          // Only navigate after successful response
          this.router.navigate(['/programs', recent.projectId]);
        },
        error: (error) => {
          console.error('Error opening recent', error);
          // Error handling is managed by the HTTP interceptor
        }
      });
  }

  deleteRecent(recent: RecentMetadata, $event: MouseEvent) {
    $event.stopPropagation();

    if (this.deleteProjectId !== null) {
      return;
    }

    this.deleteProjectId = recent.projectId;

    // Call the openProject endpoint
    this.apiService.deleteRecent(recent.projectId)
      .pipe(finalize(() => this.deleteProjectId = null))
      .subscribe({
        next: () => {
          this.recents = this.recents.filter(x => x.projectId !== recent.projectId);
        },
        error: (error) => {
          console.error('Error removing recent', error);
          // Error handling is managed by the HTTP interceptor
        }
      });
  }
}
