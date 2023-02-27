import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

@Component({
  selector: 'app-add_edit_etat_signature',
  templateUrl: './add_edit_etat_signature.component.html',
  styleUrls: ['./add_edit_etat_signature.component.css']
})
export class Add_edit_etat_signatureComponent implements OnInit {
  @Input() signature:any;
  ORDRE: string ="";
  DDP: string ="";
  NOM_PRENOM: string ="";
  TYPE: string ="";
  DDPSIG: string ="";
  NOM_PRENOMSIG: string ="";
  ABSENCE_DU: any ="";
  ABSENCE_AU: any ="";
  MOTIF: string ="";
  AgentList:any = [];
  AgentListSIG:any = [];
  CATEGORIE_AGENT: any= [];
  agent:any;
  SIGNATAIRE_OBLIGATOIRE:any;
  DDP_TEST:string="NON";
  DDPSIG_TEST:string="NON";

  constructor(private service: SharedService, private sharedService: SharedService) {}
  
  ngOnInit(): void {
    this.refreshAgentList();  
    this.refreshAgentListSIG();  
    this.ORDRE = this.signature.ORDRE;
    this.DDP = this.signature.DDP;
    this.NOM_PRENOM = this.signature.NOM_PRENOM;
    if(this.ABSENCE_DU!="" || this.ABSENCE_DU!=null) {
      this.ABSENCE_DU = this.signature.ABSENCE_DU.substring(8, 10) + "/" + this.signature.ABSENCE_DU.substring(5, 7) + "/" + this.signature.ABSENCE_DU.substring(0, 4);
    }
    if(this.ABSENCE_AU!="" || this.ABSENCE_AU!=null) {
      this.ABSENCE_AU = this.signature.ABSENCE_AU.substring(8, 10) + "/" + this.signature.ABSENCE_AU.substring(5, 7) + "/" + this.signature.ABSENCE_AU.substring(0, 4);
    }
    if(this.ABSENCE_DU==="//") {
      this.ABSENCE_DU = "";
    }
    if(this.ABSENCE_AU==="//") {
      this.ABSENCE_AU = "";
    }
    this.MOTIF = this.signature.MOTIF;
    this.DDPSIG = this.signature.DDP_SIGNATAIRE_OBLIGATOIRE;
    this.NOM_PRENOMSIG = this.signature.NOM_PRENOM_SIGNATAIRE_OBLIGATOIRE;
  }

  addsignature(){
    var val = {
      ORDRE:this.ORDRE,
      DDP:this.DDP,
      NOM_PRENOM:this.NOM_PRENOM,
      ABSENCE_DU:this.ABSENCE_DU,
      ABSENCE_AU:this.ABSENCE_AU,
      MOTIF:this.MOTIF,
      DDPSIG:this.DDPSIG,
      NOM_PRENOMSIG:this.NOM_PRENOMSIG,
    };
     this.service.addsignature(val).subscribe(res =>{
       if (res!="")
     {
         alert(res.toString())
       }
     })
  }

  updatesignature(){
    var val = {
      ORDRE:this.ORDRE,
      DDP:this.DDP,
      NOM_PRENOM:this.NOM_PRENOM,
      ABSENCE_DU:this.ABSENCE_DU,
      ABSENCE_AU:this.ABSENCE_AU,
      MOTIF:this.MOTIF,
      DDPSIG:this.DDPSIG,
    };
      this.service.updatesignature(val).subscribe(res =>{
        alert(res.toString());
      })
  }  

uploadPourFR(val: any){
  let reader=new FileReader();
  reader.onload=()=>{
    let img;
    img=reader.result!.toString().replace('data:image/jpeg;base64,','');
    this.service.uploadSignature({nom_file:img, DDP : this.DDP, TYPE:'POURFR'}).subscribe(data=>{
    this.ngOnInit();
  });
  }
  reader.readAsDataURL(val.target.files[0]);
  alert("Upload effectué !");
}
  
uploadSansFR(val: any){
  let reader=new FileReader();
  reader.onload=()=>{
    let img;
    img=reader.result!.toString().replace('data:image/jpeg;base64,','');
    this.service.uploadSignature({nom_file:img, DDP : this.DDP, TYPE:'SANSFR'}).subscribe(data=>{
    this.ngOnInit();
  });
  }
  reader.readAsDataURL(val.target.files[0]);
  alert("Upload effectué !");
}

uploadPourAR(val: any){
  let reader=new FileReader();
  reader.onload=()=>{
    let img;
    img=reader.result!.toString().replace('data:image/jpeg;base64,','');
    this.service.uploadSignature({nom_file:img, DDP : this.DDP, TYPE:'POURAR'}).subscribe(data=>{
    this.ngOnInit();
  });
  }
  reader.readAsDataURL(val.target.files[0]);
  alert("Upload effectué !");
}

uploadSansAR(val: any){
  let reader=new FileReader();
  reader.onload=()=>{
    let img;
    img=reader.result!.toString().replace('data:image/jpeg;base64,','');
    this.service.uploadSignature({nom_file:img, DDP : this.DDP, TYPE:'SANSAR'}).subscribe(data=>{
    this.ngOnInit();
  });
  }
  reader.readAsDataURL(val.target.files[0]);
  alert("Upload effectué !");
}

vider()
{
  this.DDP_TEST="NON";
}

viderSIG()
{
  this.DDPSIG_TEST="NON";
}

 choixagent(agent:any) { 
    this.DDP_TEST="OUI"; 
    var val = {
      DDP_V:this.agent.DDP,
      NOM_PRENOM_V:this.agent.NOM_PRENOM
    };
    this.DDP=val["DDP_V"];
    this.NOM_PRENOM=val["NOM_PRENOM_V"];
  }

  choixagentSig(agent:any) { 
    this.DDPSIG_TEST="OUI"; 
    var val = {
      DDPSIG_V:this.SIGNATAIRE_OBLIGATOIRE.DDP,
      NOM_PRENOMSIG_V:this.SIGNATAIRE_OBLIGATOIRE.NOM_PRENOM
    };
    this.DDPSIG=val["DDPSIG_V"];
    this.NOM_PRENOMSIG=val["NOM_PRENOMSIG_V"];
  }

  refreshAgentList() {
    var val = {
      DDP:this.service.userInfo.DDP
    };
    this.sharedService.getsignatureAgentList(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT=this.AgentList[0]["CATEGORIE"];
    });
  }

  refreshAgentListSIG() {
    var val = {
      DDP:this.service.userInfo.DDP
    };
    this.sharedService.getsignatureAgentListSIG(val).subscribe(data =>{
      this.AgentListSIG = data;
      // this.CATEGORIE_AGENT=this.AgentList[0]["CATEGORIE"];
    });
  }
}
