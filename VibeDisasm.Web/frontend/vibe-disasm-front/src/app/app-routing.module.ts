import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {ProjectsComponent} from './components/projects/projects.component';
import {ProgramsComponent} from './components/programs/programs.component';
import {ProgramDetailComponent} from './components/program-detail/program-detail.component';
import {WorkspaceComponent} from "./components/workspace/workspace.component";

const routes: Routes = [
  {path: '', redirectTo: '/projects', pathMatch: 'full'},
  {path: 'projects', component: ProjectsComponent},
  {path: 'programs/:projectId', component: ProgramsComponent},
  {path: 'program/:id', component: ProgramDetailComponent},
  {path: 'workspace/:id', component: WorkspaceComponent},
  {path: '**', redirectTo: '/projects'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
