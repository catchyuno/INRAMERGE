import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-add_edit_entete_pied',
  templateUrl: './add_edit_entete_pied.component.html',
  styleUrls: ['./add_edit_entete_pied.component.css']
})
export class Add_edit_entete_piedComponent implements OnInit {
  // @Input() signature:any;
  // ORDRE: string ="";
  // DDP: string ="";
  // NOM_PRENOM: string ="";
  // ABSENCE_DU: any ="";
  // ABSENCE_AU: any ="";
  // MOTIF: string ="";
  // AgentList:any = [];
  // CATEGORIE_AGENT: any= [];
  // agent:any;
  // DDP_TEST:string="NON";
  MSG:string="";

  constructor(private service: SharedService, private sharedService: SharedService) {}
  
  ngOnInit(): void {
  }

uploadEntete(val: any){
  let reader=new FileReader();
  reader.onload=()=>{
    let img;
    img=reader.result!.toString().replace('data:image/jpeg;base64,','');
  this.service.uploadEntete({nom_file:img}).subscribe(data=>{
      this.ngOnInit();
  });
  }
  reader.readAsDataURL(val.target.files[0]);
  alert("Upload effectué !");
}
  
uploadPied(val: any){
   let reader=new FileReader();
   reader.onload=()=>{
     let img;
     img=reader.result!.toString().replace('data:image/jpeg;base64,','');
   this.service.uploadPied({nom_file:img}).subscribe(data=>{
       this.ngOnInit();
   });
   }
   reader.readAsDataURL(val.target.files[0]);
   alert("Upload effectué !");
 }

 downloadEntete() {
  let nom_pdf : any = "\Etats\\PICS\\EN_TETE.jpg";
  this.sharedService.getdownloadEntetefile(nom_pdf).subscribe(data =>{
    const blob='data:application/jpg;base64,'+data;
    this.MSG=data.toString();
    if (this.MSG=="Aucun entete de page n'est enregistré !")
    {
       alert(data.toString());
    }
    else
    {
     FileSaver.saveAs(blob,nom_pdf);
    }
 })
}

downloadPied() {
  let nom_pdf : any = "\Etats\\PICS\\PIED.jpg";
  this.sharedService.getdownloadPiedfile(nom_pdf).subscribe(data =>{
    const blob='data:application/jpg;base64,'+data;
    this.MSG=data.toString();
    if (this.MSG=="Aucun pied de page n'est enregistré !")
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
