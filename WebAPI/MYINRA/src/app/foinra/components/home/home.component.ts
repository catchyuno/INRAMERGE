import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import * as moment from 'moment';
import { MatDialog } from '@angular/material/dialog';
import { FactureComponent } from 'src/app/foinra/components/facture/facture.component';
import { FactureService } from 'src/services/facture.service';
import {saveAs} from 'file-saver';
import { AuthService } from 'src/services/auth.service';
import { ProcessComponent } from '../process/process.component';
import { HistoryComponent } from '../history/history.component';
import { AddMBCComponent } from '../add-mbc/add-mbc.component';
import { FilterComponent } from '../filter/filter.component';
import * as _ from 'lodash';
import {Facture,Fournisseur,} from 'src/interfaces';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit,OnDestroy {
  filtrage={de:null,a:null,ord:null,type:null,budget:null,credit:null,unite:null,sup:0,inf:0};
  bilan={de:null,a:null};
  p=1;fournisseurs:Fournisseur[]=[];de=null;a=null;ord=null;par={id:'0',classe:0,service:0,all:0};
  subs:Subscription[]=[];filter:Facture[]=[];flnot:boolean[]=[];
  factures:Facture[]=[];classes=[];unites:any[]=[];notif:boolean[]=[];search=[];
  show=false;grade='';loading=false;
  perso=[];

  constructor(public dialog:MatDialog,public factureService:FactureService,public authService:AuthService) {
   }
  ngOnDestroy(){
    sessionStorage.setItem('filter',JSON.stringify(this.filtrage));
    this.subs.forEach(sub=>{
      sub.unsubscribe();
    })
  }
  ngOnInit() {
    this.subs.push(this.factureService.getparam('getfournisseur',{classe:0}).subscribe((data:any)=>{
      this.fournisseurs=data.fournisseurs;
      this.classes=data.classes;
      this.unites=data.unites;
    }));
    this.ord=this.authService.User.Class;
    this.filtrage.ord=this.authService.User.Class;
    this.par={id:'0',classe:0,service:0,all:0};
    this.authService.User.role.forEach(role=>{
      switch(role){
        case 0:this.par.id=this.authService.User.Id!;break;
        case 1:this.par.id=this.authService.User.Id!;this.par.classe=this.authService.User.Class!;break;
        case 2:this.par.classe=this.authService.User.Class!;break;
        case 3:this.par.classe=this.authService.User.Class!;break;
        case 4:this.par.service=this.authService.User.Unite!;break;
        case 5:this.par.all=1;break;
        case 6:this.par.all=1;break;
      }
    })
    const filt=sessionStorage.getItem('filter')
    if(filt!=null) this.filtrage=JSON.parse(filt);
    this.refresh();
  }
  refresh(){
    this.loading=true;
    console.log(this.par);

    this.subs.push(this.factureService.getparam('get',this.par).subscribe((data:any)=>{
      this.factures=data;
      this.filter=data;
      this.notif=[];
      for(let i of this.factures){
        this.notif.push(this.checkrole(i));
      }
      this.flnot=this.notif;
      this.filters(this.filtrage);
      this.loading=false;
    }));
  }
  hasunite(sm: any,u: any){
    for(let i of sm){
      if(i.idService==u) return true;
    }
    return false;
  }
  filters(data: any){
    this.filter=[];this.flnot=[];let a=data.a;let de=moment(data.de).add('hour',-1);
    if(data.a==null || data.a=='') a=new Date(2100,1,1);
    if(data.de==null || data.de=='') de=moment(new Date(0));
    for(let i of this.factures){
      if(moment(i.dateDepot).isBetween(de,a,undefined,"[]") && (i.idMarcheNavigation?.ordonnateur==data.ord || data.ord==null || data.ord=='' ) && (data.credit==null || i.credit==data.credit)
      && (i.montant!>data.sup || data.sup==0) && (i.montant!<data.inf || data.inf==0) &&
       (i.idMarcheNavigation?.budget==data.budget || data.budget==null) && (data.type==null || i.idMarcheNavigation!.typeMarche==data.type) &&
        (data.unite==null || this.hasunite(i.idMarcheNavigation!.serviceMarche,data.unite))){
        this.filter.push(i);this.flnot.push(this.checkrole(i))
      }
    }

  }
  filtering(){
    this.dialog.open(FilterComponent,{disableClose:true,minWidth:'60%',minHeight:'50%',backdropClass:'bak',data:{fournisseurs:this.fournisseurs,classes:this.classes,unites:this.unites,filtrage:this.filtrage}}).afterClosed().subscribe(data=>{
      this.filters(data);
    });
  }
  cancel(){
    this.filtrage={de:null,a:null,ord:this.ord,type:null,budget:null,credit:null,unite:null,sup:0,inf:0};
    this.de=null;this.a=null;this.ord=this.authService.User.Class;
    this.filter=this.factures;this.flnot=this.notif;
    this.refresh();
  }
  getfile(i: any){
    this.subs.push(this.factureService.getparam('getfile',{name:"facture"+i.fichier}).subscribe((data:any)=>{
      const blob='data:application/pdf;base64,'+data;
      saveAs(blob,'Facture'+i.idFacture+'.pdf');
    }));
  }
  edit(i: any){
    this.dialog.open(FactureComponent,{disableClose:true,minWidth:'60%',minHeight:'50%',backdropClass:'bak',data:{facture:i}}).afterClosed().subscribe(data=>{
      this.refresh();
    })
  }
  process(i: any){
    this.dialog.open(ProcessComponent,{disableClose:true,minWidth:'90%',backdropClass:'bak',data:i}).afterClosed().subscribe(data=>{
      if(data) this.refresh();
    })
  }
  remove(i: any){
    if(window.confirm("Confirmer la suppression de la facture : "+i.numFacture+'/ '+i.idMarcheNavigation.nom)){
      this.subs.push(this.factureService.post('remove',i).subscribe((data: any)=>{
        if(data){
          this.factureService.toast().success('Facture supprimer!');
          this.refresh();
        }else{
          this.factureService.toast().error('Facture traité ou erreur système! veuillez réessayer ultérieurement');
        }
      }))
    }
  }
  renit(i: any){
    if(window.confirm("Confirmer la réinitialisation de la facture : "+i.numFacture+'/ '+i.idMarcheNavigation.nom)){
      this.subs.push(this.factureService.post('res',i.idFacture).subscribe((data: any)=>{
        if(data){
          this.factureService.toast().success('Facture réinitialisé!');
          this.refresh();
        }else{
          this.factureService.toast().error('Error');
        }
      }))
  }
  }
  history(i: any){
    this.dialog.open(HistoryComponent,{disableClose:true,minWidth:'80%',backdropClass:'bak',data:i}).afterClosed().subscribe(data=>{
      if(data) this.refresh();
    });
  }
  openfacture(){
    this.dialog.open(FactureComponent,{disableClose:true,minWidth:'60%',minHeight:'50%',backdropClass:'bak',data:{facture:null,fs:this.factures}}).afterClosed().subscribe(data=>{
      if(data) this.refresh();
    });
  }
  getice(i: any){
    for(let x of this.fournisseurs){
      if(i===x.ice){
        return x.nomSc;
      }
    }
    return '';
  }
  tomoment(d: moment.MomentInput){
    if(null) return null;
    return moment(d).format('DD/MM/YYYY');
  }
  view(i:any){
    this.dialog.open(AddMBCComponent,{disableClose:false,backdropClass:'bak',data:{new:false,view:true,mb:i.idMarcheNavigation}}).afterClosed().subscribe(data=>{
      if(data) this.refresh();
    });
  }
  getunite(c: any){
    for(let i of this.unites){
      if(i.UNITE==c) return i.LIBAFFEC;
    }
    return '';
  }
  excel(){
    let ex=_.clone(this.filtrage);
    if(this.filtrage.budget==null) ex.budget!=0;
    if(this.filtrage.credit==null) ex.credit!=0;
    if(this.filtrage.type==null) ex.type!=0;
    if(this.filtrage.unite==null) ex.unite!=0;
    if(this.filtrage.ord==null) ex.ord!=0;
    if(this.filtrage.sup<0) ex.sup=0;
    if(this.filtrage.inf<0) ex.inf=0;
    let param="&ord="+ex.ord+"&id=0&budget="+ex.budget+"&credit="+ex.credit+"&type="+ex.type+"&unite="+ex.unite+"&sup="+ex.sup+"&inf="+ex.inf;
    if(this.filtrage.de!=null && this.filtrage.de!='') param+="&de="+moment(this.filtrage.de).format('DD/MM/YYYY');
    if(this.filtrage.a!=null && this.filtrage.a!='') param+="&a="+moment(this.filtrage.a).format('DD/MM/YYYY');
    this.subs.push(this.factureService.etat('getsuivi',{param:param}).subscribe((data: string | Blob)=>{
      saveAs(data,'Suivifactures.xls');
    }));
  }
  checkrole(i:any){
    let role=-3;
    switch(i.status){
      case "Rejeté":role=-1;break;
      case "A rectifier":role=1;break;
      case "Déposé":role=1;break;
      case "Réceptionné":role=4;break;
      case "Certifié":role=1;break;
      case "Vérifié":role=2;break;
      case "Ordonnancé":role=2;break;
      case "Envoyé au TP":role=3;break;
      case "Pris en charge au TP":role=3;break;
      case "Dossier visé":role=2;break;
      case "Dossier visé reçu":role=2;break;
    }
    if(role==4){
      let there=false;
      for(let x of i.idMarcheNavigation.serviceMarche){
        if(this.authService.User.Unite==x.idService){
          there=true;
        }
      }
      if(!there) role=-1;
    }
    if(i.userSaisie==this.authService.User.Id && role==1) return true;
    if(i.idMarcheNavigation.ordonnateur!=this.authService.User.Class && !this.authService.hasrole(4)) return false;
    return this.authService.hasrole(role);
  }
  findBill(term: any,item: any){
    term=term.toLowerCase();
    if(String(item.idFacture).toLowerCase().includes(term) || String(item.numFacture).toLowerCase().includes(term) ||String(item.idMarcheNavigation.nom).toLowerCase().includes(term)) return true;
    return false;
  }
  getBill(){
    this.filter=[...this.search];

  }
}
