import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/services/auth.service';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {
  password="";
  cpassword="";
  user:any;
  constructor(public authservice:AuthService) { }

  ngOnInit() {
    this.user=Object.create(this.authservice.User);
  }
  refresh(){
    this.user=Object.create(this.authservice.User);
    this.password="";
    this.cpassword="";
    
  }
  valid(){
    let use;
      if(this.password!=='' && this.password!==null ){
      if(this.password.length>5 && this.password==this.cpassword ){
        use={email:this.user.email,fullName:this.user.fullName,id:this.user.id,password:this.password,role:[]};
        this.authservice.post('update',use).subscribe((data:any)=>{
          if(data.success){
            this.authservice.notif('Vos modification ont été enregistrer','alert-success');
            this.authservice.logOut();
          }else{
            this.authservice.notif(data.err,'alert-danger');
          }
        })
      }else{
        this.authservice.notif('Mot de passe ou confirmation incorrect!','alert-warning');
      }
      }else{
        use={userName:this.user.userName,email:this.user.email,fullName:this.user.fullName,id:this.user.id,role:[]};
        this.authservice.post('update',use).subscribe((data:any)=>{
          if(data.success){
            this.authservice.notif('Vos modification ont été enregistrer','alert-success');
            this.authservice.logOut();
          }else{
            this.authservice.notif(data.err,'alert-danger');
          }
        })
      }
      
    
    
  }

}
