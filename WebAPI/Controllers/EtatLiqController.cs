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
using System.Data;
using System.Data.SqlClient;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Drawing;
using System.Text;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Paragraph = Spire.Doc.Documents.Paragraph;
using PdfSharp.Drawing;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtatLiqController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public EtatLiqController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // LISTE DES DEMANDES DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeDemandes")]
        public IActionResult ListeDemandes(EtatLiq Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM etat_liquidation_demandes where DDP_DEMANDEUR =  @DDP_DEMANDEUR AND DDP =  @DDP ORDER BY DATE DESC", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES AGENTS SUR LISTE DEROULANTE SELON DROITS DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeAgents")]
        public IActionResult ListeAgents(EtatLiq Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var catégorie = myCon.Query("SELECT distinct(CATEGORIE) FROM etats_droits where MODULE='ETAT_LIQ' AND DDP =  @DDP", Demande).ToList();
                foreach (var rows in catégorie)
                {
                    var fields = rows as IDictionary<string, object>;
                    var CATEGORIE = fields["CATEGORIE"];
                    if ((string)CATEGORIE == "AGENT")
                    {
                        var test = myCon.Query("SELECT a.DDP,a.NOM_PRENOM,b.CATEGORIE FROM etat_liquidation_infos a JOIN etats_droits b ON a.DDP=b.DDP where a.DDP=@DDP AND b.MODULE='ETAT_LIQ' GROUP BY a.DDP,a.NOM_PRENOM,b.CATEGORIE order by a.NOM_PRENOM", Demande).ToList();
                        return Ok(test);
                    }
                    if ((string)CATEGORIE == "ADMIN")
                    {
                        var test = myCon.Query("SELECT DDP, NOM_PRENOM FROM etat_liquidation_infos GROUP BY DDP, NOM_PRENOM ORDER BY NOM_PRENOM", Demande).ToList();
                        return Ok(test);
                    }
                    if ((string)CATEGORIE == "HIERARCHIE")
                    {
                        var DIB_REQ = myCongespers.Query("SELECT * FROM PERSONEL WHERE DOTI=@DDP", Demande).ToList();
                        foreach (var ligne_dib in DIB_REQ)
                        {
                            var fields_mdib = ligne_dib as IDictionary<string, object>;
                            var DIB = fields_mdib["MDIB"];
                            int RESP_REQ = myCongespers.ExecuteScalar<int>("SELECT * FROM FONCRESPS WHERE MDIB='" + DIB + @"' AND DFIN IS NULL", Demande);
                            if (RESP_REQ != 0)
                            {
                                var AFF_REQ = myCongespers.Query("SELECT * FROM AFFECTATIONS WHERE MDIB='" + DIB + @"' AND DFIN IS NULL", Demande).ToList();
                                foreach (var ligne_aff in AFF_REQ)
                                {
                                    var fields_aff = ligne_aff as IDictionary<string, object>;
                                    var AFF = fields_aff["AFFECTATION"];
                                    var AGENT_REQ = myCongespers.Query("SELECT a.MDIB,a.DOTI as DDP,a.NOM as NOM_PRENOM FROM PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' GROUP BY a.MDIB,a.DOTI,a.NOM order by a.NOM", Demande).ToList();
                                    var AGENT_REQ_2 = myCongespers.Query("SELECT a.MDIB,a.DOTI as DDP,a.NOM as NOM_PRENOM FROM PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' GROUP BY a.MDIB,a.DOTI,a.NOM order by a.NOM", Demande).ToList();
                                    AGENT_REQ_2.Clear();
                                    foreach (var row_AGENT_REQ in AGENT_REQ)
                                    {
                                        var fields_AGENT_REQ = row_AGENT_REQ as IDictionary<string, object>;
                                        var DDP = fields_AGENT_REQ["DDP"];
                                        var NOM_PRENOM = fields_AGENT_REQ["NOM_PRENOM"];
                                        var DIB_V = fields_AGENT_REQ["MDIB"];
                                        var AGENT_REQ_3 = myCongespers.Query("SELECT TOP 1 AFFECTATION as AFF FROM AFFECTATIONS WHERE MDIB='" + DIB_V + @"' AND AFFECTATION<>'255' ORDER BY nligne DESC", Demande).ToList();
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
                                                int EXISTE_REVENU = myCon.ExecuteScalar<int>("SELECT * FROM etat_liquidation_infos where DDP='" + DDP + @"'", Demande);
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
                        var AGENT_REQ_TOTAL = myCon.Query("SELECT * FROM etats_droits_custom WHERE MODULE='ETAT_LIQ' AND DDP=@DDP", Demande).ToList();
                        AGENT_REQ_TOTAL.Clear();
                        var AFF_REQ = myCon.Query("SELECT * FROM etats_droits_custom WHERE MODULE='ETAT_LIQ' AND DDP=@DDP", Demande).ToList();
                        foreach (var ligne_aff in AFF_REQ)
                        {
                            var fields_aff = ligne_aff as IDictionary<string, object>;
                            var AFF = fields_aff["CODE_AFF"];
                            var AGENT_REQ = myCongespers.Query("SELECT a.MDIB,a.DOTI as DDP,a.NOM as NOM_PRENOM FROM PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' GROUP BY a.MDIB,a.DOTI,a.NOM order by a.NOM", Demande).ToList();
                            var AGENT_REQ_2 = myCongespers.Query("SELECT a.MDIB,a.DOTI as DDP,a.NOM as NOM_PRENOM FROM PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' GROUP BY a.MDIB,a.DOTI,a.NOM order by a.NOM", Demande).ToList();
                            AGENT_REQ_2.Clear();
                            foreach (var row_AGENT_REQ in AGENT_REQ)
                            {
                                var fields_AGENT_REQ = row_AGENT_REQ as IDictionary<string, object>;
                                var DDP = fields_AGENT_REQ["DDP"];
                                var NOM_PRENOM = fields_AGENT_REQ["NOM_PRENOM"];
                                var DIB_V = fields_AGENT_REQ["MDIB"];
                                var AGENT_REQ_3 = myCongespers.Query("SELECT TOP 1 AFFECTATION as AFF FROM AFFECTATIONS WHERE MDIB='" + DIB_V + @"' AND AFFECTATION<>'255' ORDER BY nligne DESC", Demande).ToList();
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
                                        int EXISTE_REVENU = myCon.ExecuteScalar<int>("SELECT * FROM etat_liquidation_infos where DDP='" + DDP + @"'", Demande);
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
        public IActionResult ListeAnnees(EtatLiq Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT distinct(ANNEE) FROM etat_liquidation_infos where DDP =  @DDP order by ANNEE DESC", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES MOIS INSCRITS SUR LA BASE POUR L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE ET L'ANNEE SELECTIONNEE
        [HttpPost]
        [Route("Mois")]
        public IActionResult ListeMois(EtatLiq Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("Select distinct(MOIS) from etat_liquidation_infos where ANNEE=@ANNEE and DDP=@DDP order by MOIS desc", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES PERIODES INSCRITES SUR LA BASE POUR L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE, L'ANNEE ET LE MOIS SELECTIONNEE
        [HttpPost]
        [Route("Periode")]
        public IActionResult Periode(EtatLiq Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("Select ORDRE, DATE_DEBUT_SITUATION, DATE_FIN_SITUATION from etat_liquidation_infos where ANNEE=@ANNEE and MOIS=@MOIS and DDP=@DDP order by ORDRE", Demande).ToList();
                var PERIODE_REQ_TOTAL=myCon.Query("Select ORDRE, DATE_DEBUT_SITUATION, DATE_FIN_SITUATION from etat_liquidation_infos where ANNEE=@ANNEE and MOIS=@MOIS and DDP=@DDP order by ORDRE", Demande).ToList();
                PERIODE_REQ_TOTAL.Clear();
                var ORDRE_V="";
                PERIODE_REQ_TOTAL.Add(new { ORDRE = "TOUS"});
                foreach (var rows in test)
                {
                    var fields = rows as IDictionary<string, object>;
                    var ORDRE = fields["ORDRE"];
                    int ORDR_V = int.Parse(ORDRE.ToString());

                    var DATE_DEBUT_SITUATION = fields["DATE_DEBUT_SITUATION"].ToString();
                    DATE_DEBUT_SITUATION = DATE_DEBUT_SITUATION.Substring(0, 10);
                    var DATE_FIN_SITUATION = fields["DATE_FIN_SITUATION"].ToString();
                    DATE_FIN_SITUATION = DATE_FIN_SITUATION.Substring(0, 10);
                    //var DATE_DEBUT_SITUATION = (string)fields["DATE_DEBUT_SITUATION"].ToString().Substring(6,4) + "/" + (string)fields["DATE_DEBUT_SITUATION"].ToString().Substring(3, 4) +"/"+ (string)fields["DATE_DEBUT_SITUATION"].ToString().Substring(0, 2);
                    //var DATE_FIN_SITUATION = (string)fields["DATE_FIN_SITUATION"];
                    if (ORDRE.ToString().Length == 1)
                    {
                        ORDRE_V = "00" + ORDRE + " - " + DATE_DEBUT_SITUATION + " - " + DATE_FIN_SITUATION;
                    }
                    else if (ORDRE.ToString().Length == 2)
                    {
                        ORDRE_V = "0" + ORDRE + " - " + DATE_DEBUT_SITUATION + " - " + DATE_FIN_SITUATION;
                    }
                    else
                    {
                        ORDRE_V = ORDRE + " - " + DATE_DEBUT_SITUATION + " - " + DATE_FIN_SITUATION;
                    }
                    PERIODE_REQ_TOTAL.Add(new { ORDRE = ORDRE_V });
                }
                //myCon.close();
                return Ok(PERIODE_REQ_TOTAL);
            }
        }

        // TELECHARGEMENT DE L'ETAT DE LIQUIDATION EN FORMAT (PDF)
        [HttpGet]
        [Route("file")]
        public IActionResult getfile(string nom_file)
        {
            var f = System.IO.File.ReadAllBytes(@nom_file);
            return Ok(f);
        }

        // AJOUT DE LA DEMANDE DE L'ETAT DE LIQUIDATION DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE ET GENERATION DE L'ETAT DE LIQUIDATION
        [HttpPost]
        [Route("Ajout")]
        public IActionResult Post(EtatLiq Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string DATE_V = (string)Demande.DATE.ToString("yyyy-MM-dd");
                int teste = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etat_liquidation_demandes WHERE DATE='" + DATE_V + @"' AND ORDRE=@ORDRE AND DDP=@DDP AND DDP_DEMANDEUR=@DDP_DEMANDEUR AND ANNEE =  @ANNEE AND MOIS =  @MOIS", Demande);
                if (teste != 0)
                {
                    goto cleanup;
                }
                string fileName = "LIQUIDATION.docx";
                string sourcePath = @".\Etats\";
                string targetPath = @".\Etats\SORTIE\ETAT_LIQ";
                string fileoutputglobal = (string)Demande.DDP + " _ " + (string)Demande.ORDRE.Replace("/","-") + " _ " + (string)Demande.ANNEE + "-" + (string)Demande.MOIS + " _ " + Demande.DATE.ToString().Substring(0, 10).Replace("/", "-") +  " (" + (string)Demande.DDP_DEMANDEUR + ")";
                string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                string[] files = new string[999];
                int m = 1;
                if (Demande.ORDRE=="TOUS")
                {
                     m = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etat_liquidation_infos WHERE ANNEE=@ANNEE and MOIS=@MOIS and DDP=@DDP order by ORDRE", Demande);
                }
                int ORD_CHOISI;
                for (int p = 0; p < m; p++)
                {
                    if (m != 1)
                    {
                        ORD_CHOISI = (p + 1);
                    }
                    else
                    {
                        if (m == 1 && Demande.ORDRE == "TOUS")
                        {
                            ORD_CHOISI = (p + 1);
                        }
                        else
                        {
                            ORD_CHOISI = int.Parse(Demande.ORDRE.Substring(0, 3));
                        }
                    }
                    //var test="";
                    // if (Demande.ORDRE == "TOUS")
                    //{
                    var test = myCon.Query("Select * from etat_liquidation_infos where ANNEE=@ANNEE and MOIS=@MOIS and DDP=@DDP AND  ORDRE='" + ORD_CHOISI + @"'", Demande).ToList();
                    //}
                    //else
                    //{
                    //   var  test = myCon.Query("Select * from etat_liquidation_infos where ANNEE=@ANNEE and MOIS=@MOIS and DDP=@DDP and ORDRE='" + decimal.Parse(Demande.ORDRE.Substring(0, 3)) + @"' order by ORDRE", Demande).ToList();
                    //}
                    //ANNEE_V = Demande.ANNEE;
                    //MOIS_V = Demande.MOIS;
                    string fileoutput = (string)Demande.DDP + " _ " + (string)Demande.ORDRE.Replace("/", "-") + " _ " + (string)Demande.ANNEE + "-" + (string)Demande.MOIS + " _ " + (p + 1) + " _ " + Demande.DATE.ToString().Substring(0, 10).Replace("/", "-") + " (" + (string)Demande.DDP_DEMANDEUR + ")";
                    System.IO.Directory.CreateDirectory(targetPath);
                    string destFile = System.IO.Path.Combine(targetPath, fileoutput + ".docx");
                    System.IO.File.Copy(sourceFile, destFile, true);
                    Document doc = new Document();
                    doc.LoadFromFile(destFile);
                    foreach (var rows in test)
                    {
                        var fields = rows as IDictionary<string, object>;
                        var ANNEE = fields["ANNEE"];
                        var MOIS = fields["MOIS"];
                        var DDP = fields["DDP"];
                        var NOM_PRENOM = fields["NOM_PRENOM"];
                        var CIN = fields["CIN"];
                        var SEXE = fields["SEXE"];
                        //var DATE_NAISSANCE = fields["DATE_NAISSANCE"];
                        var DUREE = fields["DUREE_SITUATION"];
                        var IMPUTATION = fields["IMPUTATION"];
                        var RESIDENCE = fields["RESIDENCE"];
                        var BASE_IMPOSABLE = fields["BASE_IMPOSABLE"];
                        var ORDRE = fields["ORDRE"];
                        var NET = fields["NOUVEAU_NET"];
                        var GRADE = fields["GRADE"];
                        var DU = fields["DATE_DEBUT_SITUATION"];
                        var AU = fields["DATE_FIN_SITUATION"];
                        var ECHELLE = fields["ECHELLE"];
                        var SAISISSABLE = fields["SAISISSABLE"];
                        var RAPPEL = fields["RAPPEL"];
                        var INDICE = fields["INDICE"];
                        var ZONE = fields["ZONE"];
                        //var STATUT = fields["STATUT"];
                        var SITUATION_FAMILIALE = fields["SITUATION_FAMILIALE"];
                        var NBRE_ENFANTS = fields["NBRE_ENFANTS"];
                        var DEDUCTION = fields["DEDUCTION"];
                       // var NATIONALITE = fields["NATIONALITE"];
                        var ORD = fields["ORDRE"];
                        var ANCIEN_NET = fields["ANCIEN_NET"];
                        var NOUVEAU_NET = fields["NOUVEAU_NET"];
                        var DIFFERENCE_NET = fields["DIFFERENCE_NET"];
                        doc.Replace("<DATE>", (string)DateTime.Now.ToString("dd/MM/yyyy"), true, true);
                        doc.Replace("<ANNEE>", (string)ANNEE, true, true);
                        doc.Replace("<MOIS>", (string)MOIS, true, true);
                        doc.Replace("<DDP>", (string)DDP, true, true);
                        doc.Replace("<NOM_PRENOM>", (string)NOM_PRENOM, true, true);
                        if ((string)CIN is null)
                        {
                            doc.Replace("<CIN>", "", true, true);
                        }
                        else
                        {
                            doc.Replace("<CIN>", (string)CIN, true, true);
                        }
                        doc.Replace("<SEXE>", (string)SEXE, true, true);
                        //if ((string)STATUT is null)
                        //{

                        //}
                        //else
                        //{
                        //    doc.Replace("<STATUT>", (string)STATUT, true, true);
                        //}
                        
                        doc.Replace("<IMPUTATION>", (string)IMPUTATION, true, true);
                        doc.Replace("<RESIDENCE>", (string)RESIDENCE, true, true);
                        decimal BASE_IMP_ARRONDI = decimal.Parse(BASE_IMPOSABLE.ToString());
                        decimal SAISISSABLE_ARRONDI = decimal.Parse(SAISISSABLE.ToString());
                        decimal NET_MENSUEL_ARRONDI = decimal.Parse(NET.ToString());
                        doc.Replace("<BASE_IMP>", BASE_IMP_ARRONDI.ToString("#,0.00"), true, true);
                        doc.Replace("<SAISISSABLE>", SAISISSABLE_ARRONDI.ToString("#,0.00"), true, true);
                        doc.Replace("<NET_MENSUEL>", NET_MENSUEL_ARRONDI.ToString("#,0.00"), true, true);
                        doc.Replace("<GRADE>", (string)GRADE, true, true);
                        if ((string)ECHELLE is null)
                        {
                            doc.Replace("<ECHELLE>", "", true, true);
                        }
                        else
                        {
                            doc.Replace("<ECHELLE>", (string)ECHELLE, true, true);
                        }
                        doc.Replace("<INDICE>", INDICE.ToString(), true, true);
                        doc.Replace("<ZONE>", (string)ZONE, true, true);
                        doc.Replace("<SIT_FAM>", (string)SITUATION_FAMILIALE, true, true);
                        doc.Replace("<ENF>", NBRE_ENFANTS.ToString(), true, true);
                        doc.Replace("<DED>", DEDUCTION.ToString(), true, true);
                        //if ((string)NATIONALITE is null)
                        //{
                        //    doc.Replace("<NAT>", (string)NATIONALITE, true, true);
                        //}
                        //else
                        //{
                        //    doc.Replace("<NAT>", (string)NATIONALITE, true, true);
                        //}
                        decimal ANCIEN_NET_ARRONDI = decimal.Parse(ANCIEN_NET.ToString());
                        decimal NOUVEAU_NET_ARRONDI = decimal.Parse(NOUVEAU_NET.ToString());
                        decimal DIFFERENCE_NET_ARRONDI = decimal.Parse(DIFFERENCE_NET.ToString());
                        doc.Replace("<TOT_ANC>", ANCIEN_NET_ARRONDI.ToString("#,0.00"), true, true);
                        doc.Replace("<TOT_NEW>", NOUVEAU_NET_ARRONDI.ToString("#,0.00"), true, true);
                        doc.Replace("<TOT_DIFF>", DIFFERENCE_NET_ARRONDI.ToString("#,0.00"), true, true);
                        doc.Replace("<NET>", NOUVEAU_NET_ARRONDI.ToString("#,0.00"), true, true);
                        decimal RAPPEL_ARRONDI = decimal.Parse(RAPPEL.ToString());
                        doc.Replace("<RAPPEL>", RAPPEL_ARRONDI.ToString("#,0.00"), true, true);
                        decimal DUREE_ARRONDI = decimal.Parse(DUREE.ToString());
                        doc.Replace("<DUREE>", DUREE_ARRONDI.ToString(), true, true);
                        string DU_V = (string)DU.ToString();
                        string AU_V = (string)AU.ToString();
                        doc.Replace("<DU>", (string)DU_V.Substring(0, 10), true, true);
                        doc.Replace("<AU>", (string)AU_V.Substring(0, 10), true, true);
                        decimal ORDRE_ARRONDI = decimal.Parse(ORDRE.ToString());
                        doc.Replace("<ORD>", ORDRE_ARRONDI.ToString(), true, true);
                        // doc.Replace("<CIN>", (string)CIN, true, true);
                        // }
                        int i = 1;
                        var ind = myCon.Query("Select * from etat_liquidation_rubriques where ANNEE=@ANNEE and MOIS=@MOIS and DDP=@DDP and ORDRE='" + ORDRE + @"'  AND (CODE_RUBRIQUE like '1%' OR CODE_RUBRIQUE like '2%') order by CODE_RUBRIQUE", Demande).ToList();
                        foreach (var rowsind in ind)
                        {
                            var fieldsind = rowsind as IDictionary<string, object>;
                            var CODE_RUBRIQUE = fieldsind["CODE_RUBRIQUE"];
                            var RUBRIQUE = fieldsind["RUBRIQUE"];
                            var ANCIEN_MONTANT = fieldsind["ANCIEN_MONTANT"];
                            if (ANCIEN_MONTANT is null)
                            {
                                ANCIEN_MONTANT = 0;
                            }
                            var NOUVEAU_MONTANT = fieldsind["NOUVEAU_MONTANT"];
                            if (NOUVEAU_MONTANT is null)
                            {
                                NOUVEAU_MONTANT = 0;
                            }
                            var DIFFERENCE_MONTANT = fieldsind["DIFFERENCE_MONTANT"];
                            if (DIFFERENCE_MONTANT is null)
                            {
                                DIFFERENCE_MONTANT = 0;
                            }
                            doc.Replace("<RUB_" + i + ">", (string)CODE_RUBRIQUE, true, true);
                            if ((string)RUBRIQUE is null)
                            {

                            }
                            else
                            {
                                doc.Replace("<LIB_" + i + ">", (string)RUBRIQUE, true, true);
                            }
                            decimal ANCIEN_MONTANT_ARRONDI = decimal.Parse(ANCIEN_MONTANT.ToString());
                            decimal NOUVEAU_MONTANT_ARRONDI = decimal.Parse(NOUVEAU_MONTANT.ToString());
                            decimal DIFFERENCE_MONTANT_ARRONDI = decimal.Parse(DIFFERENCE_MONTANT.ToString());
                            doc.Replace("<ANC_" + i + ">", ANCIEN_MONTANT_ARRONDI.ToString("#,0.00"), true, true);
                            doc.Replace("<NEW_" + i + ">", NOUVEAU_MONTANT_ARRONDI.ToString("#,0.00"), true, true);
                            doc.Replace("<DIFF_" + i + ">", DIFFERENCE_MONTANT_ARRONDI.ToString("#,0.00"), true, true);
                            i = i + 1;
                        }
                        var brut = myCon.Query("Select * from etat_liquidation_infos where ANNEE=@ANNEE and MOIS=@MOIS and DDP=@DDP and ORDRE='" + ORDRE+ @"'", Demande).ToList();
                        foreach (var rowsbrut in brut)
                        {
                            var fieldsbrut = rowsbrut as IDictionary<string, object>;
                            var ANCIEN_BRUT = fieldsbrut["ANCIEN_BRUT"];
                            var NOUVEAU_BRUT = fieldsbrut["NOUVEAU_BRUT"];
                            var DIFFERENCE_BRUT = fieldsbrut["DIFFERENCE_BRUT"];
                            doc.Replace("<RUB_" + i + ">", "", true, true);
                            doc.Replace("<LIB_" + i + ">", "BRUT", true, true);
                            decimal ANCIEN_BRUT_ARRONDI = decimal.Parse(ANCIEN_BRUT.ToString());
                            decimal NOUVEAU_BRUT_ARRONDI = decimal.Parse(NOUVEAU_BRUT.ToString());
                            decimal DIFFERENCE_BRUT_ARRONDI = decimal.Parse(DIFFERENCE_BRUT.ToString());
                            doc.Replace("<ANC_" + i + ">", ANCIEN_BRUT_ARRONDI.ToString("#,0.00"), true, true);
                            doc.Replace("<NEW_" + i + ">", NOUVEAU_BRUT_ARRONDI.ToString("#,0.00"), true, true);
                            doc.Replace("<DIFF_" + i + ">", DIFFERENCE_BRUT_ARRONDI.ToString("#,0.00"), true, true);
                            i = i + 1;
                        }
                        var ret = myCon.Query("Select * from etat_liquidation_rubriques where ANNEE=@ANNEE and MOIS=@MOIS and DDP=@DDP and ORDRE='" + ORDRE + @"'  AND (CODE_RUBRIQUE like '4%') order by CODE_RUBRIQUE", Demande).ToList();
                        foreach (var rowsret in ret)
                        {
                            var fieldsret = rowsret as IDictionary<string, object>;
                            var CODE_RUBRIQUE = fieldsret["CODE_RUBRIQUE"];
                            var RUBRIQUE = fieldsret["RUBRIQUE"];
                            var ANCIEN_MONTANT = fieldsret["ANCIEN_MONTANT"];
                            if (ANCIEN_MONTANT is null)
                            {
                                ANCIEN_MONTANT = 0;
                            }
                            var NOUVEAU_MONTANT = fieldsret["NOUVEAU_MONTANT"];
                            if (NOUVEAU_MONTANT is null)
                            {
                                NOUVEAU_MONTANT = 0;
                            }
                            var DIFFERENCE_MONTANT = fieldsret["DIFFERENCE_MONTANT"];
                            if (DIFFERENCE_MONTANT is null)
                            {
                                DIFFERENCE_MONTANT = 0;
                            }
                            //var NOUVEAU_MONTANT = fieldsret["NOUVEAU_MONTANT"];
                            //var DIFFERENCE_MONTANT = fieldsret["DIFFERENCE_MONTANT"];
                            doc.Replace("<RUB_" + i + ">", (string)CODE_RUBRIQUE, true, true);
                            if ((string)RUBRIQUE is null)
                            {

                            }
                            else
                            {
                                doc.Replace("<LIB_" + i + ">", (string)RUBRIQUE, true, true);
                            }
                            decimal ANCIEN_MONTANT_ARRONDI = decimal.Parse(ANCIEN_MONTANT.ToString());
                            decimal NOUVEAU_MONTANT_ARRONDI = decimal.Parse(NOUVEAU_MONTANT.ToString());
                            decimal DIFFERENCE_MONTANT_ARRONDI = decimal.Parse(DIFFERENCE_MONTANT.ToString());
                            doc.Replace("<ANC_" + i + ">", ANCIEN_MONTANT_ARRONDI.ToString("#,0.00"), true, true);
                            doc.Replace("<NEW_" + i + ">", NOUVEAU_MONTANT_ARRONDI.ToString("#,0.00"), true, true);
                            doc.Replace("<DIFF_" + i + ">", DIFFERENCE_MONTANT_ARRONDI.ToString("#,0.00"), true, true);
                            i = i + 1;
                        }
                        for (int j = i; j < 20; j++)
                        {
                            doc.Replace("<RUB_" + j + ">", "", true, true);
                            doc.Replace("<LIB_" + j + ">", "", true, true);
                            doc.Replace("<ANC_" + j + ">", "", true, true);
                            doc.Replace("<NEW_" + j + ">", "", true, true);
                            doc.Replace("<DIFF_" + j + ">", "", true, true);
                            doc.SaveToFile(destFile, Spire.Doc.FileFormat.Docx2013);
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
                        files[p] = targetPath + "\\" + fileoutput + ".pdf";
                    }
                }
                PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
                foreach (string file in files)
                {
                    if (file != null)
                    {
                        PdfSharp.Pdf.PdfDocument inputDocument1 = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                        int count1 = inputDocument1.PageCount;
                        for (int idx = 0; idx < count1; idx++)
                        {
                            PdfPage page = inputDocument1.Pages[idx];
                            outputDocument.AddPage(page);
                        }
                    }
                }
                string filename = targetPath + "\\" + fileoutputglobal + ".pdf";
                outputDocument.Save(filename);
                foreach (string file in files)
                {
                    if (file != null)
                    {
                        System.IO.File.Delete(file);
                    }
                }
                string CHEMIN = targetPath + "\\" + fileoutputglobal + ".pdf";
                PdfSharp.Pdf.PdfDocument outputDocument1 = new PdfSharp.Pdf.PdfDocument();
                var filename1 = @".\Etats\SORTIE\ETAT_LIQ\" + fileoutputglobal + ".pdf";
                PdfSharp.Pdf.PdfDocument inputDocument = PdfReader.Open(filename1, PdfDocumentOpenMode.Modify);
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
                            gfx.DrawImage(imagesigne, 0, page.Height - 200, page.Width, 110);
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
                            gfx.DrawImage(imagesigne, 0, page.Height - 200, page.Width, 110);
                        }
                        //XImage imagepied = XImage.FromFile(@".\Etats\PICS\PIED.jpg");
                        //gfx.DrawImage(imagepied, 0, page.Height - 80, page.Width, 80);
                    }   
                }
                inputDocument.Save(filename);
                var result = myCon.Query("Insert into etat_liquidation_demandes values (@DATE,@DDP,@NOM_PRENOM, @ANNEE, @MOIS, @ORDRE, 'TRAITE', @DDP_DEMANDEUR,@NOM_PRENOM_DEMANDEUR,@CHEMIN)", new { Demande.DATE, Demande.DDP, Demande.NOM_PRENOM, Demande.ANNEE, Demande.MOIS, Demande.ORDRE, Demande.DDP_DEMANDEUR, Demande.NOM_PRENOM_DEMANDEUR, CHEMIN }).ToList();
                //myCon.close();
                return Ok(result);
            }
        cleanup: return new JsonResult("Etat de liquidation déjà éditée !");
        }
    }
}
