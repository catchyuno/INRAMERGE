import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";


@Injectable({
  providedIn: 'root'
})
export class SharedService {
  //AgentList:any = [];
  //CATEGORIE_AGENT: any= []
  userInfo={DDP:'',NOM_PRENOM:"", CATEGORIE_AGENT:"AGENT"};

  readonly APIUrl = "api";
  constructor(private http: HttpClient) {
    let y =JSON.parse(localStorage.getItem('det')!);
    if(y!=null){

      this.userInfo={DDP:y.DDP,NOM_PRENOM:y.FullName, CATEGORIE_AGENT:"AGENT"};
      localStorage.setItem('curuser',JSON.stringify(this.userInfo));

    }
   }

  getStudentList(): Observable<any[]>{
    return this.http.get<any>(this.APIUrl + '/Avance_salaire');
  }

  getpersonnelList(): Observable<any[]>{
    //let np = {DDP: "1298240"};
    return this.http.get<any>(this.APIUrl + '/Avance_salaire/M');
  }

  addStudent(val:any){
    return this.http.post(this.APIUrl + '/Avance_salaire',val);
  }

  updateStudent(val:any){
    return this.http.put(this.APIUrl + '/Avance_salaire', val);
  }

  deleteStudent(id: any){
    return this.http.delete(this.APIUrl + '/Avance_salaire/'+id);
  }

                      // ETAT D'ENGAGEMENT //
  // --> AJOUT DE LA DEMANDE DE L'ETAT D'ENGAGEMENT
  addEtatEng(val:any){return this.http.post(this.APIUrl + '/EtatEng/Ajout',val);}
  // --> AFFICHAGE DE LA LISTE DES DEMANDES DE L'ETAT D'ENGAGEMENT
  getEtatEngList(val:any) {return this.http.post(this.APIUrl + '/EtatEng/ListeDemandes', val);}
  // --> REMPLISSAGE DE LA LISTE DES ANNEES INSCRITES D'UN AGENT POUR LA DEMANDE DE L'ETAT D'ENGAGEMENT
  getAnneeList(val:any) {return this.http.post(this.APIUrl + '/EtatEng/Annee', val);}
  // --> REMPLISSAGE DE LA LISTE DES ANNEES INSCRITES D'UN AGENT POUR LA DEMANDE DE L'ETAT D'ENGAGEMENT
  getMoisList(val:any) {return this.http.post(this.APIUrl + '/EtatEng/Mois', val);}
  // --> TELECHARGEMENT DE FICHIER PDF
  getdownloadfile(NOM_FILE: any): Observable<any[]>{
    const headers =new Headers();
    headers.append('Content-Type', 'application/json')
    const params = new HttpParams()
  .set('nom_file', NOM_FILE)
    return this.http.get<any>(this.APIUrl + '/EtatEng/file',{params});
  }
  // --> AFFICHAGE DE LA LISTE DES AGENTS (AGENT/HIERARCHIE/ADMIN) SELON LES DROITS
  getAgentList(val:any) {return this.http.post(this.APIUrl + '/EtatEng/ListeAgents', val);}

  getCatégorieAgentPaie(val:any) {return this.http.post(this.APIUrl + '/EtatEng/CatégorieAgentPaie', val);}
  getCatégorieAgentRH(val:any) {return this.http.post(this.APIUrl + '/EtatEng/CatégorieAgentRH', val);}
  getParamétrage(val:any) {return this.http.post(this.APIUrl + '/EtatEng/Paramétrage', val);}
  getSuiviDemandes(val:any) {return this.http.post(this.APIUrl + '/EtatEng/SuiviDemandes', val);}
  getAdministration(val:any) {return this.http.post(this.APIUrl + '/EtatEng/Administration', val);}
  getAlerteNbrePrécompte(val:any) {return this.http.post(this.APIUrl + '/EtatEng/AlerteNbrePrécompte', val);}
  getAlerteNbreCIN(val:any) {return this.http.post(this.APIUrl + '/EtatEng/AlerteNbreCIN', val);}
  getAlerteNbreCCP(val:any) {return this.http.post(this.APIUrl + '/EtatEng/AlerteNbreCCP', val);}
  getAlerteNbreEng(val:any) {return this.http.post(this.APIUrl + '/EtatEng/AlerteNbreEng', val);}
  getAlerteNbreMainLevee(val:any) {return this.http.post(this.APIUrl + '/EtatEng/AlerteNbreMainLevee', val);}
  getAlerteNbreInfosBanque(val:any) {return this.http.post(this.APIUrl + '/EtatEng/AlerteNbreInfosBanque', val);}
  getAlerteNbreTotal(val:any) {return this.http.post(this.APIUrl + '/EtatEng/AlerteNbreTotal', val);}
  gettestCIN(val:any) {return this.http.post(this.APIUrl + '/EtatEng/Test_CIN', val);}
  gettestACCES(val:any) {return this.http.post(this.APIUrl + '/EtatEng/Test_ACCES', val);}

