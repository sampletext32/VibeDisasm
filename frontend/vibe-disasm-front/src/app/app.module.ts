import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi} from '@angular/common/http';
import {ReactiveFormsModule} from '@angular/forms';
import {ElectronService} from './services/electron.service';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {ProjectsComponent} from './components/projects/projects.component';
import {ProgramsComponent} from './components/programs/programs.component';
import {HeaderComponent} from './components/header/header.component';
import {NewProjectDialogComponent} from './components/new-project-dialog/new-project-dialog.component';
import {WorkspaceComponent} from "./components/workspace/workspace.component";
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {SplitPaneComponent} from "./components/shared/split-pane/split-pane.component";
import {ThreeSplitPaneComponent} from "./components/shared/three-split-pane/three-split-pane.component";

// Angular Material
import {MatDialogModule} from '@angular/material/dialog';
import {MatButtonModule} from '@angular/material/button';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSnackBarModule} from '@angular/material/snack-bar';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import {MatCardModule} from '@angular/material/card';
import {MatDividerModule} from '@angular/material/divider';
import {MatIconModule} from '@angular/material/icon';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatListModule} from '@angular/material/list';
import {MatTabsModule} from '@angular/material/tabs';
import {MatTableModule} from '@angular/material/table';
import {MatSortModule} from '@angular/material/sort';

// Interceptors
import {ErrorInterceptor} from './interceptors/error-interceptor';
import {ConfirmDialogComponent} from "./components/confirm-dialog/confirm-dialog.component";
import {CdkFixedSizeVirtualScroll, CdkVirtualForOf, CdkVirtualScrollViewport} from "@angular/cdk/scrolling";

@NgModule({
  declarations: [
    AppComponent,
    ProjectsComponent,
    ProgramsComponent,
    HeaderComponent,
    NewProjectDialogComponent,
    WorkspaceComponent,
    SplitPaneComponent,
    ThreeSplitPaneComponent,
    ConfirmDialogComponent,
  ],
  bootstrap: [AppComponent], imports: [BrowserModule,
    AppRoutingModule,
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
    MatListModule, CdkVirtualScrollViewport, CdkVirtualForOf, CdkFixedSizeVirtualScroll],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true},
    ElectronService,
    provideHttpClient(withInterceptorsFromDi())
  ]
})
export class AppModule {
}
