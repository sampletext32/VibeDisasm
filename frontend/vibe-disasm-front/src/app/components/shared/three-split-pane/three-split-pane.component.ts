import { Component, ElementRef, ViewChild, AfterViewInit, OnDestroy, Input } from '@angular/core';

@Component({
  selector: 'app-three-split-pane',
  templateUrl: './three-split-pane.component.html',
  styleUrl: './three-split-pane.component.scss',
  standalone: false
})
export class ThreeSplitPaneComponent implements AfterViewInit, OnDestroy {
  @ViewChild('leftPane') leftPane!: ElementRef<HTMLElement>;
  @ViewChild('middlePane') middlePane!: ElementRef<HTMLElement>;
  @ViewChild('rightPane') rightPane!: ElementRef<HTMLElement>;
  @ViewChild('leftResizer') leftResizer!: ElementRef<HTMLElement>;
  @ViewChild('rightResizer') rightResizer!: ElementRef<HTMLElement>;
  
  @Input() initialLeftWidth = 33; // Default left pane width in percentage
  @Input() initialMiddleWidth = 33; // Default middle pane width in percentage
  @Input() minPaneWidth = 10; // Minimum pane width in percentage
  @Input() maxPaneWidth = 80; // Maximum pane width in percentage
  
  private isResizingLeft = false;
  private isResizingRight = false;
  private leftMouseMoveHandler: ((e: MouseEvent) => void) | null = null;
  private rightMouseMoveHandler: ((e: MouseEvent) => void) | null = null;
  private mouseUpHandler: ((e: MouseEvent) => void) | null = null;

  ngAfterViewInit(): void {
    // Apply initial widths
    this.leftPane.nativeElement.style.width = `${this.initialLeftWidth}%`;
    this.middlePane.nativeElement.style.width = `${this.initialMiddleWidth}%`;
    
    // Calculate right pane width
    const rightWidth = 100 - this.initialLeftWidth - this.initialMiddleWidth;
    this.rightPane.nativeElement.style.width = `${rightWidth}%`;
    
    // Setup handlers
    this.leftMouseMoveHandler = this.resizeLeft.bind(this);
    this.rightMouseMoveHandler = this.resizeRight.bind(this);
    this.mouseUpHandler = this.stopResize.bind(this);

    // Add mousedown event listeners to the resizers
    this.leftResizer.nativeElement.addEventListener('mousedown', this.startLeftResize.bind(this));
    this.rightResizer.nativeElement.addEventListener('mousedown', this.startRightResize.bind(this));
  }

  ngOnDestroy(): void {
    // Clean up event listeners
    this.stopResize();
  }

  startLeftResize(e: MouseEvent): void {
    e.preventDefault();
    this.isResizingLeft = true;
    this.leftResizer.nativeElement.classList.add('active');
    
    // Add event listeners for mouse movement and release
    document.addEventListener('mousemove', this.leftMouseMoveHandler!);
    document.addEventListener('mouseup', this.mouseUpHandler!);
  }

  startRightResize(e: MouseEvent): void {
    e.preventDefault();
    this.isResizingRight = true;
    this.rightResizer.nativeElement.classList.add('active');
    
    // Add event listeners for mouse movement and release
    document.addEventListener('mousemove', this.rightMouseMoveHandler!);
    document.addEventListener('mouseup', this.mouseUpHandler!);
  }

  resizeLeft(e: MouseEvent): void {
    if (!this.isResizingLeft) return;

    const container = this.leftPane.nativeElement.parentElement!;
    const containerRect = container.getBoundingClientRect();
    
    // Calculate the new width for left pane as a percentage of the container width
    const newLeftWidth = ((e.clientX - containerRect.left) / containerRect.width) * 100;
    
    // Apply constraints
    const constrainedLeftWidth = Math.min(Math.max(newLeftWidth, this.minPaneWidth), this.maxPaneWidth);
    
    // Get current total width of left and middle panes
    const currentLeftWidth = parseFloat(this.leftPane.nativeElement.style.width);
    const currentMiddleWidth = parseFloat(this.middlePane.nativeElement.style.width);
    const totalWidth = currentLeftWidth + currentMiddleWidth;
    
    // Calculate new middle width to maintain the total
    const newMiddleWidth = totalWidth - constrainedLeftWidth;
    
    // Only apply if middle width is within constraints
    if (newMiddleWidth >= this.minPaneWidth && newMiddleWidth <= this.maxPaneWidth) {
      this.leftPane.nativeElement.style.width = `${constrainedLeftWidth}%`;
      this.middlePane.nativeElement.style.width = `${newMiddleWidth}%`;
    }
  }

  resizeRight(e: MouseEvent): void {
    if (!this.isResizingRight) return;

    const container = this.leftPane.nativeElement.parentElement!;
    const containerRect = container.getBoundingClientRect();
    
    // Calculate the left pane width
    const leftWidth = parseFloat(this.leftPane.nativeElement.style.width);
    
    // Calculate the position where the middle pane starts
    const middleStartPosition = containerRect.left + (containerRect.width * leftWidth / 100);
    
    // Calculate the new width for middle pane as a percentage
    const newMiddleWidth = ((e.clientX - middleStartPosition) / containerRect.width) * 100;
    
    // Apply constraints
    const constrainedMiddleWidth = Math.min(Math.max(newMiddleWidth, this.minPaneWidth), this.maxPaneWidth);
    
    // Get current total width of middle and right panes
    const currentMiddleWidth = parseFloat(this.middlePane.nativeElement.style.width);
    const currentRightWidth = parseFloat(this.rightPane.nativeElement.style.width);
    const totalWidth = currentMiddleWidth + currentRightWidth;
    
    // Calculate new right width to maintain the total
    const newRightWidth = totalWidth - constrainedMiddleWidth;
    
    // Only apply if right width is within constraints
    if (newRightWidth >= this.minPaneWidth && newRightWidth <= this.maxPaneWidth) {
      this.middlePane.nativeElement.style.width = `${constrainedMiddleWidth}%`;
      this.rightPane.nativeElement.style.width = `${newRightWidth}%`;
    }
  }

  stopResize(): void {
    if (this.isResizingLeft) {
      this.isResizingLeft = false;
      document.removeEventListener('mousemove', this.leftMouseMoveHandler!);
      if (this.leftResizer?.nativeElement) {
        this.leftResizer.nativeElement.classList.remove('active');
      }
    }
    
    if (this.isResizingRight) {
      this.isResizingRight = false;
      document.removeEventListener('mousemove', this.rightMouseMoveHandler!);
      if (this.rightResizer?.nativeElement) {
        this.rightResizer.nativeElement.classList.remove('active');
      }
    }
    
    // Remove mouseup event listener
    document.removeEventListener('mouseup', this.mouseUpHandler!);
  }
}
