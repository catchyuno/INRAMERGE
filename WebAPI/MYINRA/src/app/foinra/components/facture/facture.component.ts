import { Component, OnInit, Inject } from '@angular/core';
import { FactureService } from 'src/services/facture.service';
import * as moment from 'moment';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AuthService } from 'src/services/auth.service';
import { clone } from 'lodash';
@Component({
  selector: 'app-facture',
  templateUrl: './facture.component.html',
  styleUrls: ['./facture.component.scss']
})
export class FactureComponent implements OnInit {
  facture={idFacture:null,numFacture:null,montant:null,dateDepot:null,fichier:null,idMarche:null,credit:null,idMarcheNavigation:null,status:"Déposé",dateSaisie:null,userSaisie:null};
  file=null;filename='';fournisseurs=[];edit=false;classes=[];unites=[];Marches=[];
  constructor(public authService:AuthService,public factureService:FactureService,public dialog:MatDialogRef<FactureComponent>,@Inject(MAT_DIALOG_DATA) public data:any) { }

  ngOnInit() {
    this.factureService.get('getfournisseur').subscribe((data:any)=>{
      this.fournisseurs=data.fournisseurs;
      this.classes=data.classes;
      this.unites=data.unites;
      this.facture.dateDepot!=moment(data.date).startOf('day');
    });
    let par=1;
    if(this.authService.hasrole(0)) par=0;
    this.factureService.getparam('getmb',{set:par}).subscribe((data:any)=>{
      this.Marches=data;
    })
    if(this.data.facture!=null){
      this.edit=true;
      this.facture=clone(this.data.facture);
    }else{
      this.edit=false;
      this.facture.userSaisie=this.authService.User.Id;
    }
  }

  valid(){
    const f=this.facture;
    for(let i of this.data.fs){
      if(new String(this.facture.numFacture).toLowerCase()==new String(i.numFacture).toLowerCase() && i.status!="Rejeté" && i.idMarche==this.facture.idMarche){
        this.factureService.toast().warning('Numéro de facture éxistant!');
        return 0;
      }
    }
      if(!this.edit){
        if(f.dateDepot!=null && f.idMarche!=null && f.montant!=null && this.file!=null && this.file!=undefined ){
          this.factureService.post('add',{f:this.facture,file:this.file}).subscribe((success:any)=>{
            if(success){
              this.factureService.toast().success('Facture ajoutée');
              this.dialog.close(true);
            }else{
              this.factureService.toast().error('Erreur inconnue!veuiller réessayer ultérieurement.');
            }
          })
        }else{
          this.factureService.toast().warning('* Veuillez remplir tout les champs obligatoire!');
        }
      }else{
        if(f.dateDepot!=null && f.idMarche!=null && f.montant!=null){
          let fi=null;
          if(this.file!=null || this.file!=undefined){
            fi=this.file;
          }
          f.idMarcheNavigation=null;
          this.factureService.post('edit',{f:this.facture,file:fi}).subscribe((success:any)=>{
            if(success){
              this.factureService.toast().success('Facture modifié');
              this.dialog.close(true);
            }else{
              this.factureService.toast().error('Erreur inconnue!veuiller réessayer ultérieurement.');
            }
          })
        }else{
          this.factureService.toast().warning('* Veuillez remplir tout les champs obligatoire!');
        }
      }
  }
  cancel(){
    this.dialog.close(false);
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
}
