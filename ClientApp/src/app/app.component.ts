import { Component, OnInit } from '@angular/core';
import { NavbarComponent } from './navbar/navbar.component';
import { FooterComponent } from './footer/footer.component';
import { RouterOutlet } from '@angular/router';
import { AccountService } from './account/account.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,
            NavbarComponent, 
            FooterComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {

  constructor(private accountService: AccountService) {}

  ngOnInit(): void {
    if(typeof window !== 'undefined'){
      this.refreshUser();
    }
  }

  private refreshUser() {
    const jwt = this.accountService.getJWT();
    if (jwt) {
      this.accountService.refreshUser(jwt).subscribe({
        next: _ => {},
        error: _ => {
          this.accountService.logout();
        }
      })
    }
    else {
      this.accountService.refreshUser(null).subscribe();
    }
  }
}
