import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { Validators, FormBuilder, FormGroup, FormControl, FormArray } from '@angular/forms';
import { Show_banqueComponent } from "src/app/banque/show_banque/show_banque.component";

//import { DatePipe } from '@angular/common';
//import { NgxSpinnerService } from 'ngx-spinner';
import { banqueComponent } from '../banque.component';

// declare var require: any
// const FileSaver = require('file-saver');

@Component({
  selector: 'app-add_edit_banque',  
  templateUrl: './add_edit_banque.component.html',
  styleUrls: ['./add_edit_banque.component.css']
})
export class Add_edit_banqueComponent implements OnInit {
  @Input() banque:any;
  ABBREVIATION: string ="";
  BANQUE_FR: string ="";  
  BANQUE_AR: string ="";
  BANQUE_FR_ANCIEN:string="";
  constructor(private service: SharedService, private sharedService: SharedService) { }

  ngOnInit(): void {
    this.BANQUE_FR = this.banque.BANQUE_FR.replaceAll("_", "'");
    this.BANQUE_AR = this.banque.BANQUE_AR;
    this.BANQUE_FR_ANCIEN= this.banque.BANQUE_FR.replaceAll("_", "'");
  }

  addbanque(){
    var val = {
      BANQUE_FR:this.BANQUE_FR,
      BANQUE_AR:this.BANQUE_AR,
    }
     this.service.addbanque(val).subscribe(res =>{
       if (res!="")
     {
         alert(res.toString())
       }
     })
  }

  updatebanque(){
    var val = {
      BANQUE_FR:this.BANQUE_FR,
      BANQUE_AR:this.BANQUE_AR,
      BANQUE_FR_ANCIEN:this.BANQUE_FR_ANCIEN,
    };
      this.service.updatebanque(val).subscribe(res =>{
        alert(res.toString());
      })
  }  
}
