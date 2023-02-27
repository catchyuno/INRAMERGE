import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

@Component({
  selector: 'app-show_banque_code',
  templateUrl: './show_banque_code.component.html',
  styleUrls: ['./show_banque_code.component.css']
})
export class Show_banque_codeComponent implements OnInit {
  p: number = 1;
  CodeBanqueList:any = [];
  BanqueList:any = [];  
  modopen=false;
  modalTitle:any;
  BANQUE_FR: string="";
  banque:any;

  constructor(private service: SharedService, private sharedService: SharedService) {  }
  ngOnInit() {
    this.refreshBanqueList();
     this.refreshCodeBanqueList();
  }

  choixbanque() {  
    if (this.banque.BANQUE_FR!=null)
    {
      var val = {
        BANQUE_FR:this.banque.BANQUE_FR
      };
        this.sharedService.getBanque(val).subscribe(data =>{
        this.CodeBanqueList = data;
      }); 
      this.p=1;
    }
  }

  refreshBanqueList() {
    var val = {
    };
    this.sharedService.getBanqueList(val).subscribe(data =>{
      this.BanqueList = data;
    });  
    this.p=1;
  }

  refreshCodeBanqueList() {
    var val = {
    };
    this.sharedService.getCodeBanqueList_SANS_MAJ(val).subscribe(data =>{
      this.CodeBanqueList = data;
    });  
    this.p=1;
  }

  refreshCodeBanqueList2() {
    var val = {
      BANQUE_FR:this.banque.BANQUE_FR
    };
    this.sharedService.getCodeBanqueList(val).subscribe(data =>{
      this.CodeBanqueList = data;
    });  
  }

  EditCodeBanque(item: any){
    this.banque = item;
    this.modopen=false;
    this.modopen=true;
  }

  closeClick(){
    this.modopen=false;
    this.refreshCodeBanqueList2();
  }
}
