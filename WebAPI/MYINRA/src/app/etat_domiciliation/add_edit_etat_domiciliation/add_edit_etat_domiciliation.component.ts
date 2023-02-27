import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { DatePipe } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-add_edit_etat_domiciliation',
  templateUrl: './add_edit_etat_domiciliation.component.html',
  styleUrls: ['./add_edit_etat_domiciliation.component.css']
})
export class Add_edit_etat_domiciliationComponent implements OnInit {
  today: Date;
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  DATE:any ;
  DDP: string ="";
  NOM_PRENOM: string ="";
  DDP_DEMANDEUR: string ="";
  NOM_PRENOM_DEMANDEUR: string ="";
  DATE_PDF:any = [];
  domic:any;
  RIB_BANQUE_LISTE:any = [];
  BANQUE: any;
  RIB:any;
  
  constructor(private spinner: NgxSpinnerService, private service: SharedService, private sharedService: SharedService, private datePipe: DatePipe) { 
    this.today =new Date();
  }

  ngOnInit(): void {
    this.DATE=new Date();
    this.NOM_PRENOM=this.UserInf.NOM_PRENOM;
    this.DDP=this.UserInf.DDP;
    this.RIB_BANQUE();
  }

  RIB_BANQUE() {
    var val = {
      DDP:this.UserInf.DDP,
    };
    this.sharedService.getRIB_BANQUE_DOMC(val).subscribe(data =>{
     this.RIB_BANQUE_LISTE = data;
      this.BANQUE=this.RIB_BANQUE_LISTE[0]["BANQUE"];
      this.RIB=this.RIB_BANQUE_LISTE[0]["RIB"];
    });  
  }

  addEtatDom(){
    var val = {
      BANQUE:this.BANQUE,
      DATE:this.DATE,
      DDP:this.DDP,
      NOM_PRENOM:this.NOM_PRENOM,
      DDP_DEMANDEUR:this.sharedService.userInfo.DDP,
      NOM_PRENOM_DEMANDEUR:this.sharedService.userInfo.NOM_PRENOM
    };
    this.spinner.show();
    this.service.addEtatDom(val).subscribe(res =>{
      if (res!="")
      {
        alert(res.toString())
      }
      if (res!="Il faut paramétrer la banque avant de valider !")
      {
        setTimeout( () => {
        this.spinner.hide();
        this.downloadfile();
        }, 500);
      }
      if (res=="Il faut paramétrer la banque avant de valider !")
      {
        this.spinner.hide();
      }
    })
  }

  downloadfile() {
    this.DATE_PDF  = this.datePipe.transform(this.DATE, 'dd-MM-yyyy');
    let nom_pdf : any = this.DDP + "_" + this.DATE_PDF + " (" + this.sharedService.userInfo.DDP + ")";
    let NOM_FILE: any=".\\Etats\\SORTIE\\ETAT_DOMICILIATION\\" + nom_pdf + ".pdf";
    this.sharedService.getdownloadDomfile(NOM_FILE).subscribe(data =>{
       const blob='data:application/pdf;base64,'+data;
       FileSaver.saveAs(blob,nom_pdf);
     })
   }
  
}
