import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  menu=[
    {title:"RESSOURCES HUMAINES / FORMATION / PAIE",links:[
      {name:"Attestations",description:"Travail, Salaire, etc.",icon:"far fa-file",ext:false,link:"profil"},
      {name:"Gespers",description:"Situation Administrative",icon:"fa fa-users",ext:true,link:"http://www.inravictoire.ma:8888"},
      {name:"Congés",description:"Travail, Salaire, etc.",icon:"fa fa-plane-departure",ext:true,link:"http://www.inravictoire.ma:90/congeinra/Connexion"},
      {name:"Evaluation",description:"EAP, Evaluation Ing & Chercheurs",icon:"fa fa-class",ext:true,link:"http://www.inravictoire.ma:8383"},
      {name:"Main d'Oeuvre",description:"Main d'oeuvre saisonnière",icon:"far fa-file",ext:false,link:""},
      {name:"Saisies",description:"Adha, congés, notation",icon:"fa fa-laptop",ext:true,link:"http://www.inravictoire.ma:8888"},
      {name:"Recrutement",description:"Traitement des candidatures",icon:"fa fa-laptop",ext:false,link:""},
      {name:"Manuel Procédures",description:"Administratif, RH et Finance",icon:"fa fa-book-open",ext:false,link:""}
    ]},
    {title:"FINANCE",links:[
      {name:"Vectis",description:"AO, Marchés, BC, Budget",icon:"far fa-file",ext:true,link:"http://www.inravictoire.ma:7772/Vectis_Prod_INRA"},
      {name:"FOINRA",description:"Suivi Paiement Factures",icon:"far fa-file",ext:false,link:"facture"},
      {name:"Fiches Projet",description:"Expression et suivi des besoins",icon:"far fa-file",ext:false,link:""},
      {name:"Déplacement",description:"Indemnités de déplacement",icon:"far fa-file",ext:false,link:""},
      {name:"Comptabilité",description:"Générale & Analytique",icon:"far fa-file",ext:false,link:""},
      {name:"Facturation",description:"Gestion commerciale",icon:"fa fa-file-invoice",ext:false,link:""},
      {name:"Stock",description:"Sorties & Entrées Magasins",icon:"fa fa-warehouse",ext:false,link:""},
      {name:"Praxis",description:"Parc Automobile     ",icon:"fa fa-car",ext:true,link:"http://www.inravictoire.ma:7772/Praxis_Prod_INRA"}
    ]}
  ]
  constructor(public router:Router) { }

  ngOnInit(): void {
  }

  routeto(ext:boolean,link:string){
    if(ext){
      window.open(link);
    }else{
      this.router.navigate([link]);
    }

  }

}
