import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { Router } from '@angular/router';
//import { AppComponent } from './app.com';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_etat_engagement_periode',
  templateUrl: './show_etat_engagement_periode.component.html',
  styleUrls: ['./show_etat_engagement_periode.component.css'] 
})
export class Show_etat_engagement_periodeComponent implements OnInit {
  p: number = 1;
  CATEGORIE_AGENT: any= []
  EtatEngList:any = [];
  AgentList:any = [];
  modopen=false;
  agent:any;
  modalTitle:any;
  activateAddEditStuCom:boolean = false;
  EtatEng:any;
  DDP_SELECTIONNE: string="";
  CATEGORIE_AGENT_ENG_PERIODE_ACTIF : any= [];

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
      this.CATEGORIE_AGENT_ENG_PERIODE_ACTIF=this.AgentList[8]["ACTIF"];
      //console.log(this.CATEGORIE_AGENT_ENG_PERIODE_ACTIF)
      if (this.CATEGORIE_AGENT_ENG_PERIODE_ACTIF=="NON")
      {
        this.router.navigate([''])
      }
    });
  }

  choixagent(agent:any) {   
    this.DDP_SELECTIONNE="OUI" ;
    var val = {
      DDP:this.agent.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatEngPerList(val).subscribe(data =>{
      this.EtatEngList = data;
    }); 
    this.p=1;
  }

  vider() {    
    this.DDP_SELECTIONNE="NON";
    var val = {
    };
    this.sharedService.getEtatEngPerList(val).subscribe(data =>{
      this.EtatEngList = data;
    }); 
    this.p=1;
  }

  refreshEtatEngList() {
    var val = {
      DDP:this.service.userInfo.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatEngPerList(val).subscribe(data =>{
      this.EtatEngList = data;
      this.agent={DDP:this.service.userInfo.DDP ,NOM_PRENOM: this.service.userInfo.NOM_PRENOM};
    });  
  }

  refreshEtatEngList2() {
    var val = {
      DDP:this.agent.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatEngPerList(val).subscribe(data =>{
      this.EtatEngList = data;
    });  
  }

  refreshAgentList() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getAgentPerList(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT=this.AgentList[0]["CATEGORIE"];
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
  
  downloadfile(i: any) {
   let nom_pdf : any = i.DDP + "_" + i.ANNEE_DU + "-" + i.MOIS_DU + "_" +i.ANNEE_AU + "-" +i.MOIS_AU+"_"+ i.DATE.substring(0,10) + "_" + i.TYPE.substring(0,1) + " (" + i.DDP_DEMANDEUR + ")";
    this.sharedService.getdownloadfilePer(i.NOM_FILE).subscribe(data =>{
      const blob='data:application/pdf;base64,'+data;
      FileSaver.saveAs(blob,nom_pdf);
    })
  }
}
