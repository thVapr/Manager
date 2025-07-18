import { Component } from '@angular/core';
import { AuthService } from '../../services/auth/auth.service';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
    standalone: false
})
export class LoginComponent {
  loginForm = new FormGroup({
    login: new FormControl('', [Validators.required, Validators.minLength(4)]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)])
  });

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit() {
    const email = this.loginForm.value.login;
    const password = this.loginForm.value.password;
    this.authService.login(email!, password!)
      .subscribe({
       next: () => {
        console.log('Login successful');
        this.router.navigate(['member/about/' + this.authService.getId()]).then(() => {
          window.location.reload();
        });
       },
       error: (error) => console.error('Login failed', error)
      });
  }
}
