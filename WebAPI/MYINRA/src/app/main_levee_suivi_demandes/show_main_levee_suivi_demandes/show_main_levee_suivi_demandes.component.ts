import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

import { Router } from '@angular/router';


declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_main_levee_suivi_demandes',
  templateUrl: './show_main_levee_suivi_demandes.component.html',
  styleUrls: ['./show_main_levee_suivi_demandes.component.css']
})
export class Show_main_levee_suivi_demandesComponent implements OnInit {
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  p: number = 1;
  ETAT: string ="TOUS";
  MainLeveeSuiviList:any = [];
  AgentList:any = [];
  MSG:string="";
  DDP: string ="";
  NOM_PRENOM: string ="";
 // DDP_DEMANDEUR: string ="";
  //NOM_PRENOM_DEMANDEUR: string ="";
  modopen=false;
  agent:any;
  suiviMainLevee:any="";
  ATTESTATION_ETAT_MAIN_ACTIF : any= [];

  constructor(private router: Router, private service: SharedService, private sharedService: SharedService) {}
  ngOnInit() {
    this.droits_suivi_demandes();
    this.refreshAgentList();  
    this.vider();
    this.agent={NOM_PRENOM:'TOUS',DDP:'-------'}
    this.refreshMainLeveeSuiviList()
  }

  droits_suivi_demandes() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getSuiviDemandes(val).subscribe(data =>{
      this.AgentList = data;
      this.ATTESTATION_ETAT_MAIN_ACTIF=this.AgentList[3]["ACTIF"];
      if (this.ATTESTATION_ETAT_MAIN_ACTIF=="NON")
      {
        this.router.navigate([''])
      }
    });
  }

  choixagent(agent:any) {    
    var val = {
      DDP:this.agent.DDP,
    };
    this.sharedService.getMainLeveeSuiviList(val).subscribe(data =>{
      this.MainLeveeSuiviList = data;
    }); 
    this.p=1;
  }

  refreshMainLeveeSuiviList() {
    var val = {
      DDP:this.agent.DDP,
      ETAT:this.ETAT
    };
    this.sharedService.getMainLeveeSuiviList(val).subscribe(data =>{
      this.MainLeveeSuiviList = data;
    });  
    this.p=1;
  }

  vider() {
    var val = {
      DDP:"",
      ETAT:this.ETAT
    };
    this.sharedService.getMainLeveeSuiviList(val).subscribe(data =>{
      this.MainLeveeSuiviList = data;
    });  
    this.p=1;
  }

  refreshMainLeveeSuiviList2() {
    var val = {
      DDP:this.agent.DDP,
      ETAT:this.ETAT
    };
    this.sharedService.getMainLeveeSuiviList(val).subscribe(data =>{
      this.MainLeveeSuiviList = data;
    });  
    this.p=1;
  }

  refreshAgentList() {
    var val = {
      ETAT:this.ETAT
    };
    this.sharedService.getMainLeveeSuiviAgentList(val).subscribe(data =>{
      this.AgentList = data;
    });
   this.agent={NOM_PRENOM:'TOUS',DDP:'-------'}
    this.refreshMainLeveeSuiviList()
    this.p=1;
  }

  EditMainLevee(i:any){
        this.suiviMainLevee = i;
        this.modopen=false;
        this.modopen=true;
      }

      closeClick(){
        this.modopen=false;
        this.refreshMainLeveeSuiviList2();
      }

      downloadfile(i: any) {
        let nom_pdf : any = i.DDP + "_" + i.DATE_PDF + " (" + i.DDP_DEMANDEUR + ")";
        this.sharedService.getdownloadMainLeveeSuivifile(i.NOM_FILE).subscribe(data =>{
          const blob='data:application/pdf;base64,'+data;
          FileSaver.saveAs(blob,nom_pdf);
        })
      }

}
