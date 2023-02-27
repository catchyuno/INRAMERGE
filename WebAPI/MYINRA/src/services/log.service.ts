import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HttpTransportType } from '@microsoft/signalr';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class LogService {
  hubConnection:HubConnection | undefined;

  constructor(public authService:AuthService) { }
  serv="";
  startconnection(){
    const options = {
      transport: HttpTransportType.WebSockets
      };
      this.hubConnection=new HubConnectionBuilder().withUrl(this.serv+'log',options)
      .build();
      this.hubConnection.start()
        .then(()=>{
          this.hubConnection!.invoke('GetConnectionId').then((connectionId: any)=>{
            console.log(connectionId);
          })
          console.log('on',this.hubConnection!.state);
        }).catch((err: string)=>{
          console.log('error while connecting :'+err);
        })
  }

}
