import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgxPaginationModule } from 'ngx-pagination';
import { SelectDropDownModule } from 'ngx-select-dropdown';
import { NgxSpinnerModule } from "ngx-spinner";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FilePickerModule } from  'ngx-awesome-uploader';

// Material Form Controls
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatSliderModule } from '@angular/material/slider';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
// Material Navigation
import { MatMenuModule } from '@angular/material/menu';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
// Material Layout
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatListModule } from '@angular/material/list';
import { MatStepperModule } from '@angular/material/stepper';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTreeModule } from '@angular/material/tree';
// Material Buttons & Indicators
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatBadgeModule } from '@angular/material/badge';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatRippleModule, MAT_DATE_LOCALE } from '@angular/material/core';
// Material Popups & Modals
import { MatBottomSheetModule } from '@angular/material/bottom-sheet';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
// Material Data tables
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatMomentDateModule, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from '@angular/material-moment-adapter';
import fr from '@angular/common/locales/fr-MA';



import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AvanceSalaireComponent } from './avance-salaire/avance-salaire.component';
import { ShowAvanceSalaireComponent } from './avance-salaire/show-avance-salaire/show-avance-salaire.component';


import { HttpClientModule } from "@angular/common/http";
import { SharedService } from "./shared.service";
import { FormsModule,ReactiveFormsModule } from "@angular/forms";
import { DatePipe, registerLocaleData } from '@angular/common';
import { AddEditAvanceSalaireComponent } from './avance-salaire/add-edit-avance-salaire/add-edit-avance-salaire.component'
import { Add_edit_etat_engagementComponent } from './etat_engagement/add_edit_etat_engagement/add_edit_etat_engagement.component'
import { Show_etat_engagementComponent } from './etat_engagement/show_etat_engagement/show_etat_engagement.component';
import { Etat_engagementComponent } from './etat_engagement/etat_engagement.component';

import { Add_edit_etat_liquidationComponent } from './etat_liquidation/add_edit_etat_liquidation/add_edit_etat_liquidation.component'
import { Show_etat_liquidationComponent } from './etat_liquidation/show_etat_liquidation/show_etat_liquidation.component';
import { Etat_liquidationComponent } from './etat_liquidation/etat_liquidation.component';

import { Add_edit_etat_engagement_periodeComponent } from './etat_engagement_periode/add_edit_etat_engagement_periode/add_edit_etat_engagement_periode.component'
import { Show_etat_engagement_periodeComponent } from './etat_engagement_periode/show_etat_engagement_periode/show_etat_engagement_periode.component';
import { Etat_engagement_periodeComponent } from './etat_engagement_periode/etat_engagement_periode.component';

import { Add_edit_etat_engagement_regroupeComponent } from './etat_engagement_regroupe/add_edit_etat_engagement_regroupe/add_edit_etat_engagement_regroupe.component'
import { Show_etat_engagement_regroupeComponent } from './etat_engagement_regroupe/show_etat_engagement_regroupe/show_etat_engagement_regroupe.component';
import { Etat_engagement_regroupeComponent } from './etat_engagement_regroupe/etat_engagement_regroupe.component';

import { Add_edit_etat_primeComponent } from './etat_prime/add_edit_etat_prime/add_edit_etat_prime.component'
import { Show_etat_primeComponent } from './etat_prime/show_etat_prime/show_etat_prime.component';
import { Etat_primeComponent } from './etat_prime/etat_prime.component';

import { Add_edit_etat_travailComponent } from './etat_travail/add_edit_etat_travail/add_edit_etat_travail.component'
import { Show_etat_travailComponent } from './etat_travail/show_etat_travail/show_etat_travail.component';
import { Etat_travailComponent } from './etat_travail/etat_travail.component';

import { Add_edit_etat_revenuComponent } from './etat_revenu/add_edit_etat_revenu/add_edit_etat_revenu.component'
import { Show_etat_revenuComponent } from './etat_revenu/show_etat_revenu/show_etat_revenu.component';
import { Etat_revenuComponent } from './etat_revenu/etat_revenu.component';

import { Add_edit_salaire_rubriqueComponent } from './salaire_rubrique/add_edit_salaire_rubrique/add_edit_salaire_rubrique.component'
 import { Show_salaire_rubriqueComponent } from './salaire_rubrique/show_salaire_rubrique/show_salaire_rubrique.component';
import { salaire_rubriqueComponent } from './salaire_rubrique/salaire_rubrique.component';

//import { Add_edit_etat_ccpComponent } from './etat_ccp/add_edit_etat_ccp/add_edit_etat_ccp.component'
import { Show_etat_ccpComponent } from './etat_ccp/show_etat_ccp/show_etat_ccp.component';
import { Etat_ccpComponent } from './etat_ccp/etat_ccp.component';

