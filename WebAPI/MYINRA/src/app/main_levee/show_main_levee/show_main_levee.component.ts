import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import * as rap from 'ngx-select-dropdown';
//import { AppComponent } from './app.com';  
import { Router } from '@angular/router';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_main_levee',
  templateUrl: './show_main_levee.component.html',
  styleUrls: ['./show_main_levee.component.css'] 
})
export class Show_main_leveeComponent implements OnInit {
  p: number = 1;
  CATEGORIE_AGENT: any= []
  MainLeveeList:any = [];
  AgentList:any = [];
  modopen=false;
  agent:any;
  modalTitle:any;
  activateAddEditStuCom:boolean = false;
  MainLevee:any;
  DDP_SELECTIONNE: string="";
  CATEGORIE_AGENT_MAIN_LEVEE_ACTIF : any= [];

  constructor(private router: Router, private service: SharedService, private sharedService: SharedService) {  }
  ngOnInit() {
    this.droits_paie();
    this.refreshMainLeveeList();
    this.refreshAgentList();    
    this.DDP_SELECTIONNE="NON";  
  }

  droits_paie() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getCatégorieAgentPaie(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT_MAIN_LEVEE_ACTIF=this.AgentList[5]["ACTIF"];
      if (this.CATEGORIE_AGENT_MAIN_LEVEE_ACTIF=="NON")
      {
        this.router.navigate([''])
      }
    });
  }

  vider() {    
    this.DDP_SELECTIONNE="NON";
    var val = {
      // DDP:this.agent.DDP,
      // DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getMainLeveeList(val).subscribe(data =>{
      this.MainLeveeList = data;
    }); 
    this.p=1;
  }

  choixagent(agent:any) { 
    this.DDP_SELECTIONNE="OUI";   
    var val = {
      DDP:this.agent.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getMainLeveeList(val).subscribe(data =>{
      this.MainLeveeList = data;
    }); 
    this.p=1;
  }

  refreshMainLeveeList() {
    var val = {
      DDP:this.service.userInfo.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getMainLeveeList(val).subscribe(data =>{
      this.MainLeveeList = data;
    });  
  }

  refreshMainLeveeList2() {
    var val = {
      DDP:this.agent.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getMainLeveeList(val).subscribe(data =>{
      this.MainLeveeList = data;
    });  
  }

  refreshAgentList() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getAgentMainLeveeList(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT=this.AgentList[0]["CATEGORIE"];
    });
  }

  AddMainLevee(){
    this.MainLevee={
      DATE:"",
      DDP:this.agent.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP,
      NOM_PRENOM:this.agent.NOM_PRENOM,
    }
    this.modopen=false;
    this.modopen=true;
  }

  deleteMainLevee(item: any){
    this.MainLevee = item;
    if(confirm('Etes vous sûr de vouloir supprimer cette main levée ?')){
      var val = {
        STATUT:this.MainLevee.ETAT,
        DDP:this.MainLevee.DDP,
        DDP_DEMANDEUR:this.sharedService.userInfo.DDP,
        DATE:this.MainLevee.DATE,
        BANQUE:this.MainLevee.BANQUE,
        RIB:this.MainLevee.RIB,
      };
      
        this.sharedService.deleteMainLevee(val).subscribe(data =>{
        alert(data.toString());
        this.refreshMainLeveeList2();
      })
    }
  }

  // EditMainLevee(item: any){
  //   this.MainLevee = item;
  //   this.modopen=false;
  //   this.modopen=true;
  // }

  closeClick(){
    this.modopen=false;
    this.refreshMainLeveeList2();
  }
  
  downloadfile(i: any) {
    let nom_pdf : any = i.DDP + "_" + i.DATE_PDF + " (" + i.DDP_DEMANDEUR + ")";
    this.sharedService.getdownloadMainLeveefile(i.NOM_FILE).subscribe(data =>{
      const blob='data:application/pdf;base64,'+data;
      FileSaver.saveAs(blob,nom_pdf);
    })
  }
}
