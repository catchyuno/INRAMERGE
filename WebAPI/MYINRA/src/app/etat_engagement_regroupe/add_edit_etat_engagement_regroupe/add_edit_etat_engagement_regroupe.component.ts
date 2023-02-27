import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { DatePipe } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-add_edit_etat_engagement_regroupe',
  templateUrl: './add_edit_etat_engagement_regroupe.component.html',
  styleUrls: ['./add_edit_etat_engagement_regroupe.component.css']
})

export class Add_edit_etat_engagement_regroupeComponent implements OnInit {
  today: Date;
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  DATE:any ;
  DDP: string ="";
  NOM_PRENOM: string ="";
  DDP_DEMANDEUR: string ="";
  NOM_PRENOM_DEMANDEUR: string ="";
  DU: any="";
  AU: any="";
  CACHER_AJOUTER: string ="";

  constructor(private spinner: NgxSpinnerService, private service: SharedService, private sharedService: SharedService, private datePipe: DatePipe) { 
    this.today =new Date();
  }

  ngOnInit(): void {
    this.DATE=new Date();
    this.NOM_PRENOM=this.UserInf.NOM_PRENOM;
    this.DDP=this.UserInf.DDP;
    this.CACHER_AJOUTER="OUI";
  }
 
  addEngagementRegroupe(){
    var val = {
      DATE:this.DATE,
      DDP:this.DDP,
      NOM_PRENOM:this.NOM_PRENOM,
      DDP_DEMANDEUR:this.sharedService.userInfo.DDP,
      NOM_PRENOM_DEMANDEUR:this.sharedService.userInfo.NOM_PRENOM,
      DU:this.DU,
      AU:this.AU,
      STATUT:"EN COURS"
    };
    this.service.addEtatEngagementRegroupe(val).subscribe(res =>{
    if (res!="")
    {
        alert(res.toString())
      }
    })
  }
  
}
