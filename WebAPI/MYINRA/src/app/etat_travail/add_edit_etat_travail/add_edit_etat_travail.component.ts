import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { DatePipe } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-add_edit_etat_travail',
  templateUrl: './add_edit_etat_travail.component.html',
  styleUrls: ['./add_edit_etat_travail.component.css']
})
export class Add_edit_etat_travailComponent implements OnInit {
  today: Date;
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  DATE:any ;
  DDP: string ="";
  NOM_PRENOM: string ="";
  DDP_DEMANDEUR: string ="";
  NOM_PRENOM_DEMANDEUR: string ="";
  ANNEE: string ="";
  //MOIS: string ="";
  LANGUE: string ="";
  an: string="";
  //AnneeList:any = [];
  //MoisList:any = [];
  DATE_PDF:any = [];

  constructor(private spinner: NgxSpinnerService, private service: SharedService, private sharedService: SharedService, private datePipe: DatePipe) { 
    this.today =new Date();
  }

  ngOnInit(): void {
    this.DATE=new Date();
    this.NOM_PRENOM=this.UserInf.NOM_PRENOM;
    this.DDP=this.UserInf.DDP;
    this.NOM_PRENOM_DEMANDEUR=this.UserInf.NOM_PRENOM;
    this.DDP_DEMANDEUR=this.UserInf.DDP;
    this.LANGUE = "FRANCAIS"
  }

  addEtatTravail(){
    var val = {
      DATE:this.DATE,
      DDP:this.DDP,
      NOM_PRENOM:this.NOM_PRENOM,
      DDP_DEMANDEUR:this.sharedService.userInfo.DDP,
      NOM_PRENOM_DEMANDEUR:this.sharedService.userInfo.NOM_PRENOM,
      LANGUE:this.LANGUE
    };
    this.spinner.show();
    this.service.addEtatTravail(val).subscribe(res =>{
      if (res!="")
      {
        alert(res.toString())
        this.spinner.hide();
      }
      if (res!="Le nom en arabe n'est pas saisi, l'édition ne peut se faire !")
      {
      setTimeout( () => {
        this.spinner.hide();
            this.downloadfile();
        }, 500);
      }
    })
  }

  downloadfile() {
    this.DATE_PDF  = this.datePipe.transform(this.DATE, 'dd-MM-yyyy');
    let nom_pdf : any = this.DDP + "_" + this.DATE_PDF + "_" + this.LANGUE.substring(0,2) + " (" + this.sharedService.userInfo.DDP + ")";
    let NOM_FILE: any=".\\Etats\\SORTIE\\ETAT_TRAVAIL\\" + nom_pdf + ".pdf";
    this.sharedService.getdownloadTravailfile(NOM_FILE).subscribe(data =>{
       const blob='data:application/pdf;base64,'+data;
       FileSaver.saveAs(blob,nom_pdf);
     })
   }
  
}
