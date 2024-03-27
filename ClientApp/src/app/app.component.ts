import { Component } from '@angular/core';
import { NavbarComponent } from './navbar/navbar.component';
import { FooterComponent } from './footer/footer.component';
import { RouterOutlet } from '@angular/router';
import { CommonModule, NgIf } from '@angular/common';
import { AccountService } from './account/account.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,
            NavbarComponent, 
            FooterComponent,
            NgIf,
            CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'ClientApp';
  constructor(public accountService: AccountService){}
}
