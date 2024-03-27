import { Injectable } from '@angular/core';
import { ReplaySubject, map } from 'rxjs';
import { User } from '../shared/models/user';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment.development';

@Injectable({
    providedIn: 'root'
  })

  export class AccountService {
    private userSource = new ReplaySubject<User | null>(1);
    user$ = this.userSource.asObservable();

    constructor(private http: HttpClient, private router: Router) { }

    authorized() {
        return this.http.get<User>(`${environment.appUrl}/api/account/currentUser`).pipe(
            map((user: User) => {
                if (user) {
                    this.userSource.next(user);
                }
            })
          );
    }

    logout() {
        this.userSource.next(null);
        if (typeof window !== "undefined") {
            window.location.href = `${environment.appUrl}/api/Account/Logout`;
        }
    }
  }