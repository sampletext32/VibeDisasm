import {Component, OnInit} from '@angular/core';
import {FormControl, FormGroup, UntypedFormBuilder, UntypedFormGroup, Validators} from '@angular/forms';
import {MatDialogRef} from '@angular/material/dialog';
import {ApiService} from '../../services/api.service';
import {StateService} from "../../services/state.service";
import {Project} from "../../services/project";

@Component({
    selector: 'app-new-project-dialog',
    templateUrl: './new-project-dialog.component.html',
    styleUrls: ['./new-project-dialog.component.scss'],
    standalone: false
})
export class NewProjectDialogComponent implements OnInit {
    projectForm: FormGroup<
        {
            title: FormControl<string>
        }
    > = this.fb.group({
        title: ['', [Validators.required]]
    });

    isCreating = false;

    constructor(
        private fb: UntypedFormBuilder,
        private dialogRef: MatDialogRef<NewProjectDialogComponent>,
        private apiService: ApiService
    ) {
    }

    ngOnInit(): void {
        this.projectForm = this.fb.group({
            title: ['', [Validators.required]]
        });
    }

    onSubmit(): void {
        if (this.projectForm.valid && !this.isCreating) {
            this.isCreating = true;
            const title = this.projectForm.value.title!;

            this.apiService.createProject(title).subscribe({
                next: (projectId) => {
                    this.isCreating = false;
                    this.dialogRef.close(projectId);
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
