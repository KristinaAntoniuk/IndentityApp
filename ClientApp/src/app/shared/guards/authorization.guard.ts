import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../../account/account.service';
import { Observable, map } from 'rxjs';
import { User } from '../models/account/user';
import { SharedService } from '../shared.service';
import { inject } from '@angular/core';

export const authorizationGuard: CanActivateFn = (route, state) : Observable<boolean> => {
  const accountService: AccountService = inject(AccountService);
  const sharedService: SharedService = inject(SharedService);
  const router: Router = inject(Router);

  return accountService.user$.pipe(
    map((user: User | null) => {
      if (user) {
        return true;
      }
      else {
        sharedService.open(false, 'Restricted Area', 'Leave immidiately!');
        router.navigate(['account/login'], {queryParams: {returnUrl: state.url}});
        return false;
      }
    })
  );
};
