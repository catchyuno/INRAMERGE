import { Component } from '@angular/core';
import { Router } from '@angular/router';
//import { DatePipe } from '@angular/common';
import { SharedService } from "src/app/shared.service";
import { AuthService } from 'src/services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'MYINRA';
  modopen=false;
  CodeBanqueList:any = [];
  AlerteListPrecompte:any = [];
  AlerteListCCP:any = [];
  AlerteListEngReg:any = [];
  AlerteListMainLevee:any = [];
  AlerteListInfosBanque:any = [];
  AlerteListcin:any = [];
  AlerteTotal:any = [];
  profil:any;
  //CATEGORIE_V:any;
  //  pipe = new DatePipe('en-US');
  // now = Date.now();
  // mySimpleFormat = this.pipe.transform(this.now, 'MM/dd/yyyy');
  // myShortFormat = this.pipe.transform(this.now, 'short');
 // this.CATEGORIE_AGENT=this.AgentList[0]["CATEGORIE"];
 constructor(private service: SharedService, private sharedService: SharedService,public authService:AuthService,public router:Router) {

  if(this.authService.loggedIn()){
    let x = JSON.parse(localStorage.getItem('user')!);


  setTimeout(() => {
    if(!this.authService.loggedIn()){
      this.authService.login();
    }
  }, ((new Date(x.exp*1000).getTime())-new Date().getTime())+999);
  };
  }

  ngOnInit() {
    //this.refreshEtatEngList();
    this.refreshCodeBanqueList();
    this.droits_paie();
    this.droits_rh();
    this.droits_paramétrage();
    this.droits_suivi_demandes();
    this.administration();
    this.miseajourlangue();
    this.miseajouractifs();
    this.miseajourrubriques();
    this.miseajourinfosbanque();
    this.testCIN();
    this.testACCES();
    //this.nav.show();
    //let x = {name:'xyz'}
    this.modopen=false;
    this.modopen=true;
   this.Alerte_Nbre_Précompte();
   this.Alerte_Nbre_CCP();
   this.ShowProfil();
   // this.profil={
   //   DDP: this.service.userInfo.DDP,
   //   NOM_PRENOM:this.service.userInfo.NOM_PRENOM
   // }
  }

 Calcul_Nbre()
 {
   this.Alerte_Nbre_Précompte();
   this.Alerte_Nbre_CCP();
   this.Alerte_Nbre_Eng();
   this.Alerte_Nbre_MainLevee();
   this.Alerte_Nbre_CIN();
   this.Alerte_Nbre_InfosBanque();
   this.Alerte_Nbre_Total();
 }

 Alerte_Nbre_Total() {
   var val = {
   };
   this.sharedService.getAlerteNbreTotal(val).subscribe(data =>{
     this.AlerteTotal = data;
   });
 }

 Alerte_Nbre_CIN() {
   var val = {
   };
   this.sharedService.getAlerteNbreCIN(val).subscribe(data =>{
     this.AlerteListcin = data;
   });
 }

 Alerte_Nbre_Précompte() {
   var val = {
   };
   this.sharedService.getAlerteNbrePrécompte(val).subscribe(data =>{
     this.AlerteListPrecompte = data;
   });
 }

 Alerte_Nbre_CCP() {
   var val = {
   };
   this.sharedService.getAlerteNbreCCP(val).subscribe(data =>{
     this.AlerteListCCP = data;
   });
 }

 Alerte_Nbre_Eng() {
   var val = {
   };
   this.sharedService.getAlerteNbreEng(val).subscribe(data =>{
     this.AlerteListEngReg = data;
   });
 }

 Alerte_Nbre_MainLevee() {
   var val = {
   };
   this.sharedService.getAlerteNbreMainLevee(val).subscribe(data =>{
     this.AlerteListMainLevee = data;
   });
 }

 Alerte_Nbre_InfosBanque() {
   var val = {
   };
   this.sharedService.getAlerteNbreInfosBanque(val).subscribe(data =>{
     this.AlerteListInfosBanque = data;
   });
 }
   CATEGORIE_AGENT_ENG: any= [];
   CATEGORIE_AGENT_ENG_PERIODE: any= [];
   CATEGORIE_AGENT_ENG_REGROUPE: any= [];
   CATEGORIE_AGENT_LIQ: any= [];
   CATEGORIE_AGENT_PRIME: any= [];
   CATEGORIE_AGENT_REVENU: any= [];
   CATEGORIE_AGENT_DOMIC: any= [];
   CATEGORIE_AGENT_PRECOMPTE: any= [];
   CATEGORIE_AGENT_CCP: any= [];
   CATEGORIE_AGENT_TRAVAIL: any= [];
   CATEGORIE_AGENT_MAIN_LEVEE: any= [];
   CATEGORIE_AGENT_ENG_ACTIF: any= [];
   CATEGORIE_AGENT_ENG_PERIODE_ACTIF: any= [];
   CATEGORIE_AGENT_ENG_REGROUPE_ACTIF: any= [];
   CATEGORIE_AGENT_LIQ_ACTIF: any= [];
   CATEGORIE_AGENT_PRIME_ACTIF: any= [];
   CATEGORIE_AGENT_REVENU_ACTIF: any= [];
   CATEGORIE_AGENT_DOMIC_ACTIF: any= [];
   CATEGORIE_AGENT_PRECOMPTE_ACTIF: any= [];
   CATEGORIE_AGENT_CCP_ACTIF: any= [];
   CATEGORIE_AGENT_TRAVAIL_ACTIF: any= [];
   CATEGORIE_AGENT_MAIN_LEVEE_ACTIF: any= [];
   PAIE_RUBRIQUE: any= [];
   PAIE_BANQUE: any= [];
   PAIE_SIGNATURE: any= [];
   RH_LANGUE: any= [];
   RH_SIGNATURE: any= [];
   PAIE_RUBRIQUE_ACTIF: any= [];
   PAIE_BANQUE_ACTIF: any= [];
   PAIE_SIGNATURE_ACTIF: any= [];
   RH_LANGUE_ACTIF: any= [];
   RH_SIGNATURE_ACTIF: any= [];
   PAIE_DOCS: any= [];
   PAIE_DOCS_ACTIF: any= [];
   RH_DOCS_ACTIF: any= [];
   RH_DOCS: any= [];
   CIN: any= [];
   ATTESTATION_ETAT_CCP: any= [];
   ATTESTATION_ETAT_CCP_ACTIF: any= [];
   ATTESTATION_ETAT_PRECOMPTE: any= [];
   ATTESTATION_ETAT_PRECOMPTE_ACTIF: any= [];
   ATTESTATION_ETAT_MAIN: any= [];
   INFOS_BANQUE: any= [];
   ATTESTATION_ETAT_MAIN_ACTIF: any= [];
   ATTESTATION_ETAT_ENG: any= [];
   ATTESTATION_ETAT_ENG_ACTIF: any= [];
   DROITS_ACCES: any= [];
   DROITS_ACCES_ACTIF: any= [];
   INFOS_BANQUE_ACTIF: any= [];
   CIN_ACTIF: any= [];

   EtatEngList:any = [];
   AgentList:any = [];
   ValidationCIN:any = [];
   ValidationCINValeur: any= [];
   ACCES:any = [];
   ACCESValeur: any= [];
   droits_paie() {
   var val = {
     DDP:this.service.userInfo.DDP,
   };
   this.sharedService.getCatégorieAgentPaie(val).subscribe(data =>{
     this.AgentList = data;
     this.CATEGORIE_AGENT_ENG=this.AgentList[0]["CATEGORIE"];
     this.CATEGORIE_AGENT_LIQ=this.AgentList[1]["CATEGORIE"];
     this.CATEGORIE_AGENT_PRIME=this.AgentList[2]["CATEGORIE"];
     this.CATEGORIE_AGENT_REVENU=this.AgentList[3]["CATEGORIE"];
     this.CATEGORIE_AGENT_DOMIC=this.AgentList[4]["CATEGORIE"];
     this.CATEGORIE_AGENT_MAIN_LEVEE=this.AgentList[5]["CATEGORIE"];
     this.CATEGORIE_AGENT_PRECOMPTE=this.AgentList[6]["CATEGORIE"];
     this.CATEGORIE_AGENT_CCP=this.AgentList[7]["CATEGORIE"];
     this.CATEGORIE_AGENT_ENG_PERIODE=this.AgentList[8]["CATEGORIE"];
     this.CATEGORIE_AGENT_ENG_REGROUPE=this.AgentList[9]["CATEGORIE"];
     this.CATEGORIE_AGENT_ENG_ACTIF=this.AgentList[0]["ACTIF"];
     this.CATEGORIE_AGENT_LIQ_ACTIF=this.AgentList[1]["ACTIF"];
     this.CATEGORIE_AGENT_PRIME_ACTIF=this.AgentList[2]["ACTIF"];
     this.CATEGORIE_AGENT_REVENU_ACTIF=this.AgentList[3]["ACTIF"];
     this.CATEGORIE_AGENT_DOMIC_ACTIF=this.AgentList[4]["ACTIF"];
     this.CATEGORIE_AGENT_MAIN_LEVEE_ACTIF=this.AgentList[5]["ACTIF"];
     this.CATEGORIE_AGENT_PRECOMPTE_ACTIF=this.AgentList[6]["ACTIF"];
     this.CATEGORIE_AGENT_CCP_ACTIF=this.AgentList[7]["ACTIF"];
     this.CATEGORIE_AGENT_ENG_PERIODE_ACTIF=this.AgentList[8]["ACTIF"];
     this.CATEGORIE_AGENT_ENG_REGROUPE_ACTIF=this.AgentList[9]["ACTIF"];
   });
 }
 droits_rh() {
   var val = {
     DDP:this.service.userInfo.DDP,
   };
   this.sharedService.getCatégorieAgentRH(val).subscribe(data =>{
     this.AgentList = data;
     this.CATEGORIE_AGENT_TRAVAIL=this.AgentList[0]["CATEGORIE"];
     this.CATEGORIE_AGENT_TRAVAIL_ACTIF=this.AgentList[0]["ACTIF"];
   });
 }
 droits_paramétrage() {
   var val = {
     DDP:this.service.userInfo.DDP,
   };
   this.sharedService.getParamétrage(val).subscribe(data =>{
     this.AgentList = data;
     this.PAIE_RUBRIQUE=this.AgentList[0]["CATEGORIE"];
     this.PAIE_BANQUE=this.AgentList[1]["CATEGORIE"];
     this.PAIE_SIGNATURE=this.AgentList[2]["CATEGORIE"];
     this.PAIE_DOCS=this.AgentList[3]["CATEGORIE"];
     ///////////////////////////////////////////////////
     this.PAIE_RUBRIQUE_ACTIF=this.AgentList[0]["ACTIF"];
     this.PAIE_BANQUE_ACTIF=this.AgentList[1]["ACTIF"];
     this.PAIE_SIGNATURE_ACTIF=this.AgentList[2]["ACTIF"];
     this.PAIE_DOCS_ACTIF=this.AgentList[3]["ACTIF"];
     ///////////////////////////////////////////////////
     this.RH_LANGUE=this.AgentList[4]["CATEGORIE"];
     this.RH_SIGNATURE=this.AgentList[5]["CATEGORIE"];
     this.RH_DOCS=this.AgentList[6]["CATEGORIE"];
     ///////////////////////////////////////////////////
     this.RH_LANGUE_ACTIF=this.AgentList[4]["ACTIF"];
     this.RH_SIGNATURE_ACTIF=this.AgentList[5]["ACTIF"];
     this.RH_DOCS_ACTIF=this.AgentList[6]["CATEGORIE"];
   });
 }

 droits_suivi_demandes() {
   var val = {
     DDP:this.service.userInfo.DDP,
   };
   this.sharedService.getSuiviDemandes(val).subscribe(data =>{
     this.AgentList = data;
     this.ATTESTATION_ETAT_CCP=this.AgentList[0]["CATEGORIE"];
     this.ATTESTATION_ETAT_PRECOMPTE=this.AgentList[1]["CATEGORIE"];
     this.ATTESTATION_ETAT_ENG=this.AgentList[2]["CATEGORIE"];
     this.ATTESTATION_ETAT_MAIN=this.AgentList[3]["CATEGORIE"];
     this.INFOS_BANQUE=this.AgentList[4]["CATEGORIE"];
     this.CIN=this.AgentList[5]["CATEGORIE"];
     ///////////////////////////////////////////////////
     this.ATTESTATION_ETAT_CCP_ACTIF=this.AgentList[0]["ACTIF"];
     this.ATTESTATION_ETAT_PRECOMPTE_ACTIF=this.AgentList[1]["ACTIF"];
     this.ATTESTATION_ETAT_ENG_ACTIF=this.AgentList[2]["ACTIF"];
     this.ATTESTATION_ETAT_MAIN_ACTIF=this.AgentList[3]["ACTIF"];
     this.INFOS_BANQUE_ACTIF=this.AgentList[4]["ACTIF"];
     this.CIN_ACTIF=this.AgentList[5]["ACTIF"];
     console.log(this.CIN_ACTIF)
   });
 }

 administration() {
   var val = {
     DDP:this.service.userInfo.DDP,
   };
   this.sharedService.getAdministration(val).subscribe(data =>{
     this.AgentList = data;
     this.DROITS_ACCES=this.AgentList[0]["CATEGORIE"];
     ///////////////////////////////////////////////////
     this.DROITS_ACCES_ACTIF=this.AgentList[0]["ACTIF"];
   });
 }

 miseajourlangue() {
   var val = {
     //CATEGORIE:this.CATEGORIE.CATEGORIE
   };
   this.service.MAJLangue(val).subscribe(res =>{
          if (res!="")
        {
           // alert(res)
          }
        });
 }

 miseajouractifs() {
   var val = {
     //CATEGORIE:this.CATEGORIE.CATEGORIE
   };
   this.service.MAJActifs(val).subscribe(res =>{
          if (res!="")
        {
           // alert(res)
          }
        });
 }

 miseajourrubriques() {
   var val = {
     //CATEGORIE:this.CATEGORIE.CATEGORIE
   };
   this.service.MAJRubs(val).subscribe(res =>{
          if (res!="")
        {
           // alert(res)
          }
        });
 }

 testACCES() {
   var val = {
     DDP:this.service.userInfo.DDP,
   };
   this.sharedService.gettestACCES(val).subscribe(data =>{
     this.ACCES = data;
    this.ACCESValeur=this.ACCES[0]["ACTIVER_ACCES"];
     if (this.ACCESValeur == 'NON' && this.ValidationCINValeur == 'NON')
   {
     alert("Vous devez mettre à jour vos données CIN !");
   }
   });

 }

 ShowProfil(){
   this.modopen=false;
   this.profil={
     DDP: this.service.userInfo.DDP,
     NOM_PRENOM:this.service.userInfo.NOM_PRENOM
   }
   this.modopen=true;
 }

 testCIN() {
   var val = {
     DDP:this.service.userInfo.DDP,
   };
   this.sharedService.gettestCIN(val).subscribe(data =>{
     this.ValidationCIN = data;
     this.ValidationCINValeur=this.ValidationCIN[0]["VALIDE"];
   });
 }
 miseajourinfosbanque() {
   var val = {
   };
   this.service.MAJInfosBanque(val).subscribe(res =>{
          if (res!="")
        {
          }
        });
 }

 refreshCodeBanqueList() {
   var val = {
   };
   this.sharedService.getCodeBanqueList(val).subscribe(data =>{
     this.CodeBanqueList = data;
   });
  // this.p=1;
 }

 closeClick(){
   this.modopen=false;
  // this.refreshhelpdeskList2();
 }


logout(){
  this.authService.logOut();
}

 }





