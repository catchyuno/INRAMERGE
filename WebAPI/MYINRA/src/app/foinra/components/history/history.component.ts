import { Component, OnInit, Inject } from '@angular/core';
import * as moment from 'moment';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AuthService } from 'src/services/auth.service';
import { FactureService } from 'src/services/facture.service';
import { Facture } from 'src/interfaces';
import * as saveAs from 'file-saver';

@Component({
  selector: 'app-history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.scss']
})
export class HistoryComponent implements OnInit {
  trajets:any[]=[];p=1;facture:Facture|undefined;sums={sejSoc:0,sejUnite:0,sejTP:0,sejT:0};excele=false;
  constructor(public factureService:FactureService,public dialog:MatDialogRef<HistoryComponent>,public authService:AuthService,@Inject(MAT_DIALOG_DATA) public data:any) { }

  ngOnInit() {
    this.facture=this.data;
    this.trajets=this.data.trajetFacture;
    this.calcul();
    if(['Réglé','Dossier visé','Dossier visé reçu'].includes(this.facture!.status!)) this.excele=true;
  }


  color(i:any,x:any){
    switch(x){
      case "soc":x/30;break;
      case "unite":x/45;break;
      case "tp":x/15;break;
    }
  }
  getfile(i:any){
    this.factureService.getparam('getfile',{name:"process"+i.fichier}).subscribe(data=>{
      const blob='data:application/pdf;base64,'+data;
      saveAs(blob,'Attach'+i.idFacture+'.pdf');
    })
  }
  calcul(){
    this.factureService.getparam('getcalcul',{id:this.data.idFacture}).subscribe((full:any)=>{
      console.log(full);

      if(full.length!=0){
        const data=full[0];
         this.sums={sejSoc:data.sejourSoc,sejUnite:data.sejourUnite,sejTP:data.sejourTP,sejT:data.sejourGlobal};
      }


    })
  }
  excel(){
    let param="&ord=0&budget=0&credit=0&type=0&unite=0&sup=0&inf=0&id="+this.facture!.idFacture;
    this.factureService.etat('getsuivi',{param:param}).subscribe(data=>{
      saveAs(data,'Suivifacture '+this.facture!.numFacture+'.xls');
    })
  }
  tomoment(d:any){
    if(d==null) return null;
    return moment(d).format('DD/MM/YYYY');
  }
  cancel(){
    this.dialog.close(false);
  }
}

