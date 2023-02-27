import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { Router } from '@angular/router';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-show_etat_signature',
  templateUrl: './show_etat_signature.component.html',
  styleUrls: ['./show_etat_signature.component.css'] 
})
export class Show_etat_signatureComponent implements OnInit {
  p: number = 1;
  signatureList:any = [];
  modopen=false;
  modalTitle:any;
  MSG:string="";
  //activateAddEditStuCom:boolean = false;
  signature:any;
  PAIE_SIGNATURE_ACTIF : any= [];
  AgentList:any = [];

  constructor(private router: Router, private service: SharedService, private sharedService: SharedService) {  }
  ngOnInit() {
    this.droits_paramétrage();
    this.refreshsignatureList();
  }

  droits_paramétrage() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getParamétrage(val).subscribe(data =>{
      this.AgentList = data;
      this.PAIE_SIGNATURE_ACTIF=this.AgentList[2]["ACTIF"];
      if (this.PAIE_SIGNATURE_ACTIF=="NON")
      {
        this.router.navigate([''])
      }
    });
  }

  refreshsignatureList() {
    var val = {
    };
    this.sharedService.getsignatureList(val).subscribe(data =>{
      this.signatureList = data;
    });  
  }

  refreshsignatureList2() {
    var val = {
    };
    this.sharedService.getsignatureList(val).subscribe(data =>{
      this.signatureList = data;
    });  
  }

  Addsignature(){
    this.signature={
      ORDRE:"",
      DDP:"",
      NOM_PRENOM:"",
      ABSENCE_DU:"",
      ABSENCE_AU:"",
      MOTIF:""
    }
    this.modopen=false;
    this.modopen=true;
  }

  UpdateEntetePied(){
    this.modopen=false;
    this.modopen=true;
  }

  Editsignature(item: any){
    this.signature = item;
    this.modopen=false; 
    this.modopen=true;
  }

  deletesignature(item: any){
    this.signature = item;
    if(confirm('Etes vous sûr de vouloir supprimer ce signataire ?')){
      var val = {
        DDP:this.signature.DDP,
          };
        this.sharedService.deletesignature(val).subscribe(data =>{
        alert(data.toString());
        this.refreshsignatureList2();
      })
    }
  } 

  closeClick(){
    this.modopen=false;
    this.refreshsignatureList2();
  }

  downloadfile(i: any) {
     //let nom_pdf : any = "\Etats\\SIGNATURE\\FR\\POUR_DIRECTEUR\\" + i.DDP + ".jpg";
     let nom_pdf : any =i.DDP;
     this.sharedService.getdownloadsignaturefile(nom_pdf).subscribe(data =>{
       const blob='data:application/pdf;base64,'+data;
       this.MSG=data.toString();
       if (this.MSG=="Aucune signature n'est enregistrée pour cet agent !")
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