  // INFORMATION BANCAIRE (AGENTS DOMICILIES SORTANTS)
  getInfosBanqueList(val:any) {return this.http.post(this.APIUrl + '/InfosBanque/ListeAgents', val);}
  addInfosBanque(val:any){return this.http.post(this.APIUrl + '/InfosBanque/Ajout',val);}
  getdownloadInfosBanquefile(NOM_FILE: any): Observable<any[]>{
    const headers =new Headers();
    headers.append('Content-Type', 'application/json')
    const params = new HttpParams()
  .set('nom_file', NOM_FILE)
    return this.http.get<any>(this.APIUrl + '/InfosBanque/file',{params});
  }

                   // ETAT D'ENGAGEMENT PERIODIQUE //
  // --> AJOUT DE LA DEMANDE DE L'ETAT D'ENGAGEMENT
  addEtatEngPer(val:any){return this.http.post(this.APIUrl + '/EtatEngPer/Ajout',val);}
  // --> AFFICHAGE DE LA LISTE DES DEMANDES DE L'ETAT D'ENGAGEMENT
  getEtatEngPerList(val:any) {return this.http.post(this.APIUrl + '/EtatEngPer/ListeDemandes', val);}
  // --> REMPLISSAGE DE LA LISTE DES ANNEES INSCRITES D'UN AGENT POUR LA DEMANDE DE L'ETAT D'ENGAGEMENT
  getAnneePerDuList(val:any) {return this.http.post(this.APIUrl + '/EtatEngPer/AnneeDu', val);}
  // --> REMPLISSAGE DE LA LISTE DES ANNEES INSCRITES D'UN AGENT POUR LA DEMANDE DE L'ETAT D'ENGAGEMENT
  getMoisPerDuList(val:any) {return this.http.post(this.APIUrl + '/EtatEngPer/MoisDu', val);}
  // --> TELECHARGEMENT DE FICHIER PDF
  getAnneePerAuList(val:any) {return this.http.post(this.APIUrl + '/EtatEngPer/AnneeAu', val);}
  // --> REMPLISSAGE DE LA LISTE DES ANNEES INSCRITES D'UN AGENT POUR LA DEMANDE DE L'ETAT D'ENGAGEMENT
  getMoisPerAuList(val:any) {return this.http.post(this.APIUrl + '/EtatEngPer/MoisAu', val);}

  getdownloadfilePer(NOM_FILE: any): Observable<any[]>{
    const headers =new Headers();
    headers.append('Content-Type', 'application/json')
    const params = new HttpParams()
  .set('nom_file', NOM_FILE)
    return this.http.get<any>(this.APIUrl + '/EtatEngPer/file',{params});
  }
  // --> AFFICHAGE DE LA LISTE DES AGENTS (AGENT/HIERARCHIE/ADMIN) SELON LES DROITS
  getAgentPerList(val:any) {return this.http.post(this.APIUrl + '/EtatEngPer/ListeAgents', val);}

  // getCatégorieAgentPaiePer(val:any) {return this.http.post(this.APIUrl + '/EtatEng/CatégorieAgentPaie', val);}
  // getCatégorieAgentRHPer(val:any) {return this.http.post(this.APIUrl + '/EtatEng/CatégorieAgentRH', val);}
  // getParamétragePer(val:any) {return this.http.post(this.APIUrl + '/EtatEng/Paramétrage', val);}


