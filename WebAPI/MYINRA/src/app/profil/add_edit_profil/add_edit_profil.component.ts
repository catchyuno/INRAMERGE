import { Component, Input, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-add_edit_profil',
  templateUrl: './add_edit_profil.component.html',
  styleUrls: ['./add_edit_profil.component.css']
})
export class Add_edit_profilComponent implements OnInit {
  DDP: string ="";
  MS:string="";
  CATEGORIE_AGENT: any= []
  CIN:any= []
  ADRESSE:any= []
  DNAISSANCE:any= []
  SITFAMILLE:any= []
  ENFANTS:any= []
  DRECRUTE:any= []
  RETRAITE:any= []
  RCAR:any= []
  NOM:any= []
  NOMAR:any= []
  GRADE:any= []
  ECHELLE:any= []
  ECHELON:any= []
  GRADE_CODE:any= []
  DEFFET_GRADE:any= []
  DEFFET_ECHELON:any= []
  DEFFET_FONCTION:any= []
  DEFFET_AFF:any= []
  FONCTION:any= []
  DSORTIE:any= []
  BANQUE:any= []
  MOTIF_SORTIE:any= []
  RIB:any= []
  AFFECTATION:any= []
  ProfilList:any = [];
  AgentList:any = [];
  MotifSortieList:any = [];
  modopen=false;
  agent:any;
  aff:any;
  fonct:any;
  grade:any;
  profil:any;
  //fromprofil:any="OUI";
  conjoint:any;
  modalTitle:any;
  imagePath:any;

  constructor(private service: SharedService, private sharedService: SharedService, private datePipe: DatePipe) {  }
  ngOnInit() {
    this.refreshAgentList();
    this.agent={DDP:this.service.userInfo.DDP ,NOM_PRENOM: this.service.userInfo.NOM_PRENOM};
    this.refreshmonprofil();
    this.downloadfile(this.agent);
    //this.imagePath="/assets/PICS_PROFIL/" + this.agent.DDP + ".jpg";
    this.profil={
      DDP: this.agent.DDP,
    }
    this.grade={
      DDP: this.agent.DDP,
    }
    this.fonct={
      DDP: this.agent.DDP,
    }
    this.conjoint={
      DDP: this.agent.DDP,
    }
    this.aff={
      DDP: this.agent.DDP,
    }
  }

  vider()
  {
    this.agent={DDP:this.service.userInfo.DDP ,NOM_PRENOM: this.service.userInfo.NOM_PRENOM};
    this.refreshmonprofil();
    //this.imagePath="/assets/PICS_PROFIL/" + this.agent.DDP + ".jpg";

  }
  choixagent(agent:any) {
    var val = {
      DDP:this.agent.DDP
    };
    this.refreshmonprofil();
    //this.imagePath="/assets/PICS_PROFIL/" + this.agent.DDP + ".jpg";
   // this.MOTIF_S()
  }

