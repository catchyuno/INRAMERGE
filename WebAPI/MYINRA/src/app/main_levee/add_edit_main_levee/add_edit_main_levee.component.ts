import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { DatePipe } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';

declare var require: any
const FileSaver = require('file-saver');

@Component({
  selector: 'app-add_edit_main_levee',
  templateUrl: './add_edit_main_levee.component.html',
  styleUrls: ['./add_edit_main_levee.component.css']
})
export class Add_edit_main_leveeComponent implements OnInit {
  @Input() MainLevee:any;
  today: Date;
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  DATE:any ;
  DDP: string ="";
  NOM_PRENOM: string ="";
  BANQUE: string ="";
  DDP_DEMANDEUR: string ="";
  NOM_PRENOM_DEMANDEUR: string ="";
  RIB: string ="";
  DATE_PDF:any = [];
  RIB_BANQUE_LISTE:any = [];
  
  constructor(private spinner: NgxSpinnerService, private service: SharedService, private sharedService: SharedService, private datePipe: DatePipe) { 
    this.today =new Date();
  }

  ngOnInit(): void {
    this.DATE=new Date();
    this.DDP=this.MainLevee.DDP,
    this.NOM_PRENOM=this.MainLevee.NOM_PRENOM,
    this.RIB_BANQUE();

    
  }

  RIB_BANQUE() {
    var val = {
      DDP:this.MainLevee.DDP,
    };
    this.DDP=this.MainLevee.DDP,
    this.NOM_PRENOM=this.MainLevee.NOM_PRENOM,
    this.sharedService.getRIB_BANQUE(val).subscribe(data =>{
     this.RIB_BANQUE_LISTE = data;
      this.BANQUE=this.RIB_BANQUE_LISTE[0]["BANQUE"];
      this.RIB=this.RIB_BANQUE_LISTE[0]["RIB"];
    });  
  }

  upload(val: any){
    let reader=new FileReader();
    reader.onload=()=>{
    let img;
    img=reader.result!.toString().replace('data:application/pdf;base64,','');
    this.service.uploadMainLevee({nom_file:img, NOM_PRENOM_DEMANDEUR:this.sharedService.userInfo.NOM_PRENOM, DDP : this.DDP, NOM_PRENOM:this.MainLevee.NOM_PRENOM, BANQUE:this.BANQUE, RIB:this.RIB, DATE:this.DATE, DDP_DEMANDEUR:this.sharedService.userInfo.DDP}).subscribe(data=>{
    this.ngOnInit();
  });
  }
  reader.readAsDataURL(val.target.files[0]);
  alert("Upload effectué !");
}

  downloadfile() {
    this.DATE_PDF  = this.datePipe.transform(this.DATE, 'dd-MM-yyyy');
    let nom_pdf : any = this.DDP + "_" + this.DATE_PDF + " (" + this.sharedService.userInfo.DDP + ")";
    let NOM_FILE: any=".\\Etats\\SORTIE\\MAINLEVEE\\" + nom_pdf + ".pdf";
    this.sharedService.getdownloadMainLeveefile(NOM_FILE).subscribe(data =>{
       const blob='data:application/pdf;base64,'+data;
       FileSaver.saveAs(blob,nom_pdf);
     })
   }
  
}
