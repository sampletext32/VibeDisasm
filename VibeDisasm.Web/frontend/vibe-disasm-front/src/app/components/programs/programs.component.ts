import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { finalize } from 'rxjs/operators';
import { ApiService, Program } from '../../services/api.service';

@Component({
  selector: 'app-programs',
  templateUrl: './programs.component.html',
  styleUrls: ['./programs.component.scss']
})
export class ProgramsComponent implements OnInit {
  projectId: string = '';
  programs: Program[] = [];
  loading = false;

  constructor(
    private apiService: ApiService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('projectId');
      if (id) {
        this.projectId = id;
        this.loadPrograms();
      } else {
        this.router.navigate(['/projects']);
      }
    });
  }

  loadPrograms(): void {
    this.loading = true;
    this.apiService.listPrograms(this.projectId)
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (programs) => {
          this.programs = programs;
        },
        error: (error) => {
          console.error('Error loading programs', error);
          this.snackBar.open('Failed to load programs. Please try again.', 'Close', {
            duration: 5000,
            panelClass: ['error-snackbar']
          });
        }
      });
  }

  importProgram(): void {
    this.loading = true;
    this.apiService.importProgram(this.projectId)
      .pipe(finalize(() => {
        // Only set loading to false if we're not immediately loading programs
        // which would set its own loading state
      }))
      .subscribe({
        next: (programId) => {
          this.snackBar.open('Program imported successfully', 'Close', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });
          this.loadPrograms(); // Reload the programs list
        },
        error: (error) => {
          console.error('Error importing program', error);
          this.loading = false;
          this.snackBar.open('Failed to import program. Please try again.', 'Close', {
            duration: 5000,
            panelClass: ['error-snackbar']
          });
        }
      });
  }

  goBack(): void {
    this.router.navigate(['/projects']);
  }
}
