import { Component, OnInit } from '@angular/core';
import { PlayService } from './play.service';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-play',
  standalone: true,
  imports: [NgIf],
  templateUrl: './play.component.html',
  styleUrl: './play.component.css'
})
export class PlayComponent implements OnInit {
  message: string | undefined;

  constructor(private playService: PlayService) {}

  ngOnInit(): void {
    this.playService.getPlayers().subscribe({
      next: (response: any) => this.message = response.value.message,
      error: error => console.log(error)
    })
  }
}
