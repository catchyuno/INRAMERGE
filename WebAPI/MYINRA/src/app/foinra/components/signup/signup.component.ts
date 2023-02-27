import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/services/auth.service';
import { FactureService } from 'src/services/facture.service';
import { MatDialog } from '@angular/material/dialog';
import { LoginComponent } from '../login/login.component';
import { Router } from '@angular/router';
import { Fournisseur } from 'src/interfaces';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent implements OnInit {
  classes=[];cpassword=null;msg='';usermsg='';fournisseurs:Fournisseur[]=[];
  user={fullName:null,unite:0,role:[0],userName:null,email:null,class:null,password:'',enabled:0};
  fournisseur={email: null,ice: null,idFiscal: null,idUser: null,nomResp:null,nomSc: null,enabled:0,raisonSociale: null,registreCommerce: null,tel: null,Classf:null,type: null};
  constructor(public authService:AuthService,public factureService:FactureService,public dialog:MatDialog,public router:Router) { }

  ngOnInit() {
    this.factureService.getparam('getfournisseur',{classe:0}).subscribe((data:any)=>{
      this.fournisseurs=data.fournisseurs;
      this.classes=data.classes;
    });
  }
  valid(){
    this.user.email=this.fournisseur.email;
    let use;
    let fu=false;
    for(let i of this.fournisseurs){
      if(this.fournisseur.ice==i.ice){
        if(i.idUser!=null){
          fu=true;
        }else{
          this.authService.notif('Ice existe et lié avec un compte, si vous avez oublié votre mot de passe veuiller nous contacter','alert-info');
          return false;
        }
      }
    }
    if(this.user.userName == null || this.fournisseur.email==null || this.fournisseur.Classf==null || this.fournisseur.ice==null || this.fournisseur.nomResp==null){
      this.authService.notif('veuillez remplir les champs obligatoire','alert-danger')
    }else{
      if(this.user.password!=='' && this.user.password!==null ){
        if(this.user.password.length>5 && this.user.password==this.cpassword ){
          use={userName:this.user.userName,enabled:0,role:[0],unite:this.user.unite,email:this.user.email,class:this.fournisseur.Classf,fullName:this.fournisseur.nomResp,password:this.user.password};
            this.authService.post('register',use).subscribe((data:any)=>{
              if(data.success){
                this.fournisseur.idUser=data.userId;
                if(fu){
                  this.factureService.postnoauth('editfournisseur',this.fournisseur).subscribe(data=>{
                    if(data){
                      this.router.navigate(['out']);
                    }
                  })
                }else{
                  this.factureService.postnoauth('addfournisseur',this.fournisseur).subscribe(data=>{
                    if(data){
                      this.router.navigate(['out']);
                    }
                  })
                }
              }else{
                this.authService.notif(data.err,'alert-danger');
              }
            })
        }else{
          this.msg='Mot de passe incorrect';
          this.authService.notif('Mot de passe ou confirmation incorrect!,le mot de pass doit être 6 caractère minimum!','alert-warning');
        }
        }
        return false;
    }

  }
  conn(){
    this.dialog.open(LoginComponent),{
      disableClose:true
    };
  }


}
