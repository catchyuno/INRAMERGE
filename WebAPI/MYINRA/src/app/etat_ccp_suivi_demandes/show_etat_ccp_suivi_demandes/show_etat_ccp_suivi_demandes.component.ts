import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { Router } from '@angular/router';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_etat_ccp_suivi_demandes',
  templateUrl: './show_etat_ccp_suivi_demandes.component.html',
  styleUrls: ['./show_etat_ccp_suivi_demandes.component.css']
})
export class Show_etat_ccp_suivi_demandesComponent implements OnInit {
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  p: number = 1;
  STATUT: string ="TOUS";
  EtatSuiviCCPList:any = [];
  AgentList:any = [];
  MSG:string="";
  DDP: string ="";
  NOM_PRENOM: string ="";
  DDP_DEMANDEUR: string ="";
  NOM_PRENOM_DEMANDEUR: string ="";
  modopen=false;
  agent:any;
  suiviCCP:any="";
  ATTESTATION_ETAT_CCP_ACTIF : any= [];

  constructor(private router: Router, private service: SharedService, private sharedService: SharedService) {}
  ngOnInit() {
    this.droits_suivi_demandes();
    this.refreshAgentList();  
    this.vider();
    this.agent={NOM_PRENOM:'TOUS',DDP:'-------'}
    this.refreshEtatSuiviCCPList()
  }

  droits_suivi_demandes() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getSuiviDemandes(val).subscribe(data =>{
      this.AgentList = data;
      this.ATTESTATION_ETAT_CCP_ACTIF=this.AgentList[0]["ACTIF"];
      if (this.ATTESTATION_ETAT_CCP_ACTIF=="NON")
      {
        this.router.navigate([''])
      }
    });
  }

  choixagent(agent:any) {    
    var val = {
      DDP:this.agent.DDP,
    };
    this.sharedService.getEtatSuiviCCPList(val).subscribe(data =>{
      this.EtatSuiviCCPList = data;
    }); 
    this.p=1;
  }

  refreshEtatSuiviCCPList() {
    var val = {
      DDP:this.agent.DDP,
      STATUT:this.STATUT
    };
    this.sharedService.getEtatSuiviCCPList(val).subscribe(data =>{
      this.EtatSuiviCCPList = data;
    });  
    this.p=1;
  }

  vider() {
    var val = {
      DDP:"",
      STATUT:this.STATUT
    };
    this.sharedService.getEtatSuiviCCPList(val).subscribe(data =>{
      this.EtatSuiviCCPList = data;
    });  
    this.p=1;
  }

  refreshEtatSuiviCCPList2() {
    var val = {
      DDP:this.agent.DDP,
      STATUT:this.STATUT
    };
    this.sharedService.getEtatSuiviCCPList(val).subscribe(data =>{
      this.EtatSuiviCCPList = data;
    });  
    this.p=1;
  }

  refreshAgentList() {
    var val = {
      STATUT:this.STATUT
    };
    this.sharedService.getSuiviCCPAgentList(val).subscribe(data =>{
      this.AgentList = data;
    });
    this.agent={NOM_PRENOM:'TOUS',DDP:'-------'}
    this.refreshEtatSuiviCCPList()
    this.p=1;
  }

  AddEtatSuiviCCP(i:any){
        this.suiviCCP = i;
        this.modopen=false;
        this.modopen=true;
      }

      closeClick(){
        this.modopen=false;
        this.refreshEtatSuiviCCPList2();
      }

  downloadfile(i: any) {
      let nom_pdf : any = i.DDP + "_" + i.DATE.substring(0,10)  + " (" + i.DDP_DEMANDEUR + ")";
      this.sharedService.getdownloadCCPfile(i.NOM_FILE).subscribe(data =>{
      const blob='data:application/pdf;base64,'+data;
      this.MSG=data.toString();
      if (this.MSG=="Etat CCP non encore généré !")
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
