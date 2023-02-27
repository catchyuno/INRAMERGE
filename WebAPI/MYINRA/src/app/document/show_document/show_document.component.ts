import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { ActivatedRoute} from '@angular/router';
import { Router } from '@angular/router';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_document',
  templateUrl: './show_document.component.html',
  styleUrls: ['./show_document.component.css'] 
})
export class Show_documentComponent implements OnInit {
  p: number = 1;
  CATEGORIE: any= [];
  CATEGORIE_V: any= []; 
  INTITULE: any= []; 
  documentList:any = [];
  CategorieList:any = [];
  categorienomList:any = [];
  modopen=false;
  modalTitle:any;
  document:any;
  PAIE_DOCS_ACTIF : any= [];
  AgentList:any = [];

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private service: SharedService, private sharedService: SharedService) {  }
  ngOnInit() {
    this.droits_paramétrage();
    this.activatedRoute.queryParams.subscribe(params => {
      const name = params['name'];
      var val = {
        LINK:name
      };
      this.sharedService.getcategorienom(val).subscribe(data =>{
        this.categorienomList = data;
        this.CATEGORIE=this.categorienomList[0]["CATEGORIE"];
        this.refreshdocumentList();
      }); 
    });
  }

  droits_paramétrage() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getParamétrage(val).subscribe(data =>{
      this.AgentList = data;
      this.PAIE_DOCS_ACTIF=this.AgentList[3]["ACTIF"];
      if (this.PAIE_DOCS_ACTIF=="NON")
      {
        this.router.navigate([''])
      }
    });
  }

  refreshdocumentList() {
    var val = {
      CATEGORIE:this.CATEGORIE
    };
    this.sharedService.getdocumentList(val).subscribe(data =>{
      this.documentList = data;
    });  
    this.p=1;
  }

  refreshdocumentList2() {
    var val = {
      CATEGORIE:this.document.CATEGORIE
    };
    this.sharedService.getdocumentList(val).subscribe(data =>{
      this.documentList = data;
    });  
    this.p=1;
  }

  Adddocument(){
    this.document={
      CATEGORIE:this.CATEGORIE,
      INTITULE:""
    }
    this.modopen=false;
    this.modopen=true;
  }

  Editdocument(item: any){
    this.document = item;
    this.modopen=false;
    this.modopen=true;
  }

  deletedocument(item: any){
    this.document = item;
    if(confirm('Etes vous sûr de vouloir supprimer ce document ?')){
      var val = {
        CATEGORIE:this.CATEGORIE,
        INTITULE:this.document.INTITULE,
      };
        this.sharedService.deletedocument(val).subscribe(data =>{
        alert(data.toString());
        this.refreshdocumentList2();
      })
    }
  }

  downloadfile(i: any) {
    let nom_pdf : any = "\Etats\\DOCUMENTS\\" + i.CATEGORIE + "\\" +i.INTITULE.replaceAll("_","'") + ".pdf";
     this.sharedService.getdownloaddocumentfile(nom_pdf).subscribe(data =>{
       const blob='data:application/jpg;base64,'+data;
        FileSaver.saveAs(blob,nom_pdf);
    })
  }
    closeClick(){
    this.modopen=false;
    this.refreshdocumentList2();
  }
}
