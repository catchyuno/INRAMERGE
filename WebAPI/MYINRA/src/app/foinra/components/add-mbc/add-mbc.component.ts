import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import * as saveAs from 'file-saver';
import { clone } from 'lodash';
import { Facture, Fournisseur,MarcheBc, serviceMarche } from 'src/interfaces';
import { AuthService } from 'src/services/auth.service';
import { FactureService } from 'src/services/facture.service';

@Component({
  selector: 'app-add-mbc',
  templateUrl: './add-mbc.component.html',
  styleUrls: ['./add-mbc.component.scss']
})
export class AddMBCComponent implements OnInit {
  edit=false;user:any;p=1;view=true;file=null;filename='';enval=false;
  classes=[];unites=[];service:any;facture:Facture | undefined;
  fournisseurs:Fournisseur[]=[];
  mb:MarcheBc={
    ice:'',
    intitule: '',
    montant: 0,
    nom: '',
    ordonnateur: this.authService.User.Class!,
    annee:new Date().getFullYear(),
    budget:"Budget général",
    serviceMarche: [],
    typeMarche:'',
    status:true,
    userSaisie:this.authService.User.Id!
  };
  marches=[];

  constructor(public factureService:FactureService,public authService:AuthService,public dialog:MatDialogRef<AddMBCComponent>,@Inject(MAT_DIALOG_DATA) public data:any) { }

  ngOnInit() {
    if(this.data.new){
      this.edit=false;
      this.view=false;
      this.mb={
        ice:'',
        intitule: '',
        montant: 0,
        nom: '',
        ordonnateur: this.authService.User.Class!,
        annee:new Date().getFullYear(),
        budget:"Budget général",
        serviceMarche: [],
        typeMarche:'',
        status:true,
        userSaisie:this.authService.User.Id!
      };
    }else if(!this.data.new && !this.data.view){
      this.mb=clone(this.data.mb);
      this.edit=true;
      this.view=false;
    }else{
      this.edit=true;this.view=true;
      this.mb=this.data.mb;
    }
    this.factureService.getparam('getfournisseur',{classe:this.authService.User.Class,isadmin:(this.authService.hasrole(6) ||this.authService.hasrole(5))}).subscribe((data:any)=>{
      this.fournisseurs=data.fournisseurs;
      this.classes=data.classes;
      this.unites=data.unites;
      this.marches=data.marchebc;
    });
  }
  addsm(){
    if(this.service!=null){
      for(let i of this.mb.serviceMarche!){
        if(i.idService == this.service.UNITE) return false;
      }
      this.mb.serviceMarche!.push({idService:this.service.UNITE,nomServ:this.service.LIBAFFEC,status:0});
    }
  }
  remove(i:any){
    if(this.edit){
      for(let x of this.mb.serviceMarche!){
        if(x.idService==i.idService){
          this.factureService.post('deletesm',x).subscribe(data=>{
            if(data){
              this.factureService.toast().info('Service/Unité supprimer!');
            }else{
              return false;
            }
          });
        }
      }
    }
    for(let x of this.mb.serviceMarche!){
      if(x.idService==i.idService) this.mb.serviceMarche!.splice(this.mb.serviceMarche!.indexOf(x),1);
    }
  }
  valid(){
    for(let i of this.data.fs){
      if(new String(this.facture!.numFacture).toLowerCase()==new String(i.numFacture).toLowerCase() && i.status!="Rejeté" && i.idMarche==this.facture!.idMarche){
        this.factureService.toast().warning('Numéro de facture éxistant!');
        return 0;
      }
    if(this.mb.serviceMarche!.length!=0 && this.mb.nom!=null && this.mb.nom!="" && this.mb.ordonnateur!=null && this.mb.ordonnateur.toString() != "" && this.mb.ice!=null && this.mb.ice!="" && this.mb.montant!=null && this.mb.montant.toString()!=""){
      this.enval=true;
      if(this.edit && !this.view){
        let marb=this.mb;
        this.factureService.post('editmb',{mb:this.mb,file:this.file}).subscribe(data=>{
          if(data){
            this.factureService.toast().success('Marché/BC Modifier');
            this.dialog.close();
          }else{
            this.factureService.toast().error('Erreur inconnu ! veuillez réessayer ultérieurement');
            this.enval=false;
          }
        })
      }else if(!this.edit){
        this.factureService.post('addmb',{mb:this.mb,file:this.file}).subscribe(data=>{
          if(data){
            this.factureService.toast().success('Marché/BC ajouter');
            this.dialog.close();
          }else{
            this.factureService.toast().error('Erreur inconnu ! veuillez réessayer ultérieurement');
            this.enval=false;
          }
        })
      }
    }else{
      this.factureService.toast().warning('Veuiller remplir les champs obligatoire*');
    }
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
  getfile(i:any){
    this.factureService.getparam('getfile',{name:"MarcheBc"+i.fichier}).subscribe(data=>{
      const blob='data:application/pdf;base64,'+data;
      saveAs(blob,'MarcheBc '+i.nom+'.pdf');
    })
  }
  cancel(){
    this.dialog.close(false);
  }
}
