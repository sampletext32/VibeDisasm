import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
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
    private router: Router
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
    this.apiService.listPrograms(this.projectId).subscribe({
      next: (programs) => {
        this.programs = programs;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading programs', error);
        this.loading = false;
      }
    });
  }

  importProgram(): void {
    this.loading = true;
    this.apiService.importProgram(this.projectId).subscribe({
      next: (programId) => {
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
