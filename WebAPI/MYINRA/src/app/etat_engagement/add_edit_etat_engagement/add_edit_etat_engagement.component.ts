import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

import { SharedService } from "src/app/shared.service";
import { DatePipe } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-add_edit_etat_engagement',
  templateUrl: './add_edit_etat_engagement.component.html',
  styleUrls: ['./add_edit_etat_engagement.component.css']
})
export class Add_edit_etat_engagementComponent implements OnInit {
  today: Date;
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  @Output() rafraichir = new EventEmitter<{ ok: string }>();
  DATE:any ;
  DDP: string ="";
  NOM_PRENOM: string ="";
  ANNEE: string ="";
  DDP_DEMANDEUR: string ="";
  NOM_PRENOM_DEMANDEUR: string ="";
  MOIS: string ="";
  TYPE: string ="";
  an: string="";
  AnneeList:any = [];
  MoisList:any = [];
  DATE_PDF:any = [];
  
  constructor(private spinner: NgxSpinnerService, private service: SharedService, private sharedService: SharedService, private datePipe: DatePipe) { 
    this.today =new Date();
  }

  ngOnInit(): void {
    this.DATE=new Date();  
    this.NOM_PRENOM=this.UserInf.NOM_PRENOM;
    this.DDP=this.UserInf.DDP;   
    this.TYPE = "SANS CREDIT"
    this.refreshAnneeList();
  }

  refreshAnneeList() {
    var val = {
      DDP:this.UserInf.DDP
    };
    this.sharedService.getAnneeList(val).subscribe(data =>{
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
    this.sharedService.getMoisList(val).subscribe(data =>{
      this.MoisList = data;
      this.MOIS=this.MoisList[0]["MOIS"];
    });  
  }

  addEtatEng(){
    var val = {
      DATE:this.DATE,
      DDP:this.DDP,
      NOM_PRENOM:this.NOM_PRENOM,
      DDP_DEMANDEUR:this.sharedService.userInfo.DDP,
      NOM_PRENOM_DEMANDEUR:this.sharedService.userInfo.NOM_PRENOM,
      ANNEE:this.ANNEE,
      MOIS:this.MOIS,
      TYPE:this.TYPE 
    };
    this.spinner.show();
    this.service.addEtatEng(val).subscribe(res =>{
      if (res!="")
      {
        alert(res.toString())
      }
      setTimeout( () => {
        this.spinner.hide();
        this.downloadfile();
        }, 500);
      this.rafraichir.emit({ ok: "true" });
    })
  }

  downloadfile() {
    this.DATE_PDF  = this.datePipe.transform(this.DATE, 'dd-MM-yyyy');
    let nom_pdf : any = this.DDP + "_" + this.ANNEE + "_" + this.MOIS + "_" + this.DATE_PDF + "_" + this.TYPE.substring(0,1) + " (" + this.sharedService.userInfo.DDP + ")";
    let NOM_FILE: any=".\\Etats\\SORTIE\\ETAT_ENG_MENSUEL\\" + nom_pdf + ".pdf";
    this.sharedService.getdownloadfile(NOM_FILE).subscribe(data =>{
       const blob='data:application/pdf;base64,'+data;
       FileSaver.saveAs(blob,nom_pdf);
     })
   }
  
}