                      // ETAT DE PRIME //
  // --> AJOUT DE LA DEMANDE DE L'ETAT D'ENGAGEMENT
  addEtatPrime(val:any){return this.http.post(this.APIUrl + '/EtatPrime/Ajout',val);}
  // --> AFFICHAGE DE LA LISTE DES DEMANDES DE L'ETAT D'ENGAGEMENT
  getEtatPrimeList(val:any) {return this.http.post(this.APIUrl + '/EtatPrime/ListeDemandes', val);}
  // --> REMPLISSAGE DE LA LISTE DES ANNEES INSCRITES D'UN AGENT POUR LA DEMANDE DE L'ETAT D'ENGAGEMENT
  getPrimeAnneeList(val:any) {return this.http.post(this.APIUrl + '/EtatPrime/Annee', val);}
  // --> REMPLISSAGE DE LA LISTE DES ANNEES INSCRITES D'UN AGENT POUR LA DEMANDE DE L'ETAT D'ENGAGEMENT
  getPrimePrimeList(val:any) {return this.http.post(this.APIUrl + '/EtatPrime/Prime', val);}
  // --> TELECHARGEMENT DE FICHIER PDF
  getdownloadPrimefile(NOM_FILE: any): Observable<any[]>{
    const headers =new Headers();
    headers.append('Content-Type', 'application/json')
    const params = new HttpParams()
  .set('nom_file', NOM_FILE)
    return this.http.get<any>(this.APIUrl + '/EtatPrime/file',{params});
  }
  // --> AFFICHAGE DE LA LISTE DES AGENTS (AGENT/HIERARCHIE/ADMIN) SELON LES DROITS
  getPrimeAgentList(val:any) {return this.http.post(this.APIUrl + '/EtatPrime/ListeAgents', val);}

                      // ETAT DE TRAVAIL //
  // --> AJOUT DE LA DEMANDE DE L'ETAT D'ENGAGEMENT
  addEtatTravail(val:any){return this.http.post(this.APIUrl + '/EtatTravail/Ajout',val);}
  // --> AFFICHAGE DE LA LISTE DES DEMANDES DE L'ETAT D'ENGAGEMENT
  getEtatTravailList(val:any) {return this.http.post(this.APIUrl + '/EtatTravail/ListeDemandes', val);}
  // --> REMPLISSAGE DE LA LISTE DES ANNEES INSCRITES D'UN AGENT POUR LA DEMANDE DE L'ETAT D'ENGAGEMENT
  //getPrimeAnneeList(val:any) {return this.http.post(this.APIUrl + '/EtatPrime/Annee', val);}
  // --> REMPLISSAGE DE LA LISTE DES ANNEES INSCRITES D'UN AGENT POUR LA DEMANDE DE L'ETAT D'ENGAGEMENT
  //getPrimePrimeList(val:any) {return this.http.post(this.APIUrl + '/EtatPrime/Prime', val);}
  // --> TELECHARGEMENT DE FICHIER PDF
  getdownloadTravailfile(NOM_FILE: any): Observable<any[]>{
    const headers =new Headers();
    headers.append('Content-Type', 'application/json')
    const params = new HttpParams()
  .set('nom_file', NOM_FILE)
    return this.http.get<any>(this.APIUrl + '/EtatTravail/file',{params});
  }
  // --> AFFICHAGE DE LA LISTE DES AGENTS (AGENT/HIERARCHIE/ADMIN) SELON LES DROITS
  getTravailAgentList(val:any) {return this.http.post(this.APIUrl + '/EtatTravail/ListeAgents', val);}

                   // ETAT DE REVENU //
  // --> AJOUT DE LA DEMANDE DE L'ETAT DE REVENU
  addEtatRevenu(val:any){return this.http.post(this.APIUrl + '/EtatRevenu/Ajout',val);}
  // --> AFFICHAGE DE LA LISTE DES DEMANDES DE L'ETAT DE REVENU
  getEtatRevenuList(val:any) {return this.http.post(this.APIUrl + '/EtatRevenu/ListeDemandes', val);}
  // --> REMPLISSAGE DE LA LISTE DES ANNEES INSCRITES D'UN AGENT POUR LA DEMANDE DE L'ETAT DE REVENU
  getRevenuAnneeList(val:any) {return this.http.post(this.APIUrl + '/EtatRevenu/Annee', val);}
  // --> REMPLISSAGE DE LA LISTE DES ANNEES INSCRITES D'UN AGENT POUR LA DEMANDE DE L'ETAT D'ENGAGEMENT
  // getPrimePrimeList(val:any) {return this.http.post(this.APIUrl + '/EtatPrime/Prime', val);}
  // --> TELECHARGEMENT DE FICHIER PDF
  getdownloadRevenufile(NOM_FILE: any): Observable<any[]>{
    const headers =new Headers();
    headers.append('Content-Type', 'application/json')
    const params = new HttpParams()
  .set('nom_file', NOM_FILE)
    return this.http.get<any>(this.APIUrl + '/EtatRevenu/file',{params});
  }
  // --> AFFICHAGE DE LA LISTE DES AGENTS (AGENT/HIERARCHIE/ADMIN) SELON LES DROITS
  getRevenuAgentList(val:any) {return this.http.post(this.APIUrl + '/EtatRevenu/ListeAgents', val);}

