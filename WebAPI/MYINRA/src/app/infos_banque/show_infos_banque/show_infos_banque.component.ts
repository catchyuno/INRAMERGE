import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

import { Router } from '@angular/router';


declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_infos_banque',
  templateUrl: './show_infos_banque.component.html',
  styleUrls: ['./show_infos_banque.component.css']
})
export class Show_infos_banqueComponent implements OnInit {
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  p: number = 1;
  STATUT: string ="TOUS";
  InfosBanqueList:any = [];
  AgentList:any = [];
  MSG:string="";
  DDP: string ="";
  NOM_PRENOM: string ="";
  // DDP_DEMANDEUR: string ="";
  // NOM_PRENOM_DEMANDEUR: string ="";
  modopen=false;
  agent:any;
  InfosBanque:any="";
  INFOS_BANQUE_ACTIF : any= [];

  constructor(private router: Router, private service: SharedService, private sharedService: SharedService) {}
  ngOnInit() {
    this.droits_suivi_demandes();
    //this.refreshAgentList();  
    //this.vider();
   // this.agent={NOM_PRENOM:'TOUS',DDP:'-------'}
    this.refreshInfosBanqueList()
  }

  droits_suivi_demandes() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getSuiviDemandes(val).subscribe(data =>{
      this.AgentList = data;
      this.INFOS_BANQUE_ACTIF=this.AgentList[4]["ACTIF"];
      if (this.INFOS_BANQUE_ACTIF=="NON")
      {
        this.router.navigate([''])
      }
    });
  }

  // choixStatut(statut:any) {    
  //   var val = {
  //     STATUT:this.STATUT,
  //   };
  //   this.sharedService.getInfosBanqueList(val).subscribe(data =>{
  //     this.InfosBanqueList = data;
  //   }); 
  //   this.p=1;
  // }

  refreshInfosBanqueList() {
    var val = {
      STATUT:this.STATUT,
    };
    this.sharedService.getInfosBanqueList(val).subscribe(data =>{
      this.InfosBanqueList = data;
    });  
    this.p=1;
  }

  // vider() {
  //   var val = {
  //     DDP:"",
  //     STATUT:this.STATUT
  //   };
  //   this.sharedService.getEtatSuiviEngagementList(val).subscribe(data =>{
  //     this.EtatSuiviEngagementList = data;
  //   });  
  //   this.p=1;
  // }

  refreshInfosBanqueList2() {
    var val = {
      STATUT:this.STATUT,
    };
    this.sharedService.getInfosBanqueList(val).subscribe(data =>{
      this.InfosBanqueList = data;
    });  
    this.p=1;
  }

     AddInfosBanque(i:any){
        this.InfosBanque = i;
        this.modopen=false;
        this.modopen=true;
      }

      closeClick(){
        this.modopen=false;
        this.refreshInfosBanqueList2();
      }

  downloadfile(i: any) {
    let nom_pdf : any = i.DDP;
    this.sharedService.getdownloadInfosBanquefile(i.NOM_FILE).subscribe(data =>{
    const blob='data:application/pdf;base64,'+data;
    FileSaver.saveAs(blob,nom_pdf);
  })
}
}
