import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SharedService } from "src/app/shared.service";

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_etat_engagement',
  templateUrl: './show_etat_engagement.component.html',
  styleUrls: ['./show_etat_engagement.component.css'] 
})
export class Show_etat_engagementComponent implements OnInit {
  p: number = 1;
  CATEGORIE_AGENT: any= []
  EtatEngList:any = [];
  AgentList:any = [];
  modopen=false;
  agent:any;
  modalTitle:any;
  EtatEng:any;
  DDP_SELECTIONNE: string="";
  CATEGORIE_AGENT_ENG_ACTIF: any= [];

  constructor(private router: Router, private service: SharedService, private sharedService: SharedService) {  }
  ngOnInit() {
    this.droits_paie();
    this.refreshEtatEngList();
    this.refreshAgentList();   
    this.DDP_SELECTIONNE="OUI";  
  }

  droits_paie() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getCatégorieAgentPaie(val).subscribe(data =>{
      this.AgentList = data; 
      this.CATEGORIE_AGENT_ENG_ACTIF=this.AgentList[0]["ACTIF"];
      if (this.CATEGORIE_AGENT_ENG_ACTIF=="NON")
      {
        this.router.navigate([''])
      }
    });
  }

  vider() {    
    this.DDP_SELECTIONNE="NON";
    var val = {
      // DDP:this.agent.DDP,
      // DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatEngList(val).subscribe(data =>{
      this.EtatEngList = data;
    }); 
    this.p=1;
  }

  choixagent(agent:any) {    
    this.DDP_SELECTIONNE="OUI";
    var val = {
      DDP:this.agent.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatEngList(val).subscribe(data =>{
      this.EtatEngList = data;
    }); 
    this.p=1;
  }

  refreshEtatEngList() {
    var val = {
      DDP:this.service.userInfo.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatEngList(val).subscribe(data =>{
      this.EtatEngList = data;
      this.agent={DDP:this.service.userInfo.DDP ,NOM_PRENOM: this.service.userInfo.NOM_PRENOM};
    });  
  }

  refreshEtatEngList2() {
    var val = {
      DDP:this.agent.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatEngList(val).subscribe(data =>{
      this.EtatEngList = data;
    });  
  }

  refreshAgentList() {
    var val = {
      DDP:this.service.userInfo.DDP,
      //CATEGORIE_AGENT:this.service.userInfo.CATEGORIE_AGENT
    };
    this.sharedService.getAgentList(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT=this.AgentList[0]["CATEGORIE"];
      //console.log(this.CATEGORIE_AGENT)
    });
  }

  AddEtatEng(){
    this.EtatEng={
      DATE:"",
      DDP:"",
      DDP_DEMANDEUR:"",
      NOM_PRENOM:"",
      ANNEE:"",
      MOIS:"",
      TYPE:"",
      STATUT:""
    }
    this.modopen=false;
    this.modopen=true;
  }

  closeClick(){
    this.modopen=false;
    this.refreshEtatEngList2();
  }

  // closeClick(){
  //   this.modopen=false;
  //   this.refreshEtatEngList2();
  // }
  
  downloadfile(i: any) {
   let nom_pdf : any = i.DDP + "_" + i.ANNEE + "_" + i.MOIS + "_" + i.DATE.substring(0,10) + "_" + i.TYPE.substring(0,1) + " (" + i.DDP_DEMANDEUR + ")";
    this.sharedService.getdownloadfile(i.NOM_FILE).subscribe(data =>{
      const blob='data:application/pdf;base64,'+data;
      FileSaver.saveAs(blob,nom_pdf);
    })
  }
}
