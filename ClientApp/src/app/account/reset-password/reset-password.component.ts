import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account.service';
import { SharedService } from '../../shared/shared.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { take } from 'rxjs';
import { User } from '../../shared/models/account/user';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgIf } from '@angular/common';
import { ValidationMessagesComponent } from '../../shared/components/errors/validation-messages/validation-messages.component';
import { ResetPassword } from '../../shared/models/account/resetPassword';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [ReactiveFormsModule,
            NgIf,
            ValidationMessagesComponent,
            RouterModule],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent implements OnInit {
  newPasswordForm: FormGroup = new FormGroup({});
  submitted = false;
  errorMessages: string[] = [];
  token: string | null = null;
  email: string | null = null;

  constructor(private accountService: AccountService,
    private formBuilder: FormBuilder,
    private sharedService: SharedService,
    private router: Router,
    private activatedRoute: ActivatedRoute) {
      this.accountService.user$.pipe(take(1)).subscribe({
        next: (user: User | null) => {
          if (user) {
            this.router.navigateByUrl('/');
          }
          else {  
            this.activatedRoute.queryParamMap.subscribe({
              next: (params: any) => {
                this.token = params.get('token');
                this.email = params.get('email');
              }
            })
          }
        }
      })
    }
    
  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.newPasswordForm = this.formBuilder.group({
      email: [this.email],
      token: [this.token],
      newPassword: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(15)]],
      confirmNewPassword: ['', Validators.required]
    },
    {
      validators: this.matchPasswords
    })
  }

  resetPassword() {
    this.submitted = true;
    this.errorMessages = [];

    if(this.newPasswordForm.valid) {    
      this.accountService.resetPassword(this.newPasswordForm.value).subscribe({
        next: (response: any) => {
          this.sharedService.open(true, response.value.title, response.value.message);
          this.router.navigateByUrl('/account/login');
        },
        error: error => {
          if (error.error.errors) {
            this.errorMessages = error.error.errors;
          } else {
            this.errorMessages.push(error.error);
          }
        }
      });
    }
  }

  matchPasswords(group: AbstractControl) {
    let newPassword: string = group.get('newPassword')?.value;
    let confirmNewPassword: string = group.get('confirmNewPassword')?.value;
    if (newPassword !== '' && confirmNewPassword !== '' && newPassword !== confirmNewPassword) {
        group.get('confirmNewPassword')?.setErrors({ mismatch: true });
        return { mismatch: true };
    } else {
        return null;
    }
  }

  cancel(){
    this.router.navigateByUrl('/account/login');
  }
}
