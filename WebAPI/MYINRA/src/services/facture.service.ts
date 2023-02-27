import { Injectable } from '@angular/core';
import {HttpClient,HttpHeaders,HttpResponse} from '@angular/common/http';
import {JwtHelperService} from '@auth0/angular-jwt';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from './auth.service';
import { MatDialog } from '@angular/material/dialog';
import { HotToastService } from '@ngneat/hot-toast';

@Injectable({
  providedIn: 'root'
})
export class FactureService {
  serv="";
  constructor(public http:HttpClient,public helper:JwtHelperService,public router:Router,public authService:AuthService,public dialog:MatDialog,public toastService:HotToastService) { }
  post(url: string,param:any) {
    let headers = new HttpHeaders();
    this.authService.loadToken();
    headers.append('Content-Type', 'application/json');
    headers.append('Authorization',this.authService.authToken);
    return this.http.post(this.serv+'api/facture/'+url, param, { headers: headers })
  }
  get(url: string) {
    let headers = new HttpHeaders();
    this.authService.loadToken();
    headers.append('Content-Type', 'application/json');
    headers.append('Authorization',this.authService.authToken);
    return this.http.get(this.serv+'api/facture/'+url, { headers: headers })
  }
  getparam(url: string,param:any) {
    let headers = new HttpHeaders();
    this.authService.loadToken();
    headers.append('Content-Type', 'application/json');
    return this.http.get(this.serv+'api/facture/'+url, { headers: headers,params:param })
  }
  postnoauth(url: string,param: any){
    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json');
    return this.http.post(this.serv+'api/facture/'+url, param, { headers: headers })
  }
  etat(url:string,param:any){
    let headers = new HttpHeaders();
    this.authService.loadToken();
    headers.append('Content-Type', 'application/json');
    headers.append('Authorization',this.authService.authToken);
    return this.http.get(this.serv+'api/facture/'+url,{params:param,responseType:'blob',headers: headers})
  }
  notif(msg: string,css: string){
    //this.alert.show(msg,{cssClass:css,timeout:6000,closeOnClick:true});
  }
  toast():HotToastService{
    return this.toastService;
    //this.alert.show(msg,{cssClass:css,timeout:300000,closeOnClick:true});
  }
}
