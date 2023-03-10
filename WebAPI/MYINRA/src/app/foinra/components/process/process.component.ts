import { Component, OnInit, Inject } from '@angular/core';
import { FactureService } from 'src/services/facture.service';
import { AuthService } from 'src/services/auth.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import * as moment from 'moment';
import { Facture, Fournisseur, TrajetFacture } from 'src/interfaces';

@Component({
  selector: 'app-process',
  templateUrl: './process.component.html',
  styleUrls: ['./process.component.scss']
})
export class ProcessComponent implements OnInit {
  fournisseurs:Fournisseur[]=[];operation=null;file=null;filename='';mindate=new Date();unites:any[]=[];classes:any[]=[];last:any=null;etape=0;rejeter=false;perm=false;multiplesm:any[]=[];p=1;
  trajet:any={idFacture:null,etape:null,commentaire:null,nomServ:null,idService:null,fichier:null,type:1,op:null,userSaisie:null,datePhysique:new Date(),dateFait:new Date(),dureePrec:null};
  facture:any={idFacture:null,numFacture:null,idMarcheNavigation:null,credit:null,montant:null,dateDepot:new Date(),numDossier:null,idMarche:null,status:null,dateSaisie:null,userSaisie:null,trajetFacture:[]};
  constructor(public factureService:FactureService,public authService:AuthService,public dialog:MatDialogRef<ProcessComponent>,@Inject(MAT_DIALOG_DATA) public data:any) { }

