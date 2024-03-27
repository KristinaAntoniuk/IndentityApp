import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AccountService } from '../account/account.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {

    constructor (private accountService: AccountService){}

    logout() {
      this.accountService.logout();
    }
}
