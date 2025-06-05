import { Component, OnInit } from '@angular/core';
import { ElectronService } from './services/electron.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    standalone: false
})
export class AppComponent implements OnInit {
  title = 'VibeDisasm';
  isElectron = false;

  constructor(private electronService: ElectronService) {}

  ngOnInit(): void {
    this.isElectron = this.electronService.isElectron;
    console.log('Running in Electron:', this.isElectron);
  }
}
