import { NgClass } from '@angular/common';
import { Component, Input, inject } from '@angular/core';
import { NgbActiveModal, NgbModal, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [NgClass],
  providers: [NgbModalConfig, NgbModal],
  templateUrl: './notification.component.html',
  styleUrl: './notification.component.css'
})
export class NotificationComponent {
  @Input()  
    isSuccess: boolean = true;
    title: string = '';
    message: string = '';

    activeModal = inject(NgbActiveModal);
}