import { Add_edit_banqueComponent } from './banque/add_edit_banque/add_edit_banque.component'
import { Show_banqueComponent } from './banque/show_banque/show_banque.component';
import { banqueComponent } from './banque/banque.component';

import { Add_edit_arabeComponent } from './arabe/add_edit_arabe/add_edit_arabe.component'
import { Show_arabeComponent } from './arabe/show_arabe/show_arabe.component';
import { arabeComponent } from './arabe/arabe.component';

import { Add_edit_etat_domiciliationComponent } from './etat_domiciliation/add_edit_etat_domiciliation/add_edit_etat_domiciliation.component'
import { Show_etat_domiciliationComponent } from './etat_domiciliation/show_etat_domiciliation/show_etat_domiciliation.component';
import { Show_etat_domiciliation_gestionnaireComponent } from './etat_domiciliation/show_etat_domiciliation_gestionnaire/show_etat_domiciliation_gestionnaire.component';
import { Etat_domiciliationComponent } from './etat_domiciliation/etat_domiciliation.component';

import { Add_edit_main_leveeComponent } from './main_levee/add_edit_main_levee/add_edit_main_levee.component'
import { Show_main_leveeComponent } from './main_levee/show_main_levee/show_main_levee.component';
import { main_leveeComponent } from './main_levee/main_levee.component';

import { Add_edit_infos_banqueComponent } from './infos_banque/add_edit_infos_banque/add_edit_infos_banque.component'
import { Show_infos_banqueComponent } from './infos_banque/show_infos_banque/show_infos_banque.component';
import { infos_banqueComponent } from './infos_banque/infos_banque.component';


import { Add_edit_etat_signatureComponent } from './etat_signature/add_edit_etat_signature/add_edit_etat_signature.component'
import { Show_etat_signatureComponent } from './etat_signature/show_etat_signature/show_etat_signature.component';
import { etat_signatureComponent } from './etat_signature/etat_signature.component';
import { Add_edit_entete_piedComponent } from './etat_signature/add_edit_entete_pied/add_edit_entete_pied.component'

import { Add_edit_banque_codeComponent } from './banque_code/add_edit_banque_code/add_edit_banque_code.component'
import { Show_banque_codeComponent } from './banque_code/show_banque_code/show_banque_code.component';
import { banque_codeComponent } from './banque_code/banque_code.component';

import { Add_edit_documentComponent } from './document/add_edit_document/add_edit_document.component'
import { Show_documentComponent } from './document/show_document/show_document.component';
import { documentComponent } from './document/document.component';
import { view_documentComponent } from './document/view_document/view_document.component';

import { Add_edit_etat_precompteComponent } from './etat_precompte/add_edit_etat_precompte/add_edit_etat_precompte.component'
import { Show_etat_precompteComponent } from './etat_precompte/show_etat_precompte/show_etat_precompte.component';
import { Etat_precompteComponent } from './etat_precompte/etat_precompte.component';

import { Add_edit_etat_ccp_suivi_demandesComponent } from './etat_ccp_suivi_demandes/add_edit_etat_ccp_suivi_demandes/add_edit_etat_ccp_suivi_demandes.component'
import { Show_etat_ccp_suivi_demandesComponent } from './etat_ccp_suivi_demandes/show_etat_ccp_suivi_demandes/show_etat_ccp_suivi_demandes.component';
import { Etat_ccp_suivi_demandesComponent } from './etat_ccp_suivi_demandes/etat_ccp_suivi_demandes.component';

import { Add_edit_etat_precompte_suivi_demandesComponent } from './etat_precompte_suivi_demandes/add_edit_etat_precompte_suivi_demandes/add_edit_etat_precompte_suivi_demandes.component'
import { Show_etat_precompte_suivi_demandesComponent } from './etat_precompte_suivi_demandes/show_etat_precompte_suivi_demandes/show_etat_precompte_suivi_demandes.component';
import { Etat_precompte_suivi_demandesComponent } from './etat_precompte_suivi_demandes/etat_precompte_suivi_demandes.component';

import { Add_edit_etat_engagement_suivi_demandesComponent } from './etat_engagement_suivi_demandes/add_edit_etat_engagement_suivi_demandes/add_edit_etat_engagement_suivi_demandes.component'
import { Show_etat_engagement_suivi_demandesComponent } from './etat_engagement_suivi_demandes/show_etat_engagement_suivi_demandes/show_etat_engagement_suivi_demandes.component';
import { Etat_engagement_suivi_demandesComponent } from './etat_engagement_suivi_demandes/etat_engagement_suivi_demandes.component';