 // DEMANDE D'ETAT CCP
  addEtatCCP(val:any){return this.http.post(this.APIUrl + '/EtatCCP/Ajout',val);}
  getEtatCCPList(val:any) {return this.http.post(this.APIUrl + '/EtatCCP/ListeDemandes', val);}
  deleteCCP(val: any){return this.http.post(this.APIUrl + '/EtatCCP/Delete',val);}
  getdownloadCCPfile(NOM_FILE: any): Observable<any[]>{
    const headers =new Headers();
    headers.append('Content-Type', 'application/json')
    const params = new HttpParams()
  .set('nom_file', NOM_FILE)
    return this.http.get<any>(this.APIUrl + '/EtatCCP/file',{params});
  }
  getCCPAgentList(val:any) {return this.http.post(this.APIUrl + '/EtatCCP/ListeAgents', val);}

  // SUIVI DEMANDE D'ETAT CCP
  // addEtatSuiviCCP(val:any){return this.http.post(this.APIUrl + '/EtatSuiviCCP/Ajout',val);}
  getEtatSuiviCCPList(val:any) {return this.http.post(this.APIUrl + '/EtatSuiviCCP/ListeDemandes', val);}
  getdownloadSuiviCCPfile(NOM_FILE: any): Observable<any[]>{
    const headers =new Headers();
    headers.append('Content-Type', 'application/json')
    const params = new HttpParams()
  .set('nom_file', NOM_FILE)
    return this.http.get<any>(this.APIUrl + '/EtatCCP/file',{params});
  }
  getSuiviCCPAgentList(val:any) {return this.http.post(this.APIUrl + '/EtatSuiviCCP/ListeAgents', val);}
  uploadSuiviCCP(val:any){return this.http.post(this.APIUrl + '/EtatSuiviCCP/upload',val);}

  // SUIVI DEMANDE D'ETAT PRECOMPTE
  getEtatSuiviPrecompteList(val:any) {return this.http.post(this.APIUrl + '/EtatSuiviPrecompte/ListeDemandes', val);}
  getdownloadSuiviPrecomptefile(NOM_FILE: any): Observable<any[]>{
    const headers =new Headers();
    headers.append('Content-Type', 'application/json')
    const params = new HttpParams()
  .set('nom_file', NOM_FILE)
    return this.http.get<any>(this.APIUrl + '/EtatPrecompte/file',{params});
  }
  getSuiviPrecompteAgentList(val:any) {return this.http.post(this.APIUrl + '/EtatSuiviPrecompte/ListeAgents', val);}
  uploadSuiviPrecompte(val:any){return this.http.post(this.APIUrl + '/EtatSuiviPrecompte/upload',val);}

  // SUIVI MAJ DES CIN
  updateCIN(val:any){return this.http.post(this.APIUrl + '/CIN/Update_CIN',val);}
  getCINList(val:any) {return this.http.post(this.APIUrl + '/CIN/ListeDemandes', val);}
  getdownloadCINfile(NOM_FILE: any): Observable<any[]>{
    const headers =new Headers();
    headers.append('Content-Type', 'application/json')
    const params = new HttpParams()
  .set('nom_file', NOM_FILE)
    return this.http.get<any>(this.APIUrl + '/CIN/file',{params});
  }
  // getCINAgentList(val:any) {return this.http.post(this.APIUrl + '/CIN/ListeAgents', val);}


  // SUIVI ENGAGEMENT REGROUPE
  getEtatSuiviEngagementList(val:any) {return this.http.post(this.APIUrl + '/EtatSuiviEngagement/ListeDemandes', val);}
  getdownloadSuiviEngagementfile(NOM_FILE: any): Observable<any[]>{
    const headers =new Headers();
    headers.append('Content-Type', 'application/json')
    const params = new HttpParams()
  .set('nom_file', NOM_FILE)
    return this.http.get<any>(this.APIUrl + '/EtatSuiviEngagement/file',{params});
  }
  getSuiviEngagementAgentList(val:any) {return this.http.post(this.APIUrl + '/EtatSuiviEngagement/ListeAgents', val);}
  uploadSuiviEngagement(val:any){return this.http.post(this.APIUrl + '/EtatSuiviEngagement/upload',val);}

