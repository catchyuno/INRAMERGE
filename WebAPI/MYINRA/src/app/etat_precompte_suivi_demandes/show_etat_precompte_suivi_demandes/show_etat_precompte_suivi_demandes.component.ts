import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { Router } from '@angular/router';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_etat_precompte_suivi_demandes',
  templateUrl: './show_etat_precompte_suivi_demandes.component.html',
  styleUrls: ['./show_etat_precompte_suivi_demandes.component.css']
})
export class Show_etat_precompte_suivi_demandesComponent implements OnInit {
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  p: number = 1;
  STATUT: string ="TOUS";
  EtatSuiviPrecompteList:any = [];
  AgentList:any = [];
  MSG:string="";
  DDP: string ="";
  NOM_PRENOM: string ="";
  DDP_DEMANDEUR: string ="";
  NOM_PRENOM_DEMANDEUR: string ="";
  modopen=false;
  agent:any;
  suiviPrecompte:any="";
  ATTESTATION_ETAT_PRECOMPTE_ACTIF : any= [];

  constructor(private router: Router, private service: SharedService, private sharedService: SharedService) {}
  ngOnInit() {
    this.droits_suivi_demandes();
    this.refreshAgentList();  
    this.vider();
    this.agent={NOM_PRENOM:'TOUS',DDP:'-------'}
    this.refreshEtatSuiviPrecompteList()  
  }

  droits_suivi_demandes() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getSuiviDemandes(val).subscribe(data =>{
      this.AgentList = data;
      this.ATTESTATION_ETAT_PRECOMPTE_ACTIF=this.AgentList[1]["ACTIF"];
      if (this.ATTESTATION_ETAT_PRECOMPTE_ACTIF=="NON")
      {
        this.router.navigate([''])
      }
    });
  }

  choixagent(agent:any) {    
    var val = {
      DDP:this.agent.DDP,
    };
    this.sharedService.getEtatSuiviPrecompteList(val).subscribe(data =>{
      this.EtatSuiviPrecompteList = data;
    }); 
    this.p=1;
  }

  refreshEtatSuiviPrecompteList() {
    var val = {
      DDP:this.agent.DDP,
      STATUT:this.STATUT
    };
    this.sharedService.getEtatSuiviPrecompteList(val).subscribe(data =>{
      this.EtatSuiviPrecompteList = data;
    });  
    this.p=1;
  }

  vider() {
    var val = {
      DDP:"",
      STATUT:this.STATUT
    };
    this.sharedService.getEtatSuiviPrecompteList(val).subscribe(data =>{
      this.EtatSuiviPrecompteList = data;
    });  
    this.p=1;
  }

  refreshEtatSuiviPrecompteList2() {
    var val = {
      DDP:this.agent.DDP,
      STATUT:this.STATUT
    };
    this.sharedService.getEtatSuiviPrecompteList(val).subscribe(data =>{
      this.EtatSuiviPrecompteList = data;
    });  
    this.p=1;
  }

  refreshAgentList() {
    var val = {
      STATUT:this.STATUT
    };
    this.sharedService.getSuiviPrecompteAgentList(val).subscribe(data =>{
      this.AgentList = data;
    });
    this.agent={NOM_PRENOM:'TOUS',DDP:'-------'}
    this.refreshEtatSuiviPrecompteList()
    this.p=1;
  }

  AddEtatSuiviPrecompte(i:any){
        this.suiviPrecompte = i;
        this.modopen=false;
        this.modopen=true;
      }

      closeClick(){
        this.modopen=false;
        this.refreshEtatSuiviPrecompteList2();
      }

  downloadfile(i: any) {
    let nom_pdf : any = i.DDP + "_" + i.DATE.substring(0,10) + "_" + i.TYPE + "_" + i.CODE_RUBRIQUE + " (" + i.DDP_DEMANDEUR + ")";
    this.sharedService.getdownloadPrecomptefile(i.NOM_FILE).subscribe(data =>{
    const blob='data:application/pdf;base64,'+data;
    this.MSG=data.toString();
    if (this.MSG=="Etat Précompte non encore généré !")
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
