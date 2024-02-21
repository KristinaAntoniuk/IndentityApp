import { Injectable, inject } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationComponent } from './components/modals/notification/notification.component';

@Injectable({
  providedIn: 'root'
})
export class SharedService {

  constructor() { }

  private modalService = inject(NgbModal);

	open(isSuccess: boolean, title: string, message: string) {
		const modalRef = this.modalService.open(NotificationComponent);
		modalRef.componentInstance.message = message;
    modalRef.componentInstance.title = title;
    modalRef.componentInstance.isSuccess = isSuccess;
	}
}
