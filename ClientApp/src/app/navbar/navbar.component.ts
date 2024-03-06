import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AccountService } from '../account/account.service';
import { CommonModule, NgIf } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule, NgIf, CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
    constructor(public accountService: AccountService) {}
    logout(){
      this.accountService.logout();
    }
}
