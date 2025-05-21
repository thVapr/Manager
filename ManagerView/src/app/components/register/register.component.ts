import { Router } from '@angular/router';
import { Component } from '@angular/core';
import { AuthService } from '../../services/auth/auth.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss'],
    standalone: false
})
export class RegisterComponent {
  registrationForm = new FormGroup({
    login: new FormControl('', [Validators.required,  Validators.minLength(4)]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)])
  });

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit() {
    const email = this.registrationForm.value.login;
    const password = this.registrationForm.value.password;
    this.authService.register(email!, password!)
    .subscribe({
      next: () => {
       console.log('Register successful');
       this.router.navigate(['/member']).then(() => {
        window.location.reload();
      });
      },
      error: (error) => console.error('Register failed', error)
     });
  }
}
