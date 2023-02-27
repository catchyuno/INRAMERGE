import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { Router } from '@angular/router';

@Component({
  selector: 'app-show_salaire_rubrique',
  templateUrl: './show_salaire_rubrique.component.html',
  styleUrls: ['./show_salaire_rubrique.component.css']
})
export class Show_salaire_rubriqueComponent implements OnInit {
  p: number = 1;
  RubriqueList:any = [];
  modopen=false;
  modalTitle:any;
  CATEGORIE: string ="TOUS";
  RUBRIQUE: string="";
  SalaireRubriqueList:any = [];
  rubrique:any;
  SalaireRubrique:any;
  PAIE_RUBRIQUE_ACTIF : any= [];
  AgentList:any = [];

  constructor(private router: Router, private service: SharedService, private sharedService: SharedService) {  }
  ngOnInit() {
    this.droits_paramétrage();
    this.refreshSalaireRubriqueList();
  }

  droits_paramétrage() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getParamétrage(val).subscribe(data =>{
      this.AgentList = data;
      this.PAIE_RUBRIQUE_ACTIF=this.AgentList[0]["ACTIF"];
      if (this.PAIE_RUBRIQUE_ACTIF=="NON")
      {
        this.router.navigate([''])
      }
    });
  }

  choixrubrique() {  
    // if (this.rubrique.CODE_RUBRIQUE!=null)
    // {
      var val = {
        CODE_RUBRIQUE:this.rubrique.CODE_RUBRIQUE
      };
        this.sharedService.getRubrique(val).subscribe(data =>{
        this.SalaireRubriqueList = data;
      }); 
      this.p=1;
    // }
  }

  choixcategorie() {  
    //this.rubrique="";
    this.rubrique={CODE_RUBRIQUE:"", RUBRIQUE: ""};
    //this.rubrique=this.rubrique.replace(":","");
    var val = {
      CATEGORIE:this.CATEGORIE
    };
      this.sharedService.getCategorie(val).subscribe(data =>{
      this.SalaireRubriqueList = data;
    }); 
    this.p=1;
  }

  refreshSalaireRubriqueList() {
    var val = {
    };
    this.sharedService.getSalaireRubriqueList(val).subscribe(data =>{
      this.SalaireRubriqueList = data;
    });  
    this.p=1;
  }

  refreshSalaireRubriqueList2() {
    var val = {
    };
    this.sharedService.getSalaireRubriqueList(val).subscribe(data =>{
      this.SalaireRubriqueList = data;
    });  
  }

  EditSalaireRubrique(item: any){
    this.rubrique = item;
    this.modopen=false;
    this.modopen=true;
  }

  closeClick(){
    this.modopen=false;
    this.refreshSalaireRubriqueList2();
  }
}
