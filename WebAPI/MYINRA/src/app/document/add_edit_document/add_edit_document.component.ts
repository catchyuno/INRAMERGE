import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

@Component({
  selector: 'app-add_edit_document',
  templateUrl: './add_edit_document.component.html',
  styleUrls: ['./add_edit_document.component.css']
})
export class Add_edit_documentComponent implements OnInit {
  @Input() document:any;
  CATEGORIE: string ="";
  INTITULE: string ="";
  ACTIVATE: string =""; 
  INTITULE_ANCIEN: string ="";
  categorieList:any = []; 

  constructor(private service: SharedService, private sharedService: SharedService) { }

  ngOnInit(): void {
    this.CATEGORIE = this.document.CATEGORIE;
    this.INTITULE = this.document.INTITULE.replaceAll("_", "'");
    this.INTITULE_ANCIEN= this.document.INTITULE.replaceAll("_", "'");
    this.ACTIVATE="NON";
    this.activer()
  }

  adddocument(){
    var val = {
      CATEGORIE:this.CATEGORIE,
      INTITULE:this.INTITULE,
      INTITULE_ANCIEN:this.INTITULE_ANCIEN,
    }
     this.service.adddocument(val).subscribe(res =>{
       if (res!="")
     {
         alert(res.toString())
       }
     })
  }

  activer()
  {
    if (this.INTITULE!="")
    {
      this.ACTIVATE="OUI"
    }
    else
    {
      this.ACTIVATE="NON"
    }
  }

  upload(val: any){
  let reader=new FileReader();
  reader.onload=()=>{
    let pdf;
    pdf=reader.result!.toString().replace('data:application/pdf;base64,','');
    this.service.uploaddocument({nom_file:pdf,CATEGORIE : this.CATEGORIE,  INTITULE : this.INTITULE}).subscribe(data=>{
      this.ngOnInit();
  });
  }
  reader.readAsDataURL(val.target.files[0]);
  if (this.INTITULE!=this.INTITULE_ANCIEN)
  {
    this.adddocument()
  }
  else
  {
    alert("Ajout effectué !");
  }
}

  updatedocument(){
    var val = {
      CATEGORIE:this.CATEGORIE,
      INTITULE:this.INTITULE,
      INTITULE_ANCIEN:this.INTITULE_ANCIEN,
    };
      this.service.updatedocument(val).subscribe(res =>{
        alert(res.toString());
      })
  }  
}
