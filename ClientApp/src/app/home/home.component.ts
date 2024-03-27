import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AccountService } from '../account/account.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent  {

  constructor(private accountService: AccountService) { }

  public go(){

    this.accountService.authorized().subscribe({});
  }

}
