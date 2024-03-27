import { Component, OnInit } from '@angular/core';
import { environment } from '../../../environments/environment.development';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  ngOnInit(): void {
    if (typeof window !== "undefined") {
      window.location.href = `${environment.appUrl}/api/Account/Login`;
    }
  }
}
