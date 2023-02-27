import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { Router } from '@angular/router';
import * as rap from 'ngx-select-dropdown';
//import { AppComponent } from './app.com';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_etat_domiciliation',
  templateUrl: './show_etat_domiciliation.component.html',
  styleUrls: ['./show_etat_domiciliation.component.css'] 
})
export class Show_etat_domiciliationComponent implements OnInit {
  p: number = 1;
  MSG:string="";
  CATEGORIE_AGENT: any= []
  EtatDomList:any = [];
  AgentList:any = [];
  modopen=false;
  agent:any;
  modalTitle:any;
  activateAddEditStuCom:boolean = false;
  EtatDom:any;
  DDP_SELECTIONNE: string="";
  CATEGORIE_AGENT_DOMIC_ACTIF : any= [];

  constructor(private router: Router, private service: SharedService, private sharedService: SharedService) {  }
  ngOnInit() {
    this.droits_paie();
    this.refreshEtatDomList();
    this.refreshAgentList();    
    this.DDP_SELECTIONNE="OUI";  
  }

  droits_paie() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getCatégorieAgentPaie(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT_DOMIC_ACTIF=this.AgentList[4]["ACTIF"];
      if (this.CATEGORIE_AGENT_DOMIC_ACTIF=="NON")
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
    this.sharedService.getEtatDomList(val).subscribe(data =>{
      this.EtatDomList = data;
    }); 
    this.p=1;
  }

  vider() {    
    this.DDP_SELECTIONNE="NON";
    var val = {
    };
    this.sharedService.getEtatDomList(val).subscribe(data =>{
      this.EtatDomList = data;
    }); 
    this.p=1;
  }

  refreshEtatDomList() {
    var val = {
      DDP:this.service.userInfo.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatDomList(val).subscribe(data =>{
      this.EtatDomList = data;
      this.agent={DDP:this.service.userInfo.DDP ,NOM_PRENOM: this.service.userInfo.NOM_PRENOM};
    });  
  }

  refreshEtatDomList2() {
    var val = {
      DDP:this.agent.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatDomList(val).subscribe(data =>{
      this.EtatDomList = data;
    });  
  }

  refreshAgentList() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getAgentDomList(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT=this.AgentList[0]["CATEGORIE"];
    });
  }

  AddEtatDom(){
    this.EtatDom={
      DATE:"",
      DDP:"",
      DDP_DEMANDEUR:"",
      NOM_PRENOM:"",
      STATUT:""
    }
    this.modopen=false;
    this.modopen=true;
  }

  ListeAgentsDom(){
    // this.EtatDom={
    //   DATE:"",
    //   DDP:"",
    //   DDP_DEMANDEUR:"",
    //   NOM_PRENOM:"",
    //   STATUT:""
    // }
    this.modopen=false;
    this.modopen=true;
  }

  closeClick(){
    this.modopen=false;
    this.refreshEtatDomList2();
  }
  
  downloadfile(i: any) {
   let nom_pdf : any = i.DDP + "_" + i.DATE.substring(0,10) + " (" + i.DDP_DEMANDEUR + ")";
    this.sharedService.getdownloadDomfile(i.NOM_FILE).subscribe(data =>{
      const blob='data:application/pdf;base64,'+data;
      this.MSG=data.toString();
    if (this.MSG=="Etat de domiciliation non éxistante !")
    {
      alert(data.toString());
    }
    else
    {
      FileSaver.saveAs(blob,nom_pdf);
    }
  })
  }
}
