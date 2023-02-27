import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

@Component({
  selector: 'app-add_edit_cin',  
  templateUrl: './add_edit_cin.component.html',
  styleUrls: ['./add_edit_cin.component.css']
})
export class Add_edit_cinComponent implements OnInit {
  //@Input() UserInf={NOM_PRENOM:'',DDP:''};
  @Input() suivicin:any;
  CIN:any;
  DDP: string ="";
  NOM_PRENOM: string ="";  
  NOMAR: string ="";  
  VALIDE: string ="";  
  ADRESSE: string ="";
  ProfilList:any = [];

  constructor(private service: SharedService, private sharedService: SharedService) { }

  ngOnInit(): void {
    // this.DATE=new Date();
    this.NOM_PRENOM=this.suivicin.NOM_PRENOM;
    this.DDP=this.suivicin.DDP;
    this.NOMAR=this.suivicin.NOMAR;
    this.ADRESSE=this.suivicin.ADRESSE;
    this.VALIDE=this.suivicin.VALIDE;
    this.CIN=this.suivicin.CIN;
   }

  updateCIN(){
  var val = {
    DDP:this.DDP,
    CIN:this.CIN,
    NOMAR:this.NOMAR,
    NOM_PRENOM:this.NOM_PRENOM,
    ADRESSE:this.ADRESSE,
  }
   this.service.updateCIN(val).subscribe(res =>{
     if (res!="")
   {
       alert(res.toString())
     }
   })
}

}
