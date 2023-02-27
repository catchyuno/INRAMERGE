import { Injectable } from '@angular/core';
import { Router,CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from 'src/services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(public router:Router,public authService:AuthService) {


  }
  canActivate(next:ActivatedRouteSnapshot) {
    console.log(this.authService.loggedIn());
    if (this.authService.loggedIn()) {
      
      const path=next.routeConfig!.path;
      console.log('path is:'+path);
      // if(((!this.authService.hasrole(1) && !this.authService.hasrole(5)) && path==='Marche') ||(!this.authService.hasrole(5) && path==='usermng') || (!this.authService.hasrole(1) && path==='fournisseurs') ){
      //   this.router.navigate(['factures']);
      //     return false;
      // }
      return true;
    } else {
      this.authService.poplogin();
      return false;
    }
  }
}