  // SUIVI MAINLEVEE
  getMainLeveeSuiviList(val:any) {return this.http.post(this.APIUrl + '/MainLeveeSuivi/ListeDemandes', val);}
  getdownloadMainLeveeSuivifile(NOM_FILE: any): Observable<any[]>{
    const headers =new Headers();
    headers.append('Content-Type', 'application/json')
    const params = new HttpParams()
  .set('nom_file', NOM_FILE)
    return this.http.get<any>(this.APIUrl + '/MainLeveeSuivi/file',{params});
  }
  getMainLeveeSuiviAgentList(val:any) {return this.http.post(this.APIUrl + '/MainLeveeSuivi/ListeAgents', val);}
  ValiderMainLevee(val:any){return this.http.post(this.APIUrl + '/MainLeveeSuivi/Ajout',val);}

   // DEMANDE D'ETAT PRECOMPTE
   addEtatPrecompte(val:any){return this.http.post(this.APIUrl + '/EtatPrecompte/Ajout',val);}
   getEtatPrecompteList(val:any) {return this.http.post(this.APIUrl + '/EtatPrecompte/ListeDemandes', val);}
   deletePrecompte(val: any){return this.http.post(this.APIUrl + '/EtatPrecompte/Delete',val);}
   getdownloadPrecomptefile(NOM_FILE: any): Observable<any[]>{
     const headers =new Headers();
     headers.append('Content-Type', 'application/json')
     const params = new HttpParams()
   .set('nom_file', NOM_FILE)
     return this.http.get<any>(this.APIUrl + '/EtatPrecompte/file',{params});
   }
   getPrecompteAgentList(val:any) {return this.http.post(this.APIUrl + '/EtatPrecompte/ListeAgents', val);}
   getPrecompteRubriqueList(val:any) {return this.http.post(this.APIUrl + '/EtatPrecompte/ListeRubrique', val);}


   // DEMANDE D'ETAT ENGAGEMENT REGROUPE
   addEtatEngagementRegroupe(val:any){return this.http.post(this.APIUrl + '/EtatEngagementRegroupe/Ajout',val);}
   //addEtatEngagementRegroupe(val:any){return this.http.post(this.APIUrl + '/EtatEngagementRegroupe/Ajout',val);}
   getEtatEngagementRegroupeList(val:any) {return this.http.post(this.APIUrl + '/EtatEngagementRegroupe/ListeDemandes', val);}
   deleteEngagementRegroupe(val: any){return this.http.post(this.APIUrl + '/EtatEngagementRegroupe/Delete',val);}
   getdownloadEngagementRegroupefile(NOM_FILE: any): Observable<any[]>{
     const headers =new Headers();
     headers.append('Content-Type', 'application/json')
     const params = new HttpParams()
   .set('nom_file', NOM_FILE)
     return this.http.get<any>(this.APIUrl + '/EtatEngagementRegroupe/file',{params});
   }
   getEngagementRegroupeAgentList(val:any) {return this.http.post(this.APIUrl + '/EtatEngagementRegroupe/ListeAgents', val);}

                   // SALAIRE RUBRIQUES //
  // --> MISE A JOUR DE LA RUBRIQUE
       // updatesignature(val:any){return this.http.put(this.APIUrl + '/Signature', val);}
  updateSalaireRubrique(val:any){return this.http.put(this.APIUrl + '/SalaireRubrique',val);}
  getSalaireRubriqueList(val:any) {return this.http.post(this.APIUrl + '/SalaireRubrique/ListeRubrique', val);}
  getRubrique(val:any) {return this.http.post(this.APIUrl + '/SalaireRubrique/Rubriqu', val);}
  getCategorie(val:any) {return this.http.post(this.APIUrl + '/SalaireRubrique/Categorie', val);}

  // CODES DE LA BANQUE
  getBanque(val:any) {return this.http.post(this.APIUrl + '/CodeBanque/Banque', val);}
  getCodeBanqueList(val:any) {return this.http.post(this.APIUrl + '/CodeBanque/Liste', val);}
  getCodeBanqueList_SANS_MAJ(val:any) {return this.http.post(this.APIUrl + '/CodeBanque/Liste_sansmaj', val);}
  updatecodebanque(val:any){return this.http.put(this.APIUrl + '/CodeBanque',val);}
  getBanqueList(val:any) {return this.http.post(this.APIUrl + '/CodeBanque/ListeBanque', val);}
  getBanqueAR(val:any) {return this.http.post(this.APIUrl + '/CodeBanque/BanqueAR', val);}


