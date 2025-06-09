import {Component, HostListener, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {ElectronService} from "../../services/electron.service";
import {StateService} from "../../services/state.service";
import {ApiService} from "../../services/api.service";
import {finalize} from "rxjs/operators";
import {MatSnackBar} from "@angular/material/snack-bar";

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

    isSaving: () => boolean =
        () => this.stateService.getSaving();

    isProjectOpened: () => boolean =
        () => this.stateService.getProjectId() !== '';

    constructor(
        private electronService: ElectronService,
        private router: Router,
        private stateService: StateService,
        private apiService: ApiService,
        private snackBar: MatSnackBar
    ) {
    }

    ngOnInit(): void {
        this.isElectron = this.electronService.isElectron;
    }

    private tooltipMap: Record<string, string> = {
        'file-menu': 'File operations',
        'new-project': 'Create a new project',
        'recent-project': 'Go to recent projects',
        'save-project': 'Saves current project to an archive',
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
        // TODO: implement project creation logic (use new-project-dialog)
        alert('TODO: implement project creation logic (use new-project-dialog)');
    }

    recentProjects(event: Event): void {
        event.stopPropagation();
        this.router.navigate(['/projects']);
        this.hideTooltip();
        this.activeMenu = null;
    }

    saveProject(event: MouseEvent) {
        event.stopPropagation();
        this.hideTooltip();
        this.activeMenu = null;
        if (this.stateService.getProjectId() === '') {
            this.snackBar.open('No project is opened', 'Close', {
                duration: 3000,
                panelClass: ['error-snackbar']
            });
            return;
        }
        this.stateService.setSaving(true);
        this.apiService.saveProject(this.stateService.getProjectId())
            .pipe(finalize(() => {
                this.stateService.setSaving(false);
            }))
            .subscribe();
    }

    closeProject(event: MouseEvent) {
        event.stopPropagation();
        this.hideTooltip();
        this.activeMenu = null;
        if (this.stateService.getProjectId() === '') {
            this.snackBar.open('No project is opened', 'Close', {
                duration: 3000,
                panelClass: ['error-snackbar']
            });
            return;
        }
        this.stateService.setProjectId('');
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

            const offset = 50;
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
