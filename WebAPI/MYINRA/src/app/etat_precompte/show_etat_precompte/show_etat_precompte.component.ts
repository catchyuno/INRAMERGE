import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { Router } from '@angular/router';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_etat_precompte',
  templateUrl: './show_etat_precompte.component.html',
  styleUrls: ['./show_etat_precompte.component.css']
})
export class Show_etat_precompteComponent implements OnInit {
  today: Date;
  p: number = 1;
  modopen=false;
  STATUT: any= [];  
  ACTIVER_DEMANDE: any= []
  CATEGORIE_AGENT: any= []
  EtatPrecompteList:any = [];
  AgentList:any = [];
  MSG:string="";
  DATE:any ;
  DDP: string ="";
  NOM_PRENOM: string ="";
  DDP_DEMANDEUR: string ="";
  NOM_PRENOM_DEMANDEUR: string ="";
  agent:any;
  EtatPrecompte:any;
  CODE_RUBRIQUE: string ="";
  RUBRIQUE_ABBREVEE: string ="";
  TYPE: string ="";
  precompte:any;
  CATEGORIE_AGENT_PRECOMPTE_ACTIF : any= [];

  constructor(private router: Router, private service: SharedService, private sharedService: SharedService) { this.today =new Date(); }
  ngOnInit() {
    this.droits_paie();
    this.refreshEtatPrecompteList();
    this.refreshAgentList();    
  }

  droits_paie() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getCatégorieAgentPaie(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT_PRECOMPTE_ACTIF=this.AgentList[6]["ACTIF"];
      if (this.CATEGORIE_AGENT_PRECOMPTE_ACTIF=="NON")
      {
        this.router.navigate([''])
      }
    });
  }

  choixagent(agent:any) {    
    var val = {
      DDP:this.agent.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatPrecompteList(val).subscribe(data =>{
      this.EtatPrecompteList = data;
    }); 
    this.ACTIVER_DEMANDE="OUI";
    this.p=1;
  }

  vider() {    
    var val = {
      DDP:"",
      DDP_DEMANDEUR:""
    };
    this.sharedService.getEtatPrecompteList(val).subscribe(data =>{
      this.EtatPrecompteList = data;
    }); 
    this.ACTIVER_DEMANDE="NON";
    this.p=1;
  }

  refreshEtatPrecompteList() {
    var val = {
      DDP:this.service.userInfo.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatPrecompteList(val).subscribe(data =>{
      this.EtatPrecompteList = data;
      this.agent={DDP:this.service.userInfo.DDP ,NOM_PRENOM: this.service.userInfo.NOM_PRENOM};
    });  
  }

  refreshEtatPrecompteList2() {
    var val = {
      DDP:this.agent.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatPrecompteList(val).subscribe(data =>{
      this.EtatPrecompteList = data;
    });  
  }

  refreshAgentList() {
    var val = {
      DDP:this.service.userInfo.DDP
    };
    this.sharedService.getPrecompteAgentList(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT=this.AgentList[0]["CATEGORIE"];
    });
  }

  deleteprecompte(item: any){
    this.precompte = item;
    if(confirm('Etes vous sûr de vouloir supprimer cet demande de précompte ?')){
      var val = {
        TYPE:this.precompte.TYPE,
        DDP:this.agent.DDP,
        DATE:this.precompte.DATE,
        DDP_DEMANDEUR:this.sharedService.userInfo.DDP,
        CODE_RUBRIQUE:this.precompte.CODE_RUBRIQUE,
      };
        this.sharedService.deletePrecompte(val).subscribe(data =>{
        alert(data.toString());
        this.refreshEtatPrecompteList2();
      })
    }
  }

  AddEtatPrecompte(){
    this.EtatPrecompte={
      DATE:"",
      DDP:"",
      DDP_DEMANDEUR:"",
      NOM_PRENOM:"",
      ANNEE:"",
      MOIS:"",
      TYPE:"",
      CODE_RUBRIQUE:"",
      RUBRIQUE_ABBREVEE:"",
      STATUT:"EN COURS"
    }
    this.modopen=false;
    this.modopen=true;
  }

  closeClick(){
    this.modopen=false;
    this.refreshEtatPrecompteList2();
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
