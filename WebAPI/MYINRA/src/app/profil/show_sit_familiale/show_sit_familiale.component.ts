import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

@Component({
  selector: 'app-show_sit_familiale',
  templateUrl: './show_sit_familiale.component.html',
  styleUrls: ['./show_sit_familiale.component.css']
})
export class show_sit_familialeComponent implements OnInit {
  ps: number = 1;
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  @Input() conjoint:any;
  DDP: string =""; 
  SOURCE: string ="";  
  conjointList:any = [];
  enfantList:any = [];  
  situationList:any = [];  
  enfant:any;
  modopen=false;
  modalTitle:any;
  i:number=0;
  
  constructor(private service: SharedService, private sharedService: SharedService) { }
 
  ngOnInit() {
    this.DDP=this.conjoint.DDP;
    this.refreshconjointList();
  }

  refreshconjointList() {
    var val = {
      DDP:this.conjoint.DDP
    };
    this.modopen=false;
    this.modopen=true;
    this.sharedService.getMonProfilConjointList(val).subscribe(data =>{
     this.conjointList = data;
     });  
  }

  refreshenfantList() {
    this.enfant="";
    var val;
     val = {
      DDP:"1",
      SOURCE:"1",
      ENFANT:"1"
    };
    this.sharedService.getMonProfilSituationList(val).subscribe(data =>{
      this.situationList = data;
  });  
     val = {
      DDP:this.DDP,
      SOURCE:this.conjoint.nom_prenom_conj_enf
    };
    this.modopen=false;
    this.modopen=true;
    this.sharedService.getMonProfilEnfantList(val).subscribe(data =>{
     this.enfantList = data;
    });  
  }

  choixsituationvide(){
    var val = {
      DDP:"1",
      SOURCE:"1",
      ENFANT:"1"
    };
    this.sharedService.getMonProfilSituationList(val).subscribe(data =>{
      this.situationList = data;
  });  
}

choixenfantvide(){
  var val = {
    DDP:"1",
    SOURCE:"1",
    ENFANT:"1"
  };
  this.sharedService.getMonProfilEnfantList(val).subscribe(data =>{
    this.enfantList = data;
   });
   this.enfant="";
  this.sharedService.getMonProfilSituationList(val).subscribe(data =>{
    this.situationList = data;
});  
}

  refreshsituationList() {
    this.ps=1;
    var val;
    if (this.enfant.nom_prenom_conj_enf === 'undefined')
    {
       val = {
        DDP:this.DDP,
        SOURCE:this.conjoint.nom_prenom_conj_enf.replaceAll("'", "_"),
        ENFANT:""
      };
    }
    else
    {
     val = {
      DDP:this.DDP,
      SOURCE:this.conjoint.nom_prenom_conj_enf.replaceAll("'", "_"),
      ENFANT:this.enfant.nom_prenom_conj_enf.replaceAll("'", "_")
    };
  }
    this.modopen=false;
    this.modopen=true;
    this.sharedService.getMonProfilSituationList(val).subscribe(data =>{
     this.situationList = data;
    });  
  }

  closeClick(){
    this.modopen=false;
  }
}
