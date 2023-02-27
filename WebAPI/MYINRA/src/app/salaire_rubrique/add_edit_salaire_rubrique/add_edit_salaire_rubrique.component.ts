import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

@Component({
  selector: 'app-add_edit_salaire_rubrique',
  templateUrl: './add_edit_salaire_rubrique.component.html',
  styleUrls: ['./add_edit_salaire_rubrique.component.css']
})
export class Add_edit_salaire_rubriqueComponent implements OnInit {
  @Input() rubrique:any;
  CODE_RUBRIQUE: string ="";
  RUBRIQUE: string ="";
  RUBRIQUE_ABBREVEE: string ="";
  CATEGORIE: string ="";
  RubriqueList:any = [];

  constructor(private service: SharedService, private sharedService: SharedService) { }

  ngOnInit(): void {
    this.CODE_RUBRIQUE = this.rubrique.CODE_RUBRIQUE;
    this.RUBRIQUE = this.rubrique.RUBRIQUE.replaceAll("_","'")
    this.RUBRIQUE_ABBREVEE = this.rubrique.RUBRIQUE_ABBREVEE.replaceAll("_","'")
    this.CATEGORIE = this.rubrique.CATEGORIE;
  }

  updateSalaireRubrique(){
    var val = {
      CODE_RUBRIQUE:this.CODE_RUBRIQUE,
      RUBRIQUE:this.RUBRIQUE,
      RUBRIQUE_ABBREVEE:this.RUBRIQUE_ABBREVEE,
      CATEGORIE:this.CATEGORIE
    };
    this.service.updateSalaireRubrique(val).subscribe(res =>{
      alert(res.toString());
    })
}
}
