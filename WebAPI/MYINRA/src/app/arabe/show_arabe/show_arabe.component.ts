import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { Router } from '@angular/router';

declare var require: any

@Component({
  selector: 'app-show_arabe',
  templateUrl: './show_arabe.component.html',
  styleUrls: ['./show_arabe.component.css'] 
})
export class Show_arabeComponent implements OnInit {
  p: number = 1;
  arabeList:any = [];
  CategorieList:any = [];
  modopen=false;
  CATEGORIE:any;
  modalTitle:any;
  arabe:any;
  RH_LANGUE_ACTIF : any= [];
  AgentList:any = [];

  constructor(private router: Router, private service: SharedService, private sharedService: SharedService) {  }
  ngOnInit() {
    this.droits_paramétrage();
    this.refresharabeList();
    this.refreshcategorieList();  
  }

  droits_paramétrage() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getParamétrage(val).subscribe(data =>{
      this.AgentList = data;
      this.RH_LANGUE_ACTIF=this.AgentList[4]["ACTIF"];
      if (this.RH_LANGUE_ACTIF=="NON")
      {
        this.router.navigate([''])
      }
    });
  }

  choixcategorie(CATEGORIE:any) {    
    var val = {
      CATEGORIE:this.CATEGORIE.CATEGORIE
    };
    
    this.sharedService.getarabeList(val).subscribe(data =>{
      this.arabeList = data;
    }); 
    this.p=1;
  }

  vider() {    
    var val = {
    };
    this.sharedService.getarabeList(val).subscribe(data =>{
      this.arabeList = data;
    }); 
    this.p=1;
  }

  refreshcategorieList() {
    var val = {
    };
    this.sharedService.getcategorieList(val).subscribe(data =>{
      this.CategorieList = data;
    });
  }

  refresharabeList() {
    var val = {
    };
    this.sharedService.getarabeList(val).subscribe(data =>{
      this.arabeList = data;
    });  
  }

  refresharabeList2() {
    var val = {
      CATEGORIE:this.CATEGORIE.CATEGORIE
    };
    this.sharedService.getarabeList(val).subscribe(data =>{
      this.arabeList = data;
    });  
  }

  Editarabe(item: any){
    this.arabe = item;
    this.modopen=false;
    this.modopen=true;
  }

  closeClick(){
    this.modopen=false;
    this.refresharabeList2();
  }
}
