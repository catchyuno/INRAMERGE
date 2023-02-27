import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { DatePipe } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-add_edit_etat_engagement_periode',
  templateUrl: './add_edit_etat_engagement_periode.component.html',
  styleUrls: ['./add_edit_etat_engagement_periode.component.css']
})
export class Add_edit_etat_engagement_periodeComponent implements OnInit {
  today: Date;
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  DATE:any ;
  DDP: string ="";
  NOM_PRENOM: string ="";
  ANNEE: string ="";
  ANNEE_DU: string ="";
  ANNEE_AU: string ="";
  DDP_DEMANDEUR: string ="";
  NOM_PRENOM_DEMANDEUR: string ="";
  MOIS_DU: string ="";
  MOIS_AU: string ="";
  TYPE: string ="";
  an: string="";
  AnneeListDu:any = [];
  MoisListDu:any = [];
  AnneeListAu:any = [];
  MoisListAu:any = [];
  DATE_PDF:any = [];

  constructor(private spinner: NgxSpinnerService, private service: SharedService, private sharedService: SharedService, private datePipe: DatePipe) { 
    this.today =new Date();
  }

  ngOnInit(): void {
    this.DATE=new Date();
    this.NOM_PRENOM=this.UserInf.NOM_PRENOM;
    this.DDP=this.UserInf.DDP;
    this.TYPE = "SANS CREDIT"
    this.refreshAnneeDuList();
    this.refreshAnneeAuList();
  }

  refreshAnneeDuList() {
    var val = {
      DDP:this.UserInf.DDP
    };
    this.sharedService.getAnneePerDuList(val).subscribe(data =>{
     this.AnneeListDu = data;
     this.ANNEE_DU=this.AnneeListDu[0]["ANNEE_DU"];
     this.refreshMoisDuList(this.AnneeListDu[0]["ANNEE"]);
    });  
  }
  
  refreshAnneeAuList() {
    var val = {
      DDP:this.UserInf.DDP
    };
    this.sharedService.getAnneePerAuList(val).subscribe(data =>{
     this.AnneeListAu = data;
     this.ANNEE_AU=this.AnneeListAu[0]["ANNEE_AU"];
     this.refreshMoisAuList(this.AnneeListAu[0]["ANNEE_AU"]);
    });  
  }

  refreshMoisDuList(i:string) {
    var val = {
      DDP:this.UserInf.DDP,
      ANNEE_DU:this.ANNEE_DU
    };
    this.sharedService.getMoisPerDuList(val).subscribe(data =>{
      this.MoisListDu = data;
      this.MOIS_DU=this.MoisListDu[0]["MOIS_DU"];
    });  
  }

  refreshMoisAuList(i:string) {
    var val = {
      DDP:this.UserInf.DDP,
      ANNEE_AU:this.ANNEE_AU
    };
    this.sharedService.getMoisPerAuList(val).subscribe(data =>{
      this.MoisListAu = data;
      this.MOIS_AU=this.MoisListAu[0]["MOIS_AU"];
    });  
  }

  addEtatEng(){
    var val = { 
      DATE:this.DATE,
      DDP:this.DDP,
      NOM_PRENOM:this.NOM_PRENOM,
      DDP_DEMANDEUR:this.sharedService.userInfo.DDP,
      NOM_PRENOM_DEMANDEUR:this.sharedService.userInfo.NOM_PRENOM,
      ANNEE_DU:this.ANNEE_DU,
      MOIS_DU:this.MOIS_DU,
      ANNEE_AU:this.ANNEE_AU,
      MOIS_AU:this.MOIS_AU,
      TYPE:this.TYPE
    };
    this.spinner.show();
    this.service.addEtatEngPer(val).subscribe(res =>{
    //if (res=="")
      // { 
      //   setTimeout( () => {
      //     this.spinner.hide();
      //     this.downloadfile();
      //     }, 500);
      // }
      if (res!="")
      {
        alert(res.toString())
        this.spinner.hide();
      }
      if (res=="")
      {
        setTimeout( () => {
          this.spinner.hide();
          this.downloadfile();
          }, 500);
      }
      if (res=="Etat d'engagement déjà éditée !")
      {
        //alert(res.toString())
        setTimeout( () => {
        this.spinner.hide();
        this.downloadfile();
        }, 500);
      }
      if (res=="Le mois début doit être strictement inférieur au mois fin !")
      {
        //alert(res.toString())
        setTimeout( () => {
        this.spinner.hide();
       // this.downloadfile();
        }, 500);
      }
      if (res=="L'année début doit être inférieure à l'année fin !")
      {
        //alert(res.toString())
        setTimeout( () => {
        this.spinner.hide();
       // this.downloadfile();
        }, 500);
      }
    })
  }

  // addEtatTravail(){
  //   var val = {
  //     DATE:this.DATE,
  //     DDP:this.DDP,
  //     NOM_PRENOM:this.NOM_PRENOM,
  //     DDP_DEMANDEUR:this.sharedService.userInfo.DDP,
  //     NOM_PRENOM_DEMANDEUR:this.sharedService.userInfo.NOM_PRENOM,
  //     LANGUE:this.LANGUE
  //   };
  //   this.spinner.show();
  //   this.service.addEtatTravail(val).subscribe(res =>{
  //     if (res!="")
  //     {
  //       alert(res.toString())
  //       this.spinner.hide();
  //     }
  //     if (res!="Le nom en arabe n'est pas saisi, l'édition ne peut se faire !")
  //     {
  //     setTimeout( () => {
  //       this.spinner.hide();
  //           this.downloadfile();
  //       }, 500);
  //     }
  //   })
  // }

  downloadfile() {
    this.DATE_PDF  = this.datePipe.transform(this.DATE, 'dd-MM-yyyy');
    let nom_pdf : any = this.DDP + "_" + this.ANNEE_DU + "-" + this.MOIS_DU + "_" + this.ANNEE_AU + "-" + this.MOIS_AU + "_"+ this.DATE_PDF + "_" + this.TYPE.substring(0,1) + " (" + this.sharedService.userInfo.DDP + ")";
    let NOM_FILE: any=".\\Etats\\SORTIE\\ETAT_ENG_PERIODE\\" + nom_pdf + ".pdf";
    this.sharedService.getdownloadfilePer(NOM_FILE).subscribe(data =>{
       const blob='data:application/pdf;base64,'+data;
       FileSaver.saveAs(blob,nom_pdf);
     })
   }
}
