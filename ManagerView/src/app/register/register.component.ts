import { Component } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  registrationForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)])
  });

  constructor(private authService: AuthService) {}

  OnSubmit() {
    const email = this.registrationForm.value.email;
    const password = this.registrationForm.value.password;
    this.authService.register(email!, password!)
      .subscribe(response => {
        console.log('Registration successful');
      }, error => {
        console.error('Registration failed', error);
      });
  }
}
