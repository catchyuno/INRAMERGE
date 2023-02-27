import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import * as saveAs from 'file-saver';
import { MarcheBc } from 'src/interfaces';
import { AuthService } from 'src/services/auth.service';
import { FactureService } from 'src/services/facture.service';
import { AddMBCComponent } from '../add-mbc/add-mbc.component';
import { FournisseurComponent } from '../fournisseur/fournisseur.component';

@Component({
  selector: 'app-marche-bc',
  templateUrl: './marche-bc.component.html',
  styleUrls: ['./marche-bc.component.scss']
})
export class MarcheBcComponent implements OnInit {
  add=false;textadd: any;editable=false;user: any;p=1;
  classes=[];unites=[];filtered:MarcheBc[]=[];
  Marches:MarcheBc[]=[];fournisseurs=[];filter={type:null,ord:null,annee:null,budget:null};
  constructor(public authService:AuthService,public factureService:FactureService,public dialog:MatDialog) { }

  ngOnInit() {
    this.factureService.getparam('getfournisseur',{classe:this.authService.User.Class,isadmin:(this.authService.hasrole(6) ||this.authService.hasrole(5))}).subscribe((data:any)=>{
      this.fournisseurs=data.fournisseurs;
      this.classes=data.classes;
      this.unites=data.unites;
      this.Marches=data.marchebc;
      this.filtered=data.marchebc;
      this.filters(this.filter);
    });
  }
  filters(data: { type: any; ord: any; annee: any; budget: any; }){
    this.filtered=[];
    for(let i of this.Marches){
      if((i.ordonnateur==data.ord || data.ord==null || data.ord=='' ) && (data.type==null || i.typeMarche==data.type) && (data.budget==null || i.budget==data.budget) && (data.annee==null || i.annee==data.annee)){
        this.filtered.push(i);
      }
    }
  }
  addmarche(){
    this.dialog.open(AddMBCComponent,{disableClose:true,backdropClass:'bak',data:{new:true,view:false,mb:null}}).afterClosed().subscribe(data=>{
      this.ngOnInit();
    });
  }
  editmarche(i: any){
    this.dialog.open(AddMBCComponent,{disableClose:true,backdropClass:'bak',data:{new:false,view:false,mb:i}}).afterClosed().subscribe(data=>{
      this.ngOnInit();
    });
  }
  view(i: any){
    this.dialog.open(AddMBCComponent,{disableClose:false,backdropClass:'bak',data:{new:false,view:true,mb:i}}).afterClosed().subscribe(data=>{
      this.ngOnInit();
    });
  }
  addfournisseur(){
    this.dialog.open(FournisseurComponent,{disableClose:true,backdropClass:'bak'}).afterClosed().subscribe(data=>{
      this.ngOnInit();
    })
  }
  closemb(i: any){
    if(confirm("Fermer l'accès du Marché/BC/Convention id : "+i.id)){
      i.status=false;
    this.factureService.post('editmb',{mb:i,file:null}).subscribe(data=>{
      if(data){
        this.factureService.notif("Marché/BC id :"+i.id+" fermé","alert-success");
      }
    })
    }

  }
  openmb(i:any){
    if(confirm("Ouvrir l'accès du Marché/BC/Convention id : "+i.id)){
      i.status=true;
    this.factureService.post('editmb',{mb:i,file:null}).subscribe(data=>{
      if(data){
        this.factureService.notif("Marché/BC id :"+i.id+" Ouvert","alert-success");
      }
    })
    }

  }
  remove(i: any){
    if(confirm("Confirmer la suppression du Marché/BC/Convention id :"+i.id)){
      this.factureService.post('deletemb',i).subscribe(data=>{
        if(data){
          this.ngOnInit();
          this.factureService.notif("Marché/BC/Convention Supprimer! ",'alert-success');
        }else{
          this.factureService.notif("Erreur inconnu",'alert-warning');
        }
      })
    }
  }
  getfile(i:any){
    this.factureService.getparam('getfile',{name:"MarcheBc"+i.fichier}).subscribe(data=>{
      const blob='data:application/pdf;base64,'+data;
      saveAs(blob,'MarcheBc '+i.nom+'.pdf');
    })
  }

}


