import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/services/auth.service';

@Component({
  selector: 'app-token',
  templateUrl: './token.component.html',
  styleUrls: ['./token.component.scss']
})
export class TokenComponent implements OnInit {
  token="";
  constructor(public route:ActivatedRoute,public authService:AuthService,public router:Router) {
    let param=route.snapshot.params['token'];
    if(param!= null && param!=''){
      this.token=param;
      this.getuser();
    }

   }

  ngOnInit() {

  }
  getuser(){
    console.log(this.token);

    this.authService.requestUserAuth(this.token).subscribe((data:any)=>{
      if(data.success){
        this.authService.storeUserData(data.token,data.user);
      }
      location.href='';
    })
  }

}
