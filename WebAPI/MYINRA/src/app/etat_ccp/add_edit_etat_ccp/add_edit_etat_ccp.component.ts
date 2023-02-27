import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
// import { DatePipe } from '@angular/common';
// import { NgxSpinnerService } from 'ngx-spinner';

// declare var require: any
// const FileSaver = require('file-saver');

// @Component({
//   selector: 'app-add_edit_etat_revenu',
//   templateUrl: './add_edit_etat_revenu.component.html',
//   styleUrls: ['./add_edit_etat_revenu.component.css']
// })
// export class Add_edit_etat_revenuComponent implements OnInit {
//   today: Date;
//   @Input() UserInf={NOM_PRENOM:'',DDP:''};
//   DATE:any ;
//   DDP: string ="";
//   NOM_PRENOM: string ="";
//   DDP_DEMANDEUR: string ="";
//   NOM_PRENOM_DEMANDEUR: string ="";
//   ANNEE: string ="";
//   //PRIME: string ="";
//   //TYPE: string ="";
//   an: string="";
//   AnneeList:any = [];
//   PrimeList:any = [];
//   DATE_PDF:any = [];

//   constructor(private spinner: NgxSpinnerService, private service: SharedService, private sharedService: SharedService, private datePipe: DatePipe) { 
//     this.today =new Date();
//   }

//   ngOnInit(): void {
//     this.DATE=new Date();
//     this.NOM_PRENOM=this.UserInf.NOM_PRENOM;
//     this.DDP=this.UserInf.DDP;
//    // this.TYPE = "SANS CREDIT"
//     this.refreshAnneeList();
//   }

//   refreshAnneeList() {
//     var val = {
//       DDP:this.UserInf.DDP
//     };
//     this.sharedService.getRevenuAnneeList(val).subscribe(data =>{
//      this.AnneeList = data;
//      this.ANNEE=this.AnneeList[0]["ANNEE"];
//      //this.refreshPrimeList(this.AnneeList[0]["ANNEE"]);
//     });  
//   }
  
//   refreshRevenuList(i:string) {
//     var val = {
//       DDP:this.UserInf.DDP,
//       ANNEE:this.ANNEE
//     };
//     this.sharedService.getPrimePrimeList(val).subscribe(data =>{
//       this.PrimeList = data;
//       //this.PRIME=this.PrimeList[0]["PRIME"];
//     });  
//   }

//   addEtatRevenu(){
//     var val = {
//       DATE:this.DATE,
//       DDP:this.DDP,
//       NOM_PRENOM:this.NOM_PRENOM,
//       DDP_DEMANDEUR:this.sharedService.userInfo.DDP,
//       NOM_PRENOM_DEMANDEUR:this.sharedService.userInfo.NOM_PRENOM,
//       ANNEE:this.ANNEE,
//      // PRIME:this.PRIME
//       //TYPE:this.TYPE
//     };
//     this.spinner.show();
//     this.service.addEtatRevenu(val).subscribe(res =>{
//       if (res!="")
//       {
//         alert(res.toString())
//       }
//      // alert(res.toString())
//       setTimeout( () => {
//         this.spinner.hide();
//             this.downloadfile();
//         }, 2000);
//     })
//   }

//   downloadfile() {
//     this.DATE_PDF  = this.datePipe.transform(this.DATE, 'dd-MM-yyyy');
//     let nom_pdf : any = this.DDP + "_" + this.ANNEE + "_" + this.DATE_PDF + " (" + this.sharedService.userInfo.DDP + ")";
//     let NOM_FILE: any=".\\Etats\\SORTIE\\ETAT_REVENU\\" + nom_pdf + ".pdf";
//     this.sharedService.getdownloadfile(NOM_FILE).subscribe(data =>{
//        const blob='data:application/pdf;base64,'+data;
//        FileSaver.saveAs(blob,nom_pdf);
//      })
//    }
  
// }
