import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

@Component({
  selector: 'app-add_edit_arabe',
  templateUrl: './add_edit_arabe.component.html',
  styleUrls: ['./add_edit_arabe.component.css']
})
export class Add_edit_arabeComponent implements OnInit {
  @Input() arabe:any;
  CATEGORIE: string ="";
  FRANCAIS_GESPERS: string ="";
  FRANCAIS_M: string ="";
  ARABE_M: string ="";
  FRANCAIS_F: string ="";
  ARABE_F: string ="";
  constructor(private service: SharedService, private sharedService: SharedService) { }

  ngOnInit(): void {
    this.CATEGORIE = this.arabe.CATEGORIE;
    this.FRANCAIS_GESPERS = this.arabe.FRANCAIS_GESPERS;
    this.FRANCAIS_M = this.arabe.FRANCAIS_M;
    this.ARABE_M = this.arabe.ARABE_M;
    this.FRANCAIS_F = this.arabe.FRANCAIS_F;
    this.ARABE_F = this.arabe.ARABE_F;
  }

  updatearabe(){
    var val = {CATEGORIE:this.CATEGORIE,
      FRANCAIS_GESPERS:this.FRANCAIS_GESPERS,
      FRANCAIS_M:this.FRANCAIS_M,
      ARABE_M:this.ARABE_M,
      FRANCAIS_F:this.FRANCAIS_F,
      ARABE_F:this.ARABE_F,
    };
      this.service.updatearabe(val).subscribe(res =>{
        alert(res.toString());
      })
  }  
}
