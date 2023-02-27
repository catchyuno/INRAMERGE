import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { DatePipe } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-add_edit_etat_engagement_suivi_demandes',
  templateUrl: './add_edit_etat_engagement_suivi_demandes.component.html',
  styleUrls: ['./add_edit_etat_engagement_suivi_demandes.component.css']
})
export class Add_edit_etat_engagement_suivi_demandesComponent implements OnInit {
  @Input() suiviEngagement:any;
  DATE:any ;
  DDP: string ="";
  NOM_PRENOM: string ="";
  DDP_DEMANDEUR: string ="";
  NOM_PRENOM_DEMANDEUR: string ="";
  STATUT: string ="";
  DU: string ="";
  AU: string ="";
  //TYPE: string ="";

  constructor(private spinner: NgxSpinnerService, private service: SharedService, private sharedService: SharedService, private datePipe: DatePipe) { }

  ngOnInit(): void {
    this.STATUT = this.suiviEngagement.STATUT; 
    this.DATE = this.suiviEngagement.DATE; 
    this.NOM_PRENOM = this.suiviEngagement.NOM_PRENOM;
    this.DDP = this.suiviEngagement.DDP;
    //this.TYPE= this.suiviPrecompte.TYPE;
    // this.DU= this.suiviEngagement.DU;
    // this.AU= this.suiviEngagement.AU;
    this.DDP_DEMANDEUR=this.suiviEngagement.DDP_DEMANDEUR;
    if(this.DU!="" || this.DU!=null) {
      this.DU = this.suiviEngagement.DU.substring(8, 10) + "/" + this.suiviEngagement.DU.substring(5, 7) + "/" + this.suiviEngagement.DU.substring(0, 4);
    }
    if(this.AU!="" || this.AU!=null) {
      this.AU = this.suiviEngagement.AU.substring(8, 10) + "/" + this.suiviEngagement.AU.substring(5, 7) + "/" + this.suiviEngagement.AU.substring(0, 4);
    }
    if(this.DU==="//") {
      this.DU = "";
    }
    if(this.AU==="//") {
      this.AU = "";
    }
  }

    upload(val: any){
      let reader=new FileReader();
      reader.onload=()=>{
        let img;
        img=reader.result!.toString().replace('data:application/pdf;base64,','');
      this.service.uploadSuiviEngagement({nom_file:img, DDP : this.DDP, DDP_DEMANDEUR:this.DDP_DEMANDEUR, DATE:this.DATE, DU:this.DU, AU:this.AU}).subscribe(data=>{
          this.ngOnInit();
      });
      }
      reader.readAsDataURL(val.target.files[0]);
      alert("Upload effectué !");
    }
}
