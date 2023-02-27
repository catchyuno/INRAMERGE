import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { Router } from '@angular/router';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_etat_ccp',
  templateUrl: './show_etat_ccp.component.html',
  styleUrls: ['./show_etat_ccp.component.css']
})
export class Show_etat_ccpComponent implements OnInit {
  today: Date;
  p: number = 1;
  STATUT: any= [];   
  CATEGORIE_AGENT: any= []
  EtatCCPList:any = [];
  AgentList:any = [];
  ACTIVER_DEMANDE: any= []
  MSG:string="";
  DATE:any ;
  DDP: string ="";
  NOM_PRENOM: string ="";
  DDP_DEMANDEUR: string ="";
  NOM_PRENOM_DEMANDEUR: string ="";
  //TYPE: string ="";
  agent:any;
  ccp:any;
  EtatCCP:any;
  CATEGORIE_AGENT_CCP_ACTIF : any= [];

  constructor(private router: Router, private service: SharedService, private sharedService: SharedService) { this.today =new Date(); }
  ngOnInit() {
    this.droits_paie();
    this.refreshEtatCCPList();
    this.refreshAgentList();  
    this.ACTIVER_DEMANDE="NON";  
  }

  droits_paie() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getCatégorieAgentPaie(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT_CCP_ACTIF=this.AgentList[7]["ACTIF"];
      if (this.CATEGORIE_AGENT_CCP_ACTIF=="NON")
      {
        this.router.navigate([''])
      }
    });
  }

  choixagent(agent:any) {    
    var val = {
      DDP:this.agent.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatCCPList(val).subscribe(data =>{
      this.EtatCCPList = data;
    }); 
    this.ACTIVER_DEMANDE="OUI";
    this.p=1;
  }

  vider() {    
    var val = {
      DDP:"",
      DDP_DEMANDEUR:""
    };
    this.sharedService.getEtatPrecompteList(val).subscribe(data =>{
      this.EtatCCPList = data;
    }); 
    this.ACTIVER_DEMANDE="NON";
    this.p=1;
  }
  
  refreshEtatCCPList() {
    var val = {
      DDP:this.service.userInfo.DDP,
      DDP_DEMANDEUR:this.sharedService.userInfo.DDP
    };
    this.sharedService.getEtatCCPList(val).subscribe(data =>{
      this.EtatCCPList = data;
    });  
  }

  refreshEtatCCPList2() {
    var val = {
      DDP:this.agent.DDP,
      DDP_DEMANDEUR:this.service.userInfo.DDP
    };
    this.sharedService.getEtatCCPList(val).subscribe(data =>{
      this.EtatCCPList = data;
    });  
  }

  refreshAgentList() {
    var val = {
      DDP:this.service.userInfo.DDP
    };
    this.sharedService.getCCPAgentList(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT=this.AgentList[0]["CATEGORIE"];
    });
  }

  deleteEtatCCP(item: any){
    this.ccp = item;
    if(confirm('Etes vous sûr de vouloir supprimer cet demande de CCP ?')){
      var val = {
        DDP:this.agent.DDP,
        DATE:this.ccp.DATE,
        DDP_DEMANDEUR:this.sharedService.userInfo.DDP,
      };
        this.sharedService.deleteCCP(val).subscribe(data =>{
        alert(data.toString());
        this.refreshEtatCCPList2();
      })
    }
  }

  AddEtatCCP(){
        var val = {
          DATE:this.today,
          DDP:this.agent.DDP,
          NOM_PRENOM:this.agent.NOM_PRENOM,
          DDP_DEMANDEUR:this.sharedService.userInfo.DDP,
          NOM_PRENOM_DEMANDEUR:this.sharedService.userInfo.NOM_PRENOM,
          STATUT:"EN COURS"
        };   
        this.service.addEtatCCP(val).subscribe(res =>{
          if (res!="")
          {
            alert(res.toString())
          }
          this.refreshEtatCCPList2();
        })
      }

  downloadfile(i: any) {
      let nom_pdf : any = i.DDP + "_" + i.DATE.substring(0,10)  + " (" + i.DDP_DEMANDEUR + ")";
      this.sharedService.getdownloadCCPfile(i.NOM_FILE).subscribe(data =>{
      const blob='data:application/pdf;base64,'+data;
      this.MSG=data.toString();
      if (this.MSG=="Etat CCP non encore généré !")
      {
         alert(data.toString());
      }
      else
      {
       FileSaver.saveAs(blob,nom_pdf);
      }
    })
  }
}
