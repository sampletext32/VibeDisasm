import {Component, HostListener, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {ElectronService} from "../../services/electron.service";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  standalone: false
})
export class HeaderComponent implements OnInit {
  activeMenu: string | null = null;
  tooltipVisible = false;
  tooltipText = '';
  tooltipTop = 0;
  tooltipLeft? = 0;
  tooltipRight? = 0;

  isElectron = false;

  constructor(
    private electronService: ElectronService,
    private router: Router) {
  }

  ngOnInit(): void {
    this.isElectron = this.electronService.isElectron;
    console.log('Running in Electron:', this.isElectron);
  }

  // Tooltip content mapping
  private tooltipMap: Record<string, string> = {
    'file-menu': 'File operations',
    'new-project': 'Create a new project',
    'open-project': 'Open an existing project',
    'edit-menu': 'Edit operations',
    'cut': 'Cut selected content',
    'copy': 'Copy selected content',
    'paste': 'Paste content from clipboard',
    'electron': 'This app can run in electron and web. Currently running in Electron',
    'non-electron': 'This app can run in electron and web. Currently running in a web browser',
  };

  toggleMenu(menu: string): void {
    this.activeMenu = this.activeMenu === menu ? null : menu;
  }

  createNewProject(event: Event): void {
    event.stopPropagation();
    this.hideTooltip();
    this.activeMenu = null;
    // Implement new project functionality
    console.log('Create new project');
  }

  openProject(event: Event): void {
    event.stopPropagation();
    this.hideTooltip();
    this.activeMenu = null;
    this.router.navigate(['/projects']);
  }

  actionUnknown($event: MouseEvent) {
    $event.stopPropagation();
    this.hideTooltip();
    this.activeMenu = null;
  }

  showTooltip(tooltipKey: string, event: MouseEvent, left: boolean = false): void {
    this.tooltipText = this.tooltipMap[tooltipKey] || '';
    if (this.tooltipText) {
      this.tooltipVisible = true;

      // Position the tooltip near the cursor
      const offset = 50; // Offset from cursor
      this.tooltipTop = event.clientY;
      if (!left) {
        this.tooltipLeft = event.clientX + offset;
        this.tooltipRight = undefined;
      } else {
        this.tooltipRight = offset;
        this.tooltipLeft = undefined;
      }
    }
  }

  hideTooltip(): void {
    this.tooltipVisible = false;
  }

  @HostListener('document:click', ['$event'])
  closeMenuOnOutsideClick(event: Event): void {
    if (!(event.target as HTMLElement).closest('.navigation-item')) {
      this.activeMenu = null;
    }
  }
}
