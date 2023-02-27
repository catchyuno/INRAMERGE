import { Component, OnInit } from '@angular/core';
//import { Console } from 'console';
import { SharedService } from "src/app/shared.service";
import { Router } from '@angular/router';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_etat_engagement_regroupe',
  templateUrl: './show_etat_engagement_regroupe.component.html',
  styleUrls: ['./show_etat_engagement_regroupe.component.css']
})
export class Show_etat_engagement_regroupeComponent implements OnInit {
  today: Date;
  p: number = 1;
  modopen=false;
  STATUT: any= [];  
  ACTIVER_DEMANDE: any= []
  CATEGORIE_AGENT: any= []
  EtatEngagementRegroupeList:any = [];
  AgentList:any = [];
  MSG:string="";
  DATE:any ;
  DDP: string ="";
  NOM_PRENOM: string ="";
  DDP_DEMANDEUR: string ="";
  NOM_PRENOM_DEMANDEUR: string ="";
  agent:any;
  EtatEngagementRegroupe:any;
  DU: any ="";
  AU: any ="";
  //TYPE: string ="";
  EngagementRegroupe:any;
  DDP_SELECTIONNE: string="";
  CATEGORIE_AGENT_ENG_REGROUPE_ACTIF : any= [];

  constructor(private router: Router, private service: SharedService, private sharedService: SharedService) { this.today =new Date(); }
  ngOnInit() {
    this.droits_paie();
    this.refreshEtatEngagementRegroupeList();
    this.refreshAgentList();    
    this.DDP_SELECTIONNE="OUI";
    // this.refreshAgentList();   
    // this.agent={DDP:this.service.userInfo.DDP ,NOM_PRENOM: this.service.userInfo.NOM_PRENOM}; 
    // this.refreshEtatEngagementRegroupeList();
  }

  droits_paie() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getCatégorieAgentPaie(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT_ENG_REGROUPE_ACTIF=this.AgentList[9]["ACTIF"];
      if (this.CATEGORIE_AGENT_ENG_REGROUPE_ACTIF=="NON")
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
    this.sharedService.getEtatEngagementRegroupeList(val).subscribe(data =>{
      this.EtatEngagementRegroupeList = data;
    }); 
    this.ACTIVER_DEMANDE="OUI";
    this.p=1;
  }

  vider() {    
    this.DDP_SELECTIONNE="NON";
    var val = {
      DDP:"",
      DDP_DEMANDEUR:""
    };
    this.sharedService.getEtatEngagementRegroupeList(val).subscribe(data =>{
      this.EtatEngagementRegroupeList = data;
    }); 
    this.ACTIVER_DEMANDE="NON";
    this.p=1;
  }

  refreshEtatEngagementRegroupeList() {
    var val = {
      DDP:this.service.userInfo.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatEngagementRegroupeList(val).subscribe(data =>{
      this.EtatEngagementRegroupeList = data;
      this.agent={DDP:this.service.userInfo.DDP ,NOM_PRENOM: this.service.userInfo.NOM_PRENOM};
    });  
  }

  refreshEtatEngagementRegroupeList2() {
    var val = {
      DDP:this.agent.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatEngagementRegroupeList(val).subscribe(data =>{
      this.EtatEngagementRegroupeList = data;
    });  
  }

  refreshAgentList() {
    var val = {
      DDP:this.service.userInfo.DDP
    };
    this.sharedService.getEngagementRegroupeAgentList(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT=this.AgentList[0]["CATEGORIE"];
    });
  }

  deleteEngagementRegroupe(item: any){
    this.EngagementRegroupe = item;
    if(confirm('Etes vous sûr de vouloir supprimer cette demande ?')){
      var val = {
        DDP:this.agent.DDP,
        DATE:this.EngagementRegroupe.DATE,
        DDP_DEMANDEUR:this.sharedService.userInfo.DDP,
        DU:this.EngagementRegroupe.DU,
        AU:this.EngagementRegroupe.AU,
      };
        this.sharedService.deleteEngagementRegroupe(val).subscribe(data =>{
        alert(data.toString());
        this.refreshEtatEngagementRegroupeList2();
      })
    }
  }

  AddEtatEngagementRegroupe(){
    this.EtatEngagementRegroupe={
      DATE:"",
      DDP:"",
      DDP_DEMANDEUR:"",
      NOM_PRENOM:"",
      ANNEE:"",
      MOIS:"",
      //TYPE:"",
      DU:"",
      AU:"",
      STATUT:"EN COURS"
    }
    this.modopen=false;
    this.modopen=true;
  }

  closeClick(){
    this.modopen=false;
    this.refreshEtatEngagementRegroupeList2();
  }


  downloadfile(i: any) {
    let nom_pdf : any = i.DDP + "_" + i.DU.substring(0,10) + "-" + i.AU.substring(0,10) + "_" + i.DATE.substring(0,10) + " (" + i.DDP_DEMANDEUR + ")";
    this.sharedService.getdownloadSuiviEngagementfile(i.NOM_FILE).subscribe(data =>{
      // let nom_pdf : any = i.DDP + "_" + i.DATE.substring(0,10) + "_" + i.TYPE + "_" + i.CODE_RUBRIQUE + " (" + i.DDP_DEMANDEUR + ")";
      // this.sharedService.getdownloadPrecomptefile(i.NOM_FILE).subscribe(data =>{
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
