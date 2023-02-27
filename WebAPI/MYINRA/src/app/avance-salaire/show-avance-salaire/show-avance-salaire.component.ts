import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

@Component({
  selector: 'app-show-avance-salaire',
  templateUrl: './show-avance-salaire.component.html',
  styleUrls: ['./show-avance-salaire.component.css']
})
export class ShowAvanceSalaireComponent implements OnInit {

  studentList:any = [];
  personnelList:any = [];
  modalTitle:any;
  activateAddEditStuCom:boolean = false;
  student:any;
  
  constructor(private sharedService: SharedService) { }

  ngOnInit(): void {
    this.refreshStudentList();
    this.refreshpersonnelList();
  }

  refreshpersonnelList() {
    this.sharedService.getpersonnelList().subscribe(data =>{
      this.personnelList = data;
    });  
  }

  refreshStudentList() {
    this.sharedService.getStudentList().subscribe(data =>{
      this.studentList = data;
    });  
  }

  AddStudent(){
    this.student={
      DATE_DEMANDE:"",
      DDP:"",
      NOM_PRENOM:"",
      AVANCE_ACCORDEE:"",
      MENS_AVANCE:"",
      DUREE_AVANCE:"",
      DU:"",
      AU:""
    }
    this.modalTitle = "Add Student";
    this.activateAddEditStuCom = true;
  }

  EditStudent(item: any){
    this.student = item;
    this.activateAddEditStuCom = true;
    this.modalTitle = "Update Student";
  }

  deleteClick(item: any){
    if(confirm('Are you sure??')){
      this.sharedService.deleteStudent(item.StudentId).subscribe(data =>{
        alert(data.toString());
        this.refreshStudentList();
      })
    }
  }

  closeClick(){
    this.activateAddEditStuCom=false;
    this.refreshStudentList();
  }

}