  // --> REMPLISSAGE DE LA LISTE DES ANNEES INSCRITES D'UN AGENT POUR LA DEMANDE DE L'ETAT DE REVENU
  // getRevenuAnneeList(val:any) {return this.http.post(this.APIUrl + '/EtatRevenu/Annee', val);}
  // --> REMPLISSAGE DE LA LISTE DES ANNEES INSCRITES D'UN AGENT POUR LA DEMANDE DE L'ETAT D'ENGAGEMENT
  // getPrimePrimeList(val:any) {return this.http.post(this.APIUrl + '/EtatPrime/Prime', val);}
  // --> TELECHARGEMENT DE FICHIER PDF
  // getdownloadRevenufile(NOM_FILE: any): Observable<any[]>{
  //   const headers =new Headers();
  //   headers.append('Content-Type', 'application/json')
  //   const params = new HttpParams()
  // .set('nom_file', NOM_FILE)
  //   return this.http.get<any>(this.APIUrl + '/EtatRevenu/file',{params});
  // }
  // --> AFFICHAGE DE LA LISTE DES AGENTS (AGENT/HIERARCHIE/ADMIN) SELON LES DROITS
  // getRevenuAgentList(val:any) {return this.http.post(this.APIUrl + '/EtatRevenu/ListeAgents', val);}
  // SUPPRESSION DE LA RUBRIQUE
  deleteSalaireRubrique(id: any){return this.http.delete(this.APIUrl + '/SalaireRubrique/'+id);}



  // ETAT DE LIQUIDATION
  addEtatLiq(val:any){return this.http.post(this.APIUrl + '/EtatLiq/Ajout',val);}
  getEtatLiqList(val:any) {return this.http.post(this.APIUrl + '/EtatLiq/ListeDemandes', val);}
  getAnneeLiqList(val:any) {return this.http.post(this.APIUrl + '/EtatLiq/Annee', val);}
  getMoisLiqList(val:any) {return this.http.post(this.APIUrl + '/EtatLiq/Mois', val);}
  getPeriodeLiqList(val:any) {return this.http.post(this.APIUrl + '/EtatLiq/Periode', val);}
  getdownloadLiqfile(NOM_FILE: any): Observable<any[]>{
    const headers =new Headers();
    headers.append('Content-Type', 'application/json')
    const params = new HttpParams()
  .set('nom_file', NOM_FILE)
    return this.http.get<any>(this.APIUrl + '/EtatLiq/file',{params});
  }
  getAgentLiqList(val:any) {return this.http.post(this.APIUrl + '/EtatLiq/ListeAgents', val);}



//SIGNATURE
addsignature(val:any){return this.http.post(this.APIUrl + '/Signature/Ajout',val);}
deletesignature(val: any){return this.http.post(this.APIUrl + '/Signature/Delete',val);}
getsignatureList(val:any) {return this.http.post(this.APIUrl + '/Signature/Liste', val);}
updatesignature(val:any){return this.http.put(this.APIUrl + '/Signature', val);}
getsignatureAgentList(val:any) {return this.http.post(this.APIUrl + '/Signature/ListeAgents', val);}
getsignatureAgentListSIG(val:any) {return this.http.post(this.APIUrl + '/Signature/ListeAgentsSiG', val);}
getdownloadsignaturefile(NOM_FILE: any): Observable<any[]>{
  const headers =new Headers();
  headers.append('Content-Type', 'application/json')
  const params = new HttpParams()
.set('nom_file', NOM_FILE)
  return this.http.get<any>(this.APIUrl + '/Signature/file',{params});
}
uploadSignature(val:any){return this.http.post(this.APIUrl + '/Signature/Upload',val);}
uploadEntete(val:any){return this.http.post(this.APIUrl + '/Signature/UploadEntete',val);}
uploadPied(val:any){return this.http.post(this.APIUrl + '/Signature/UploadPied',val);}
getdownloadEntetefile(NOM_FILE: any): Observable<any[]>{
  const headers =new Headers();
  headers.append('Content-Type', 'application/json')
  const params = new HttpParams()
.set('nom_file', NOM_FILE)
  return this.http.get<any>(this.APIUrl + '/Signature/Entetefile',{params});
}
getdownloadPiedfile(NOM_FILE: any): Observable<any[]>{
  const headers =new Headers();
  headers.append('Content-Type', 'application/json')
  const params = new HttpParams()
.set('nom_file', NOM_FILE)
  return this.http.get<any>(this.APIUrl + '/Signature/Piedfile',{params});
}
  //BANQUE
  addbanque(val:any){return this.http.post(this.APIUrl + '/Banque/Ajout',val);}
  deletebanque(val: any){return this.http.post(this.APIUrl + '/Banque/Delete',val);}
  getbanqueList(val:any) {return this.http.post(this.APIUrl + '/Banque/Liste', val);}
  updatebanque(val:any){return this.http.put(this.APIUrl + '/Banque', val);}
  //getlignebanque(val:any) {return this.http.post(this.APIUrl + '/Banque/Ligne', val);}
 //LANGUE
  getarabeList(val:any) {return this.http.post(this.APIUrl + '/Arabe/Liste', val);}
  updatearabe(val:any){return this.http.put(this.APIUrl + '/Arabe', val);}
  getcategorieList(val:any) {return this.http.post(this.APIUrl + '/Arabe/Listecategories', val);}
  MAJLangue(val:any) {return this.http.post(this.APIUrl + '/Arabe/MAJ', val);}
  MAJActifs(val:any) {return this.http.post(this.APIUrl + '/Arabe/MAJ_ACTIFS', val);}
  MAJRubs(val:any) {return this.http.post(this.APIUrl + '/Arabe/MAJ_RUBS', val);}
  MAJInfosBanque(val:any) {return this.http.post(this.APIUrl + '/InfosBanque/MAJ_InfosBanque', val);}

