import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { Router } from '@angular/router';

declare var require: any

@Component({
  selector: 'app-show_banque',
  templateUrl: './show_banque.component.html',
  styleUrls: ['./show_banque.component.css'] 
})
export class Show_banqueComponent implements OnInit {
  p: number = 1;
  CATEGORIE_AGENT: any= []
  banqueList:any = [];
  modopen=false;
  agent:any;
  modalTitle:any;
  activateAddEditStuCom:boolean = false;
  banque:any;
  PAIE_BANQUE_ACTIF : any= [];
  AgentList:any = [];

  constructor(private router: Router, private service: SharedService, private sharedService: SharedService) {  }
  ngOnInit() {
    this.droits_paramétrage();
    this.refreshbanqueList();
  }

  droits_paramétrage() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getParamétrage(val).subscribe(data =>{
      this.AgentList = data;
      this.PAIE_BANQUE_ACTIF=this.AgentList[1]["ACTIF"];
      if (this.PAIE_BANQUE_ACTIF=="NON")
      {
        this.router.navigate([''])
      }
    });
  }

  refreshbanqueList() {
    var val = {
    };
    this.sharedService.getbanqueList(val).subscribe(data =>{
      this.banqueList = data;
    });  
    this.p=1;
  }

  refreshbanqueList2() {
    var val = {
    };
    this.sharedService.getbanqueList(val).subscribe(data =>{
      this.banqueList = data;
    });  
    this.p=1;
  }

  Addbanque(){
    this.banque={
      ABBREVIATION:"",
      BANQUE_FR:"",
      BANQUE_AR:""
    }
    this.modopen=false;
    this.modopen=true;
  }

  Editbanque(item: any){
    this.banque = item;
    this.modopen=false;
    this.modopen=true;
  }

  deletebanque(item: any){
    this.banque = item;
    if(confirm('Etes vous sûr de vouloir supprimer cette banque ?')){
      var val = {
        BANQUE_FR:this.banque.BANQUE_FR,
      };
        this.sharedService.deletebanque(val).subscribe(data =>{
        alert(data.toString());
        this.refreshbanqueList2();
      })
    }
  }

  closeClick(){
    this.modopen=false;
    this.refreshbanqueList2();
  }
}
