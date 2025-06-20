import {Component, OnInit} from '@angular/core';
import {SplitPaneComponent} from '../shared/split-pane/split-pane.component';
import {ApiService} from "../../services/api.service";
import {StateService} from "../../services/state.service";
import {ActivatedRoute, Router} from "@angular/router";
import {finalize} from "rxjs/operators";

@Component({
  selector: 'app-workspace',
  templateUrl: './workspace.component.html',
  styleUrl: './workspace.component.scss',
  standalone: false
})
export class WorkspaceComponent implements OnInit {
  constructor(
    private api: ApiService,
    private state: StateService,
    private route: ActivatedRoute,
    private router: Router,
  ) {
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const programId = params.get('programId');
      if (programId) {

        this.api.listingAt(this.state.getProjectId(), programId, 0)
          .pipe(finalize(() => {
          }))
          .subscribe({
            next: listing => {
            }
          });
      } else {
        this.router.navigate(['/projects']);
      }
    });
  }


}
