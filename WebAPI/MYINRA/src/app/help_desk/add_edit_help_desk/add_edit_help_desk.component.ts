import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-add_edit_help_desk',
  templateUrl: './add_edit_help_desk.component.html',
  styleUrls: ['./add_edit_help_desk.component.css']
})
export class Add_edit_help_deskComponent implements OnInit {
  @Input() helpdesk:any;
  today: Date;
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  DATE:any ;
  CATEGORIE: string ="";
  INTITULE: string ="";
  INTITULE_ANCIEN: string ="";
  DESCRIPTION: any= []; 
  DESCRIPTION_ANCIEN: any= [];  
  REPONSE: any= []; 
  STATUT: any= [];   
  helpdeskList:any = [];
  VoletList:any = [];
  MenuList:any = [];
  VOLET:any = [];
  MENU:any = [];
  DDP: string ="";
  NOM_PRENOM: string ="";
  MSG:string="";

  constructor(private service: SharedService, private sharedService: SharedService, private datePipe: DatePipe) { 
    this.today =new Date();
  }

  ngOnInit(): void {
    if (this.helpdesk.DATE!="")
   {
   this.DATE=this.helpdesk.DATE;
   }
   else
   {
    this.DATE=new Date();
   }
    this.INTITULE = this.helpdesk.INTITULE;
    this.INTITULE_ANCIEN= this.helpdesk.INTITULE;
    this.DESCRIPTION= this.helpdesk.DESCRIPTION.replaceAll("_","'");
    this.DESCRIPTION_ANCIEN= this.helpdesk.DESCRIPTION.replaceAll("_","'");
    this.REPONSE= this.helpdesk.REPONSE.replaceAll("_","'");
    this.DDP=this.sharedService.userInfo.DDP,
    this.NOM_PRENOM=this.sharedService.userInfo.NOM_PRENOM,
    this.STATUT = this.helpdesk.STATUT;
    this.MENU = this.helpdesk.MENU;
    this.VOLET = this.helpdesk.VOLET;
  }

  Addhelpdesk(){
    var val = {
      DATE:this.DATE,
      DDP:this.DDP,
      NOM_PRENOM:this.NOM_PRENOM,
      STATUT:this.STATUT,
      INTITULE:this.INTITULE,
      DESCRIPTION:this.DESCRIPTION,
      REPONSE:this.REPONSE,
      VOLET:this.VOLET,
      MENU:this.MENU,
    }
     this.service.addhelpdesk(val).subscribe(res =>{
       if (res!="")
     {
         alert(res.toString())
       }
     })
  }

  upload(val: any){
       let reader=new FileReader();
       reader.onload=()=>{
       let img;
       img=reader.result!.toString().replace('data:image/jpeg;base64,','');
       this.service.uploadhelpdesk({nom_file:img, DDP : this.DDP, DATE:this.DATE, STATUT:this.STATUT, INTITULE:this.INTITULE, VOLET:this.VOLET, MENU:this.MENU}).subscribe(data=>{
       this.ngOnInit();
     });
     }
     reader.readAsDataURL(val.target.files[0]);
     alert("Upload effectué !");
   }

  updatehelpdesk(){
    var val = {
      DATE:this.DATE,
      DDP:this.sharedService.userInfo.DDP,
      NOM_PRENOM:this.sharedService.userInfo.NOM_PRENOM,
      STATUT:this.STATUT,
      INTITULE:this.INTITULE,
      INTITULE_ANCIEN:this.INTITULE_ANCIEN,
      DESCRIPTION:this.DESCRIPTION,
      DESCRIPTION_ANCIEN:this.DESCRIPTION_ANCIEN,
      REPONSE:this.REPONSE,
      VOLET:this.VOLET,
      MENU:this.MENU,
    };
      this.service.updatehelpdesk(val).subscribe(res =>{
        alert(res.toString());
      })
  }  
}