import { show_infos_help_deskComponent } from './help_desk/show_infos_help_desk/show_infos_help_desk.component'
import { Add_edit_help_deskComponent } from './help_desk/add_edit_help_desk/add_edit_help_desk.component'
import { Show_help_deskComponent } from './help_desk/show_help_desk/show_help_desk.component';
import { help_deskComponent } from './help_desk/help_desk.component';

import { show_infos_help_desk_suiviComponent } from './help_desk_suivi/show_infos_help_desk_suivi/show_infos_help_desk_suivi.component'
import { Add_edit_help_desk_suiviComponent } from './help_desk_suivi/add_edit_help_desk_suivi/add_edit_help_desk_suivi.component'
import { Show_help_desk_suiviComponent } from './help_desk_suivi/show_help_desk_suivi/show_help_desk_suivi.component';
import { help_desk_suiviComponent } from './help_desk_suivi/help_desk_suivi.component';

import { Add_edit_profilComponent } from './profil/add_edit_profil/add_edit_profil.component'
import { profilComponent } from './profil/profil.component'
import { show_gradesComponent } from './profil/show_grades/show_grades.component'
import { show_affectationsComponent } from './profil/show_affectations/show_affectations.component'
import { show_fonctionsComponent } from './profil/show_fonctions/show_fonctions.component'
import { show_sit_familialeComponent } from './profil/show_sit_familiale/show_sit_familiale.component'
import { Add_edit_profil_infosComponent } from './profil/add_edit_profil_infos/add_edit_profil_infos.component'

import { Add_edit_cinComponent } from './cin/add_edit_cin/add_edit_cin.component'
import { Show_cinComponent } from './cin/show_cin/show_cin.component';
import { cinComponent } from './cin/cin.component';

import { Add_edit_main_levee_suivi_demandesComponent } from './main_levee_suivi_demandes/add_edit_main_levee_suivi_demandes/add_edit_main_levee_suivi_demandes.component'
import { Show_main_levee_suivi_demandesComponent } from './main_levee_suivi_demandes/show_main_levee_suivi_demandes/show_main_levee_suivi_demandes.component';
import { main_levee_suivi_demandesComponent } from './main_levee_suivi_demandes/main_levee_suivi_demandes.component';


import { AuthService } from 'src/services/auth.service';
import { JwtModule } from '@auth0/angular-jwt';
import { MDBBootstrapModule } from 'angular-bootstrap-md';
import { LoginComponent } from './foinra/components/login/login.component';
import { HomeComponent } from './foinra/components/home/home.component';
import { NavbarComponent } from './foinra/components/navbar/navbar.component';
import { UserComponent } from './user/user.component';
import { OutComponent } from './foinra/components/out/out.component';
import { FoinraManagementComponent } from './foinra/components/user-management/user-management.component';
import { FactureComponent } from './foinra/components/facture/facture.component';
import { ProcessComponent } from './foinra/components/process/process.component';
import { HistoryComponent } from './foinra/components/history/history.component';
import { FournisseurComponent } from './foinra/components/fournisseur/fournisseur.component';
import { SignupComponent } from './foinra/components/signup/signup.component';
import { PresComponent } from './foinra/components/pres/pres.component';
import { MarcheBcComponent } from './foinra/components/marche-bc/marche-bc.component';
import { AddMBCComponent } from './foinra/components/add-mbc/add-mbc.component';
import { FilterComponent } from './foinra/components/filter/filter.component';
import { TokenComponent } from './foinra/components/token/token.component';
import { HotToastModule } from '@ngneat/hot-toast';
import { UserManagementComponent } from './user-management/user-management.component';

registerLocaleData(fr);

