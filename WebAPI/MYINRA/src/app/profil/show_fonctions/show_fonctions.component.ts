import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

@Component({
  selector: 'app-show_fonctions',
  templateUrl: './show_fonctions.component.html',
  styleUrls: ['./show_fonctions.component.css']
})
export class show_fonctionsComponent implements OnInit {
  pf: number = 1;
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  @Input() fonct:any;
  DDP: string =""; 
  fonctionList:any = [];
  modopen=false;
  modalTitle:any;

  constructor(private service: SharedService, private sharedService: SharedService) { }
 
  ngOnInit() {
    this.pf=1;
    this.refreshfonctionList();
  }

  refreshfonctionList() {
    var val = {
      DDP:this.fonct.DDP
    };
    this.modopen=false;
    this.modopen=true;
    this.sharedService.getMonProfilfonctionList(val).subscribe(data =>{
     this.fonctionList = data;
    });  
  }

  closeClick(){
    this.modopen=false;
  }
}
