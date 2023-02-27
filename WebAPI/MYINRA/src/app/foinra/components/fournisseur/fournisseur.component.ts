import { Component, OnInit } from '@angular/core';
import { FactureService } from 'src/services/facture.service';
import { AuthService } from 'src/services/auth.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Fournisseur } from 'src/interfaces';

@Component({
  selector: 'app-fournisseur',
  templateUrl: './fournisseur.component.html',
  styleUrls: ['./fournisseur.component.scss']
})
export class FournisseurComponent implements OnInit {
  add=false;textadd:string='Ajouter un fournisseur';editable=false;user: any;p=1;
  classes=[];unites=[];
  fournisseurs:Fournisseur[]=[];
  fournisseur:Fournisseur={
    email: '',
    ice: '',
    idFiscal: '',
    idUser: '',
    nomResp:'',
    enabled:true,
    nomSc: '',
    raisonSociale: '',
    registreCommerce: '',
    tel: '',
    classf:0,
    type: ''
  };
  constructor(public factureService:FactureService,public authService:AuthService,public dialog:MatDialogRef<FournisseurComponent>) { }

  ngOnInit() {
    this.editable=false;
    this.factureService.getparam('getfournisseur',{classe:this.authService.User.Class}).subscribe((data:any)=>{
      this.fournisseurs=data.fournisseurs;
      this.classes=data.classes;
      this.unites=data.unites;
      console.log(data);
    });
  }
  addmode(){
    this.add=true;
    this.textadd='Ajouter un fournisseur';
    this.editable=true;
    this.fournisseur={
      email: '',
      ice: '',
      idFiscal: '',
      idUser: '',
      nomResp:'',
      enabled:true,
      nomSc: '',
      raisonSociale: '',
      registreCommerce: '',
      tel: '',
      classf:0,
      type: ''
    };
  }
  valid(){
    if(this.fournisseur.ice!='' && this.fournisseur.ice!="" && this.fournisseur.nomSc!='' && this.fournisseur.nomSc!=""){
      if(this.add){
        this.factureService.post('addfournisseur',this.fournisseur).subscribe(data=>{
          if(data){
            this.factureService.toast().success('Fournisseur ajouter!');
            this.ngOnInit();
          }else{
            this.factureService.toast().error('ICE ou nom de société existant');
          }
        });
      }else{
        this.factureService.post('editfournisseur',this.fournisseur).subscribe(data=>{
          if(data){
            this.factureService.toast().success('Fournisseur Modifier  !');
            this.ngOnInit();
          }else{
            this.factureService.toast().error('ICE ou nom de société existant');
          }
        });
      }
    }else{
      this.factureService.toast().warning('Veuiller remplir les champs obligatoire*');
    }


  }
  confirm(i: any){
    if(window.confirm('Activer le compte Fournisseur ice :'+String(i.ice).toUpperCase()+' ?' )){
      i.enabled=true;
      this.authService.post('fournisseur',i).subscribe(data=>{
        if(data){
          this.factureService.toast().success('Le Compte du fournisseur est activé , un email sera envoyé pour le notifier');
        }
      })
    }
  }
  remove(i: any){
    if(window.confirm('Supprimer Fournisseur ice :'+String(i.ice).toUpperCase()+' ?' )){
      if(i.enabled && i.idUser==null){
        this.factureService.post('deletefournisseur',i).subscribe(data=>{
          if(data){
            this.factureService.toast().success('Fournisseur supprimé');
            this.ngOnInit();
          }else{
            this.factureService.toast().warning('Ce fournisseur a des factures');
          }
        })
      }else{
        this.authService.post('deletefournisseur',i).subscribe(data=>{
          if(data){
            this.factureService.toast().success('Fournisseur supprimé');
            this.ngOnInit();
          }else{
            this.factureService.toast().warning('Ce fournisseur a des factures');
          }
        })
      }
    }
  }
  cancel(){
    this.dialog.close();
  }
  reset(){
    this.editable=false;
    this.ngOnInit();
  }
  edit(i:Fournisseur){
    this.add=false;
    this.textadd='Modifier fournisseur '+i.ice;
    this.editable=true;
    this.fournisseur=i;
    console.log(i);

  }

}
