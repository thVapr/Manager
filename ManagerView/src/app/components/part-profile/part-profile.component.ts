import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PartService } from '../../services/part/part.service';
import { Part } from '../models/part';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { AuthService } from 'src/app/services/auth/auth.service';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-part-profile',
    templateUrl: './part-profile.component.html',
    styleUrls: ['./part-profile.component.scss'],
    standalone: false,
    providers: [MessageService],
})
export class PartProfileComponent implements OnInit {
  part : Part = {};
  hasAccessForEdit : boolean = false;
  
  updatePartForm = new FormGroup({
    name: new FormControl('', [Validators.minLength(4)]),
    description: new FormControl('', [Validators.minLength(4)]),
  });

  constructor(private partService : PartService,
              private authService : AuthService,
              private route : Router,
              private app : AppComponent,
              private messageService : MessageService) {}

  ngOnInit(): void {
    this.update();
  }

  update() : void {
    this.partService.getPartById(this.partService.getPartId()!).subscribe((part) => {
      this.part = part;
      this.updatePartForm.setValue({
        name: part.name!,
        description: part.description!
      });
      this.partService.hasPrivileges(this.authService.getId(), this.part.id!, 5).subscribe((response) => {
        this.hasAccessForEdit = response;
        console.log(this.hasAccessForEdit);
      });
    });
  }

  updatePart() {
    let updatedPart : Part = {
      id : this.part.id!,
      name : this.updatePartForm.value.name!,
      description : this.updatePartForm.value.description!
    };
    this.partService.updatePart(updatedPart).subscribe(() => {
      this.update();
        this.messageService.add(
        { severity: 'success', summary: 'Успешное обновление', detail: 'Сущность была обновлена', life: 3000 }
      );
    });
  }

  removePart() {
    this.partService.remove(this.part.id!).subscribe(() => {
      this.partService.removePartData();
      this.app.updateMenuItems();
      this.route.navigate(['parts']);
    });
  }
}
