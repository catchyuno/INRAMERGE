import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { Router } from '@angular/router';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_etat_travail',
  templateUrl: './show_etat_travail.component.html',
  styleUrls: ['./show_etat_travail.component.css']
})
export class Show_etat_travailComponent implements OnInit {
//    config = {
//     displayFn:(i:any)=>{return i.DDP+' : '+i.NOM_PRENOM},
//     displayKey: 'DDP', 
//     search: true, 
//     noResultsFound: 'Agent inéxistant!', 
//     searchPlaceholder:'Rechercher',
//     placeholder:'Liste des agents'
// }
  p: number = 1;
  CATEGORIE_AGENT: any= []
  EtatTravailList:any = [];
  AgentList:any = [];
  modopen=false;
  agent:any;
  modalTitle:any;
  activateAddEditStuCom:boolean = false;
  EtatTravail:any;
  CATEGORIE_AGENT_TRAVAIL_ACTIF : any= [];
  

  constructor(private router: Router, private service: SharedService, private sharedService: SharedService) {  }
  ngOnInit() {
    this.droits_rh();
    this.refreshEtatTravailList();
    this.refreshAgentList();     
  }

  droits_rh() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getCatégorieAgentRH(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT_TRAVAIL_ACTIF=this.AgentList[0]["ACTIF"];
      if (this.CATEGORIE_AGENT_TRAVAIL_ACTIF=="NON")
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
    this.sharedService.getEtatTravailList(val).subscribe(data =>{
      this.EtatTravailList = data;
    });
    this.p=1;
  }

  refreshEtatTravailList() {
    var val = {
      DDP:this.service.userInfo.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatTravailList(val).subscribe(data =>{
      this.EtatTravailList = data;
      this.agent={DDP:this.service.userInfo.DDP ,NOM_PRENOM: this.service.userInfo.NOM_PRENOM};
    });  
  }

  refreshEtatTravailList2() {
    var val = {
      DDP:this.agent.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatTravailList(val).subscribe(data =>{
      this.EtatTravailList = data;
    });  
  }

  refreshAgentList() {
    var val = {
      DDP:this.service.userInfo.DDP
    };
    this.sharedService.getTravailAgentList(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT=this.AgentList[0]["CATEGORIE"];
    });
  }

  AddEtatTravail(){
    this.EtatTravail={
      DATE:"",
      DDP:"",
      NOM_PRENOM:"",
      DDP_DEMANDEUR:"",
      LANGUE:"",
      STATUT:""
    }
    this.modopen=false;
    this.modopen=true;
  }

  closeClick(){
    this.modopen=false;
    this.refreshEtatTravailList2();
  }

  downloadfile(i: any) {
   let nom_pdf : any = i.DDP + "_" + i.DATE.substring(0,10) + "_" + i.LANGUE.substring(0,2) + " (" + i.DDP_DEMANDEUR + ")";
    this.sharedService.getdownloadTravailfile(i.NOM_FILE).subscribe(data =>{
      const blob='data:application/pdf;base64,'+data;
      FileSaver.saveAs(blob,nom_pdf);
    })
  }
}
