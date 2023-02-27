import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
//import { AppComponent } from './app.com';

import { Router } from '@angular/router';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_etat_liquidation',
  templateUrl: './show_etat_liquidation.component.html',
  styleUrls: ['./show_etat_liquidation.component.css'] 
})
export class Show_etat_liquidationComponent implements OnInit {
  p: number = 1;
  CATEGORIE_AGENT: any= []
  EtatLiqList:any = [];
  AgentList:any = [];
  modopen=false;
  agent:any;
  modalTitle:any;
  activateAddEditStuCom:boolean = false;
  EtatLiq:any;
  DDP_SELECTIONNE: string="";
  CATEGORIE_AGENT_LIQ_ACTIF: any= [];
  

  constructor(private router: Router,private service: SharedService, private sharedService: SharedService) {  }
  ngOnInit() {
    this.droits_paie();
    this.refreshEtatLiqList();
    this.refreshAgentList();    
    this.DDP_SELECTIONNE="OUI";
  }

  droits_paie() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getCatégorieAgentPaie(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT_LIQ_ACTIF=this.AgentList[1]["ACTIF"];
      if (this.CATEGORIE_AGENT_LIQ_ACTIF=="NON")
      {
        this.router.navigate([''])
      }
    });
  }

  choixagent(agent:any) {    
    this.DDP_SELECTIONNE="OUI";
    var val = {
      DDP:this.agent.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatLiqList(val).subscribe(data =>{
      this.EtatLiqList = data;
    }); 
    this.p=1;
  }

  vider() {    
    this.DDP_SELECTIONNE="NON";
    var val = {
    };
    this.sharedService.getEtatLiqList(val).subscribe(data =>{
      this.EtatLiqList = data;
    }); 
    this.p=1;
  }

  refreshEtatLiqList() {
    var val = {
      DDP:this.service.userInfo.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatLiqList(val).subscribe(data =>{
      this.EtatLiqList = data;
      this.agent={DDP:this.service.userInfo.DDP ,NOM_PRENOM: this.service.userInfo.NOM_PRENOM};
    });  
  }

  refreshEtatLiqList2() {
    var val = {
      DDP:this.agent.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatLiqList(val).subscribe(data =>{
      this.EtatLiqList = data;
    });  
  }

  refreshAgentList() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getAgentLiqList(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT=this.AgentList[0]["CATEGORIE"];
    });
  }

  AddEtatLiq(){
    this.EtatLiq={
      DATE:"",
      DDP:"",
      DDP_DEMANDEUR:"",
      NOM_PRENOM:"",
      ANNEE:"",
      MOIS:"",
      ORDRE:"",
      STATUT:""
    }
    this.modopen=false;
    this.modopen=true;
  }

  closeClick(){
    this.modopen=false;
    this.refreshEtatLiqList2();
  }
  
  downloadfile(i: any) {
   let nom_pdf : any = i.DDP + "_" + i.ORDRE + "-" + i.ANNEE + "-" + i.MOIS + "_"+ i.DATE.substring(0,10) +  " (" + i.DDP_DEMANDEUR + ")";
   //let nom_pdf : any = i.DDP + "_" + i.ANNEE + "_" + i.MOIS + "_" + i.DATE.substring(0,10) + " (" + i.DDP_DEMANDEUR + ")";
    this.sharedService.getdownloadfile(i.NOM_FILE).subscribe(data =>{
      const blob='data:application/pdf;base64,'+data;
      FileSaver.saveAs(blob,nom_pdf);
    })
  }
}
