import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule, NgIf } from '@angular/common';
import { ReplaySubject } from 'rxjs';
import { User } from '../shared/models/user';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule, NgIf, CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
    loginForm: FormGroup = new FormGroup({});
    submitted = false;
    errorMessages: string[] = [];
    private userSource = new ReplaySubject<User | null>(1);
    user$ = this.userSource.asObservable();

    constructor(private formBuilder: FormBuilder,
      private router: Router,
      private http: HttpClient) {}

    login() {
      this.http.post<any>(`${environment.appUrl}/api/Account/Login`, { title: 'Login POST Request' }).subscribe(r=>{});
    }
}
