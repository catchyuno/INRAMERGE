import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { DatePipe } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-add_edit_infos_banque',
  templateUrl: './add_edit_infos_banque.component.html',
  styleUrls: ['./add_edit_infos_banque.component.css']
})
export class Add_edit_infos_banqueComponent implements OnInit {
  @Input() InfosBanque:any;
  DATE:any ;
  DDP: string ="";
  NOM_PRENOM: string ="";
  BANQUE: string ="";
  RIB: string ="";
  STATUT: string ="";
  // DU: string ="";
  // AU: string ="";
  //TYPE: string ="";

  constructor(private spinner: NgxSpinnerService, private service: SharedService, private sharedService: SharedService, private datePipe: DatePipe) { }

  ngOnInit(): void {
    this.STATUT = this.InfosBanque.STATUT; 
    this.DATE = this.InfosBanque.DATE; 
    this.NOM_PRENOM = this.InfosBanque.NOM_PRENOM;
    this.DDP = this.InfosBanque.DDP;
    this.BANQUE = this.InfosBanque.BANQUE;
    this.RIB = this.InfosBanque.RIB;
    //this.TYPE= this.suiviPrecompte.TYPE;
    // this.DU= this.suiviEngagement.DU;
    // this.AU= this.suiviEngagement.AU;
    // this.DDP_DEMANDEUR=this.suiviEngagement.DDP_DEMANDEUR;
    // if(this.DU!="" || this.DU!=null) {
    //   this.DU = this.suiviEngagement.DU.substring(8, 10) + "/" + this.suiviEngagement.DU.substring(5, 7) + "/" + this.suiviEngagement.DU.substring(0, 4);
    // }
    // if(this.AU!="" || this.AU!=null) {
    //   this.AU = this.suiviEngagement.AU.substring(8, 10) + "/" + this.suiviEngagement.AU.substring(5, 7) + "/" + this.suiviEngagement.AU.substring(0, 4);
    // }
    // if(this.DU==="//") {
    //   this.DU = "";
    // }
    // if(this.AU==="//") {
    //   this.AU = "";
    // }
  }

  addInfosBanque(){
    var val = {
      DATE:this.DATE,
      DDP:this.DDP,
      NOM_PRENOM:this.NOM_PRENOM,
      // DDP_DEMANDEUR:this.sharedService.userInfo.DDP,
      // NOM_PRENOM_DEMANDEUR:this.sharedService.userInfo.NOM_PRENOM,
      BANQUE:this.BANQUE,
      RIB:this.RIB,
      STATUT:this.STATUT
    };
    // this.spinner.show();
    // this.service.addInfosBanque(val).subscribe(res =>{
    // })

    this.spinner.show();
    this.service.addInfosBanque(val).subscribe(res =>{
      // if (res!="")
      // {
      //   alert(res.toString())
      // }
     // alert(res.toString())
      setTimeout( () => {
        this.spinner.hide();
        //console.log('rrrr')
            this.downloadfile();
        }, 2000);
    })

  }

  
  downloadfile() {
   // this.DATE_PDF  = this.datePipe.transform(this.DATE, 'dd-MM-yyyy');
    let nom_pdf : any = this.DDP;
    let NOM_FILE: any=".\\Etats\\SORTIE\\INFOS_BANQUE\\" + nom_pdf + ".pdf";
    //console.log
    this.sharedService.getdownloadInfosBanquefile(NOM_FILE).subscribe(data =>{
       const blob='data:application/pdf;base64,'+data;
       FileSaver.saveAs(blob,nom_pdf);
     })
   }
   

    // upload(val: any){
    //   let reader=new FileReader();
    //   reader.onload=()=>{
    //     let img;
    //     img=reader.result!.toString().replace('data:application/pdf;base64,','');
    //   this.service.uploadSuiviEngagement({nom_file:img, DDP : this.DDP, DATE:this.DATE}).subscribe(data=>{
    //       this.ngOnInit();
    //   });
    //   }
    //   reader.readAsDataURL(val.target.files[0]);
    //   alert("Upload effectué !");
    // }
}
