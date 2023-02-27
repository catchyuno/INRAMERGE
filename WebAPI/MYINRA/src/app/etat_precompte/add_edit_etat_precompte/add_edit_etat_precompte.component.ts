import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { DatePipe } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-add_edit_etat_precompte',
  templateUrl: './add_edit_etat_precompte.component.html',
  styleUrls: ['./add_edit_etat_precompte.component.css']
})

export class Add_edit_etat_precompteComponent implements OnInit {
  today: Date;
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  DATE:any ;
  DDP: string ="";
  NOM_PRENOM: string ="";
  DDP_DEMANDEUR: string ="";
  NOM_PRENOM_DEMANDEUR: string ="";
  CODE_RUBRIQUE: string ="";
  RUBRIQUE_ABBREVEE: string ="";
  TYPE: string ="";
  CACHER_AJOUTER: string ="";
  rubrique: any;
  RubriqueList:any = [];

  constructor(private spinner: NgxSpinnerService, private service: SharedService, private sharedService: SharedService, private datePipe: DatePipe) { 
    this.today =new Date();
  }

  ngOnInit(): void {
    this.DATE=new Date();
    this.NOM_PRENOM=this.UserInf.NOM_PRENOM;
    this.DDP=this.UserInf.DDP;
    this.TYPE = "PRET"
    this.refreshrubriqueList();
    this.CACHER_AJOUTER="OUI";
  }

  refreshrubriqueList() {
    var val = {
      DDP:this.UserInf.DDP,
      TYPE:this.TYPE,
    };
    this.sharedService.getPrecompteRubriqueList(val).subscribe(data =>{
     this.RubriqueList = data;
    });
  }
  
  test_vide(){
    if (this.rubrique != null) {
      this.CACHER_AJOUTER="NON";
    }
    else
    {
      this.CACHER_AJOUTER="OUI";
    }
  }

  addPrecompte(){
    var val = {
      DATE:this.DATE,
      DDP:this.DDP,
      NOM_PRENOM:this.NOM_PRENOM,
      DDP_DEMANDEUR:this.sharedService.userInfo.DDP,
      NOM_PRENOM_DEMANDEUR:this.sharedService.userInfo.NOM_PRENOM,
      CODE_RUBRIQUE:this.rubrique.CODE_RUBRIQUE,
      RUBRIQUE_ABBREVEE:this.rubrique.RUBRIQUE_ABBREVEE,
      TYPE:this.TYPE,
      STATUT:"EN COURS"
    };
    this.service.addEtatPrecompte(val).subscribe(res =>{
      if (res!="")
    {
        alert(res.toString())
      }
    })
  }
  
}
