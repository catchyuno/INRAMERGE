using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;
using Spire.Doc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Document = Spire.Doc.Document;
using DocXToPdfConverter;
using System.Data.SqlClient;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtatEngController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public EtatEngController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // LISTE DES DEMANDES DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeDemandes")] 
        public IActionResult ListeDemandes(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * from etat_engagement_demandes where DDP_DEMANDEUR =  @DDP_DEMANDEUR AND DDP =  @DDP ORDER BY DATE DESC", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // DROITS DE L'AGENT CONNECTE (PAIE)
        [HttpPost]
        [Route("CatégorieAgentPaie")]
        public IActionResult CatégorieAgentPaie(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var catégorie = myCon.Query("SELECT * from etats_droits where VOLET='PAIE' AND DDP =  @DDP order by ORDRE", Demande).ToList();
                //myCon.close();
                return Ok(catégorie);
            }
        }

        // DROITS DE L'AGENT CONNECTE (RH)
        [HttpPost]
        [Route("CatégorieAgentRH")]
        public IActionResult CatégorieAgentRH(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var catégorie = myCon.Query("SELECT * from etats_droits where VOLET='RH' AND DDP =  @DDP order by ORDRE", Demande).ToList();
                //myCon.close();
                return Ok(catégorie);
            }
        }

        // TEST ACCES
        [HttpPost]
        [Route("Test_ACCES")]
        public IActionResult Test_ACCES(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                DateTime thisDay = DateTime.Today;
                string DATE_TODAY = thisDay.ToString("yyyy-MM-dd");
                var VALIDATION = myCon.Query("SELECT * FROM myinra.acces_myinra WHERE DATE_DEBUT<='" + DATE_TODAY + "' AND DATE_FIN>='" + DATE_TODAY + "'", Demande).ToList();
                //myCon.close();
               return Ok(VALIDATION);
            }
          //  return new JsonResult("Vous devez mettre à jour vos données CIN !");
        }

        // TEST CIN
        [HttpPost]
        [Route("Test_CIN")]
        public IActionResult Test_CIN(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var VALIDATION = myCon.Query("SELECT * from cin_maj where DDP =  @DDP", Demande).ToList();
                //myCon.close();
                return Ok(VALIDATION);
            }
        }

        // DROITS DE L'AGENT CONNECTE (PARAMETRAGE)
        [HttpPost]
        [Route("Paramétrage")]
        public IActionResult Paramétrage(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var catégorie = myCon.Query("SELECT * from etats_droits where VOLET='PARAMETRAGE' AND DDP =  @DDP order by ORDRE", Demande).ToList();
                //myCon.close();
                return Ok(catégorie);
            }
        }

        // DROITS DE L'AGENT CONNECTE (SUIVI DES DEMANDES)
        [HttpPost]
        [Route("SuiviDemandes")]
        public IActionResult SuiviDemandes(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var catégorie = myCon.Query("SELECT * from etats_droits where VOLET='SUIVI_DEMANDES' AND DDP =  @DDP order by ORDRE", Demande).ToList();
                //myCon.close();
                return Ok(catégorie);
            }
        }

        // DROITS DE L'AGENT CONNECTE (ADMINISTRATION)
        [HttpPost]
        [Route("Administration")]
        public IActionResult Administration(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var catégorie = myCon.Query("SELECT * from etats_droits where VOLET='ADMINISTRATION' AND DDP =  @DDP order by ORDRE", Demande).ToList();
                //myCon.close();
                return Ok(catégorie);
            }
        }

        // Nombre de demandes à traiter par le gestionnaire
        [HttpPost]
        [Route("AlerteNbreCIN")]
        public IActionResult AlerteNbreCIN(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                int test2 = myCon.ExecuteScalar<int>("SELECT count(*) from cin_maj where VALIDE='NON'", Demande);
                //myCon.close();
                return Ok(test2);
            }
        }

        // Nombre de demandes à traiter par le gestionnaire
        [HttpPost]
        [Route("AlerteNbreCCP")]
        public IActionResult AlerteNbreCCP(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                int test2 = myCon.ExecuteScalar<int>("SELECT count(*) from etat_ccp_demandes where STATUT='EN COURS'", Demande);
                //myCon.close();
                return Ok(test2);
            }
        }

        // Nombre de demandes à traiter par le gestionnaire
        [HttpPost]
        [Route("AlerteNbreInfosBanque")]
        public IActionResult AlerteNbreInfosBanque(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                int test2 = myCon.ExecuteScalar<int>("SELECT count(*) from etat_domiciliation_infos where STATUT='EN COURS'", Demande);
                //myCon.close();
                return Ok(test2);
            }
        }
        // Nombre de demandes à traiter par le gestionnaire
        [HttpPost]
        [Route("AlerteNbrePrécompte")]
        public IActionResult AlerteNbrePrécompte(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                int test1 = myCon.ExecuteScalar<int>("SELECT count(*) from etat_precompte_demandes where STATUT='EN COURS'", Demande);
                //myCon.close();
                return Ok(test1);
            }
        }

        // Nombre de demandes à traiter par le gestionnaire
        [HttpPost]
        [Route("AlerteNbreEng")]
        public IActionResult AlerteNbreEng(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                int test = myCon.ExecuteScalar<int>("SELECT count(*) from etat_engagement_demandes_periode_regroupée where STATUT='EN COURS'", Demande);
                //myCon.close();
                return Ok(test);
            }
        }

        // Nombre de demandes à traiter par le gestionnaire
        [HttpPost]
        [Route("AlerteNbreTotal")]
        public IActionResult AlerteNbreTotal(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                int test1 = myCon.ExecuteScalar<int>("SELECT count(*) from etat_engagement_demandes_periode_regroupée where STATUT='EN COURS'", Demande);
                int test2 = myCon.ExecuteScalar<int>("SELECT count(*) from etat_domiciliation_main_levée where ETAT='EN COURS'", Demande);
                int test3= myCon.ExecuteScalar<int>("SELECT count(*) from etat_ccp_demandes where STATUT='EN COURS'", Demande);
                int test4 = myCon.ExecuteScalar<int>("SELECT count(*) from etat_precompte_demandes where STATUT='EN COURS'", Demande);
                int test5 = myCon.ExecuteScalar<int>("SELECT count(*) from etat_domiciliation_infos where STATUT='EN COURS'", Demande);
                int test6 = myCon.ExecuteScalar<int>("SELECT count(*) from cin_maj where VALIDE='NON'", Demande);
                //myCon.close();
                return Ok(test1+ test2+ test3+ test4 + test5 + test6);
            }
        }

        // Nombre de demandes à traiter par le gestionnaire
        [HttpPost]
        [Route("AlerteNbreMainLevee")]
        public IActionResult AlerteNbreMainLevee(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                int test = myCon.ExecuteScalar<int>("SELECT count(*) from etat_domiciliation_main_levée where ETAT='EN COURS'", Demande);
                //myCon.close();
                return Ok(test);
            }
        }
        // LISTE DES AGENTS SUR LISTE DEROULANTE SELON DROITS DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeAgents")] 
        public IActionResult ListeAgents(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var catégorie = myCon.Query("SELECT distinct(CATEGORIE) from etats_droits where MODULE='ETAT_ENG' AND DDP =  @DDP", Demande).ToList();
                foreach (var rows in catégorie)
                {
                    var fields = rows as IDictionary<string, object>;
                    var CATEGORIE = fields["CATEGORIE"];
                    if ((string)CATEGORIE == "AGENT")
                    {
                        var test = myCon.Query("SELECT a.DDP,a.NOM_PRENOM,b.CATEGORIE from etat_engagement_infos a JOIN etats_droits b ON a.DDP=b.DDP where a.DDP=@DDP AND b.MODULE='ETAT_ENG' GROUP BY a.DDP,a.NOM_PRENOM,b.CATEGORIE order by a.NOM_PRENOM", Demande).ToList();
                        return Ok(test);
                    }
                    if ((string)CATEGORIE == "ADMIN")
                    {
                        var test = myCon.Query("SELECT DDP, NOM_PRENOM from etat_engagement_infos GROUP BY DDP, NOM_PRENOM ORDER BY NOM_PRENOM", Demande).ToList();
                        return Ok(test);
                    }
                    if ((string)CATEGORIE == "HIERARCHIE")
                    {
                        var DIB_REQ = myCongespers.Query("SELECT * from PERSONEL WHERE DOTI=@DDP", Demande).ToList();
                        foreach (var ligne_dib in DIB_REQ)
                        {
                            var fields_mdib = ligne_dib as IDictionary<string, object>;
                            var DIB = fields_mdib["MDIB"];
                            int RESP_REQ = myCongespers.ExecuteScalar<int>("SELECT * from FONCRESPS WHERE MDIB='" + DIB + @"' AND DFIN IS NULL", Demande);
                            if (RESP_REQ != 0)
                            {
                                var AFF_REQ = myCongespers.Query("SELECT * from AFFECTATIONS WHERE MDIB='" + DIB + @"' AND DFIN IS NULL", Demande).ToList();
                                foreach (var ligne_aff in AFF_REQ)
                                {
                                    var fields_aff= ligne_aff as IDictionary<string, object>;
                                    var AFF = fields_aff["AFFECTATION"];
                                    var AGENT_REQ = myCongespers.Query("SELECT a.MDIB,a.DOTI as DDP,a.NOM as NOM_PRENOM from PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' GROUP BY a.MDIB,a.DOTI,a.NOM order by a.NOM", Demande).ToList();
                                    var AGENT_REQ_2 = myCongespers.Query("SELECT a.MDIB,a.DOTI as DDP,a.NOM as NOM_PRENOM from PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' GROUP BY a.MDIB,a.DOTI,a.NOM order by a.NOM", Demande).ToList();
                                    AGENT_REQ_2.Clear();
                                    foreach (var row_AGENT_REQ in AGENT_REQ)
                                    {
                                        var fields_AGENT_REQ = row_AGENT_REQ as IDictionary<string, object>;
                                        var DDP = fields_AGENT_REQ["DDP"];
                                        var NOM_PRENOM = fields_AGENT_REQ["NOM_PRENOM"];
                                        var DIB_V = fields_AGENT_REQ["MDIB"];
                                        var AGENT_REQ_3 = myCongespers.Query("SELECT TOP 1 AFFECTATION as AFF from AFFECTATIONS WHERE MDIB='" + DIB_V + @"' AND AFFECTATION<>'255' ORDER BY nligne DESC", Demande).ToList();
                                        foreach (var row_AGENT_REQ_3 in AGENT_REQ_3)
                                        {
                                            var fields_AGENT_REQ_3 = row_AGENT_REQ_3 as IDictionary<string, object>;
                                            var AFF_2 = fields_AGENT_REQ_3["AFF"];
                                            decimal AFF_2_V = decimal.Parse(AFF_2.ToString());
                                            decimal AFF_V = decimal.Parse(AFF.ToString());
                                            decimal DDP_CONNECTE = decimal.Parse(Demande.DDP.ToString());
                                            decimal DDP_V = decimal.Parse(DDP.ToString());
                                            if (AFF_2_V == AFF_V )
                                            {
                                                if (DDP.ToString().Length == 6)
                                                {
                                                    DDP = "0" + DDP;
                                                }
                                                if (DDP.ToString().Length == 5)
                                                {
                                                    DDP = "00" + DDP;
                                                }
                                                int EXISTE_REVENU = myCon.ExecuteScalar<int>("SELECT * from etat_engagement_infos where DDP='" + DDP + @"'", Demande);
                                                if (EXISTE_REVENU != 0)
                                                {
                                                    if (DDP.ToString().Length == 7)
                                                    {
                                                        AGENT_REQ_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM });
                                                    }
                                                    if (DDP.ToString().Length == 6)
                                                    {
                                                        DDP = "0" + DDP;
                                                        AGENT_REQ_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM });
                                                    }
                                                    if (DDP.ToString().Length == 5)
                                                    {
                                                        DDP = "00" + DDP;
                                                        AGENT_REQ_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    return Ok(AGENT_REQ_2);
                                }
                            }
                        }
                    }
                    if ((string)CATEGORIE == "CUSTOM")
                    {
                        var AGENT_REQ_TOTAL = myCon.Query("SELECT * from etats_droits_custom WHERE MODULE='ETAT_ENG' AND DDP=@DDP", Demande).ToList();
                        AGENT_REQ_TOTAL.Clear();
                        var AFF_REQ = myCon.Query("SELECT * from etats_droits_custom WHERE MODULE='ETAT_ENG' AND DDP=@DDP", Demande).ToList();
                        foreach (var ligne_aff in AFF_REQ)
                        {
                            var fields_aff = ligne_aff as IDictionary<string, object>;
                            var AFF = fields_aff["CODE_AFF"];
                            var AGENT_REQ = myCongespers.Query("SELECT a.MDIB,a.DOTI as DDP,a.NOM as NOM_PRENOM from PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' GROUP BY a.MDIB,a.DOTI,a.NOM order by a.NOM", Demande).ToList();
                            var AGENT_REQ_2 = myCongespers.Query("SELECT a.MDIB,a.DOTI as DDP,a.NOM as NOM_PRENOM from PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' GROUP BY a.MDIB,a.DOTI,a.NOM order by a.NOM", Demande).ToList();
                            AGENT_REQ_2.Clear();
                            foreach (var row_AGENT_REQ in AGENT_REQ)
                            {
                                var fields_AGENT_REQ = row_AGENT_REQ as IDictionary<string, object>;
                                var DDP = fields_AGENT_REQ["DDP"];
                                var NOM_PRENOM = fields_AGENT_REQ["NOM_PRENOM"];
                                var DIB_V = fields_AGENT_REQ["MDIB"];
                                var AGENT_REQ_3 = myCongespers.Query("SELECT TOP 1 AFFECTATION as AFF from AFFECTATIONS WHERE MDIB='" + DIB_V + @"' AND AFFECTATION<>'255' ORDER BY nligne DESC", Demande).ToList();
                                foreach (var row_AGENT_REQ_3 in AGENT_REQ_3)
                                {
                                    var fields_AGENT_REQ_3 = row_AGENT_REQ_3 as IDictionary<string, object>;
                                    var AFF_2 = fields_AGENT_REQ_3["AFF"];
                                    decimal AFF_2_V = decimal.Parse(AFF_2.ToString());
                                    decimal AFF_V = decimal.Parse(AFF.ToString());
                                    decimal DDP_CONNECTE = decimal.Parse(Demande.DDP.ToString());
                                    decimal DDP_V = decimal.Parse(DDP.ToString());
                                    if (AFF_2_V == AFF_V)
                                    {
                                        if (DDP.ToString().Length == 6)
                                        {
                                            DDP = "0" + DDP;
                                        }
                                        if (DDP.ToString().Length == 5)
                                        {
                                            DDP = "00" + DDP;
                                        }
                                        int EXISTE_REVENU = myCon.ExecuteScalar<int>("SELECT * from etat_engagement_infos where DDP='" + DDP + @"'", Demande);
                                        if (EXISTE_REVENU != 0)
                                        {
                                            if (DDP.ToString().Length == 7)
                                            {
                                                AGENT_REQ_TOTAL.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM });
                                            }
                                            if (DDP.ToString().Length == 6)
                                            {
                                                DDP = "0" + DDP;
                                                AGENT_REQ_TOTAL.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM });
                                            }
                                            if (DDP.ToString().Length == 5)
                                            {
                                                DDP = "00" + DDP;
                                                AGENT_REQ_TOTAL.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM });
                                            }
                                        }
                                    }
                                }

                            }

                        }
                        return Ok(AGENT_REQ_TOTAL);
                    }
                }
                //myCon.close();
                return Ok();
            }
        }

        // LISTE DES ANNEES INSCRITES SUR LA BASE POUR L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("Annee")] 
        public IActionResult ListeAnnees(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT distinct(ANNEE) from etat_engagement_infos where DDP =  @DDP order by ANNEE DESC", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES MOIS INSCRITS SUR LA BASE POUR L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE ET L'ANNEE SELECTIONNEE
        [HttpPost]
        [Route("Mois")] 
        public IActionResult ListeMois(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                int teste = myCon.ExecuteScalar<int>("SELECT COUNT(*) from etats_droits where CATEGORIE='GESTIONNAIRE' AND MODULE='RUBRIQUE' AND DDP=@DDP", Demande);
                if (teste != 0)
                {
                    var test = myCon.Query("Select distinct(MOIS) from etat_engagement_infos where ANNEE=@ANNEE and DDP=@DDP order by MOIS desc", Demande).ToList();
                    return Ok(test);
                }
                else
                {
                    var test = myCon.Query("Select distinct(MOIS) from etat_engagement_infos where ANNEE=@ANNEE and DDP=@DDP order by MOIS desc", Demande).ToList();
                    var AGENT_REQ_TOTAL = myCon.Query("Select distinct(MOIS) from etat_engagement_infos where ANNEE=@ANNEE and DDP=@DDP order by MOIS desc", Demande).ToList();
                    AGENT_REQ_TOTAL.Clear();
                    foreach (var row in test)
                    {
                        var fields = row as IDictionary<string, object>;
                        DateTime thisDay = DateTime.Today;
                        var MOIS_V = fields["MOIS"];
                        int MOIS_VV = int.Parse(MOIS_V.ToString());
                        if (MOIS_VV < 12)
                        {
                            MOIS_VV = MOIS_VV + 1;
                        }
                        string MOIS_VVV = (string)MOIS_VV.ToString();
                        if (MOIS_VV.ToString().Length == 1)
                        {
                            MOIS_VVV = "0" + MOIS_VVV;
                        }
                        String DATE_MOIS = "01/" + MOIS_VVV + "/" + Demande.ANNEE;
                        DateTime DATE_MOISS = DateTime.Parse(DATE_MOIS);
                        if (thisDay >= DATE_MOISS)
                        {
                            AGENT_REQ_TOTAL.Add(new { MOIS = MOIS_V });
                        }
                    }
                    //myCon.close();
                    return Ok(AGENT_REQ_TOTAL);
                }
            }
        }

        // TELECHARGEMENT DE L'ETAT D'ENGAGEMENT EN FORMAT (PDF)
        [HttpGet]
        [Route("file")] 
        public IActionResult getfile(string nom_file)
        {
            var f = System.IO.File.ReadAllBytes(@nom_file);
            return Ok(f);
        }

        // AJOUT DE LA DEMANDE DE L'ETAT D'ENGAGEMENT DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE ET GENERATION DE L'ETAT D'ENGAGEMENT
        [HttpPost]
        [Route("Ajout")] 
        public IActionResult Post(EtatEng Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string DATE_V = (string)Demande.DATE.ToString("yyyy-MM-dd");
                int teste = myCon.ExecuteScalar<int>("SELECT COUNT(*) from etat_engagement_demandes WHERE DATE='" + DATE_V + @"' AND DDP=@DDP AND DDP_DEMANDEUR=@DDP_DEMANDEUR AND ANNEE =  @ANNEE AND MOIS =  @MOIS AND TYPE=@TYPE", Demande);
                if (teste != 0)
                {
                    goto cleanup;
                }
                string fileName = "SALAIRE_DDP.docx";
                string sourcePath = @".\Etats\";
                string targetPath = @".\Etats\SORTIE\ETAT_ENG_MENSUEL";
                string fileoutput = (string)Demande.DDP + "_" + (string)Demande.ANNEE + "_" + (string)Demande.MOIS + "_" + Demande.DATE.ToString().Substring(0, 10).Replace("/","-") + "_" + (string)Demande.TYPE.Substring(0, 1) + " (" + (string)Demande.DDP_DEMANDEUR + ")"; 
                string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                string destFile = System.IO.Path.Combine(targetPath, fileoutput + ".docx");
                System.IO.Directory.CreateDirectory(targetPath);
                System.IO.File.Copy(sourceFile, destFile, true);
                Document doc = new Document();
                doc.LoadFromFile(destFile);
                var test = myCon.Query("Select * from etat_engagement_infos where ANNEE=@ANNEE and MOIS=@MOIS and DDP=@DDP order by MOIS desc", Demande).ToList();
                foreach (var rows in test)
                {
                    var fields = rows as IDictionary<string, object>;
                    var ANNEE = fields["ANNEE"];
                    var MOIS = fields["MOIS"];
                    var DDP = fields["DDP"];
                    var NOM_PRENOM = fields["NOM_PRENOM"];
                    var CIN = fields["CIN"];
                    var SEXE = fields["SEXE"];
                    var DATE_NAISSANCE = fields["DATE_NAISSANCE"];
                    var DATE_ENTREE = fields["DATE_ENTREE"];
                    var IMPUTATION = fields["IMPUTATION"];
                    var RESIDENCE = fields["RESIDENCE"];
                    var BASE_IMPOSABLE = fields["BASE_IMPOSABLE"];
                    var BRUT = fields["BRUT"];
                    var NET = fields["NET"];
                    var GRADE = fields["GRADE"];
                    var ECHELLE = fields["ECHELLE"];
                    var ECHELON = fields["ECHELON"];
                    var INDICE = fields["INDICE"];
                    var ZONE = fields["ZONE"];
                    var SITUATION_FAMILIALE = fields["SITUATION_FAMILIALE"];
                    var NBRE_ENFANTS = fields["NBRE_ENFANTS"];
                    var DEDUCTION = fields["DEDUCTION"];
                    var NATIONALITE = fields["NATIONALITE"];
                    doc.Replace("<DATE>", (string)DateTime.Now.ToString("dd/MM/yyyy"), true, true);
                    doc.Replace("<ANNEE>", (string)ANNEE, true, true);
                    doc.Replace("<MOIS>", (string)MOIS, true, true);
                    doc.Replace("<DDP>", (string)DDP, true, true);
                    doc.Replace("<NOM_PRENOM>", (string)NOM_PRENOM, true, true);
                    doc.Replace("<CIN>", (string)CIN, true, true);
                    doc.Replace("<SEXE>", (string)SEXE, true, true);
                    doc.Replace("<NAISSANCE>", (string)DATE_NAISSANCE, true, true);
                    doc.Replace("<ENTREE>", DATE_ENTREE.ToString().Substring(0,10), true, true);
                    doc.Replace("<IMPUTATION>", (string)IMPUTATION, true, true);
                    doc.Replace("<RESIDENCE>", (string)RESIDENCE, true, true);
                    decimal BASE_IMP_ARRONDI = decimal.Parse(BASE_IMPOSABLE.ToString());
                    decimal BRUT_MEN_ARRONDI = decimal.Parse(BRUT.ToString());
                    doc.Replace("<BASE_IMP>", BASE_IMP_ARRONDI.ToString("#,0.00"), true, true);
                    doc.Replace("<BRUT_MEN>", BRUT_MEN_ARRONDI.ToString("#,0.00"), true, true);
                    doc.Replace("<GRADE>", (string)GRADE, true, true);
                    doc.Replace("<ECH>", (string)ECHELLE, true, true);
                    doc.Replace("<ECHL>", ECHELON.ToString(), true, true);
                    doc.Replace("<IND>", INDICE.ToString(), true, true);
                    doc.Replace("<ZONE>", (string)ZONE, true, true);
                    doc.Replace("<SIT_F>", (string)SITUATION_FAMILIALE, true, true);
                    doc.Replace("<ENF>", NBRE_ENFANTS.ToString(), true, true);
                    doc.Replace("<DED>", DEDUCTION.ToString(), true, true);
                    doc.Replace("<NATIONALITE>", (string)NATIONALITE, true, true);
                    int i = 1;
                    var ind = myCon.Query("Select * from etat_engagement_rubriques where ANNEE=@ANNEE and MOIS=@MOIS and DDP=@DDP and (CODE_RUBRIQUE like '1%' OR CODE_RUBRIQUE like '2%') order by CODE_RUBRIQUE", Demande).ToList();
                    foreach (var rowsind in ind)
                    {
                        var fieldsind = rowsind as IDictionary<string, object>;
                        var RUBRIQUE = fieldsind["RUBRIQUE"];
                        var MONTANT = fieldsind["MONTANT"];
                        doc.Replace("<IND_" + i + ">", (string)RUBRIQUE, true, true);
                        decimal MONTANT_ARRONDI = decimal.Parse(MONTANT.ToString());
                        doc.Replace("<MIND_" + i + ">", MONTANT_ARRONDI.ToString("#,0.00"), true, true);
                        i = i + 1;
                    }
                    for (int j = i; j < 16; j++)
                    {
                        doc.Replace("<IND_" + j + ">", "", true, true);
                        doc.Replace("<MIND_" + j + ">", "", true, true);
                    }
                    i = 1;
                    string REQUETE_RET = "";
                    if (Demande.TYPE == "AVEC CREDIT")
                    {
                        REQUETE_RET = "Select* from etat_engagement_rubriques where ANNEE = @ANNEE and MOIS = @MOIS and DDP = @DDP and CODE_RUBRIQUE like '4%' order by CODE_RUBRIQUE";
                    }
                    if (Demande.TYPE == "SANS CREDIT")
                    {
                        REQUETE_RET = "SELECT a.ANNEE,a.MOIS,a.DDP,a.CODE_RUBRIQUE,a.RUBRIQUE,a.MONTANT from etat_engagement_rubriques a left JOIN salaire_rubriques b ON a.CODE_RUBRIQUE=b.CODE_RUBRIQUE where a.ANNEE=@ANNEE and a.MOIS=@MOIS and DDP=@DDP and b.CATEGORIE='RET' and a.CODE_RUBRIQUE like '4%' order by a.CODE_RUBRIQUE";
                    }
                    decimal TOT_RET = 0;
                    var ret = myCon.Query(REQUETE_RET, Demande).ToList();
                    foreach (var rowsret in ret)
                    {
                        var fieldsret = rowsret as IDictionary<string, object>;
                        var RUBRIQUE = fieldsret["RUBRIQUE"];
                        var MONTANT = fieldsret["MONTANT"];
                        doc.Replace("<RET_" + i + ">", (string)RUBRIQUE, true, true);
                        decimal MONTANT_ARRONDI = decimal.Parse(MONTANT.ToString());
                        TOT_RET = MONTANT_ARRONDI + TOT_RET;
                        doc.Replace("<MRET_" + i + ">", MONTANT_ARRONDI.ToString("#,0.00"), true, true);
                        i = i + 1;
                    }
                    decimal NET_MENSUEL = BRUT_MEN_ARRONDI - TOT_RET;
                    decimal NET_MENSUEL_ARRONDI = decimal.Parse(NET_MENSUEL.ToString());
                    doc.Replace("<TOT_RET>", TOT_RET.ToString("#,0.00"), true, true);
                    doc.Replace("<NET_MENSUEL>", NET_MENSUEL_ARRONDI.ToString("#,0.00"), true, true);
                    for (int j = i; j < 16; j++)
                    {
                        doc.Replace("<RET_" + j + ">", "", true, true);
                        doc.Replace("<MRET_" + j + ">", "", true, true);
                        doc.SaveToFile(destFile, FileFormat.Docx2013);
                    }
                }
                using (WordprocessingDocument doc2 = WordprocessingDocument.Open(destFile, true))
                {
                    var body = doc2.MainDocumentPart.Document.Body;
                    foreach (var text in body.Descendants<Text>())
                    {
                        if (text.Text.Contains("Evaluation Warning: The document was created with Spire.Doc for .NET."))
                        {
                            text.Text = text.Text.Replace("Evaluation Warning: The document was created with Spire.Doc for .NET.", "");
                        }
                    }
                }
                string locationOfLibreOfficeSoffice = sourcePath + "\\" + "LibreOfficePortable\\App\\libreoffice\\program\\soffice.exe";
                var tes = new ReportGenerator(locationOfLibreOfficeSoffice);
                tes.Convert(destFile, targetPath + "\\" + fileoutput + ".pdf");
                System.IO.File.Delete(destFile);
                string CHEMIN = targetPath + "\\" + fileoutput + ".pdf";
                PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
                var filename = @".\Etats\SORTIE\ETAT_ENG_MENSUEL\" + fileoutput + ".pdf";
                PdfSharp.Pdf.PdfDocument inputDocument = PdfReader.Open(filename, PdfDocumentOpenMode.Modify);
                int count = inputDocument.PageCount;
                for (int idx = 0; idx < count; idx++)
                {
                    PdfPage page = inputDocument.Pages[idx];
                    XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);
                    XImage imageentete = XImage.FromFile(@".\Etats\PICS\EN_TETE.jpg");
                    gfx.DrawImage(imageentete, 0, 0, page.Width, 90);
                    DateTime thisDay = DateTime.Today;
                    string DATE_TODAY = thisDay.ToString("yyyy-MM-dd");
                    int testsignature = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etats_signataires WHERE DDP='" + Demande.DDP + @"' AND DDP_SIGNATAIRE_OBLIGATOIRE<>''", Demande);
                    if (testsignature != 0)
                    {
                        var testsig = myCon.Query("SELECT * FROM etats_signataires WHERE DDP='" + Demande.DDP + @"'", Demande).ToList();
                        foreach (var rows in testsig)
                        {
                            var fields = rows as IDictionary<string, object>;
                            var DDP = fields["DDP_SIGNATAIRE_OBLIGATOIRE"];
                            XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\FR\POUR_DIRECTEUR\" + DDP + ".jpg");
                            gfx.DrawImage(imagesigne, 0, page.Height - 240, page.Width, 110);
                        }
                        //XImage imagepied = XImage.FromFile(@".\Etats\PICS\PIED.jpg");
                        //gfx.DrawImage(imagepied, 0, page.Height - 80, page.Width, 80);
                    }
                    else
                    {
                        var testsig = myCon.Query("SELECT * FROM etats_signataires WHERE ABSENCE_DU>'" + DATE_TODAY + @"' OR ABSENCE_AU<'" + DATE_TODAY + @"' OR ABSENCE_DU IS NULL ORDER BY ORDRE LIMIT 1", Demande).ToList();
                        foreach (var rows in testsig)
                        {
                            var fields = rows as IDictionary<string, object>;
                            var DDP = fields["DDP"];
                            XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\FR\POUR_DIRECTEUR\" + DDP + ".jpg");
                            gfx.DrawImage(imagesigne, 0, page.Height - 240, page.Width, 110);
                        }
                        //XImage imagepied = XImage.FromFile(@".\Etats\PICS\PIED.jpg");
                        //gfx.DrawImage(imagepied, 0, page.Height - 80, page.Width, 80);
                    }
                }
                inputDocument.Save(filename);
                var result =myCon.Query("Insert into etat_engagement_demandes values (@DATE,@DDP,@NOM_PRENOM, @ANNEE, @MOIS, @TYPE, 'TRAITE', @DDP_DEMANDEUR,@NOM_PRENOM_DEMANDEUR,@CHEMIN)", new { Demande.DATE, Demande.DDP, Demande.NOM_PRENOM, Demande.ANNEE, Demande.MOIS, Demande.TYPE, Demande.DDP_DEMANDEUR, Demande.NOM_PRENOM_DEMANDEUR, CHEMIN}).ToList();
                //myCon.close();
                return Ok(result);
            }
        cleanup: return new JsonResult("Etat d'engagement déjà éditée !");
        }
    }
}
