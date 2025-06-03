import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ApiService, Project } from '../../services/api.service';

@Component({
  selector: 'app-new-project-dialog',
  templateUrl: './new-project-dialog.component.html',
  styleUrls: ['./new-project-dialog.component.scss']
})
export class NewProjectDialogComponent implements OnInit {
  projectForm: FormGroup = this.fb.group({
    title: ['', [Validators.required]]
  });
  
  isCreating = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<NewProjectDialogComponent>,
    private apiService: ApiService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.projectForm = this.fb.group({
      title: ['', [Validators.required]]
    });
  }

  onSubmit(): void {
    if (this.projectForm.valid && !this.isCreating) {
      this.isCreating = true;
      const title = this.projectForm.value.title;
      
      this.apiService.createProject(title).subscribe({
        next: (projectId) => {
          this.isCreating = false;
          // Return both the project ID and title to the caller
          const newProject: Project = {
            id: projectId,
            title: title
          };
          this.dialogRef.close(newProject);
        },
        error: (error) => {
          this.isCreating = false;
          this.snackBar.open('Failed to create project. Please try again.', 'Close', {
            duration: 5000,
            panelClass: ['error-snackbar']
          });
          console.error('Error creating project', error);
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
