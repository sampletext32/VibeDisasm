import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { MatSnackBar } from '@angular/material/snack-bar';

interface AssemblyInstruction {
  address: string;
  bytes: string;
  instruction: string;
}

interface DecompiledFunction {
  name: string;
  code: string;
}

@Component({
  selector: 'app-decompilation',
  templateUrl: './decompilation.component.html',
  styleUrls: ['./decompilation.component.scss']
})
export class DecompilationComponent implements OnInit {
  programId: string = '';
  programName: string = '';
  loading: boolean = false;
  
  // Assembly listing
  assemblyInstructions: AssemblyInstruction[] = [];
  
  // Decompiled code
  decompiled: DecompiledFunction = { name: '', code: '' };
  
  // No longer needed with standard HTML table

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private apiService: ApiService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.programId = this.route.snapshot.paramMap.get('id') || '';
    
    if (!this.programId) {
      this.router.navigate(['/projects']);
      return;
    }
    
    this.loadProgramDetails();
    this.loadStubData(); // In a real implementation, this would be replaced with API calls
  }

  loadProgramDetails(): void {
    // this.loading = true;
    // this.apiService.getProgramDetails(this.programId).subscribe({
    //   next: (program) => {
    //     this.programName = program.name;
    //     this.loading = false;
    //   },
    //   error: (error) => {
    //     console.error('Error loading program details:', error);
    //     this.loading = false;
    //     this.snackBar.open('Failed to load program details', 'Close', {
    //       duration: 5000,
    //       panelClass: ['error-snackbar']
    //     });
    //   }
    // });
  }

  loadStubData(): void {
    // Stub data for assembly listing - a simple function that calculates sum of two numbers
    this.assemblyInstructions = [
      { address: '0x00401000', bytes: '55', instruction: 'push ebp' },
      { address: '0x00401001', bytes: '8B EC', instruction: 'mov ebp, esp' },
      { address: '0x00401003', bytes: '83 EC 10', instruction: 'sub esp, 16' },
      { address: '0x00401006', bytes: '8B 45 08', instruction: 'mov eax, dword ptr [ebp+8]' },
      { address: '0x00401009', bytes: '8B 4D 0C', instruction: 'mov ecx, dword ptr [ebp+12]' },
      { address: '0x0040100C', bytes: '89 45 F8', instruction: 'mov dword ptr [ebp-8], eax' },
      { address: '0x0040100F', bytes: '89 4D FC', instruction: 'mov dword ptr [ebp-4], ecx' },
      { address: '0x00401012', bytes: '8B 55 F8', instruction: 'mov edx, dword ptr [ebp-8]' },
      { address: '0x00401015', bytes: '03 55 FC', instruction: 'add edx, dword ptr [ebp-4]' },
      { address: '0x00401018', bytes: '89 55 F4', instruction: 'mov dword ptr [ebp-12], edx' },
      { address: '0x0040101B', bytes: '8B 45 F4', instruction: 'mov eax, dword ptr [ebp-12]' },
      { address: '0x0040101E', bytes: '8B E5', instruction: 'mov esp, ebp' },
      { address: '0x00401020', bytes: '5D', instruction: 'pop ebp' },
      { address: '0x00401021', bytes: 'C3', instruction: 'ret' },
      { address: '0x00401022', bytes: '90', instruction: 'nop' },
      { address: '0x00401023', bytes: '90', instruction: 'nop' },
      { address: '0x00401024', bytes: '90', instruction: 'nop' },
      { address: '0x00401025', bytes: '90', instruction: 'nop' },
      { address: '0x00401026', bytes: '90', instruction: 'nop' },
      { address: '0x00401027', bytes: '90', instruction: 'nop' }
    ];

    // Stub data for decompiled code - showing the IR transformation pipeline
    this.decompiled = {
      name: 'add_numbers',
      code: `// Function: add_numbers
// Parameters: int a, int b
// Returns: int

int add_numbers(int a, int b) {
    // IR Block 0x00401000
    // Original assembly:
    //   push ebp
    //   mov ebp, esp
    //   sub esp, 16
    //   mov eax, dword ptr [ebp+8]
    //   mov ecx, dword ptr [ebp+12]
    //   mov dword ptr [ebp-8], eax
    //   mov dword ptr [ebp-4], ecx
    
    // IR Variable assignments:
    // var_8 = a (from parameter)
    // var_4 = b (from parameter)
    
    // IR Block 0x00401012
    // Original assembly:
    //   mov edx, dword ptr [ebp-8]
    //   add edx, dword ptr [ebp-4]
    //   mov dword ptr [ebp-12], edx
    
    int result = var_8 + var_4;  // IRAddExpr transformation
    
    // IR Block 0x0040101B
    // Original assembly:
    //   mov eax, dword ptr [ebp-12]
    //   mov esp, ebp
    //   pop ebp
    //   ret
    
    return result;
}`
    };
  }

  goBack(): void {
    this.router.navigate(['/program', this.programId]);
  }
}
