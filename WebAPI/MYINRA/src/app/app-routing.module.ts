import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
//import {AvanceSalaireComponent} from "./avance-salaire/avance-salaire.component";
import { Show_etat_engagementComponent } from './etat_engagement/show_etat_engagement/show_etat_engagement.component';
import { Show_etat_primeComponent } from './etat_prime/show_etat_prime/show_etat_prime.component';
import { Show_etat_travailComponent } from './etat_travail/show_etat_travail/show_etat_travail.component';
import { Show_etat_revenuComponent } from './etat_revenu/show_etat_revenu/show_etat_revenu.component';
import { Show_salaire_rubriqueComponent } from './salaire_rubrique/show_salaire_rubrique/show_salaire_rubrique.component';
import { Show_etat_ccpComponent } from './etat_ccp/show_etat_ccp/show_etat_ccp.component';
import { Show_etat_precompteComponent } from './etat_precompte/show_etat_precompte/show_etat_precompte.component';
import { Show_etat_engagement_periodeComponent } from './etat_engagement_periode/show_etat_engagement_periode/show_etat_engagement_periode.component';
import { Show_etat_engagement_regroupeComponent } from './etat_engagement_regroupe/show_etat_engagement_regroupe/show_etat_engagement_regroupe.component';
import { Show_etat_liquidationComponent } from './etat_liquidation/show_etat_liquidation/show_etat_liquidation.component';
import { Show_banqueComponent } from './banque/show_banque/show_banque.component';
import { Show_arabeComponent } from './arabe/show_arabe/show_arabe.component';
import { Show_etat_domiciliationComponent } from './etat_domiciliation/show_etat_domiciliation/show_etat_domiciliation.component';
import { Show_main_leveeComponent } from './main_levee/show_main_levee/show_main_levee.component';
import { Show_etat_signatureComponent } from './etat_signature/show_etat_signature/show_etat_signature.component';
import { Show_banque_codeComponent } from './banque_code/show_banque_code/show_banque_code.component';
import { Show_documentComponent } from './document/show_document/show_document.component';
import { view_documentComponent } from './document/view_document/view_document.component';
import { Show_etat_ccp_suivi_demandesComponent } from './etat_ccp_suivi_demandes/show_etat_ccp_suivi_demandes/show_etat_ccp_suivi_demandes.component';
import { Show_etat_precompte_suivi_demandesComponent } from './etat_precompte_suivi_demandes/show_etat_precompte_suivi_demandes/show_etat_precompte_suivi_demandes.component';
import { Show_help_deskComponent } from './help_desk/show_help_desk/show_help_desk.component';
import { Show_help_desk_suiviComponent } from './help_desk_suivi/show_help_desk_suivi/show_help_desk_suivi.component';
import { Add_edit_profilComponent } from './profil/add_edit_profil/add_edit_profil.component';
import { Show_etat_engagement_suivi_demandesComponent } from './etat_engagement_suivi_demandes/show_etat_engagement_suivi_demandes/show_etat_engagement_suivi_demandes.component';


import { HomeComponent } from './foinra/components/home/home.component';
import { PresComponent } from './foinra/components/pres/pres.component';
import { UserComponent } from './user/user.component';
import { OutComponent } from './foinra/components/out/out.component';
import { FoinraManagementComponent } from './foinra/components/user-management/user-management.component';
import { MarcheBcComponent } from './foinra/components/marche-bc/marche-bc.component';
import { TokenComponent } from './foinra/components/token/token.component';
import { UserManagementComponent } from './user-management/user-management.component';
import { AuthGuard } from './guards/auth.guard';
import { Show_infos_banqueComponent } from './infos_banque/show_infos_banque/show_infos_banque.component';
import { Show_cinComponent } from './cin/show_cin/show_cin.component';
import { Show_main_levee_suivi_demandesComponent } from './main_levee_suivi_demandes/show_main_levee_suivi_demandes/show_main_levee_suivi_demandes.component';
import { Show_etat_domiciliation_gestionnaireComponent } from './etat_domiciliation/show_etat_domiciliation_gestionnaire/show_etat_domiciliation_gestionnaire.component';


