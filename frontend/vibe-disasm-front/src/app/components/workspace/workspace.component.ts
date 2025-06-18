import { Component } from '@angular/core';
import { SplitPaneComponent } from '../shared/split-pane/split-pane.component';

@Component({
  selector: 'app-workspace',
  templateUrl: './workspace.component.html',
  styleUrl: './workspace.component.scss',
  standalone: false
})
export class WorkspaceComponent {
  // Configuration for the split pane
  initialLeftWidth = 40; // 40% initial width for the left pane
}
