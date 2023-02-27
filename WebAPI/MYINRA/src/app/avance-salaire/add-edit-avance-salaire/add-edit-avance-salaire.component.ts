import { Component, OnInit, Input } from '@angular/core';
import { SharedService } from "src/app/shared.service";

@Component({
  selector: 'app-add-edit-avance-salaire',
  templateUrl: './add-edit-avance-salaire.component.html',
  styleUrls: ['./add-edit-avance-salaire.component.css']
})


export class AddEditAvanceSalaireComponent implements OnInit {
  personnelList:any = [];
  @Input() 
  DATE_DEMANDE:string = "";
  DDP: string ="";
  NOM_PRENOM: string ="";
  GRADE:string = "";
  AFFECTATION: string ="";
  SALAIRE_NET: string ="";
  TOT_CREANCES:string = "";
  VIREMENT: string ="";
  MT_DEMANDE: string ="";
  MENS_POSSIBLE:string = "";
  AVANCE_ACCORDEE: string ="";
  MENS_AVANCE: string ="";
  DUREE_AVANCE:string = "";
  DU: string ="";
  AU: string ="";
  VALIDE:string = "";
  JUSTIF_NON: string ="";
  DATE_NAISSANCE: string ="";
  BASE_CALCUL: string ="";

  today: Date;
  //np: any;

  
  constructor(private service: SharedService, private sharedService: SharedService) { 
    this.today =new Date();

  }

  remplirdata(){
    alert("t");
    
  }

  ngOnInit(): void {
    this.DATE_DEMANDE = this.DATE_DEMANDE;
    this.DDP = this.DDP;
    this.NOM_PRENOM = this.NOM_PRENOM;
    this.GRADE=this.GRADE;
    this.AFFECTATION=this.AFFECTATION;
    this.SALAIRE_NET=this.SALAIRE_NET;
    this.TOT_CREANCES=this.TOT_CREANCES;
    this.VIREMENT=this.VIREMENT;
    this.MT_DEMANDE=this.MT_DEMANDE;
    this.MENS_POSSIBLE=this.MENS_POSSIBLE;
    this.AVANCE_ACCORDEE=this.AVANCE_ACCORDEE;
    this.MENS_AVANCE=this.MENS_AVANCE;
    this.DUREE_AVANCE=this.DUREE_AVANCE;
    this.DU=this.DU;
    this.AU=this.AU;
    this.VALIDE=this.VALIDE;
    this.JUSTIF_NON=this.JUSTIF_NON;
    this.DATE_NAISSANCE=this.DATE_NAISSANCE;
    this.BASE_CALCUL=this.BASE_CALCUL;

    this.refreshpersonnelList();
  }

  refreshpersonnelList() {
    this.sharedService.getpersonnelList().subscribe(data =>{
      this.personnelList = JSON.stringify(data);
    
    //   localStorage.setItem("testJSON", this.personnelList);
    //   let text = localStorage.getItem("testJSON");
    //   let data = JSON.parse(text);
    //  //this.np=this.personnelList
      //alert(data);
     // return this.personnelList;
     //this.NOM_PRENOM="rrr"
    });  
  }

  addStudent(){
    var val = {DATE_DEMANDE:this.DATE_DEMANDE,
      DDP:this.DDP,
      NOM_PRENOM:this.NOM_PRENOM,
      GRADE:this.GRADE,
      AFFECTATION:this.AFFECTATION,
      SALAIRE_NET:this.SALAIRE_NET,
      TOT_CREANCES:this.TOT_CREANCES,
      VIREMENT:this.VIREMENT,
      MT_DEMANDE:this.MT_DEMANDE,
      MENS_POSSIBLE:this.MENS_POSSIBLE,
      AVANCE_ACCORDEE:this.AVANCE_ACCORDEE,
      MENS_AVANCE:this.MENS_AVANCE,
      DUREE_AVANCE:this.DUREE_AVANCE,
      DU:this.DU,
      AU:this.AU,
      VALIDE:this.VALIDE,
      JUSTIF_NON:this.JUSTIF_NON,
      DATE_NAISSANCE:this.DATE_NAISSANCE,
      BASE_CALCUL:this.BASE_CALCUL};
      this.service.addStudent(val).subscribe(res =>{
        alert(res.toString());
      })
  }

  updateStudent(){
    var val = {DATE_DEMANDE:this.DATE_DEMANDE,
      DDP:this.DDP,
      NOM_PRENOM:this.NOM_PRENOM};
      this.service.updateStudent(val).subscribe(res =>{
        alert(res.toString());
      })
  }


  
}
