import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/services/auth.service';
import { LogService } from 'src/services/log.service';
import {saveAs} from 'file-saver';
import { FactureService } from 'src/services/facture.service';
@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {

  constructor(public factureService:FactureService,public authService:AuthService,public logService:LogService) { }

  ngOnInit() {
  }
  logOut(){
    this.authService.logOut();
    this.logService.hubConnection!.stop().then(()=>console.log(this.logService.hubConnection!.state,123)
    )
    
    
  }
  manuel(){
    this.factureService.getparam('getfile',{name:"manuel"}).subscribe((data:any)=>{
      const b64Data=data;   
      const byteCharacters = atob(b64Data);
      const byteArrays = [];
      
      for (let offset = 0; offset < byteCharacters.length; offset += 512) {
        const slice = byteCharacters.slice(offset, offset + 512);
        
        const byteNumbers = new Array(slice.length);
        for (let i = 0; i < slice.length; i++) {
          byteNumbers[i] = slice.charCodeAt(i);
        }
        
        const byteArray = new Uint8Array(byteNumbers);
        byteArrays.push(byteArray);
      }
      
      const blob = new Blob(byteArrays, {type: 'application/pdf'});
      
      const url=URL.createObjectURL(new File([blob],'Manuel',{lastModified:new Date().valueOf(),type:'application/pdf'}));
      open(url,'Manuel');
    })
  }
}
