import * as signalR from '@microsoft/signalr';
import { Injectable } from '@angular/core';
import { AuthService } from './auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class UpdateService {

  constructor(private authService : AuthService) {}
  private hubConnection: signalR.HubConnection = null!;

  public startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:6732/hubs/update',
        {
           accessTokenFactory: () => this.authService.getAccessToken()! 
        })
      .build();

    this.hubConnection
      .start()
      .catch(err => console.log('Error while starting connection: ' + err));
  }

  public onReceiveUpdate(callback: (data: any) => void) {
    this.hubConnection.on('ReceiveUpdate', callback);
  }
}