  //getlignebanque(val:any) {return this.http.post(this.APIUrl + '/Banque/Ligne', val);}

  // CIN
  addCIN(val:any){return this.http.post(this.APIUrl + '/MonProfil/Ajout_CIN',val);}
  uploadCIN(val:any){return this.http.post(this.APIUrl + '/MonProfil/Upload_CIN',val);}
  getpicprofil(ddp:string):Observable<any[]>{
    const headers =new Headers();
    headers.append('Content-Type', 'application/json')
    const params = new HttpParams()
  .set('ddp', ddp)
    return this.http.get<any>(this.APIUrl + '/MonProfil/pic',{params});
  }
  // DOCUMENT
  getcategorienom(val:any) {return this.http.post(this.APIUrl + '/Document/CategorieNom', val);}
  adddocument(val:any){return this.http.post(this.APIUrl + '/Document/Ajout',val);}
  deletedocument(val: any){return this.http.post(this.APIUrl + '/Document/Delete',val);}
  getdocumentList(val:any) {return this.http.post(this.APIUrl + '/Document/Liste', val);}
  getdocumentListAll(val:any) {return this.http.post(this.APIUrl + '/Document/ListeAll', val);}
  getdocumentListCat(val:any) {return this.http.post(this.APIUrl + '/Document/ListeCat', val);}
  getDOCSCatégorieList(val:any) {return this.http.post(this.APIUrl + '/Document/ListeCatégories', val);}
  updatedocument(val:any){return this.http.put(this.APIUrl + '/Document', val);}
  uploaddocument(val:any){return this.http.post(this.APIUrl + '/Document/Upload',val);}
  getdownloaddocumentfile(NOM_FILE: any): Observable<any[]>{
    const headers =new Headers();
    headers.append('Content-Type', 'application/json')
    const params = new HttpParams()
  .set('nom_file', NOM_FILE)
    return this.http.get<any>(this.APIUrl + '/Document/file',{params});
  }

  //HELP DESK
    getVoletList(val:any) {return this.http.post(this.APIUrl + '/HelpDesk/VoletListe', val);}
    getMenuList(val:any) {return this.http.post(this.APIUrl + '/HelpDesk/MenuListe', val);}
    addhelpdesk(val:any){return this.http.post(this.APIUrl + '/HelpDesk/Ajout',val);}
    deletehelpdesk(val: any){return this.http.post(this.APIUrl + '/HelpDesk/Delete',val);}
    gethelpdeskList(val:any) {return this.http.post(this.APIUrl + '/HelpDesk/Liste', val);}
    updatehelpdesk(val:any){return this.http.put(this.APIUrl + '/HelpDesk', val);}
    uploadhelpdesk(val:any){return this.http.post(this.APIUrl + '/HelpDesk/Upload',val);}
    // getdeletecapturehelpdeskfile(NOM_FILE: any): Observable<any[]>{
    //   const headers =new Headers();
    //   headers.append('Content-Type', 'application/json')
    //   const params = new HttpParams()
    // .set('nom_file', NOM_FILE)
    //   return this.http.get<any>(this.APIUrl + '/HelpDesk/file',{params});
    // }
    getdownloadhelpdeskfile(NOM_FILE: any): Observable<any[]>{
      const headers =new Headers();
      headers.append('Content-Type', 'application/json')
      const params = new HttpParams()
    .set('nom_file', NOM_FILE)
      return this.http.get<any>(this.APIUrl + '/HelpDesk/file',{params});
    }

