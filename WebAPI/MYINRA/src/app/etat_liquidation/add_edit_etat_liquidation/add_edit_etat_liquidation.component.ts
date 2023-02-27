import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { DatePipe } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-add_edit_etat_liquidation',
  templateUrl: './add_edit_etat_liquidation.component.html',
  styleUrls: ['./add_edit_etat_liquidation.component.css']
})
export class Add_edit_etat_liquidationComponent implements OnInit {
  today: Date;
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  DATE:any ;
  DDP: string ="";
  NOM_PRENOM: string ="";
  ANNEE: string ="";
  DDP_DEMANDEUR: string ="";
  NOM_PRENOM_DEMANDEUR: string ="";
  MOIS: string ="";
  PERIODE: string ="";
  PERIODE_V: string ="";
  ORDRE: string ="";
  DATE_DEBUT_SITUATION: string ="";
  DATE_FIN_SITUATION: string ="";

  // TYPE: string ="";
  an: string="";
  AnneeList:any = [];
  MoisList:any = [];
  PeriodeList:any = [];
  DATE_PDF:any = [];

  constructor(private spinner: NgxSpinnerService, private service: SharedService, private sharedService: SharedService, private datePipe: DatePipe) { 
    this.today =new Date();
  }

  ngOnInit(): void {
    this.DATE=new Date();
    this.NOM_PRENOM=this.UserInf.NOM_PRENOM;
    this.DDP=this.UserInf.DDP;
    // this.TYPE = "SANS CREDIT"
    this.refreshAnneeList();
  }

  refreshAnneeList() {
    var val = {
      DDP:this.UserInf.DDP
    };
    this.sharedService.getAnneeLiqList(val).subscribe(data =>{
     this.AnneeList = data;
     this.ANNEE=this.AnneeList[0]["ANNEE"];
     this.refreshMoisList(this.AnneeList[0]["ANNEE"]);
    });  
  }
  
  refreshMoisList(i:string) {
    var val = {
      DDP:this.UserInf.DDP,
      ANNEE:this.ANNEE
    };
    this.sharedService.getMoisLiqList(val).subscribe(data =>{
      this.MoisList = data;
      this.MOIS=this.MoisList[0]["MOIS"];
      this.refreshPeriodeList(this.MoisList[0]["MOIS"]);
    });  
  }

  refreshPeriodeList(i:string) {
    var val = {
      DDP:this.UserInf.DDP,
      ANNEE:this.ANNEE,
      MOIS:this.MOIS
    };
    this.sharedService.getPeriodeLiqList(val).subscribe(data =>{
      this.PeriodeList = data;
      this.PERIODE=this.PeriodeList[0]["ORDRE"];
    });  
  }

  addEtatLiq(){
    var val = {
      DATE:this.DATE,
      DDP:this.DDP,
      NOM_PRENOM:this.NOM_PRENOM,
      DDP_DEMANDEUR:this.sharedService.userInfo.DDP,
      NOM_PRENOM_DEMANDEUR:this.sharedService.userInfo.NOM_PRENOM,
      ANNEE:this.ANNEE,
      MOIS:this.MOIS,
      ORDRE:this.PERIODE
    };
    this.spinner.show();
    this.service.addEtatLiq(val).subscribe(res =>{
    if (res=="")
      { 
        setTimeout( () => {
          this.spinner.hide();
          this.downloadLiqfile();
          }, 500);
      }
      if (res=="Etat de liquidation déjà éditée !")
      {
        alert(res.toString())
        setTimeout( () => {
        this.spinner.hide();
        this.downloadLiqfile();
        }, 500);
      }
    })
  }

  downloadLiqfile() {
    if (this.PERIODE=="TOUS")
    {
    this.PERIODE_V = "TOUS"
    }
    else
    {
      this.PERIODE_V = this.PERIODE.replace(/\//g, "-");
    }

    this.DATE_PDF  = this.datePipe.transform(this.DATE, 'dd-MM-yyyy');
    let nom_pdf : any = this.DDP + " _ " + this.PERIODE_V +  " _ " + this.ANNEE + "-" + this.MOIS + " _ "+ this.DATE_PDF + " (" + this.sharedService.userInfo.DDP + ")";
    let NOM_FILE: any=".\\Etats\\SORTIE\\ETAT_LIQ\\" + nom_pdf + ".pdf";
    this.sharedService.getdownloadLiqfile(NOM_FILE).subscribe(data =>{
       const blob='data:application/pdf;base64,'+data;
       FileSaver.saveAs(blob,nom_pdf);
     })
   }
  
}
