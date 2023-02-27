import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AuthService } from 'src/services/auth.service';

@Component({
  selector: 'app-filter',
  templateUrl: './filter.component.html',
  styleUrls: ['./filter.component.scss']
})
export class FilterComponent implements OnInit {
  filtrage:any={de:null,a:null,ord:null,type:null,budget:null,credit:null,unite:null,sup:0,inf:0};fournisseurs=[];classes=[];unites=[];
  constructor(public authService:AuthService,public dialog:MatDialogRef<FilterComponent>,@Inject(MAT_DIALOG_DATA) public data:any) {    
    this.unites=data.unites;
    this.classes=data.classes;
    this.fournisseurs=data.fournisseurs;
    this.filtrage=data.filtrage;
   }

  ngOnInit() {

  }
  
  cancel(){    
    this.dialog.close(this.filtrage);
  }

}