    //HELP DESK (SUIVI)
    gethelpdesksuiviList(val:any) {return this.http.post(this.APIUrl + '/HelpDeskSuivi/Liste', val);}
    updatehelpdesksuivi(val:any){return this.http.put(this.APIUrl + '/HelpDeskSuivi', val);}
    getdownloadhelpdesksuivifile(NOM_FILE: any): Observable<any[]>{
      const headers =new Headers();
      headers.append('Content-Type', 'application/json')
      const params = new HttpParams()
    .set('nom_file', NOM_FILE)
      return this.http.get<any>(this.APIUrl + '/HelpDeskSuivi/file',{params});
    }

    // MON PROFIL
    getMonProfil (val:any) {return this.http.post(this.APIUrl + '/MonProfil/Profil', val);}
    getMonProfilCIN (val:any) {return this.http.post(this.APIUrl + '/MonProfil/ProfilCIN', val);}
    getPositionAdm (val1:any) {return this.http.post(this.APIUrl + '/MonProfil/Motif_Sortie', val1);}
    getMonProfilGrade (val:any) {return this.http.post(this.APIUrl + '/MonProfil/ProfilGrade', val);}
    getMonProfilGradeEffet (val2:any) {return this.http.post(this.APIUrl + '/MonProfil/ProfilGradeEffet', val2);}
    getMonProfilAff (val:any) {return this.http.post(this.APIUrl + '/MonProfil/ProfilAff', val);}
    getMonProfilFonction (val:any) {return this.http.post(this.APIUrl + '/MonProfil/ProfilFonction', val);}
    getMonProfilBanque (val:any) {return this.http.post(this.APIUrl + '/MonProfil/Banque', val);}
    getMonProfilGradeList(val:any) {return this.http.post(this.APIUrl + '/MonProfil/ListeGrades', val);}
    getMonProfilaffList(val:any) {return this.http.post(this.APIUrl + '/MonProfil/ListeAff', val);}
    getMonProfilfonctionList(val:any) {return this.http.post(this.APIUrl + '/MonProfil/ListeFonctions', val);}
    getMonProfilConjointList(val:any) {return this.http.post(this.APIUrl + '/MonProfil/ListeConjoints', val);}
    getMonProfilEnfantList(val:any) {return this.http.post(this.APIUrl + '/MonProfil/ListeEnfants', val);}
    getMonProfilSituationList(val:any) {return this.http.post(this.APIUrl + '/MonProfil/ListeSituations', val);}
    updateprofil(val:any){return this.http.put(this.APIUrl + '/MonProfil', val);}

    getMonProfilAgentList(val:any) {return this.http.post(this.APIUrl + '/MonProfil/ListeAgents', val);}

// DOMICILLIATION
getRIB_BANQUE_DOMC(val:any) {return this.http.post(this.APIUrl + '/EtatDom/RIB_BANQUE', val);}
addEtatDom(val:any){return this.http.post(this.APIUrl + '/EtatDom/Ajout',val);}
getEtatDomList(val:any) {return this.http.post(this.APIUrl + '/EtatDom/ListeDemandes', val);}
getEtatDomListGest(val:any) {return this.http.post(this.APIUrl + '/EtatDom/ListeDemandesGest', val);}
getdownloadDomfile(NOM_FILE: any): Observable<any[]>{
    const headers =new Headers();
    headers.append('Content-Type', 'application/json')
    const params = new HttpParams()
  .set('nom_file', NOM_FILE)
    return this.http.get<any>(this.APIUrl + '/EtatDom/file',{params});
  }
 getAgentDomList(val:any) {return this.http.post(this.APIUrl + '/EtatDom/ListeAgents', val);}
// MAIN LEVEE
addMainLevee(val:any){return this.http.post(this.APIUrl + '/MainLevee/Ajout',val);}
deleteMainLevee(val: any){return this.http.post(this.APIUrl + '/MainLevee/Delete',val);}
getMainLeveeList(val:any) {return this.http.post(this.APIUrl + '/MainLevee/ListeDemandes', val);}
getRIB_BANQUE(val:any) {return this.http.post(this.APIUrl + '/MainLevee/RIB_BANQUE', val);}
getdownloadMainLeveefile(NOM_FILE: any): Observable<any[]>{
    const headers =new Headers();
    headers.append('Content-Type', 'application/json')
    const params = new HttpParams()
  .set('nom_file', NOM_FILE)
    return this.http.get<any>(this.APIUrl + '/MainLevee/file',{params});
  }
 getAgentMainLeveeList(val:any) {return this.http.post(this.APIUrl + '/MainLevee/ListeAgents', val);}
 uploadMainLevee(val:any){return this.http.post(this.APIUrl + '/MainLevee/Upload',val);}
// SIGNATURE

}
