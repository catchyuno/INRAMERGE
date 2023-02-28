using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class MonProfil
    {
        
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string VALIDE { get; set; }
        public string MS { get; set; }
        public string SOURCE { get; set; }
        public string ENFANT { get; set; }
        public string CIN { get; set; }
        public string NOMAR { get; set; }
        public string ADRESSE { get; set; }
        public string GRADE { get; set; }
        public string nom_file { get; set; }

    }
    public class EtatEng
    {
        public DateTime DATE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string ANNEE { get; set; }
        public string MOIS { get; set; }
        public string TYPE { get; set; }
        public string DDP_DEMANDEUR { get; set; }
        public string NOM_PRENOM_DEMANDEUR { get; set; }
        // public string NOM_PRENOM { get; set; }
    }

    public class EtatEngagementRegroup
    {
        public DateTime DATE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string DU { get; set; }
        public string AU { get; set; }
        public string STATUT { get; set; }
        public string DDP_DEMANDEUR { get; set; }
        public string NOM_PRENOM_DEMANDEUR { get; set; }
        // public string NOM_PRENOM { get; set; }
    }
    public class MainLevee
    {
        public DateTime DATE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string BANQUE { get; set; }
        public string RIB { get; set; }
        public string ETAT { get; set; }
        public string DDP_DEMANDEUR { get; set; }
        public string NOM_PRENOM_DEMANDEUR { get; set; }
        public string nom_file { get; set; }
        // public string NOM_PRENOM { get; set; }
    }
    public class EtatDom
    {
        public DateTime DATE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string DDP_DEMANDEUR { get; set; }
        public string NOM_PRENOM_DEMANDEUR { get; set; }
         public string BANQUE { get; set; }
    }
    public class EtatLiq
    {
        public DateTime DATE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string ANNEE { get; set; }
        public string MOIS { get; set; }
        public string ORDRE { get; set; }
        //public string MOIS_AU { get; set; }
        //public string ANNEE { get; set; }
        //public string MOIS { get; set; }
        // public string TYPE { get; set; }
        public string DDP_DEMANDEUR { get; set; }
        public string NOM_PRENOM_DEMANDEUR { get; set; }
        // public string NOM_PRENOM { get; set; }
    }
    public class EtatEngPer
    {
        public DateTime DATE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string ANNEE_DU { get; set; }
        public string ANNEE { get; set; }
        public string MOIS { get; set; }
        public string MOIS_DU { get; set; }
        public string ANNEE_AU { get; set; }
        public string MOIS_AU { get; set; }
        public string TYPE { get; set; }
        public string DDP_DEMANDEUR { get; set; }
        public string NOM_PRENOM_DEMANDEUR { get; set; }
        // public string NOM_PRENOM { get; set; }
    }
    public class EtatPrime
    {
        public DateTime DATE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string ANNEE { get; set; }
        public string PRIME { get; set; }
        public string DDP_DEMANDEUR { get; set; }
        public string NOM_PRENOM_DEMANDEUR { get; set; }
        // public string TYPE { get; set; }
        // public string NOM_PRENOM { get; set; }
    }

    public class EtatTravail
    {
        public DateTime DATE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string DDP_DEMANDEUR { get; set; }
        public string NOM_PRENOM_DEMANDEUR { get; set; }
        public string LANGUE { get; set; }
        // public string NOM_PRENOM { get; set; }
    }

    public class EtatRevenu
    {
        public DateTime DATE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string ANNEE { get; set; }
        //public string PRIME { get; set; }
        public string DDP_DEMANDEUR { get; set; }
        public string NOM_PRENOM_DEMANDEUR { get; set; }
        // public string TYPE { get; set; }
        // public string NOM_PRENOM { get; set; }
    }
    public class Arabe
    {
        public string CATEGORIE { get; set; }
        public string FRANCAIS_GESPERS { get; set; }
        public string FRANCAIS_M { get; set; }
        public string ARABE_M { get; set; }
        public string FRANCAIS_F { get; set; }
        public string ARABE_F { get; set; }
    }
    public class Banque
    {
        public string BANQUE_FR_ANCIEN { get; set; }
        public string BANQUE_FR { get; set; }
        public string BANQUE_AR { get; set; }

    }

    public class CodeBanque
    {
        //public string BANQUE_FR_ANCIEN { get; set; }
        public string BANQUE_FR { get; set; }
        public string BANQUE_AR { get; set; }
        public string CODE { get; set; }

    }

    public class Document
    {
        public string CATEGORIE { get; set; }
        public string INTITULE { get; set; }
        public string INTITULE_ANCIEN { get; set; }
        public string nom_file { get; set; }
        public string LINK { get; set; }
    }
    public class Signature
    {
        public string ORDRE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string ABSENCE_DU { get; set; }
        public string ABSENCE_AU { get; set; }
        public string MOTIF { get; set; }
        public string nom_file { get; set; }
        public string DDPSIG { get; set; }
        public string NOM_PRENOMSIG { get; set; }
        public string TYPE { get; set; }
    }

    public class Entete
    {
        public string nom_file { get; set; }
    }
    public class Pied
    {
        public string nom_file { get; set; }
    }
    public class EtatCCP
    {
        public DateTime DATE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string nom_file { get; set; }
        public string DDP_DEMANDEUR { get; set; }
        public string NOM_PRENOM_DEMANDEUR { get; set; }
    }
    public class EtatSuiviCCP
    {
        public DateTime DATE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string nom_file { get; set; }
        public string DDP_DEMANDEUR { get; set; }
        public string NOM_PRENOM_DEMANDEUR { get; set; }
        public string STATUT { get; set; }
    }

    public class MainLeveeSuivi
    {
        public DateTime DATE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string nom_file { get; set; }
        public string BANQUE { get; set; }
        public string RIB { get; set; }
        public string ETAT { get; set; }
    }
    public class InfosBanque
    {
        public DateTime DATE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string nom_file { get; set; }
        public string BANQUE { get; set; }
        public string RIB { get; set; }
        public string STATUT { get; set; }

    }

    public class CINN
    {
        //public DateTime DATE { get; set; }
        public string CIN { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string nom_file { get; set; }
        public string NOMAR { get; set; }
        //public string NOM_PRENOM_DEMANDEUR { get; set; }
        public string VALIDE { get; set; }
        public string ADRESSE { get; set; }
        //public string CODE_RUBRIQUE { get; set; }
        //public string RUBRIQUE_ABBREVEE { get; set; }
    }
    public class EtatSuiviPrecompte
    {
        public DateTime DATE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string nom_file { get; set; }
        public string DDP_DEMANDEUR { get; set; }
        public string NOM_PRENOM_DEMANDEUR { get; set; }
        public string STATUT { get; set; }
        public string TYPE { get; set; }
        public string CODE_RUBRIQUE { get; set; }
        public string RUBRIQUE_ABBREVEE { get; set; }
    }

    public class EtatSuiviEngagement
    {
        public DateTime DATE { get; set; }
        public string DU { get; set; }
        public string AU { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string nom_file { get; set; }
        public string DDP_DEMANDEUR { get; set; }
        public string NOM_PRENOM_DEMANDEUR { get; set; }
        public string STATUT { get; set; }
        public string TYPE { get; set; }
       // public string CODE_RUBRIQUE { get; set; }
       // public string RUBRIQUE_ABBREVEE { get; set; }
    }

    public class HelpDesk
    {
        public DateTime DATE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string nom_file { get; set; }
        public string VOLET { get; set; }
        public string MENU { get; set; }
        public string STATUT { get; set; }
        public string INTITULE { get; set; }
        public string INTITULE_ANCIEN { get; set; }
        public string DESCRIPTION { get; set; }
        public string DESCRIPTION_ANCIEN { get; set; }
        public string REPONSE { get; set; }
    }

    public class HelpDeskSuivi
    {
        public DateTime DATE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string nom_file { get; set; }
        public string VOLET { get; set; }
        public string MENU { get; set; }
        public string STATUT { get; set; }
        public string INTITULE { get; set; }
        public string INTITULE_ANCIEN { get; set; }
        public string DESCRIPTION { get; set; }
        public string DESCRIPTION_ANCIEN { get; set; }
        public string REPONSE { get; set; }
    }

    public class EtatPrecompte
    {
        public DateTime DATE { get; set; }
        public string DDP { get; set; }
        public string NOM_PRENOM { get; set; }
        public string nom_file { get; set; }
        public string DDP_DEMANDEUR { get; set; }
        public string NOM_PRENOM_DEMANDEUR { get; set; }
        public string TYPE { get; set; }
        public string STATUT { get; set; }
        public string CODE_RUBRIQUE { get; set; }
        public string RUBRIQUE_ABBREVEE { get; set; }
    }
    public class SalaireRubrique
    {
        //public DateTime DATE { get; set; }
        public string CODE_RUBRIQUE { get; set; }
        public string RUBRIQUE { get; set; }
        public string RUBRIQUE_ABBREVEE { get; set; }
        //public string PRIME { get; set; }
        public string CATEGORIE { get; set; }
        //public string NOM_PRENOM_DEMANDEUR { get; set; }
        // public string TYPE { get; set; }
        // public string NOM_PRENOM { get; set; }
    }
}
