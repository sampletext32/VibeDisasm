import { Component, HostListener } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss'],
    standalone: false
})
export class HeaderComponent {
  activeMenu: string | null = null;
  tooltipVisible = false;
  tooltipText = '';
  tooltipTop = 0;
  tooltipLeft = 0;

  // Tooltip content mapping
  private tooltipMap: Record<string, string> = {
    'file-menu': 'File operations',
    'new-project': 'Create a new project',
    'open-project': 'Open an existing project',
    'edit-menu': 'Edit operations',
    'cut': 'Cut selected content',
    'copy': 'Copy selected content',
    'paste': 'Paste content from clipboard'
  };

  constructor(private router: Router) { }

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

  actionUnknown(event: MouseEvent) {
    event.stopPropagation();
    this.hideTooltip();
    this.activeMenu = null;
  }

  showTooltip(tooltipKey: string, event: MouseEvent): void {
    this.tooltipText = this.tooltipMap[tooltipKey] || '';
    if (this.tooltipText) {
      this.tooltipVisible = true;

      // Position the tooltip near the cursor
      const offset = 50; // Offset from cursor
      this.tooltipTop = event.clientY;
      this.tooltipLeft = event.clientX + offset;
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
