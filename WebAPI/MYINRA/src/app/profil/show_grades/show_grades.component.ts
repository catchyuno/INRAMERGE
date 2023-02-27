import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

@Component({
  selector: 'app-show_grades',
  templateUrl: './show_grades.component.html',
  styleUrls: ['./show_grades.component.css']
})
export class show_gradesComponent implements OnInit {
  pg: number = 1;
  @Input() UserInf={NOM_PRENOM:'',DDP:''};
  @Input() grade:any;
  DDP: string =""; 
  gradeList:any = [];
  modopen=false;
  modalTitle:any;
  
  constructor(private service: SharedService, private sharedService: SharedService) { }
 
  ngOnInit() {
    this.refresgradeList();
  }

  refresgradeList() {
    var val = {
      DDP:this.grade.DDP
    };
    this.modopen=false;
    this.modopen=true;
    this.sharedService.getMonProfilGradeList(val).subscribe(data =>{
     this.gradeList = data;
    });  

  }

  closeClick(){
    this.modopen=false;
  }
}
