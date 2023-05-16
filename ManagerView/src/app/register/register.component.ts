import { Component } from '@angular/core';
import { AuthService } from '../services/auth/auth.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';

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

  constructor(private authService: AuthService, private router: Router) {}

  OnSubmit() {
    const email = this.registrationForm.value.email;
    const password = this.registrationForm.value.password;
    this.authService.register(email!, password!)
    .subscribe({
      next: () => {
       console.log('Register successful');
       this.router.navigate(['/home']).then(() => {
        window.location.reload();
      });
      },
      error: (error) => console.error('Register failed', error)
     });
  }
}
