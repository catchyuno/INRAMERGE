import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/services/auth.service';
import { FactureService } from 'src/services/facture.service';

@Component({
  selector: 'app-foinra-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss']
})
export class FoinraManagementComponent implements OnInit {
  textadd:any;
  cpassword:any;
  user:any;editable=false;add=false;unites:any[]=[];classes:any[]=[];
  users:any[]=[];
  p=1;
  constructor(public auth:AuthService,public factureService:FactureService) { }

  ngOnInit() {
    this.auth.post('users',{}).subscribe((data:any)=>{
      this.users=data;
      for(let i of this.users){
        if(i.userName==this.auth.User.UserName){
          this.users.splice(this.users.indexOf(i),1);break;
        }
      }

    })
    this.factureService.get('getfournisseur').subscribe((data:any)=>{
      this.unites=data.unites;
      this.classes=data.classes;
    })
  }
  reset(){
    this.editable=false;
    this.ngOnInit();
  }
  addmode(){
    this.add=true;
    this.textadd='Ajouter un utilisateur';
    this.editable=true;
    this.user={email:'',fullName:null,unite:0,role:[4],userName:null,password:null,class:null,enabled:1};
  }
  edit(i:any){
    this.add=false;
    this.textadd='Modifier '+i.userName;
    this.editable=true;
    this.user=i;
  }
  valid(){
    let use;
      if(this.user.password!=='' && this.user.password!==null ){
      if(this.user.password.length>5 && this.user.password==this.cpassword ){
        use={userName:this.user.userName,unite:this.user.unite,enabled:1,role:this.user.role,class:this.user.class,email:this.user.email,fullName:this.user.fullName,id:this.user.id,password:this.user.password};
        if(this.user.id==null ||this.user.id=='' ||this.user.id==undefined){
          this.auth.post('register',this.user).subscribe((data:any)=>{
            if(data.success){
              this.auth.notif('Utilisateur ajouter!','alert-success');
              this.reset();
            }else{
              this.auth.notif(data.err,'alert-danger');
            }
          })
        }else{
          this.auth.post('update',use).subscribe((data:any)=>{
          if(data.success){
            this.auth.notif('Vos modification ont été enregistrer!','alert-success');
            this.reset();
          }else{
            this.auth.notif(data.err,'alert-danger');
          }
        })
        }

      }else{
        this.auth.notif('Mot de passe ou confirmation incorrect!','alert-warning');
      }
      }else{
        use={userName:this.user.userName,email:this.user.email,role:this.user.role,unite:this.user.unite,fullName:this.user.fullName,id:this.user.id};
        this.auth.post('update',use).subscribe((data:any)=>{
          if(data.success){
            this.auth.notif('Vos modification ont été enregistrer','alert-success');
            this.reset();
          }else{
            this.auth.notif(data.err,'alert-danger');
          }
        })
      }
  }
  remove(i:any){
    if(window.confirm('Etes-vous sûr de vouloir supprimer cet utilisateur!')){
      this.auth.post('delete',i).subscribe((data:any)=>{
        if(data.success){
          this.reset();
          this.auth.notif('Utilisateur supprimer!','alert-success');
        }else if(data.exist){
          this.auth.notif('cet utilisateur a des factures existant et active','alert-warning');
        }
      })
    }
  }
  getclasse(c:any){
    for(let i of this.classes){
      if(i.CLASSE==c) return i.LIBAFFEC;
    }
    return '';
  }
  getunite(c:any){
    for(let i of this.unites){
      if(i.UNITE==c) return i.LIBAFFEC;
    }
    return '';
  }
  torole(i:Array<number>){
    let text=[];
    for(let x of i){
      switch(x){
        case 1:text.push("Marché");break;
        case 2:text.push("Budget");break;
        case 3:text.push("TP");break;
        case 4:text.push("Service");break;
        case 5:text.push("Admin");break;
        case 6:text.push("Exploitation");break;
      }
    }
    return text;
  }
}
