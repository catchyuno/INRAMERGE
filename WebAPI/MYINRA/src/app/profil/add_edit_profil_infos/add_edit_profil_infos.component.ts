import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

@Component({
  selector: 'app-add_edit_profil_infos',  
  templateUrl: './add_edit_profil_infos.component.html',
  styleUrls: ['./add_edit_profil_infos.component.css']
})
export class Add_edit_profil_infosComponent implements OnInit {
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  @Input() profil:any;
  //@Input() fromprofil:any;
  CIN:any;
  DDP: string ="";
  NOM_PRENOM: string ="";  
  NOMAR: string ="";  
  VALIDE: string ="";  
  ADRESSE: string ="";
  ADRESSE_ANCIEN: string ="";
  ProfilList:any = [];
  ACTIVATE: string ="";
  constructor(private service: SharedService, private sharedService: SharedService) { }

  ngOnInit(): void {
    this.refreshmonprofil();
    //console.log(this.fromprofil)
    this.ACTIVATE="NON";
    this.activer()
    //console.log(this.ACTIVATE)
  }

  activer()
  {
    if ((this.ADRESSE=="") || (this.NOMAR==""))
    {
      this.ACTIVATE="NON"
    }
    else
    {
      this.ACTIVATE="OUI"
    }
  }


  
  upload(val: any){
  let reader=new FileReader();
  reader.onload=()=>{
    let pdf;
    pdf=reader.result!.toString().replace('data:application/pdf;base64,','');
  this.service.uploadCIN({nom_file:pdf,DDP : this.DDP}).subscribe(data=>{
      this.ngOnInit();
  });
  }
  reader.readAsDataURL(val.target.files[0]);
  // if (this.ADRESSE!=this.ADRESSE_ANCIEN)
  // {
  //   this.addCIN()
  // }
  // else
  // {
    alert("Ajout effectué !");
  // }
}

addCIN(){
  var val = {
    DDP:this.DDP,
    CIN:this.CIN,
    NOMAR:this.NOMAR,
    NOM_PRENOM:this.NOM_PRENOM,
    ADRESSE:this.ADRESSE,
    ADRESSE_ANCIEN:this.ADRESSE_ANCIEN,
    // VALIDE:this.VALIDE
  }
   this.service.addCIN(val).subscribe(res =>{
     if (res!="")
   {
       alert(res.toString())
     }
   })
}

  refreshmonprofil() {
    this.CIN="";
    this.ADRESSE="";
    this.NOMAR="";
    this.DDP=this.profil.DDP;
    this.NOM_PRENOM=this.profil.NOM_PRENOM;
    var val = {
      DDP:this.profil.DDP
    };
    this.sharedService.getMonProfilCIN(val).subscribe(data =>{
      this.ProfilList = data;
      this.DDP=this.DDP;
      this.CIN=this.ProfilList[0]["CIN"];
      this.ADRESSE=this.ProfilList[0]["ADRESSE"];
      this.NOMAR=this.ProfilList[0]["NOMAR"];
      this.VALIDE=this.ProfilList[0]["VALIDE"];
      //console.log(this.ProfilList)
     if ((this.ADRESSE=="") || (this.NOMAR==""))
    {
      this.ACTIVATE="NON"
    }
    else
    {
      this.ACTIVATE="OUI"
    }
    });  
  }

  updateprofil(){
    var val = {
      DDP:this.DDP,
      CIN:this.CIN,
      ADRESSE:this.ADRESSE,
      NOMAR:this.NOMAR,
    };
      this.service.updateprofil(val).subscribe(res =>{
        alert(res.toString());
      })
  }  
}
