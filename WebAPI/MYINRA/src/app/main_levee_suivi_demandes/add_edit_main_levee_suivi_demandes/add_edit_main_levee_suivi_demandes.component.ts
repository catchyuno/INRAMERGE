import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { DatePipe } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-add_edit_main_levee_suivi_demandes',
  templateUrl: './add_edit_main_levee_suivi_demandes.component.html',
  styleUrls: ['./add_edit_main_levee_suivi_demandes.component.css']
})
export class Add_edit_main_levee_suivi_demandesComponent implements OnInit {
  @Input() suiviMainLevee:any;
  DATE:any ;
  DDP: string ="";
  NOM_PRENOM: string ="";
  // DDP_DEMANDEUR: string ="";
  // NOM_PRENOM_DEMANDEUR: string ="";
  ETAT: string ="";
  BANQUE: string ="";
  RIB: string ="";
  //TYPE: string ="";

  constructor(private spinner: NgxSpinnerService, private service: SharedService, private sharedService: SharedService, private datePipe: DatePipe) { }

  ngOnInit(): void {
    this.ETAT = this.suiviMainLevee.ETAT; 
    this.DATE = this.suiviMainLevee.DATE; 
    this.NOM_PRENOM = this.suiviMainLevee.NOM_PRENOM;
    this.DDP = this.suiviMainLevee.DDP;
    this.BANQUE= this.suiviMainLevee.BANQUE;
    this.RIB= this.suiviMainLevee.RIB;
    // this.AU= this.suiviEngagement.AU;
    //this.ETAT=this.suiviMainLevee.STATUT;
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

  ValiderMainLevee(){
    var val = {
      DATE:this.DATE,
      DDP:this.DDP,
      NOM_PRENOM:this.NOM_PRENOM,
      BANQUE:this.BANQUE,
      RIB:this.RIB,
      ETAT:this.ETAT
    };
    // this.spinner.show();
    this.service.ValiderMainLevee(val).subscribe(res =>{
      if (res!="")
      {
        alert(res.toString())
      }
      // setTimeout( () => {
      //   this.spinner.hide();
      //       this.downloadfile();
      //   }, 500);
    })
  }

    // upload(val: any){
    //   let reader=new FileReader();
    //   reader.onload=()=>{
    //     let img;
    //     img=reader.result!.toString().replace('data:application/pdf;base64,','');
    //   this.service.uploadSuiviEngagement({nom_file:img, DDP : this.DDP, DDP_DEMANDEUR:this.DDP_DEMANDEUR, DATE:this.DATE, DU:this.DU, AU:this.AU}).subscribe(data=>{
    //       this.ngOnInit();
    //   });
    //   }
    //   reader.readAsDataURL(val.target.files[0]);
    //   alert("Upload effectué !");
    // }
}
