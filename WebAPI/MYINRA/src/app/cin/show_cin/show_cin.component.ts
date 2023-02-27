import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { Router } from '@angular/router';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_cin_demandes',
  templateUrl: './show_cin.component.html',
  styleUrls: ['./show_cin.component.css']
})
export class Show_cinComponent implements OnInit {
  //@Input() UserInf={NOM_PRENOM:'',DDP:''};
  p: number = 1;
  VALIDE: string ="TOUS";
  EtatcinList:any = [];
  AgentList:any = [];
  MSG:string="";
  DDP: string ="";
  NOM_PRENOM: string ="";
  NOMAR: string ="";
  CIN: string ="";
  ADRESSE: string ="";
  // DDP_DEMANDEUR: string ="";
  // NOM_PRENOM_DEMANDEUR: string ="";
  modopen=false;
  agent:any;
  suivicin:any="";
  CIN_ACTIF : any= [];

  constructor(private router: Router, private service: SharedService, private sharedService: SharedService) {}
  ngOnInit() {
    this.droits_suivi_demandes();
    // this.refreshAgentList();  
    //this.vider();
    //this.agent={NOM_PRENOM:'TOUS',DDP:'-------'}
    this.refreshcinList()  
  }

  droits_suivi_demandes() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getSuiviDemandes(val).subscribe(data =>{
      this.AgentList = data;
      this.CIN_ACTIF=this.AgentList[5]["ACTIF"];
      if (this.CIN_ACTIF=="NON")
      {
        this.router.navigate([''])
      }
    });
  }

  // choixagent(agent:any) {    
  //   var val = {
  //     DDP:this.DDP,
  //   };
  //   this.sharedService.getCINList(val).subscribe(data =>{
  //     this.EtatcinList = data;
  //   }); 
  //   this.p=1;
  // }

  refreshcinList() {
    var val = {
      //DDP:this.DDP,
      VALIDE:this.VALIDE
    };
    this.sharedService.getCINList(val).subscribe(data =>{
      this.EtatcinList = data;
    });  
    this.p=1;
  }

  // vider() {
  //   var val = {
  //     DDP:"",
  //     VALIDE:this.VALIDE
  //   };
  //   this.sharedService.getCINList(val).subscribe(data =>{
  //     this.EtatcinList = data;
  //   });  
  //   this.p=1;
  // }

  refreshcinList2() {
    var val = {
      //DDP:this.DDP,
      VALIDE:this.VALIDE
    };
    this.sharedService.getCINList(val).subscribe(data =>{
      this.EtatcinList = data;
    });  
    this.p=1;
  }

  // refreshAgentList() {
  //   var val = {
  //     VALIDE:this.VALIDE
  //   };
  //   this.sharedService.getCINAgentList(val).subscribe(data =>{
  //     this.AgentList = data;
  //   });
  //   // this.agent={NOM_PRENOM:'TOUS',DDP:'-------'}
  //   this.refreshcinList()
  //   this.p=1;
  // }

  AddEtatSuiviPrecompte(i:any){
        this.suivicin = i;
        this.modopen=false;
        this.modopen=true;
      }

      closeClick(){
        this.modopen=false;
        this.refreshcinList2();
      }

      Editcin(item: any){
        this.suivicin = item;
        this.modopen=false;
        this.modopen=true;
      }

      // downloadfile(i: any) {
      //   let nom_pdf : any = "\Etats\\DOCUMENTS\\" + i.CATEGORIE + "\\" +i.INTITULE.replaceAll("_","'") + ".pdf";
      //    this.sharedService.getdownloaddocumentfile(nom_pdf).subscribe(data =>{
      //      const blob='data:application/jpg;base64,'+data;
      //       FileSaver.saveAs(blob,nom_pdf);
      //   })
      // }


 

  downloadfile(i: any) {
    let nom_pdf : any = "\Etats\\CIN\\" + i.DDP + ".pdf";
    this.sharedService.getdownloadCINfile(nom_pdf).subscribe(data =>{
    const blob='data:application/pdf;base64,'+data;
    this.MSG=data.toString();
    if (this.MSG=="CIN non mise en ligne par l'agent !")
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
