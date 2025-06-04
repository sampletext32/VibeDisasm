import { Injectable } from '@angular/core';
import { Pure } from '../decorators/pure.decorator';

@Injectable({
  providedIn: 'root'
})
export class ElectronService {
  private _isElectron: boolean;
  private _ipcRenderer: any;
  private _fs: any;
  private _path: any;

  constructor() {
    // Check if running in Electron
    this._isElectron = !!(window && window.process && (window.process as any).type);

    // If running in Electron, load only essential modules
    if (this._isElectron) {
      try {
        const electronRequire = window['require'];
        this._ipcRenderer = electronRequire('electron').ipcRenderer;
        this._fs = electronRequire('fs');
        this._path = electronRequire('path');
      } catch (error) {
        console.error('Error initializing Electron modules', error);
      }
    }
  }

  @Pure
  get isElectron(): boolean {
    return this._isElectron;
  }

  @Pure
  get ipcRenderer(): any {
    return this._ipcRenderer;
  }

  @Pure
  get fs(): any {
    return this._fs;
  }

  @Pure
  get path(): any {
    return this._path;
  }

  /**
   * Reload the application
   */
  @Pure
  reloadApp(): void {
    if (this._isElectron && this._ipcRenderer) {
      this._ipcRenderer.send('reload-app');
    } else {
      window.location.reload();
    }
  }
}
