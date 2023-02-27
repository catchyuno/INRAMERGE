import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { DatePipe } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';

declare var require: any 
const FileSaver = require('file-saver');

@Component({
  selector: 'app-add_edit_etat_ccp_suivi_demandes',
  templateUrl: './add_edit_etat_ccp_suivi_demandes.component.html',
  styleUrls: ['./add_edit_etat_ccp_suivi_demandes.component.css']
})
export class Add_edit_etat_ccp_suivi_demandesComponent implements OnInit {
  @Input() suiviCCP:any;
  DATE:any ;
  DDP: string ="";
  NOM_PRENOM: string ="";
  DDP_DEMANDEUR: string ="";
  NOM_PRENOM_DEMANDEUR: string ="";
  STATUT: string ="";

  constructor(private spinner: NgxSpinnerService, private service: SharedService, private sharedService: SharedService, private datePipe: DatePipe) { }

  ngOnInit(): void {
    this.STATUT = this.suiviCCP.STATUT; 
    this.DATE = this.suiviCCP.DATE; 
    this.NOM_PRENOM = this.suiviCCP.NOM_PRENOM;
    this.DDP = this.suiviCCP.DDP;
    this.DDP_DEMANDEUR=this.suiviCCP.DDP_DEMANDEUR 
  }

  upload(val: any){
    let reader=new FileReader();
    reader.onload=()=>{
        let img;
        img=reader.result!.toString().replace('data:application/pdf;base64,','');
    this.service.uploadSuiviCCP({nom_file:img, DDP : this.DDP, DATE:this.DATE, DDP_DEMANDEUR:this.DDP_DEMANDEUR}).subscribe(data=>{
        this.ngOnInit();
    });
    }
    reader.readAsDataURL(val.target.files[0]);
    alert("Upload effectué !");
    }
}
