import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { ElectronService } from './services/electron.service';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ProjectsComponent } from './components/projects/projects.component';
import { ProgramsComponent } from './components/programs/programs.component';
import { ProgramDetailComponent } from './components/program-detail/program-detail.component';
import { HeaderComponent } from './components/header/header.component';
import { NewProjectDialogComponent } from './components/new-project-dialog/new-project-dialog.component';
import { DecompilationComponent } from './components/decompilation/decompilation.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// Angular Material
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';

// Interceptors
import { ErrorInterceptor } from './interceptors/error-interceptor';

@NgModule({
  declarations: [
    AppComponent,
    ProjectsComponent,
    ProgramsComponent,
    HeaderComponent,
    NewProjectDialogComponent,
    ProgramDetailComponent,
    DecompilationComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatDividerModule,
    MatIconModule,
    MatTabsModule,
    MatTableModule,
    MatSortModule,
    MatToolbarModule,
    MatListModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    ElectronService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