const routes: Routes = [
  {path:'out',component:OutComponent},
  {path:'factures',component:HomeComponent,canActivate:[AuthGuard]},
  {path:'user',component:UserComponent,canActivate:[AuthGuard]},
  {path:'usermng',component:FoinraManagementComponent,canActivate:[AuthGuard]},
  {path:'users',component:UserManagementComponent,canActivate:[AuthGuard]},
  {path:'Marche',component:MarcheBcComponent,canActivate:[AuthGuard]},
  {path:'token',redirectTo:''},
  {path:'token/:token',component:TokenComponent,canActivate:[AuthGuard]},
  {path:'ETAT_ENG', component:Show_etat_engagementComponent,canActivate:[AuthGuard]},
  {path:'ETAT_ENG_PER', component:Show_etat_engagement_periodeComponent,canActivate:[AuthGuard]},
  {path:'ETAT_ENG_REG', component:Show_etat_engagement_regroupeComponent,canActivate:[AuthGuard]},
  {path:'ETAT_LIQ', component:Show_etat_liquidationComponent,canActivate:[AuthGuard]},
  {path:'ETAT_PRIME', component:Show_etat_primeComponent,canActivate:[AuthGuard]},
  {path:'ETAT_TRAVAIL', component:Show_etat_travailComponent,canActivate:[AuthGuard]},
  {path:'ETAT_REVENU', component:Show_etat_revenuComponent,canActivate:[AuthGuard]},
  {path:'SALAIRE_RUBRIQUE', component:Show_salaire_rubriqueComponent,canActivate:[AuthGuard]},
  {path:'CCP', component:Show_etat_ccpComponent,canActivate:[AuthGuard]},
  {path:'PRECOMPTE', component:Show_etat_precompteComponent,canActivate:[AuthGuard]},
  {path:'BANQUE', component:Show_banqueComponent,canActivate:[AuthGuard]},
  {path:'ARABE', component:Show_arabeComponent,canActivate:[AuthGuard]},
  {path:'ETAT_DOM', component:Show_etat_domiciliationComponent,canActivate:[AuthGuard]},
  {path:'MAIN_LEVEE', component:Show_main_leveeComponent,canActivate:[AuthGuard]},
  {path:'SIGNATURE', component:Show_etat_signatureComponent,canActivate:[AuthGuard]},
  {path:'CODE_BANQUE', component:Show_banque_codeComponent,canActivate:[AuthGuard]},
  {path:'LISTE_DOM_GEST', component:Show_etat_domiciliation_gestionnaireComponent,canActivate:[AuthGuard]},
  {path:'DOCUMENT', component:Show_documentComponent,canActivate:[AuthGuard]},
  {path:'SUIVI_CCP', component:Show_etat_ccp_suivi_demandesComponent,canActivate:[AuthGuard]},
  {path:'SUIVI_PRECOMPTE', component:Show_etat_precompte_suivi_demandesComponent,canActivate:[AuthGuard]},
  {path:'SUIVI_ENG_REG', component:Show_etat_engagement_suivi_demandesComponent,canActivate:[AuthGuard]},
  {path:'HELPDESK', component:Show_help_deskComponent,canActivate:[AuthGuard]},
  {path:'HELPDESKSUIVI', component:Show_help_desk_suiviComponent,canActivate:[AuthGuard]},
  {path:'INFOS_BANQUE', component:Show_infos_banqueComponent,canActivate:[AuthGuard]},
  {path:'', component:Add_edit_profilComponent,canActivate:[AuthGuard]},
  {path:'DOCS_VIEW', component:view_documentComponent,canActivate:[AuthGuard]},
  {path:'CIN', component:Show_cinComponent,canActivate:[AuthGuard]},
  {path:'SUIVI_MAIN', component:Show_main_levee_suivi_demandesComponent,canActivate:[AuthGuard]},
 ];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }



