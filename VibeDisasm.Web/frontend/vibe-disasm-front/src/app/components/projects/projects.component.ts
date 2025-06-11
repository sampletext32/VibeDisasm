import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {MatDialog} from '@angular/material/dialog';
import {finalize} from 'rxjs/operators';
import {ApiService} from '../../services/api.service';
import {NewProjectDialogComponent} from '../new-project-dialog/new-project-dialog.component';
import {StateService} from "../../services/state.service";
import {ConfirmDialogComponent, ConfirmDialogData} from "../confirm-dialog/confirm-dialog.component";
import {Project} from "../../services/project";
import {RecentMetadata} from "../../services/recentMetadata";

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

  openingProjectId: string | null = null;

  openRecent(recent: RecentMetadata): void {
    this.openingProjectId = recent.projectId;

    this.apiService.openRecent(recent.projectId)
      .pipe(finalize(() => this.openingProjectId = null))
      .subscribe({
        next: () => {
          this.router.navigate(['/programs', recent.projectId]);
        },
        error: (error) => {
          console.error('Error opening recent', error);
        }
      });
  }

  deleteRecent(recent: RecentMetadata, $event: MouseEvent) {
    $event.stopPropagation();

    if (this.deleteProjectId !== null) {
      return;
    }

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: <ConfirmDialogData>{
        text: 'Are you sure you want to remove this recent project? This action cannot be undone.'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteProjectId = recent.projectId;
        this.apiService.deleteRecent(recent.projectId)
          .pipe(finalize(() => this.deleteProjectId = null))
          .subscribe({
            next: () => {
              this.recents = this.recents.filter(x => x.projectId !== recent.projectId);
            },
            error: (error) => {
              console.error('Error removing recent', error);
            }
          });
      }
    });
  }
}
