import { Component, ElementRef, ViewChild, AfterViewInit, OnDestroy, Input } from '@angular/core';

@Component({
  selector: 'app-split-pane',
  templateUrl: './split-pane.component.html',
  styleUrl: './split-pane.component.scss',
  standalone: false
})
export class SplitPaneComponent implements AfterViewInit, OnDestroy {
  @ViewChild('leftPane') leftPane!: ElementRef<HTMLElement>;
  @ViewChild('rightPane') rightPane!: ElementRef<HTMLElement>;
  @ViewChild('resizer') resizer!: ElementRef<HTMLElement>;
  
  @Input() initialLeftWidth = 40; // Default left pane width in percentage
  @Input() minLeftWidth = 10; // Minimum left pane width in percentage
  @Input() maxLeftWidth = 90; // Maximum left pane width in percentage
  
  private isResizing = false;
  private mouseDownHandler: ((e: MouseEvent) => void) | null = null;
  private mouseMoveHandler: ((e: MouseEvent) => void) | null = null;
  private mouseUpHandler: ((e: MouseEvent) => void) | null = null;

  ngAfterViewInit(): void {
    // Apply initial width
    this.leftPane.nativeElement.style.width = `${this.initialLeftWidth}%`;
    
    // Setup global event listeners
    this.mouseDownHandler = this.startResize.bind(this);
    this.mouseMoveHandler = this.resize.bind(this);
    this.mouseUpHandler = this.stopResize.bind(this);

    // Add mousedown event listener to the resizer
    this.resizer.nativeElement.addEventListener('mousedown', this.mouseDownHandler);
  }

  ngOnDestroy(): void {
    // Clean up event listeners
    if (this.mouseDownHandler && this.resizer?.nativeElement) {
      this.resizer.nativeElement.removeEventListener('mousedown', this.mouseDownHandler);
    }
    this.stopResize();
  }

  startResize(e: MouseEvent): void {
    e.preventDefault();
    this.isResizing = true;
    this.resizer.nativeElement.classList.add('active');
    
    // Add event listeners for mouse movement and release
    document.addEventListener('mousemove', this.mouseMoveHandler!);
    document.addEventListener('mouseup', this.mouseUpHandler!);
  }

  resize(e: MouseEvent): void {
    if (!this.isResizing) return;

    const container = this.leftPane.nativeElement.parentElement!;
    const containerRect = container.getBoundingClientRect();
    
    // Calculate the new width as a percentage of the container width
    const newWidth = ((e.clientX - containerRect.left) / containerRect.width) * 100;
    
    // Apply constraints
    const constrainedWidth = Math.min(Math.max(newWidth, this.minLeftWidth), this.maxLeftWidth);
    
    // Apply the new width to the left pane
    this.leftPane.nativeElement.style.width = `${constrainedWidth}%`;
  }

  stopResize(): void {
    if (!this.isResizing) return;
    
    this.isResizing = false;
    if (this.resizer?.nativeElement) {
      this.resizer.nativeElement.classList.remove('active');
    }
    
    // Remove event listeners
    document.removeEventListener('mousemove', this.mouseMoveHandler!);
    document.removeEventListener('mouseup', this.mouseUpHandler!);
  }
}
