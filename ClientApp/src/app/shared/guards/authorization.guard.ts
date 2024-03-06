import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../../account/account.service';
import { map } from 'rxjs';
import { User } from '../models/user';
import { SharedService } from '../shared.service';
import { inject } from '@angular/core';

export const authorizationGuard: CanActivateFn = (route, state) => {
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
