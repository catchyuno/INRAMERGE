import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { ActivatedRoute} from '@angular/router';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-view_document',
  templateUrl: './view_document.component.html',
  styleUrls: ['./view_document.component.css'] 
})
export class view_documentComponent implements OnInit {
  p: number = 1;
  CATEGORIE: any= [];
  CATEGORIE_V: any= []; 
  INTITULE: any= []; 
  documentList:any = [];
  CategoriList:any = [];
  CategorieList:any = [];
  categorienomList:any = [];
  modopen=false;
  modalTitle:any;
  document:any;
  categorie:any;

  constructor(private activatedRoute: ActivatedRoute, private service: SharedService, private sharedService: SharedService) {  }
  ngOnInit() {
    this.refreshdocumentList();
    this.refreshCatégorieList();    
  }

  choixcategorie(categorie:any) {    
    var val = {
      CATEGORIE:this.CATEGORIE.CATEGORIE
    };
    this.sharedService.getdocumentListCat(val).subscribe(data =>{
      this.documentList = data;
    }); 
    this.p=1;
  }

  refreshCatégorieList() {
    var val = {
    };
    this.sharedService.getDOCSCatégorieList(val).subscribe(data =>{
      this.CategoriList = data;
    });
  }

  refreshdocumentList() {
    var val = {
    };
    this.sharedService.getdocumentListAll(val).subscribe(data =>{
      this.documentList = data;
    });  
    this.p=1;
  }

  downloadfile(i: any) {
    let nom_pdf : any = "\Etats\\DOCUMENTS\\" + i.CATEGORIE + "\\" +i.INTITULE + ".pdf";
     this.sharedService.getdownloaddocumentfile(nom_pdf).subscribe(data =>{
       const blob='data:application/jpg;base64,'+data;
        FileSaver.saveAs(blob,nom_pdf);
    })
  }
}
