import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {MatSnackBar} from '@angular/material/snack-bar';
import {finalize} from 'rxjs/operators';
import {ApiService} from '../../services/api.service';
import {Program} from "../../dtos/program";

@Component({
  selector: 'app-programs',
  templateUrl: './programs.component.html',
  styleUrls: ['./programs.component.scss'],
  standalone: false
})
export class ProgramsComponent implements OnInit {
  projectId: string = '';
  projectName: string = 'Project';
  programs: Program[] = [];
  loading = false;

  constructor(
    private apiService: ApiService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
  }

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
        }
      });
  }

  importProgram(): void {
    this.loading = true;
    this.apiService.importProgram(this.projectId)
      .pipe(finalize(() => {
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
        }
      });
  }

  goBack(): void {
    this.router.navigate(['/projects']);
  }
}
