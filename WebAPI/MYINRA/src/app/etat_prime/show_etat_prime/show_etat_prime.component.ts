import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { Router } from '@angular/router';


declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_etat_prime',
  templateUrl: './show_etat_prime.component.html',
  styleUrls: ['./show_etat_prime.component.css']
})
export class Show_etat_primeComponent implements OnInit {
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
  EtatPrimeList:any = [];
  AgentList:any = [];
  modopen=false;
  agent:any;
  modalTitle:any;
  activateAddEditStuCom:boolean = false;
  EtatPrime:any;
  DDP_SELECTIONNE: string="";
  CATEGORIE_AGENT_PRIME_ACTIF : any= [];

  constructor(private router: Router,private service: SharedService, private sharedService: SharedService) {  }
  ngOnInit() {
    this.droits_paie();
    this.refreshEtatPrimeList();
    this.refreshAgentList();   
    this.DDP_SELECTIONNE="OUI"; 
  }

  droits_paie() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getCatégorieAgentPaie(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT_PRIME_ACTIF=this.AgentList[2]["ACTIF"];
     // console.log(this.CATEGORIE_AGENT_PRIME_ACTIF)
      if (this.CATEGORIE_AGENT_PRIME_ACTIF=="NON")
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
    this.sharedService.getEtatPrimeList(val).subscribe(data =>{
      this.EtatPrimeList = data;
    }); 
    this.p=1;
  }

  vider() {    
    this.DDP_SELECTIONNE="NON";
    var val = {
      // DDP:this.agent.DDP,
      // DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatPrimeList(val).subscribe(data =>{
      this.EtatPrimeList = data;
    }); 
    this.p=1;
  }

  refreshEtatPrimeList() {
    var val = {
      DDP:this.service.userInfo.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatPrimeList(val).subscribe(data =>{
      this.EtatPrimeList = data;
      this.agent={DDP:this.service.userInfo.DDP ,NOM_PRENOM: this.service.userInfo.NOM_PRENOM};
    });  
  }

  refreshEtatPrimeList2() {
    var val = {
      DDP:this.agent.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatPrimeList(val).subscribe(data =>{
      this.EtatPrimeList = data;
    });  
  }

  refreshAgentList() {
    var val = {
      DDP:this.service.userInfo.DDP
    };
    this.sharedService.getPrimeAgentList(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT=this.AgentList[0]["CATEGORIE"];
    });
  }

  AddEtatPrime(){
    this.EtatPrime={
      DATE:"",
      DDP:"",
      DDP_DEMANDEUR:"",
      NOM_PRENOM:"",
      ANNEE:"",
      PRIME:"",
      STATUT:""
    }
    this.modopen=false;
    this.modopen=true;
  }

  closeClick(){
    this.modopen=false;
    this.refreshEtatPrimeList2();
  }

  downloadfile(i: any) {
   let nom_pdf : any = i.DDP + "_" + i.ANNEE + "_" + i.PRIME + "_" + i.DATE.substring(0,10)  + " (" + i.DDP_DEMANDEUR + ")";
    this.sharedService.getdownloadPrimefile(i.NOM_FILE).subscribe(data =>{
      const blob='data:application/pdf;base64,'+data;
      FileSaver.saveAs(blob,nom_pdf);
    })
  }
}