  ngOnInit() {
    this.facture=this.data;
    this.mindate!=this.facture.dateDepot;
    for(let i of this.facture.trajetFacture!){
      if(i.etape!>this.etape){
        this.etape!=i.etape;
        this.mindate!=i.dateFait;
        this.trajet.dateFait!=i.dateFait;
        this.trajet.datePhysique!=i.datePhysique;
        this.last=i;
      }
    }
    if(this.last!=null){
      if(this.last.type==0) this.rejeter=true;
    }
    this.trajet.userSaisie=this.authService.User.Id;
    this.trajet.idService=this.authService.User.Unite;
    this.trajet.idFacture!=this.facture.idFacture;
    this.trajet.etape!=this.etape+1;
    this.calculduree();
    switch(this.facture.status){
      case "A rectifier":this.operation!="Rectification de la facture";break;
      case "D??pos??":this.operation!="R??ception de la facture";break;
      case "R??ceptionn??":this.operation!="Certification du service";break;
      case "Certifi??":this.operation!="v??rification de la certification";break;
      case "V??rifi??":this.operation!="Ordonnancement";break;
      case "Ordonnanc??":this.operation!="Envoi au TP";break;
      case "Envoy?? au TP":this.operation!="R??c??ption et prise en charge";break;
      case "Pris en charge au TP":this.operation!="Visa du dossier";break;
      case "Dossier vis??":this.operation!="R??ception du dossier vis??";break;
      case "Dossier vis?? re??u":this.operation!="R??glement";break;
    }
    this.factureService.get('getfournisseur').subscribe((data:any)=>{
      this.fournisseurs=data.fournisseurs;
      this.classes=data.classes;
      this.unites=data.unites;

      this.perm=this.checkrole();
    });
  }
  checkrole(){
    let role=null;
    switch(this.facture.status){
      case "Rejet??":role=-1;break;
      case "A rectifier":role=1;break;
      case "D??pos??":role=1;break;
      case "R??ceptionn??":role=4;break;
      case "Certifi??":role=1;break;
      case "V??rifi??":role=2;break;
      case "Ordonnanc??":role=2;break;
      case "Envoy?? au TP":role=3;break;
      case "Pris en charge au TP":role=3;break;
      case "Dossier vis??":role=2;break;
      case "Dossier vis?? re??u":role=2;break;
    }
    if(this.facture.status=="R??ceptionn??"){
        let mp:any[]=[];let there=false;
        for(let i of this.facture.idMarcheNavigation!.serviceMarche!){
          mp.push(i);
          if(this.authService.User.Unite==i.idService){
            this.trajet.nomServ!=i.nomServ;
            there=true;
          }

        }
        //check validity
        this.multiplesm!=mp;
        let ac=0;let cer=0;
        for(let i of this.facture.trajetFacture!){
          if(i.op=="Certifi??" && i.type){
            if(i.idService==this.authService.User.Unite) ac++;
            for(let x of mp){
              if(x.idService==i.idService) x.status=i.type;
            }
          }else if(i.op=="V??rifi??" && !i.type){
            cer++;
          }
        }
        if(ac>cer || !there) role=-1;
    }
    if(this.facture.userSaisie==this.authService.User.Id && role==1) return true;
    if(this.facture.idMarcheNavigation!.ordonnateur!=this.authService.User.Class && !this.authService.hasrole(4)) return false;
    return this.authService.hasrole(role!);
  }
  cancel(){
    this.dialog.close(false);
  }
  getice(i:any){
    for(let x of this.fournisseurs){
      if(i===x.ice){
        return x.nomSc;
      }
    }
  }
  valid(){
    if(this.trajet.dateFait!=null){
      if(confirm('Confirmer ?')){
        if(moment(this.trajet.dateFait).isBefore(this.mindate)){
          this.factureService.toast().warning("la Date doit ??tre post??rieur a celle de l'??tape pr??c??dente!! ")
          //notif("la Date doit ??tre post??rieur a celle de l'??tape pr??c??dente!! ","alert-warning");
          return false;
        }
        switch(this.facture.status){
          case "A rectifier":this.facture.status=this.trajet.op="R??ceptionn??";break;
          case "D??pos??":this.facture.status=this.trajet.op="R??ceptionn??";break;
          case "R??ceptionn??":this.facture.status=this.trajet.op="Certifi??";break;
          case "Certifi??":this.facture.status=this.trajet.op="V??rifi??";break;
          case "V??rifi??":this.facture.status=this.trajet.op="Ordonnanc??";break;
          case "Ordonnanc??":this.facture.status=this.trajet.op="Envoy?? au TP";break;
          case "Envoy?? au TP":this.facture.status=this.trajet.op="Pris en charge au TP";break;
          case "Pris en charge au TP":this.facture.status=this.trajet.op="Dossier vis??";break;
          case "Dossier vis??":this.facture.status=this.trajet.op="Dossier vis?? re??u";break;
          case "Dossier vis?? re??u":this.facture.status=this.trajet.op="R??gl??";break;
        }
        if(this.facture.status=="Certifi??" && this.facture.idMarcheNavigation.serviceMarche.length>1){
          let count=0;
          for(let i of this.facture.trajetFacture){
            if(i.op=="Certifi??" && i.type) count++;
          }
          if(count==this.facture.idMarcheNavigation.serviceMarche.length-1){
            this.facture.status="Certifi??";
          }else{
            this.facture.status="R??ceptionn??";
          }
        }
        this.trajet.type=1;
        this.facture.idMarcheNavigation.serviceMarche=[];
        this.factureService.post('process',{f:this.facture,t:this.trajet,file:this.file}).subscribe(data=>{
          if(data){
            this.factureService.toast().success('Op??ration r??ussite!');
            this.dialog.close(true);
          }else{
            this.factureService.toast().error('Erreur inconnu! Veuiller r??essayer ult??rieurement');
          }
        })
      }
    }else{
      this.factureService.toast().error('Veuillez saisir une date*');
    }
  }
  rejet(){
    if(this.trajet.dateFait!=null){
      if(confirm('Rejeter ?')){
        switch(this.facture.status){
          case "A rectifier":this.facture.status="Rejet??";this.trajet.op="R??ceptionn??";break;
          case "D??pos??":this.facture.status="Rejet??";this.trajet.op="R??ceptionn??";break;
          case "R??ceptionn??":this.facture.status="A rectifier";this.trajet.op="Certifi??";break;
          case "Certifi??":this.facture.status="R??ceptionn??";this.trajet.op="V??rifi??";break;
          case "V??rifi??":this.facture.status="Certifi??";this.trajet.op="Ordonnanc??";break;
          case "Ordonnanc??":this.facture.status="V??rifi??";this.trajet.op="Envoy?? au TP";break;
          case "Envoy?? au TP":this.facture.status="Ordonnanc??";this.trajet.op="Pris en charge au TP";break;
          case "Pris en charge au TP":this.facture.status="Ordonnanc??";this.trajet.op="Dossier vis??";break;
          case "Dossier vis??":this.facture.status="Pris en charge au TP";this.trajet.op="Dossier vis?? re??u";break;
          case "Dossier vis?? re??u":this.facture.status="Dossier vis??";this.trajet.op="R??gl??";break;
        }
        this.trajet.type=0;
        this.facture.idMarcheNavigation.serviceMarche=[];
        this.factureService.post('process',{f:this.facture,t:this.trajet,file:this.file}).subscribe(data=>{
          if(data){
            this.factureService.toast().success('Op??ration r??ussite!');
            this.dialog.close(true);
          }else{
            this.factureService.toast().error('Erreur inconnu! Veuiller r??essayer ult??rieurement');
          }
        })
      }
      }else{
        this.factureService.toast().error('Veuillez saisir une date*');
      }
    }
  calculduree(){
    this.trajet.dureePrec=moment(this.trajet.dateFait).startOf('day').diff(moment(this.mindate).startOf('day'),'days');
  }
  tomoment(i:any){
    if(i==null || i==''){
      return null;
    }else{
      return moment(i).format('DD/MM/YYYY');
    }
  }
  f(file:any){
    let fi=file.target.files[0]
    if(fi.size>5000000 || fi.type!='application/pdf'){
      this.factureService.toast().warning('Veuillez selectionnez un fichier pdf de taille 5mb maximum');
    }else{
      let reader=new FileReader();
      reader.onload=()=>{
      let f=reader.result!.toString().replace('data:application/pdf;base64,','');
      this.file!=f;
      this.filename=fi.name;
      }
      reader.readAsDataURL(fi);
    }
  }
  attch(){
    document.getElementById('file')!.click();
  }
  getclasse(x:any){
    for(let i of this.classes){
      if(i.CLASSE==x) return i.LIBAFFEC;
    }
    return '';
  }
  getunite(c:any){
    for(let i of this.unites){
      if(i.UNITE==c) return i.LIBAFFEC;
    }
    return '';
  }

}
