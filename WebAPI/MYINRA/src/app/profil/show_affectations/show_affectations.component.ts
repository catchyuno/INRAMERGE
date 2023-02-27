import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

@Component({
  selector: 'app-show_affectations',
  templateUrl: './show_affectations.component.html',
  styleUrls: ['./show_affectations.component.css']
})
export class show_affectationsComponent implements OnInit {
  p: number = 1;
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  @Input() aff:any;
  DDP: string =""; 
  affList:any = [];
  modopen=false;
  modalTitle:any;

  constructor(private service: SharedService, private sharedService: SharedService) { }
 
  ngOnInit() {
    this.refresaffList();
  }

  refresaffList() {
    var val = {
      DDP:this.aff.DDP
    };
    this.modopen=false;
    this.modopen=true;
    this.sharedService.getMonProfilaffList(val).subscribe(data =>{
     this.affList = data;
    });  
  }

  closeClick(){
    this.modopen=false;
  }
}
