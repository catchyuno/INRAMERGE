import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

import { Router } from '@angular/router';


declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_etat_engagement_suivi_demandes',
  templateUrl: './show_etat_engagement_suivi_demandes.component.html',
  styleUrls: ['./show_etat_engagement_suivi_demandes.component.css']
})
export class Show_etat_engagement_suivi_demandesComponent implements OnInit {
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  p: number = 1;
  STATUT: string ="TOUS";
  EtatSuiviEngagementList:any = [];
  AgentList:any = [];
  MSG:string="";
  DDP: string ="";
  NOM_PRENOM: string ="";
  DDP_DEMANDEUR: string ="";
  NOM_PRENOM_DEMANDEUR: string ="";
  modopen=false;
  agent:any;
  suiviEngagement:any="";
  ATTESTATION_ETAT_ENG_ACTIF : any= [];

  constructor(private router: Router, private service: SharedService, private sharedService: SharedService) {}
  ngOnInit() {
    this.droits_suivi_demandes();
    this.refreshAgentList();  
    this.vider();
    this.agent={NOM_PRENOM:'TOUS',DDP:'-------'}
    this.refreshEtatSuiviEngagementList()
  }

  droits_suivi_demandes() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getSuiviDemandes(val).subscribe(data =>{
      this.AgentList = data;
      this.ATTESTATION_ETAT_ENG_ACTIF=this.AgentList[2]["ACTIF"];
      if (this.ATTESTATION_ETAT_ENG_ACTIF=="NON")
      {
        this.router.navigate([''])
      }
    });
  }

  choixagent(agent:any) {    
    var val = {
      DDP:this.agent.DDP,
    };
    this.sharedService.getEtatSuiviEngagementList(val).subscribe(data =>{
      this.EtatSuiviEngagementList = data;
    }); 
    this.p=1;
  }

  refreshEtatSuiviEngagementList() {
    var val = {
      DDP:this.agent.DDP,
      STATUT:this.STATUT
    };
    this.sharedService.getEtatSuiviEngagementList(val).subscribe(data =>{
      this.EtatSuiviEngagementList = data;
    });  
    this.p=1;
  }

  vider() {
    var val = {
      DDP:"",
      STATUT:this.STATUT
    };
    this.sharedService.getEtatSuiviEngagementList(val).subscribe(data =>{
      this.EtatSuiviEngagementList = data;
    });  
    this.p=1;
  }

  refreshEtatSuiviEngagementList2() {
    var val = {
      DDP:this.agent.DDP,
      STATUT:this.STATUT
    };
    this.sharedService.getEtatSuiviEngagementList(val).subscribe(data =>{
      this.EtatSuiviEngagementList = data;
    });  
    this.p=1;
  }

  refreshAgentList() {
    var val = {
      STATUT:this.STATUT
    };
    this.sharedService.getSuiviEngagementAgentList(val).subscribe(data =>{
      this.AgentList = data;
    });
   this.agent={NOM_PRENOM:'TOUS',DDP:'-------'}
    this.refreshEtatSuiviEngagementList()
    this.p=1;
  }

  AddEtatSuiviEngagement(i:any){
        this.suiviEngagement = i;
        this.modopen=false;
        this.modopen=true;
      }

      closeClick(){
        this.modopen=false;
        this.refreshEtatSuiviEngagementList2();
      }

  downloadfile(i: any) {
    let nom_pdf : any = i.DDP + "_" + i.DU.substring(0,10) + "-" + i.AU.substring(0,10) + "_" + i.DATE.substring(0,10) + " (" + i.DDP_DEMANDEUR + ")";
    this.sharedService.getdownloadSuiviEngagementfile(i.NOM_FILE).subscribe(data =>{
    const blob='data:application/pdf;base64,'+data;
    this.MSG=data.toString();
    if (this.MSG=="Etat Engagement non encore généré !")
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
