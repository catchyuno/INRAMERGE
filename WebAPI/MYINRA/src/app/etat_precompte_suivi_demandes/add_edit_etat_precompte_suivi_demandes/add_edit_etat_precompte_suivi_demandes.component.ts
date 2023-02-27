import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { DatePipe } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-add_edit_etat_precompte_suivi_demandes',
  templateUrl: './add_edit_etat_precompte_suivi_demandes.component.html',
  styleUrls: ['./add_edit_etat_precompte_suivi_demandes.component.css']
})
export class Add_edit_etat_precompte_suivi_demandesComponent implements OnInit {
  @Input() suiviPrecompte:any;
  DATE:any ;
  DDP: string ="";
  NOM_PRENOM: string ="";
  DDP_DEMANDEUR: string ="";
  NOM_PRENOM_DEMANDEUR: string ="";
  STATUT: string ="";
  CODE_RUBRIQUE: string ="";
  RUBRIQUE_ABBREVEE: string ="";
  TYPE: string ="";

  constructor(private spinner: NgxSpinnerService, private service: SharedService, private sharedService: SharedService, private datePipe: DatePipe) { }

  ngOnInit(): void {
    this.STATUT = this.suiviPrecompte.STATUT; 
    this.DATE = this.suiviPrecompte.DATE; 
    this.NOM_PRENOM = this.suiviPrecompte.NOM_PRENOM;
    this.DDP = this.suiviPrecompte.DDP;
    this.TYPE= this.suiviPrecompte.TYPE;
    this.CODE_RUBRIQUE= this.suiviPrecompte.CODE_RUBRIQUE;
    this.RUBRIQUE_ABBREVEE= this.suiviPrecompte.RUBRIQUE_ABBREVEE;
    this.DDP_DEMANDEUR=this.suiviPrecompte.DDP_DEMANDEUR;
  }

    upload(val: any){
      let reader=new FileReader();
      reader.onload=()=>{
        let img;
        img=reader.result!.toString().replace('data:application/pdf;base64,','');
      this.service.uploadSuiviPrecompte({nom_file:img, DDP : this.DDP, DDP_DEMANDEUR:this.DDP_DEMANDEUR, DATE:this.DATE, TYPE:this.TYPE, CODE_RUBRIQUE:this.CODE_RUBRIQUE}).subscribe(data=>{
          this.ngOnInit();
      });
      }
      reader.readAsDataURL(val.target.files[0]);
      alert("Upload effectué !");
    }
}
