import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {ProjectsComponent} from './components/projects/projects.component';
import {ProgramsComponent} from './components/programs/programs.component';
import {WorkspaceComponent} from "./components/workspace/workspace.component";

const routes: Routes = [
  {path: '', redirectTo: '/projects', pathMatch: 'full'},
  {path: 'projects', component: ProjectsComponent},
  {path: 'programs/:projectId', component: ProgramsComponent},
  {path: 'workspace/:id', component: WorkspaceComponent},
  {path: '**', redirectTo: '/projects'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