  refreshmonprofil() {
    this.CIN="";
      this.ADRESSE="";
      this.DNAISSANCE="";
      this.SITFAMILLE="";
      this.ENFANTS="";
      this.DRECRUTE="";
      this.RETRAITE="";
      this.RCAR="";
      this.NOMAR="";
      this.NOM="";
      this.GRADE="";
      this.ECHELLE="";
      this.GRADE_CODE="",
      this.DEFFET_GRADE="";
      this.DEFFET_ECHELON="";
      this.ECHELON="";
      this.AFFECTATION="";
      this.DEFFET_AFF="";
      this.BANQUE="";
      this.RIB="";
      this.FONCTION="";
      this.DEFFET_FONCTION="";
      this.DSORTIE="";
      this.MOTIF_SORTIE="";
      var val = {
        DDP:this.agent.DDP
      };
    this.sharedService.getMonProfil(val).subscribe(data =>{
      this.ProfilList = data;
      this.DDP=this.agent.DDP;
      this.CIN=this.ProfilList[0]["CIN"];
      this.ADRESSE=this.ProfilList[0]["ADRESSE"];
      this.DNAISSANCE=this.datePipe.transform(this.ProfilList[0]["DNAISSANCE"], 'dd/MM/yyyy');
      this.SITFAMILLE=this.ProfilList[0]["SITFAMILLE"];
      if (this.SITFAMILLE==="M")
      {
        this.SITFAMILLE="MARIE(E)"
      }
      if (this.SITFAMILLE==="C")
      {
        this.SITFAMILLE="CELIBATAIRE"
      }
      if (this.SITFAMILLE==="D")
      {
        this.SITFAMILLE="DIVORCE(E) AVEC CHARGES"
      }
      if (this.SITFAMILLE==="F")
      {
        this.SITFAMILLE="DIVORCE(E) AVEC CHARGES"
      }
      if (this.SITFAMILLE==="V")
      {
        this.SITFAMILLE="VEUF(VE)"
      }
      if (this.SITFAMILLE==="N")
      {
        this.SITFAMILLE=""
      }
      this.ENFANTS=this.ProfilList[0]["ENFANTS"];
      this.DRECRUTE=this.datePipe.transform(this.ProfilList[0]["DRECRUTE"], 'dd/MM/yyyy');
      this.RETRAITE=this.ProfilList[0]["libtret"].toUpperCase();
      this.RCAR=this.ProfilList[0]["RCAR"];
      this.NOMAR=this.ProfilList[0]["NOMAR"];
      this.NOM=this.ProfilList[0]["NOM"].toUpperCase();
      this.DSORTIE=this.datePipe.transform(this.ProfilList[0]["DSORTIE"], 'dd/MM/yyyy');
      this.MOTIF_SORTIE=this.ProfilList[0]["POSADMIN"];
      var val1 = {
        MS:this.MOTIF_SORTIE
      };
        this.sharedService.getPositionAdm(val1).subscribe(data =>{
        this.MotifSortieList = data;
        this.MOTIF_SORTIE=this.MotifSortieList[0]["POSADMIN"];
      });
    });
    this.sharedService.getMonProfilGrade(val).subscribe(data =>{
      this.ProfilList = data;
      this.GRADE_CODE=this.ProfilList[0]["GRADE"];
      this.GRADE=this.ProfilList[0]["LIBGRADE"].toUpperCase();
      this.ECHELLE=this.ProfilList[0]["ECHELLE"];
      //this.DEFFET_GRADE=this.datePipe.transform(this.ProfilList[0]["DEFFET"], 'dd/MM/yyyy');
      this.DEFFET_ECHELON=this.datePipe.transform(this.ProfilList[0]["DECHELON"], 'dd/MM/yyyy');
      this.ECHELON=this.ProfilList[0]["ECHELON"];
      // console.log(this.ProfilList)
      // console.log(this.GRADE_CODE)
      var val2 = {
        DDP:this.agent.DDP,
        GRADE:this.GRADE_CODE,
      };
      this.sharedService.getMonProfilGradeEffet(val2).subscribe(data =>{
        console.log(val2)
        this.ProfilList = data;
        if (this.ProfilList!="")
        {

           // this.AFFECTATION=this.ProfilList[0]["LIBC"].toUpperCase();
            this.DEFFET_GRADE=this.datePipe.transform(this.ProfilList[0]["DEFFET"], 'dd/MM/yyyy');
        }
    });
    // var val2 = {
    //   DDP:this.agent.DDP,
    //   GRADE:this.GRADE_CODE,
    // };
    // this.sharedService.getMonProfilGradeEffet(val2).subscribe(data =>{
    //   console.log(val2)
    //   this.ProfilList = data;
    //   if (this.ProfilList!="")
    //   {

    //      // this.AFFECTATION=this.ProfilList[0]["LIBC"].toUpperCase();
    //       this.DEFFET_GRADE=this.datePipe.transform(this.ProfilList[0]["DEFFET"], 'dd/MM/yyyy');
    //   }
    });
    this.sharedService.getMonProfilAff(val).subscribe(data =>{
      this.ProfilList = data;
      if (this.ProfilList!="")
      {
          this.AFFECTATION=this.ProfilList[0]["LIBC"].toUpperCase();
          this.DEFFET_AFF=this.datePipe.transform(this.ProfilList[0]["DEFFET"], 'dd/MM/yyyy');
      }
    });
    this.sharedService.getMonProfilFonction(val).subscribe(data =>{
      this.ProfilList = data;
      if (this.ProfilList!="")
      {
          this.FONCTION=this.ProfilList[0]["LIBFONCR"].toUpperCase();
          this.DEFFET_FONCTION=this.datePipe.transform(this.ProfilList[0]["deffet"], 'dd/MM/yyyy');
      }
    });
    this.sharedService.getMonProfilBanque(val).subscribe(data =>{
      this.ProfilList = data;
      if (this.ProfilList!="")
      {
          this.BANQUE=this.ProfilList[0]["BANQUE_FR"].toUpperCase();
          this.RIB=this.ProfilList[0]["RIB"];
      }
    });


  }


  ShowProfil(agent:any){
    //console.log(agent)
    this.modopen=false;
    this.profil={
      DDP: this.agent.DDP,
      NOM_PRENOM:this.agent.NOM_PRENOM
    }
    this.modopen=true;
  }

  ShowGrades(agent:any){
    this.modopen=false;
    this.grade={
      DDP: this.agent.DDP,
    }
    this.modopen=true;
  }

  ShowAff(agent:any){
    this.modopen=false;
    this.aff={
      DDP: this.agent.DDP,
    }
    this.modopen=true;
  }

  ShowFonction(agent:any){
    this.modopen=false;
    this.fonct={
      DDP: this.agent.DDP,
    }
    this.modopen=true;
  }

  ShowConjoint(agent:any){
    this.modopen=false;
    this.conjoint={
      DDP: this.agent.DDP,
    }
    this.modopen=true;
  }
  refreshAgentList() {
    var val = {
      DDP:this.service.userInfo.DDP,
    };
    this.sharedService.getMonProfilAgentList(val).subscribe(data =>{
      this.AgentList = data;
      this.CATEGORIE_AGENT=this.AgentList[0]["CATEGORIE"];
    });
  }

  closeClick(){
    this.modopen=false;
    //this.refreshAgentList();
    //this.agent={DDP:this.service.userInfo.DDP ,NOM_PRENOM: this.service.userInfo.NOM_PRENOM};
    this.refreshmonprofil();
    //this.imagePath="/assets/PICS_PROFIL/" + this.agent.DDP + ".jpg";
  }
  downloadfile(i: any) {
    this.sharedService.getpicprofil(i.DDP).subscribe(data =>{
      this.imagePath='data:image/jpg;base64,'+data;

    })
  }
}
