import { HttpEvent, HttpInterceptorFn } from '@angular/common/http';
import { AccountService } from '../../account/account.service';
import { inject } from '@angular/core';
import { Observable, take } from 'rxjs';

export const jwtInterceptor: HttpInterceptorFn = (req, next): Observable<HttpEvent<any>> => {
  const accountService: AccountService = inject(AccountService);
  accountService.user$.pipe(take(1)).subscribe({
    	next: user => {
        if (user) {
          req = req.clone({
            setHeaders: {
              Authorization: `Bearer ${user.jwt}`
            }
          });
        }
      }
  })
  return next(req);
};
