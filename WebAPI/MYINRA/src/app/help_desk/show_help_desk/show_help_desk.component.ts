import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { ActivatedRoute} from '@angular/router';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_help_desk',
  templateUrl: './show_help_desk.component.html',
  styleUrls: ['./show_help_desk.component.css'] 
})
export class Show_help_deskComponent implements OnInit {
  p: number = 1; 
  INTITULE: any= []; 
  DESCRIPTION: any= []; 
  REPONSE: any= []; 
  STATUT: any= [];     
  helpdeskList:any = [];
  VoletList:any = [];
  MenuList:any = [];
  VOLET:any = [];
  MENU:any = [];
  modopen=false;  
  TEST_HIDE:string="OUI"
  modalTitle:any;
  helpdesk:any;  
  MSG:string="";

  constructor(private activatedRoute: ActivatedRoute, private service: SharedService, private sharedService: SharedService) { }
 
  ngOnInit() {
    
    var val = {
      VOLET:"",
    };
      this.sharedService.getVoletList(val).subscribe(data =>{
        this.VoletList = data;
        this.p=1;
      });
  }

  refresmenuList() {
    this.refreshhelpdeskList()
     var val = {
       VOLET:this.VOLET
     };
     this.sharedService.getMenuList(val).subscribe(data =>{
       this.MenuList = data;
     });  
     this.p=1;
   }

  refreshhelpdeskList() {
    this.TEST_HIDE="NON"
    var val = {
      DDP:this.sharedService.userInfo.DDP,
      VOLET:this.VOLET,
      MENU:this.MENU
    };
    this.sharedService.gethelpdeskList(val).subscribe(data =>{
      this.helpdeskList = data;
    });  
    this.p=1;
  }

  refreshhelpdeskList2() {
    var val = {
      DDP:this.sharedService.userInfo.DDP,
      VOLET:this.VOLET,
      MENU:this.MENU
    };
    this.sharedService.gethelpdeskList(val).subscribe(data =>{
      this.helpdeskList = data;
    });  
    this.p=1;
  }

  Addhelpdesk(){
    this.helpdesk={
      DATE:"",
      VOLET:this.VOLET,
      MENU:this.MENU,
      DDP:"",
      NOM_PRENOM:"",
      INTITULE:"",
      DESCRIPTION:"",
      REPONSE:"",
      STATUT:"EN COURS"
    }
    this.modopen=false;
    this.modopen=true;
  }

  Edithelpdesk(item: any){
    this.helpdesk = item;
    this.modopen=false;
    this.modopen=true;
  }

  infoshelpdesk(item: any){
    this.helpdesk = item;
    this.modopen=false;
    this.modopen=true;
  }

  deletehelpdesk(item: any){
    this.helpdesk = item;
    if(confirm('Etes vous sûr de vouloir supprimer cet incident ?')){
      var val = {
        VOLET:this.VOLET,
        MENU:this.MENU,
        DDP:this.helpdesk.DDP,
        DATE:this.helpdesk.DATE,
        INTITULE:this.helpdesk.INTITULE,
      };
        this.sharedService.deletehelpdesk(val).subscribe(data =>{
        alert(data.toString());
        this.refreshhelpdeskList2();
      })
    }
  }
 
  downloadfile(i: any) {
    let nom_pdf : any = i.DDP + "_" + i.DATE.substring(0,10) + "_" + i.VOLET + "_" + i.MENU + " (" + i.INTITULE + ")" + ".jpg";
    this.sharedService.getdownloadhelpdeskfile(i.NOM_FILE).subscribe(data =>{
      const blob='data:application/jpg;base64,'+data;
      this.MSG=data.toString();
      if (this.MSG=="Capture d'incident non éxistante !")
      {
         alert(data.toString());
      }
      else
      {
       FileSaver.saveAs(blob,nom_pdf);
      }
    })
  }
  
  closeClick(){
    this.modopen=false;
    this.refreshhelpdeskList2();
  }
}
