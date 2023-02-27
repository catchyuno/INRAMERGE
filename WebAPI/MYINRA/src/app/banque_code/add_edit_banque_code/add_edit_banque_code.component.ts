import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

@Component({
  selector: 'app-add_edit_banque_code',
  templateUrl: './add_edit_banque_code.component.html',
  styleUrls: ['./add_edit_banque_code.component.css']
})
export class Add_edit_banque_codeComponent implements OnInit {
  @Input() banque:any;
  CODE: string ="";
  BANQUE_FR: string ="";
  BANQUE_AR: string ="";
  BanqueList:any = [];
  BanqueAR:any = [];

  constructor(private service: SharedService, private sharedService: SharedService) {}
  
  ngOnInit(): void {
    this.refreshBanqueList();  
    this.CODE = this.banque.CODE;
    this.BANQUE_FR = this.banque.BANQUE_FR;
    this.BANQUE_AR = this.banque.BANQUE_AR;
  }

  updatecodebanque(){
    var val = {
      CODE:this.CODE,
      BANQUE_FR:this.banque.BANQUE_FR,
      BANQUE_AR:this.banque.BANQUE_AR,
    };
      this.service.updatecodebanque(val).subscribe(res =>{
        alert(res.toString());
      })
  }  

  choixbanque() {    
    var val = {
      BANQUE_FR:this.banque.BANQUE_FR,
    };
    this.sharedService.getBanqueAR(val).subscribe(data =>{
      this.BanqueAR = data;
      this.BANQUE_AR=this.BanqueAR[0]["BANQUE_AR"];
    });
  }

  choixbanquevide() {    
      this.BANQUE_AR="";
  }

  refreshBanqueList() {
    var val = {
    };
    this.sharedService.getbanqueList(val).subscribe(data =>{
      this.BanqueList = data;
    });
  }
}
