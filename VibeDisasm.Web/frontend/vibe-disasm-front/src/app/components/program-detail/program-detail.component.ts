import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';
import { ApiService, Program, PeInfo, SectionInfo, TlsInfo, EntryPointInfo, ImportInfo, ExportInfo, ResourceInfo, DelayImportInfo, ExceptionInfo, VersionInfo, StringTableInfo } from '../../services/api.service';

@Component({
    selector: 'app-program-detail',
    templateUrl: './program-detail.component.html',
    styleUrls: ['./program-detail.component.scss'],
    standalone: false
})
export class ProgramDetailComponent implements OnInit {
  programId: string = '';
  programName: string = '';
  loading: boolean = false;
  activeTab: string = 'overview';

  // Data containers for PE information
  peInfo: PeInfo | null = null;
  sections: SectionInfo[] = [];
  tlsInfo: TlsInfo | null = null;
  entryPoints: EntryPointInfo[] = [];
  imports: ImportInfo | null = null;
  exports: ExportInfo | null = null;
  resources: ResourceInfo | null = null;
  delayImports: DelayImportInfo | null = null;
  exceptions: ExceptionInfo | null = null;
  versionInfo: VersionInfo[] = [];
  stringTable: StringTableInfo[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private apiService: ApiService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.programId = this.route.snapshot.paramMap.get('id') || '';
    if (!this.programId) {
      this.router.navigate(['/projects']);
      return;
    }

    this.loadProgramDetails();
  }

  loadProgramDetails(): void {
    this.loading = true;
    // Set a default program name based on the ID
    this.programName = 'Program';
    this.loadPeInfo();
  }

  goBack(): void {
    this.router.navigate(['/programs']);
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;

    // Load data based on the selected tab
    switch (tab) {
      case 'pe-info':
        this.loadPeInfo();
        break;
      case 'sections':
        this.loadSections();
        break;
      case 'tls':
        this.loadTls();
        break;
      case 'entry-points':
        this.loadEntryPoints();
        break;
      case 'imports':
        this.loadImports();
        break;
      case 'exports':
        this.loadExports();
        break;
      case 'resources':
        this.loadResources();
        break;
      case 'delay-imports':
        this.loadDelayImports();
        break;
      case 'exceptions':
        this.loadExceptions();
        break;
      case 'version-info':
        this.loadVersionInfo();
        break;
      case 'string-table':
        this.loadStringTable();
        break;
    }
  }

  loadPeInfo(): void {
    if (this.peInfo) return; // Already loaded

    this.loading = true;
    this.apiService.getPeInfo(this.programId)
      .pipe(
        catchError(error => {
          // Error handling is now managed by the HTTP interceptor
          return of(null);
        }),
        finalize(() => this.loading = false)
      )
      .subscribe(data => {
        this.peInfo = data;
      });
  }

  loadSections(): void {
    if (this.sections.length > 0) return; // Already loaded

    this.loading = true;
    this.apiService.getSections(this.programId)
      .pipe(
        catchError(error => {
          // Error handling is now managed by the HTTP interceptor
          return of([]);
        }),
        finalize(() => this.loading = false)
      )
      .subscribe(data => {
        this.sections = data;
      });
  }

  loadTls(): void {
    if (this.tlsInfo) return; // Already loaded

    this.loading = true;
    this.apiService.getTls(this.programId)
      .pipe(
        catchError(error => {
          // Error handling is now managed by the HTTP interceptor
          return of(null);
        }),
        finalize(() => this.loading = false)
      )
      .subscribe(data => {
        this.tlsInfo = data;
      });
  }

  loadEntryPoints(): void {
    if (this.entryPoints.length > 0) return; // Already loaded

    this.loading = true;
    this.apiService.getEntryPoint(this.programId)
      .pipe(
        catchError(error => {
          // Error handling is now managed by the HTTP interceptor
          return of([]);
        }),
        finalize(() => this.loading = false)
      )
      .subscribe(data => {
        this.entryPoints = data;
      });
  }

  loadImports(): void {
    if (this.imports) return; // Already loaded

    this.loading = true;
    this.apiService.getImports(this.programId)
      .pipe(
        catchError(error => {
          // Error handling is now managed by the HTTP interceptor
          return of(null);
        }),
        finalize(() => this.loading = false)
      )
      .subscribe(data => {
        this.imports = data;
      });
  }

  loadExports(): void {
    if (this.exports) return; // Already loaded

    this.loading = true;
    this.apiService.getExports(this.programId)
      .pipe(
        catchError(error => {
          // Error handling is now managed by the HTTP interceptor
          return of(null);
        }),
        finalize(() => this.loading = false)
      )
      .subscribe(data => {
        this.exports = data;
      });
  }

  loadResources(): void {
    if (this.resources) return; // Already loaded

    this.loading = true;
    this.apiService.getResources(this.programId)
      .pipe(
        catchError(error => {
          // Error handling is now managed by the HTTP interceptor
          return of(null);
        }),
        finalize(() => this.loading = false)
      )
      .subscribe(data => {
        this.resources = data;
      });
  }

  loadDelayImports(): void {
    if (this.delayImports) return; // Already loaded

    this.loading = true;
    this.apiService.getDelayImports(this.programId)
      .pipe(
        catchError(error => {
          // Error handling is now managed by the HTTP interceptor
          return of(null);
        }),
        finalize(() => this.loading = false)
      )
      .subscribe(data => {
        this.delayImports = data;
      });
  }

  loadExceptions(): void {
    if (this.exceptions) return; // Already loaded

    this.loading = true;
    this.apiService.getExceptions(this.programId)
      .pipe(
        catchError(error => {
          // Error handling is now managed by the HTTP interceptor
          return of(null);
        }),
        finalize(() => this.loading = false)
      )
      .subscribe(data => {
        this.exceptions = data;
      });
  }

  loadVersionInfo(): void {
    if (this.versionInfo.length > 0) return; // Already loaded

    this.loading = true;
    this.apiService.getVersionInfo(this.programId)
      .pipe(
        catchError(error => {
          // Error handling is now managed by the HTTP interceptor
          return of([]);
        }),
        finalize(() => this.loading = false)
      )
      .subscribe(data => {
        this.versionInfo = data;
      });
  }

  loadStringTable(): void {
    if (this.stringTable.length > 0) return; // Already loaded

    this.loading = true;
    this.apiService.getStringTable(this.programId)
      .pipe(
        catchError(error => {
          // Error handling is now managed by the HTTP interceptor
          return of([]);
        }),
        finalize(() => this.loading = false)
      )
      .subscribe(data => {
        this.stringTable = data;
      });
  }
}
