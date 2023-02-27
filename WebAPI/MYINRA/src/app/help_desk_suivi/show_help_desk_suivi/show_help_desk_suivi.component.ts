import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { ActivatedRoute} from '@angular/router';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_help_desk_suivi',
  templateUrl: './show_help_desk_suivi.component.html',
  styleUrls: ['./show_help_desk_suivi.component.css'] 
})
export class Show_help_desk_suiviComponent implements OnInit {
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
      STATUT:"TOUS",
    };
      this.sharedService.gethelpdeskList(val).subscribe(data =>{
        this.helpdeskList = data;
        this.p=1;
      });
  }

  refreshhelpdeskList() {
    this.TEST_HIDE="NON"
    var val = {
       STATUT:this.STATUT,
    };
    this.sharedService.gethelpdesksuiviList(val).subscribe(data =>{
      this.helpdeskList = data;
    });  
    this.p=1;
  }

  refreshhelpdeskList2() {
    var val = {
      STATUT:this.STATUT,
    };
    this.sharedService.gethelpdesksuiviList(val).subscribe(data =>{
      this.helpdeskList = data;
    });  
    this.p=1;
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

  downloadfile(i: any) {
    let nom_pdf : any = i.DDP + "_" + i.DATE.substring(0,10) + "_" + i.VOLET + "_" + i.MENU + " (" + i.INTITULE + ")" + ".jpg";
    this.sharedService.getdownloadhelpdesksuivifile(i.NOM_FILE).subscribe(data =>{
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