@NgModule({
  declarations: [
    AppComponent,
    AvanceSalaireComponent,
    ShowAvanceSalaireComponent,
    AddEditAvanceSalaireComponent,
      Etat_engagementComponent,
      Show_etat_engagementComponent,
      Add_edit_etat_engagementComponent,
      Etat_engagement_regroupeComponent,
      Show_etat_engagement_regroupeComponent,
      Add_edit_etat_engagement_regroupeComponent,
      Etat_engagement_periodeComponent,
      Show_etat_engagement_periodeComponent,
      Add_edit_etat_engagement_periodeComponent,
      Etat_primeComponent,
      Show_etat_primeComponent,
      Add_edit_etat_primeComponent,
      Etat_travailComponent,
      Show_etat_travailComponent,
      Add_edit_etat_travailComponent,
      Etat_revenuComponent,
      Show_etat_revenuComponent,
      Add_edit_etat_revenuComponent,
      salaire_rubriqueComponent,
      Show_salaire_rubriqueComponent,
      Add_edit_salaire_rubriqueComponent,
      Etat_ccpComponent,
      Show_etat_ccpComponent,
      Etat_liquidationComponent,
      Show_etat_liquidationComponent,
      Add_edit_etat_liquidationComponent,
      banqueComponent,
      Show_banqueComponent,
      Add_edit_banqueComponent,
      arabeComponent,
      Show_arabeComponent,
      Add_edit_arabeComponent,
      Add_edit_etat_domiciliationComponent,
      Etat_domiciliationComponent,
      Show_etat_domiciliationComponent,
      Show_etat_domiciliation_gestionnaireComponent,
      Add_edit_main_leveeComponent,
      main_leveeComponent,
      Show_main_leveeComponent,
      Add_edit_etat_signatureComponent,
      etat_signatureComponent,
      Show_etat_signatureComponent,
      Add_edit_banque_codeComponent,
      banque_codeComponent,
      Show_banque_codeComponent,
      Add_edit_documentComponent,
      documentComponent,
      Show_documentComponent,
      Add_edit_infos_banqueComponent,
      infos_banqueComponent,
      Show_infos_banqueComponent,
      view_documentComponent,
      Add_edit_etat_precompteComponent,
      Etat_precompteComponent,
      Show_etat_precompteComponent,
      Add_edit_etat_ccp_suivi_demandesComponent,
      Etat_ccp_suivi_demandesComponent,
      Show_etat_ccp_suivi_demandesComponent,
      Add_edit_etat_precompte_suivi_demandesComponent,
      Etat_precompte_suivi_demandesComponent,
      Show_etat_precompte_suivi_demandesComponent,
      Add_edit_etat_engagement_suivi_demandesComponent,
      Etat_engagement_suivi_demandesComponent,
      Show_etat_engagement_suivi_demandesComponent,
      help_deskComponent,
      Show_help_deskComponent,
      Add_edit_help_deskComponent,
      show_infos_help_deskComponent,
      help_desk_suiviComponent,
      Show_help_desk_suiviComponent,
      Add_edit_help_desk_suiviComponent,
      show_infos_help_desk_suiviComponent,
      profilComponent,
      Add_edit_profilComponent,
      Add_edit_profil_infosComponent,
      show_gradesComponent,
      show_affectationsComponent,
      show_fonctionsComponent,
      show_sit_familialeComponent,
      Add_edit_entete_piedComponent,
      Add_edit_cinComponent,
      cinComponent,
      Show_cinComponent,
      Add_edit_main_levee_suivi_demandesComponent,
      main_levee_suivi_demandesComponent,
      Show_main_levee_suivi_demandesComponent,
      Show_salaire_rubriqueComponent,
      LoginComponent,
      HomeComponent,
      NavbarComponent,
      UserComponent,
      OutComponent,
      UserManagementComponent,
      FactureComponent,
      ProcessComponent,
      HistoryComponent,
      FournisseurComponent,
      SignupComponent,
      PresComponent,
      MarcheBcComponent,
      AddMBCComponent,
      FilterComponent,
      FoinraManagementComponent,
      TokenComponent,
   ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    NgSelectModule,
    NgxPaginationModule,
    SelectDropDownModule,
    NgxSpinnerModule,
    BrowserAnimationsModule,
    FilePickerModule,
    JwtModule.forRoot({
      config:{
        tokenGetter:AuthService.mod
      }
    }),
    MatAutocompleteModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    MatRadioModule,
    MatSelectModule,
    MatSliderModule,
    MatSlideToggleModule,
    MatMenuModule,
    MatSidenavModule,
    MatToolbarModule,
    MatCardModule,
    MatDividerModule,
    MatExpansionModule,
    MatGridListModule,
    MatListModule,
    MatStepperModule,
    MatTabsModule,
    MatTreeModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatMomentDateModule,
    MatBadgeModule,
    MatChipsModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatProgressBarModule,
    MatRippleModule,
    MatBottomSheetModule,
    MatDialogModule,
    MatSnackBarModule,
    MatTooltipModule,
    MatPaginatorModule,
    MatSortModule,
    MatTableModule,
    MDBBootstrapModule.forRoot(),
    HotToastModule.forRoot({dismissible:true})
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  entryComponents:[LoginComponent,FactureComponent,ProcessComponent,HistoryComponent,FilterComponent,FournisseurComponent,AddMBCComponent],
  providers: [SharedService, DatePipe,{provide:MAT_DATE_LOCALE,useValue:'fr'},{ provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: { useUtc: true } }],
  bootstrap: [AppComponent]
})
export class AppModule { }
