import { Injectable } from '@angular/core';
import {HttpClient,HttpHeaders} from '@angular/common/http';
import {JwtHelperService} from '@auth0/angular-jwt';
import { Router, RouterLink } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { LoginComponent } from 'src/app/foinra//components/login/login.component';
import { HotToastService } from '@ngneat/hot-toast';

const store=localStorage;
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  authToken:any;
  user:any;
  serv=""
  User={Id:null,role:[0],UserName:'',Class:null,Unite:null};

  constructor(public http:HttpClient,public helper:JwtHelperService,public router:Router,public dialog:MatDialog,public toastService:HotToastService) {
    this.user=JSON.parse(store.getItem('user')!);
    if(store.getItem('det')!=null){
      this.User=JSON.parse(store.getItem('det')!);
    }
   }
   hasrole(role: number){
     return this.User.role.includes(role);
   }
   post(link: string,duser: any) {
    let headers = new HttpHeaders();
    this.loadToken();
    headers.append('Content-Type', 'application/json');
    headers.append('Authorization',this.authToken);
    return this.http.post(this.serv+'api/auth/'+link, duser, { headers: headers });

  }
  get(link:string){
    let headers = new HttpHeaders();
    this.loadToken();
    headers.append('Content-Type', 'application/json');
    headers.append('Authorization',this.authToken);
    return this.http.get(this.serv+'api/auth/'+link, { headers: headers});
  }
  requestUserAuth(token: any){
    let headers = new HttpHeaders();
    this.loadToken();
    headers.append('Content-Type', 'application/json');
    headers.append('Authorization',token);
    return this.http.get(this.serv+'api/auth/request', { headers: headers,params:{token} });
  }
  authenticateUser(duser: { Username: String; Password: String; }) {
    let headers = new HttpHeaders();
    this.loadToken();
    headers.append('Content-Type', 'application/json');
    headers.append('Authorization',this.authToken);
    return this.http.post(this.serv+'api/auth/login', duser, { headers: headers });
  }
  loadToken() {
    const token = store.getItem('id_token');
    this.authToken = token;
    return token;
  }
  storeUserData(token: string,user: any) {
    const old=JSON.parse(store.getItem('user')!);
    store.setItem('id_token', token);
    store.setItem('det', JSON.stringify(user));
    this.user=this.helper.decodeToken(token);
    store.setItem('user',JSON.stringify(this.user));
    this.authToken = token;
    if(old==null || this.user.jti!=old.jti){
      this.router.navigate(['']);
      location.href='';
    }
    console.log('stored');
  }
  loggedIn() {
    this.loadToken();
    return !this.helper.isTokenExpired(this.authToken);
  }
  logOut() {
    this.router.navigate(['out']);
    this.authToken = null;
    this.user = null;
    store.clear();
  }
  poplogin(){
    this.dialog.open(LoginComponent,{
      disableClose:true
    });
  }
  static mod(){

    return store.getItem('id_token');
}
login(){
  this.dialog.open(LoginComponent,{
    disableClose:true
  });
}
notif(msg: any,css: any){
  this.toastService.show(msg)
  //this.alert.show(msg,{cssClass:css,timeout:6000,closeOnClick:true});
}
toast(){
  return this.toastService;
  //this.alert.show(msg,{cssClass:css,timeout:300000,closeOnClick:true});
}
}

