import { Component } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)])
  });

  constructor(private authService: AuthService) {}

  OnSubmit() {
    const email = this.loginForm.value.email;
    const password = this.loginForm.value.password;
    this.authService.login(email!, password!)
      .subscribe({
       next: () => console.log('Login successful'),
       error: (error) => console.error('Login failed', error)
      });
  }

  GetData() {
    const time = this.authService.getTokenExpiration();
    this.authService.getData()
      .subscribe({
        next: (response) => console.log('Data received:', response, time),
        error: (error) => console.error('Error occurred:', error)
      }); 
  }

}
