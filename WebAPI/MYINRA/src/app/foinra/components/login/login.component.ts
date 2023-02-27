import { Component, Input, OnInit } from '@angular/core';
import { AuthService } from '../../../../services/auth.service';
import {MatDialog, MatDialogRef} from '@angular/material/dialog'
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  logbutton=false;signmode=false;
  username: String="";
  password: String="";
  err=false;msg="";
  count=0;
  forgot=false;

  constructor(public router:Router,public authService:AuthService,public dialog:MatDialogRef<LoginComponent>,public dialogs:MatDialog) {

   }

  ngOnInit() {

  }
  login() {
    if(this.username!='' && this.password!='' && this.username!=undefined && this.password!=undefined){
      this.count++;
      if(this.count>5){
        this.forgot=true;
      }
      const user = {
        Username: this.username,
        Password: this.password
      }
      this.logbutton=true;
      this.authService.authenticateUser(user).subscribe((data:any) => {
        this.logbutton=false;
        if(data.success){
          this.authService.storeUserData(data.token,data.user);
          localStorage.setItem('curuser',JSON.stringify({DDP:data.user.DDP,NOM_PRENOM:data.user.FULLNAME, CATEGORIE_AGENT:"AGENT"}));
          if(this.router.url=="/out"){
            location.href='';
          }
          this.dialog.close();
          this.dialogs.closeAll();
        }else{
          this.msg=data.msg;
          this.err=true;
        }
      });

    }else{
      this.err=true;
      this.msg="Veuillez entrez un nom d'utilisateur et un mot de passe";
    }
    }
    sign(){
      this.router.navigate(['signup']);
      this.dialog.close();
    }
}
