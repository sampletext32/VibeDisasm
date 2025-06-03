import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
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
    private apiService: ApiService
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
          // Return the project details to the caller
          const newProject: Project = {
            id: projectId,
            title: title,
            createdAt: new Date().toISOString()
          };
          this.dialogRef.close(newProject);
        },
        error: (error) => {
          this.isCreating = false;
          console.error('Error creating project', error);
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
