

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Linq;
using WebAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;
using Spire.Doc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Document = Spire.Doc.Document;
using DocXToPdfConverter;
using System.Globalization;
using System.Data.SqlClient;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtatTravailController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public EtatTravailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // LISTE DES DEMANDES DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeDemandes")]
        public IActionResult ListeDemandes(EtatTravail Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM etat_travail_demandes where  DDP_DEMANDEUR =  @DDP_DEMANDEUR AND DDP =  @DDP ORDER BY DATE DESC", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES AGENTS SUR LISTE DEROULANTE SELON DROITS DE L'AGENT CONNECTE (AGENT / HIERARCHIE / ADMIN / GESTIONNAIRE)
        [HttpPost]
        [Route("ListeAgents")]
        public IActionResult ListeAgents(EtatTravail Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var catégorie = myCon.Query("SELECT distinct(CATEGORIE) FROM etats_droits where MODULE='ETAT_TRAVAIL' AND DDP =  @DDP", Demande).ToList();
                foreach (var rows in catégorie)
                {
                    var fields = rows as IDictionary<string, object>;
                    var CATEGORIE = fields["CATEGORIE"];
                    if ((string)CATEGORIE == "AGENT")
                    {
                        var test = myCongespers.Query("SELECT DOTI as DDP, NOM as NOM_PRENOM FROM PERSONEL WHERE DOTI=@DDP", Demande).ToList();
                        var test_2 = myCongespers.Query("SELECT DOTI as DDP, NOM as NOM_PRENOM FROM PERSONEL WHERE DOTI=@DDP", Demande).ToList();
                        test_2.Clear();
                        foreach (var row_AGENT_REQ in test)
                        {
                            var fields_AGENT_REQ = row_AGENT_REQ as IDictionary<string, object>;
                            var DDP = fields_AGENT_REQ["DDP"];
                            var NOM_PRENOM = fields_AGENT_REQ["NOM_PRENOM"];
                            if (DDP.ToString().Length == 7)
                            {
                                test_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM, CATEGORIE = CATEGORIE });
                            }
                            if (DDP.ToString().Length == 6)
                            {
                                DDP = "0" + DDP;
                                test_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM, CATEGORIE = CATEGORIE });
                            }
                            if (DDP.ToString().Length == 5)
                            {
                                DDP = "00" + DDP;
                                test_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM, CATEGORIE = CATEGORIE });
                            }
                        }
                        return Ok(test_2);
                    }
                    if ((string)CATEGORIE == "ADMIN")
                    {
                        decimal DDP_CONNECTE = decimal.Parse(Demande.DDP.ToString());
                        var test = myCongespers.Query("SELECT DOTI as DDP, NOM as NOM_PRENOM FROM PERSONEL WHERE DOTI is not null ORDER BY NOM", Demande).ToList();
                        var test_2 = myCongespers.Query("SELECT DOTI as DDP, NOM as NOM_PRENOM FROM PERSONEL WHERE DOTI is not null ORDER BY NOM", Demande).ToList();
                        test_2.Clear();
                        foreach (var row_AGENT_REQ in test)
                        {
                            var fields_AGENT_REQ = row_AGENT_REQ as IDictionary<string, object>;
                            var DDP = fields_AGENT_REQ["DDP"];
                            var NOM_PRENOM = fields_AGENT_REQ["NOM_PRENOM"];
                            if (DDP.ToString().Length == 7)
                            {
                                test_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM, CATEGORIE = CATEGORIE });
                            }
                            if (DDP.ToString().Length == 6)
                            {
                                DDP = "0" + DDP;
                                test_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM, CATEGORIE = CATEGORIE });
                            }
                            if (DDP.ToString().Length == 5)
                            {
                                DDP = "00" + DDP;
                                test_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM, CATEGORIE = CATEGORIE });
                            }
                        }
                        return Ok(test_2);
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
                                    var AGENT_REQ = myCongespers.Query("SELECT a.MDIB, a.DOTI as DDP,a.NOM as NOM_PRENOM FROM PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' GROUP BY a.MDIB, a.DOTI,a.NOM order by a.NOM", Demande).ToList();
                                    var AGENT_REQ_2 = myCongespers.Query("SELECT a.MDIB, a.DOTI as DDP,a.NOM as NOM_PRENOM FROM PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' GROUP BY a.MDIB, a.DOTI,a.NOM order by a.NOM", Demande).ToList();
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
                                            if (AFF_2_V == AFF_V)
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
                                    return Ok(AGENT_REQ_2);
                                }
                            }
                        }
                    }
                    if ((string)CATEGORIE == "CUSTOM")
                    {
                        var AGENT_REQ_TOTAL = myCon.Query("SELECT * FROM etats_droits_custom WHERE MODULE='ETAT_TRAVAIL' AND DDP=@DDP", Demande).ToList();
                        AGENT_REQ_TOTAL.Clear();
                        var AFF_REQ = myCon.Query("SELECT * FROM etats_droits_custom WHERE MODULE='ETAT_TRAVAIL' AND DDP=@DDP", Demande).ToList();
                        foreach (var ligne_aff in AFF_REQ)
                        {
                            var fields_aff = ligne_aff as IDictionary<string, object>;
                            var AFF = fields_aff["CODE_AFF"];
                            var AGENT_REQ = myCongespers.Query("SELECT a.MDIB, a.DOTI as DDP,a.NOM as NOM_PRENOM FROM PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' GROUP BY a.MDIB, a.DOTI,a.NOM order by a.NOM", Demande).ToList();
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
                                    if (AFF_2_V == AFF_V)
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
                        var AGENT_REQQ = myCon.Query("SELECT * FROM etat_engagement_infos where DDP='" + Demande.DDP + @"' GROUP BY DDP", Demande).ToList();
                        foreach (var row_AGENT_REQQ in AGENT_REQQ)
                        {
                            var fields_AGENT_REQQ = row_AGENT_REQQ as IDictionary<string, object>;
                            var NP = fields_AGENT_REQQ["NOM_PRENOM"];
                            AGENT_REQ_TOTAL.Add(new { DDP = Demande.DDP, NOM_PRENOM = NP });
                            return Ok(AGENT_REQ_TOTAL);
                        }
                    }
                    //myCon.close();
                }
                return Ok();
            }
        }

        // TELECHARGEMENT DE L'ETAT DE TRAVAIL EN FORMAT (PDF)
        [HttpGet]
        [Route("file")]
        public IActionResult getfile(string nom_file)
        {
            if (System.IO.File.Exists(@nom_file))
            {
                var f = System.IO.File.ReadAllBytes(@nom_file);
                return Ok(f);
            }
            else
            {
                return new JsonResult("Veuiller mettre à jour vos données !");
                //return Ok();
            }
        }

        // AJOUT DE LA DEMANDE DE L'ETAT DE TRAVAIL DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE ET GENERATION DE L'ETAT DE TRAVAIL
        [HttpPost]
        [Route("Ajout")]
        public IActionResult Post(EtatTravail Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string DATE_V = (string)Demande.DATE.ToString("yyyy-MM-dd");
                int teste = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etat_Travail_demandes WHERE DATE='" + DATE_V + @"' AND DDP_DEMANDEUR=@DDP_DEMANDEUR AND DDP=@DDP AND LANGUE =  @LANGUE", Demande);
                if (teste != 0)
                {
                    goto cleanup;
                }
                int teste_ordre = myCon.ExecuteScalar<int>("SELECT MAX(ORDRE) FROM etat_Travail_demandes", Demande);
                int ORDRE_V = teste_ordre+1;
                int teste2 = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etat_travail_infos WHERE DATE='" + DATE_V + @"' AND DDP=@DDP AND LANGUE =  @LANGUE", Demande);
                if (teste2 == 0)
                {
                    decimal DDP_CONNECTE = decimal.Parse(Demande.DDP.ToString());
                    if (Demande.LANGUE == "ARABE")
                    {
                        int test_AR = myCongespers.ExecuteScalar<int>("SELECT COUNT(*) FROM PERSONEL WHERE DOTI='" + DDP_CONNECTE + @"' AND NOMAR is null", Demande);
                        if (test_AR != 0)
                        {
                            return new JsonResult("Le nom en arabe n'est pas saisi, l'édition ne peut se faire !");
                            goto Sortir;
                        }
                    }
                    var test0 = myCongespers.Query("SELECT MDIB, POSADMIN, CIN, NOM, NOMAR, SEXE, DRECRUTE, DSORTIE, SITFAMILLE FROM PERSONEL WHERE DOTI='" + DDP_CONNECTE + @"'", Demande).ToList();
                    foreach (var rows0 in test0)
                    {
                        var fields = rows0 as IDictionary<string, object>;
                        var DIB = fields["MDIB"];
                        var CIN = fields["CIN"];
                        var SEXE = fields["SEXE"];
                        var NOM_PRENOM_V = "";
                        if (Demande.LANGUE == "FRANCAIS")
                        {
                            var NOM_PRENOM = fields["NOM"];
                            NOM_PRENOM_V = NOM_PRENOM.ToString().Replace("'", "_").Trim();
                        }
                        else
                        {
                            var NOM_PRENOM = fields["NOMAR"];
                            NOM_PRENOM_V = NOM_PRENOM.ToString();
                        }
                        var DATE_ENTREE = fields["DRECRUTE"];
                        var DATE_SORTIE = fields["DSORTIE"];
                        var POSITION = fields["POSADMIN"];
                        string DATE_SORTIE_V = "";
                        if (DATE_SORTIE == null)
                        {
                            DATE_SORTIE_V = "";
                        }
                        else
                        {
                            DATE_SORTIE_V = DATE_SORTIE.ToString().Substring(6, 4) + "-" + DATE_SORTIE.ToString().Substring(3, 2) + "-" + DATE_SORTIE.ToString().Substring(0, 2);
                        }
                        var SITUATION_FAM = fields["SITFAMILLE"];
                        string DATE_ENTREE_V = DATE_ENTREE.ToString().Substring(6, 4) + "-" + DATE_ENTREE.ToString().Substring(3, 2) + "-" + DATE_ENTREE.ToString().Substring(0, 2);
                        int teste3 = myCongespers.ExecuteScalar<int>("SELECT count(*) FROM FONCRESPS WHERE MDIB='" + DIB + @"' AND FONCRESP<>0 AND DFIN IS NULL", Demande);
                        var FONCTION_V = "";
                        if (teste3 == 0)
                        {
                            FONCTION_V = "";
                            var test4 = myCongespers.Query("SELECT TOP 1 AFFECTATION AS UNITE FROM AFFECTATIONS WHERE MDIB='" + DIB + @"' AND affectation<>255 ORDER BY DEFFET DESC", Demande).ToList();
                            foreach (var rows4 in test4)
                            {
                                var fields4 = rows4 as IDictionary<string, object>;
                                var UNITE = fields4["UNITE"];
                                var test5 = myCongespers.Query("SELECT CLASSE,LIBC FROM UNITES WHERE ORGANIGRAMME=2 AND UNITE='" + UNITE + @"'", Demande).ToList();
                                foreach (var rows5 in test5)
                                {
                                    var fields5 = rows5 as IDictionary<string, object>;
                                    var CLASSE = fields5["CLASSE"];
                                    var UNITE_FONCTION = fields5["LIBC"];
                                    var UNITE_FONCTION_V = UNITE_FONCTION.ToString().Replace("'", "_");
                                    var test6 = myCongespers.Query("SELECT TOP 1 LIBC FROM UNITES WHERE ORGANIGRAMME=2 AND CLASSE='" + CLASSE + @"' ORDER BY UNITE", Demande).ToList();
                                    foreach (var rows6 in test6)
                                    {
                                        var fields6 = rows6 as IDictionary<string, object>;
                                        var AFFECTATION = fields6["LIBC"];
                                        var AFFECTATION_V = AFFECTATION.ToString().Replace("'", "_");
                                        var test7 = myCongespers.Query("SELECT GRADE FROM AVANCEMENTS where MDIB='" + DIB + @"' AND DFIN IS NULL", Demande).ToList();
                                        foreach (var rows7 in test7)
                                        {
                                            var fields7 = rows7 as IDictionary<string, object>;
                                            var GRADE = fields7["GRADE"];
                                            var test8 = myCongespers.Query("SELECT LIBGRADE FROM GRADES WHERE GRADE='" + GRADE + @"'", Demande).ToList();
                                            foreach (var rows8 in test8)
                                            {
                                                var fields8 = rows8 as IDictionary<string, object>;
                                                var LIB_GRADE = fields8["LIBGRADE"];
                                                var LIB_GRADE_V = LIB_GRADE.ToString().Replace("'", "_");
                                                if (POSITION == null)
                                                {
                                                    var POSITION_ADM = "";
                                                    if (Demande.LANGUE == "FRANCAIS")
                                                    {
                                                        if ((string)SEXE == "F")
                                                        {
                                                            var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                            foreach (var rows_grade in test_grade)
                                                            {
                                                                var fields_grade = rows_grade as IDictionary<string, object>;
                                                                LIB_GRADE_V = (string)fields_grade["FRANCAIS_F"];
                                                            }
                                                        }
                                                        if ((string)SEXE == "M")
                                                        {
                                                            var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                            foreach (var rows_grade in test_grade)
                                                            {
                                                                var fields_grade = rows_grade as IDictionary<string, object>;
                                                                LIB_GRADE_V = (string)fields_grade["FRANCAIS_M"];
                                                            }
                                                        }
                                                        if ((string)SEXE == "F")
                                                        {
                                                            var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                            foreach (var rows_fonction in test_fonction)
                                                            {
                                                                var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                FONCTION_V = (string)fields_fonction["FRANCAIS_F"];
                                                            }
                                                        }
                                                        if ((string)SEXE == "M")
                                                        {
                                                            var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                            foreach (var rows_fonction in test_fonction)
                                                            {
                                                                var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                FONCTION_V = (string)fields_fonction["FRANCAIS_M"];
                                                            }
                                                        }
                                                        var test_unite = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + UNITE_FONCTION_V + @"'", Demande).ToList();
                                                        foreach (var rows_unite in test_unite)
                                                        {
                                                            var fields_unite = rows_unite as IDictionary<string, object>;
                                                            UNITE_FONCTION_V = (string)fields_unite["FRANCAIS_M"];
                                                        }
                                                        var test_aff = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + AFFECTATION_V + @"'", Demande).ToList();
                                                        foreach (var rows_aff in test_aff)
                                                        {
                                                            var fields_aff = rows_aff as IDictionary<string, object>;
                                                            AFFECTATION_V = (string)fields_aff["FRANCAIS_M"];
                                                        }
                                                    }
                                                    if (Demande.LANGUE == "ARABE")
                                                    {
                                                        if ((string)SEXE == "F")
                                                        {
                                                            var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                            foreach (var rows_grade in test_grade)
                                                            {
                                                                var fields_grade = rows_grade as IDictionary<string, object>;
                                                                LIB_GRADE_V = (string)fields_grade["ARABE_F"];
                                                            }
                                                        }
                                                        if ((string)SEXE == "M")
                                                        {
                                                            var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                            foreach (var rows_grade in test_grade)
                                                            {
                                                                var fields_grade = rows_grade as IDictionary<string, object>;
                                                                LIB_GRADE_V = (string)fields_grade["ARABE_M"];
                                                            }
                                                        }
                                                        if ((string)SEXE == "F")
                                                        {
                                                            var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                            foreach (var rows_fonction in test_fonction)
                                                            {
                                                                var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                FONCTION_V = (string)fields_fonction["ARABE_F"];
                                                            }
                                                        }
                                                        if ((string)SEXE == "M")
                                                        {
                                                            var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                            foreach (var rows_fonction in test_fonction)
                                                            {
                                                                var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                FONCTION_V = (string)fields_fonction["ARABE_M"];
                                                            }
                                                        }
                                                        var test_unite = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + UNITE_FONCTION_V + @"'", Demande).ToList();
                                                        foreach (var rows_unite in test_unite)
                                                        {
                                                            var fields_unite = rows_unite as IDictionary<string, object>;
                                                            UNITE_FONCTION_V = (string)fields_unite["ARABE_M"];
                                                        }
                                                        var test_aff = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + AFFECTATION_V + @"'", Demande).ToList();
                                                        foreach (var rows_aff in test_aff)
                                                        {
                                                            var fields_aff = rows_aff as IDictionary<string, object>;
                                                            AFFECTATION_V = (string)fields_aff["ARABE_M"];
                                                        }
                                                    }
                                                    string query = "Insert into etat_travail_infos values  ('" + ORDRE_V + "', '" + DATE_V + "', '" + Demande.DDP + "', '" + NOM_PRENOM_V + "','" + CIN + "','" + SEXE + "','" + SITUATION_FAM + "','" + DATE_ENTREE_V + "','" + DATE_SORTIE_V + "','" + POSITION_ADM + "','" + LIB_GRADE_V + "','" + FONCTION_V + "','" + UNITE_FONCTION_V + "','" + AFFECTATION_V + "','" + Demande.LANGUE + "')";
                                                    var result = myCon.Query(query).ToList();
                                                }
                                                else
                                                {
                                                    var test3 = myCongespers.Query("SELECT * FROM POSAD WHERE CODE='" + POSITION + @"'", Demande).ToList();
                                                    foreach (var rows3 in test3)
                                                    {
                                                        var fields3 = rows3 as IDictionary<string, object>;
                                                        var POSITION_ADM = fields3["POSADMIN"];
                                                        if (POSITION_ADM == null)
                                                        {
                                                            POSITION_ADM = "";
                                                        }
                                                        string query = "Insert into etat_travail_infos values  ('" + ORDRE_V + "', '" + DATE_V + "', '" + Demande.DDP + "', '" + NOM_PRENOM_V + "','" + CIN + "','" + SEXE + "','" + SITUATION_FAM + "','" + DATE_ENTREE_V + "','" + DATE_SORTIE_V + "','" + POSITION_ADM + "','" + LIB_GRADE_V + "','" + FONCTION_V + "','" + UNITE_FONCTION_V + "','" + AFFECTATION_V + "','" + Demande.LANGUE + "')";
                                                        var result = myCon.Query(query).ToList();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var test1 = myCongespers.Query("SELECT FONCRESP FROM FONCRESPS WHERE MDIB='" + DIB + @"' AND DFIN IS NULL", Demande).ToList();
                            foreach (var rows1 in test1)
                            {
                                var fields1 = rows1 as IDictionary<string, object>;
                                var FONCRESP = fields1["FONCRESP"];
                                var test2 = myCongespers.Query("SELECT LIBFONCR FROM FONCR WHERE Foncresp='" + FONCRESP + @"'", Demande).ToList();
                                foreach (var rows2 in test2)
                                {
                                    var fields2 = rows2 as IDictionary<string, object>;
                                    var FONCTION = fields2["LIBFONCR"];
                                    FONCTION_V = FONCTION.ToString().Replace("'", "_");
                                    var test4 = myCongespers.Query("SELECT TOP 1 AFFECTATION AS UNITE FROM AFFECTATIONS WHERE MDIB='" + DIB + @"' AND affectation<>255 ORDER BY DEFFET DESC", Demande).ToList();
                                    foreach (var rows4 in test4)
                                    {
                                        var fields4 = rows4 as IDictionary<string, object>;
                                        var UNITE = fields4["UNITE"];
                                        var test5 = myCongespers.Query("SELECT CLASSE,LIBC FROM UNITES WHERE ORGANIGRAMME=2 AND UNITE='" + UNITE + @"'", Demande).ToList();
                                        foreach (var rows5 in test5)
                                        {
                                            var fields5 = rows5 as IDictionary<string, object>;
                                            var CLASSE = fields5["CLASSE"];
                                            var UNITE_FONCTION = fields5["LIBC"];
                                            var UNITE_FONCTION_V = UNITE_FONCTION.ToString().Replace("'", "_");
                                            var test6 = myCongespers.Query("SELECT TOP 1 LIBC FROM UNITES WHERE ORGANIGRAMME=2 AND CLASSE='" + CLASSE + @"' ORDER BY UNITE", Demande).ToList();
                                            foreach (var rows6 in test6)
                                            {
                                                var fields6 = rows6 as IDictionary<string, object>;
                                                var AFFECTATION = fields6["LIBC"];
                                                var AFFECTATION_V = AFFECTATION.ToString().Replace("'", "_");
                                                if ((String)FONCTION == "DIRECTEUR" || (String)FONCTION == "SECRETAIRE GENERAL")
                                                {
                                                    var LIB_GRADE_V = "";
                                                    if (POSITION == null)
                                                    {
                                                        var POSITION_ADM = "";
                                                        if (Demande.LANGUE == "FRANCAIS")
                                                        {
                                                            if ((string)SEXE == "F")
                                                            {
                                                                var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                                foreach (var rows_grade in test_grade)
                                                                {
                                                                    var fields_grade = rows_grade as IDictionary<string, object>;
                                                                    LIB_GRADE_V = (string)fields_grade["FRANCAIS_F"];
                                                                }
                                                            }
                                                            if ((string)SEXE == "M")
                                                            {
                                                                var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                                foreach (var rows_grade in test_grade)
                                                                {
                                                                    var fields_grade = rows_grade as IDictionary<string, object>;
                                                                    LIB_GRADE_V = (string)fields_grade["FRANCAIS_M"];
                                                                }
                                                            }
                                                            if ((string)SEXE == "F")
                                                            {
                                                                var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                                foreach (var rows_fonction in test_fonction)
                                                                {
                                                                    var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                    FONCTION_V = (string)fields_fonction["FRANCAIS_F"];
                                                                }
                                                            }
                                                            if ((string)SEXE == "M")
                                                            {
                                                                var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                                foreach (var rows_fonction in test_fonction)
                                                                {
                                                                    var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                    FONCTION_V = (string)fields_fonction["FRANCAIS_M"];
                                                                }
                                                            }
                                                            var test_unite = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + UNITE_FONCTION_V + @"'", Demande).ToList();
                                                            foreach (var rows_unite in test_unite)
                                                            {
                                                                var fields_unite = rows_unite as IDictionary<string, object>;
                                                                UNITE_FONCTION_V = (string)fields_unite["FRANCAIS_M"];
                                                            }
                                                            var test_aff = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + AFFECTATION_V + @"'", Demande).ToList();
                                                            foreach (var rows_aff in test_aff)
                                                            {
                                                                var fields_aff = rows_aff as IDictionary<string, object>;
                                                                AFFECTATION_V = (string)fields_aff["FRANCAIS_M"];
                                                            }
                                                        }
                                                        if (Demande.LANGUE == "ARABE")
                                                        {
                                                            if ((string)SEXE == "F")
                                                            {
                                                                var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                                foreach (var rows_grade in test_grade)
                                                                {
                                                                    var fields_grade = rows_grade as IDictionary<string, object>;
                                                                    LIB_GRADE_V = (string)fields_grade["ARABE_F"];
                                                                }
                                                            }
                                                            if ((string)SEXE == "M")
                                                            {
                                                                var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                                foreach (var rows_grade in test_grade)
                                                                {
                                                                    var fields_grade = rows_grade as IDictionary<string, object>;
                                                                    LIB_GRADE_V = (string)fields_grade["ARABE_M"];
                                                                }
                                                            }
                                                            if ((string)SEXE == "F")
                                                            {
                                                                var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                                foreach (var rows_fonction in test_fonction)
                                                                {
                                                                    var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                    FONCTION_V = (string)fields_fonction["ARABE_F"];
                                                                }
                                                            }
                                                            if ((string)SEXE == "M")
                                                            {
                                                                var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                                foreach (var rows_fonction in test_fonction)
                                                                {
                                                                    var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                    FONCTION_V = (string)fields_fonction["ARABE_M"];
                                                                }
                                                            }
                                                            var test_unite = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + UNITE_FONCTION_V + @"'", Demande).ToList();
                                                            foreach (var rows_unite in test_unite)
                                                            {
                                                                var fields_unite = rows_unite as IDictionary<string, object>;
                                                                UNITE_FONCTION_V = (string)fields_unite["ARABE_M"];
                                                            }
                                                            var test_aff = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + AFFECTATION_V + @"'", Demande).ToList();
                                                            foreach (var rows_aff in test_aff)
                                                            {
                                                                var fields_aff = rows_aff as IDictionary<string, object>;
                                                                AFFECTATION_V = (string)fields_aff["ARABE_M"];
                                                            }
                                                        }
                                                        string query = "Insert into etat_travail_infos values  ('" + ORDRE_V + "', '" + DATE_V + "', '" + Demande.DDP + "', '" + NOM_PRENOM_V + "','" + CIN + "','" + SEXE + "','" + SITUATION_FAM + "','" + DATE_ENTREE_V + "','" + DATE_SORTIE_V + "','" + POSITION_ADM + "','" + LIB_GRADE_V + "','" + FONCTION_V + "','" + UNITE_FONCTION_V + "','" + AFFECTATION_V + "','" + Demande.LANGUE + "')";
                                                        var result = myCon.Query(query).ToList();
                                                    }
                                                    else
                                                    {
                                                        var test3 = myCongespers.Query("SELECT * FROM POSAD WHERE CODE='" + POSITION + @"'", Demande).ToList();
                                                        foreach (var rows3 in test3)
                                                        {
                                                            var fields3 = rows3 as IDictionary<string, object>;
                                                            var POSITION_ADM = fields3["POSADMIN"];
                                                            if (POSITION_ADM == null)
                                                            {
                                                                POSITION_ADM = "";
                                                            }
                                                            if (Demande.LANGUE == "FRANCAIS")
                                                            {
                                                                if ((string)SEXE == "F")
                                                                {
                                                                    var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                                    foreach (var rows_grade in test_grade)
                                                                    {
                                                                        var fields_grade = rows_grade as IDictionary<string, object>;
                                                                        LIB_GRADE_V = (string)fields_grade["FRANCAIS_F"];
                                                                    }
                                                                }
                                                                if ((string)SEXE == "M")
                                                                {
                                                                    var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                                    foreach (var rows_grade in test_grade)
                                                                    {
                                                                        var fields_grade = rows_grade as IDictionary<string, object>;
                                                                        LIB_GRADE_V = (string)fields_grade["FRANCAIS_M"];
                                                                    }
                                                                }
                                                                if ((string)SEXE == "F")
                                                                {
                                                                    var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                                    foreach (var rows_fonction in test_fonction)
                                                                    {
                                                                        var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                        FONCTION_V = (string)fields_fonction["FRANCAIS_F"];
                                                                    }
                                                                }
                                                                if ((string)SEXE == "M")
                                                                {
                                                                    var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                                    foreach (var rows_fonction in test_fonction)
                                                                    {
                                                                        var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                        FONCTION_V = (string)fields_fonction["FRANCAIS_M"];
                                                                    }
                                                                }
                                                                var test_unite = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + UNITE_FONCTION_V + @"'", Demande).ToList();
                                                                foreach (var rows_unite in test_unite)
                                                                {
                                                                    var fields_unite = rows_unite as IDictionary<string, object>;
                                                                    UNITE_FONCTION_V = (string)fields_unite["FRANCAIS_M"];
                                                                }
                                                                var test_aff = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + AFFECTATION_V + @"'", Demande).ToList();
                                                                foreach (var rows_aff in test_aff)
                                                                {
                                                                    var fields_aff = rows_aff as IDictionary<string, object>;
                                                                    AFFECTATION_V = (string)fields_aff["FRANCAIS_M"];
                                                                }
                                                            }
                                                            if (Demande.LANGUE == "ARABE")
                                                            {
                                                                if ((string)SEXE == "F")
                                                                {
                                                                    var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                                    foreach (var rows_grade in test_grade)
                                                                    {
                                                                        var fields_grade = rows_grade as IDictionary<string, object>;
                                                                        LIB_GRADE_V = (string)fields_grade["ARABE_F"];
                                                                    }
                                                                }
                                                                if ((string)SEXE == "M")
                                                                {
                                                                    var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                                    foreach (var rows_grade in test_grade)
                                                                    {
                                                                        var fields_grade = rows_grade as IDictionary<string, object>;
                                                                        LIB_GRADE_V = (string)fields_grade["ARABE_M"];
                                                                    }
                                                                }
                                                                if ((string)SEXE == "F")
                                                                {
                                                                    var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                                    foreach (var rows_fonction in test_fonction)
                                                                    {
                                                                        var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                        FONCTION_V = (string)fields_fonction["ARABE_F"];
                                                                    }
                                                                }
                                                                if ((string)SEXE == "M")
                                                                {
                                                                    var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                                    foreach (var rows_fonction in test_fonction)
                                                                    {
                                                                        var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                        FONCTION_V = (string)fields_fonction["ARABE_M"];
                                                                    }
                                                                }
                                                                var test_unite = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + UNITE_FONCTION_V + @"'", Demande).ToList();
                                                                foreach (var rows_unite in test_unite)
                                                                {
                                                                    var fields_unite = rows_unite as IDictionary<string, object>;
                                                                    UNITE_FONCTION_V = (string)fields_unite["ARABE_M"];
                                                                }
                                                                var test_aff = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + AFFECTATION_V + @"'", Demande).ToList();
                                                                foreach (var rows_aff in test_aff)
                                                                {
                                                                    var fields_aff = rows_aff as IDictionary<string, object>;
                                                                    AFFECTATION_V = (string)fields_aff["ARABE_M"];
                                                                }
                                                            }
                                                            string query = "Insert into etat_travail_infos values  ('" + ORDRE_V + "', '" + DATE_V + "', '" + Demande.DDP + "', '" + NOM_PRENOM_V + "','" + CIN + "','" + SEXE + "','" + SITUATION_FAM + "','" + DATE_ENTREE_V + "','" + DATE_SORTIE_V + "','" + POSITION_ADM + "','" + LIB_GRADE_V + "','" + FONCTION_V + "','" + UNITE_FONCTION_V + "','" + AFFECTATION_V + "','" + Demande.LANGUE + "')";
                                                            var result = myCon.Query(query).ToList();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    var test7 = myCongespers.Query("SELECT GRADE FROM AVANCEMENTS where MDIB='" + DIB + @"' AND DFIN IS NULL", Demande).ToList();
                                                    foreach (var rows7 in test7)
                                                    {
                                                        var fields7 = rows7 as IDictionary<string, object>;
                                                        var GRADE = fields7["GRADE"];
                                                        var test8 = myCongespers.Query("SELECT LIBGRADE FROM GRADES WHERE GRADE='" + GRADE + @"'", Demande).ToList();
                                                        foreach (var rows8 in test8)
                                                        {
                                                            var fields8 = rows8 as IDictionary<string, object>;
                                                            var LIB_GRADE = fields8["LIBGRADE"];
                                                            var LIB_GRADE_V = LIB_GRADE.ToString().Replace("'", "_");
                                                            if (POSITION == null)
                                                            {
                                                                var POSITION_ADM = "";
                                                                if (Demande.LANGUE == "FRANCAIS")
                                                                {
                                                                    if ((string)SEXE == "F")
                                                                    {
                                                                        var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                                        foreach (var rows_grade in test_grade)
                                                                        {
                                                                            var fields_grade = rows_grade as IDictionary<string, object>;
                                                                            LIB_GRADE_V = (string)fields_grade["FRANCAIS_F"];
                                                                        }
                                                                    }
                                                                    if ((string)SEXE == "M")
                                                                    {
                                                                        var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                                        foreach (var rows_grade in test_grade)
                                                                        {
                                                                            var fields_grade = rows_grade as IDictionary<string, object>;
                                                                            LIB_GRADE_V = (string)fields_grade["FRANCAIS_M"];
                                                                        }
                                                                    }
                                                                    if ((string)SEXE == "F")
                                                                    {
                                                                        var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                                        foreach (var rows_fonction in test_fonction)
                                                                        {
                                                                            var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                            FONCTION_V = (string)fields_fonction["FRANCAIS_F"];
                                                                        }
                                                                    }
                                                                    if ((string)SEXE == "M")
                                                                    {
                                                                        var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                                        foreach (var rows_fonction in test_fonction)
                                                                        {
                                                                            var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                            FONCTION_V = (string)fields_fonction["FRANCAIS_M"];
                                                                        }
                                                                    }
                                                                    var test_unite = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + UNITE_FONCTION_V + @"'", Demande).ToList();
                                                                    foreach (var rows_unite in test_unite)
                                                                    {
                                                                        var fields_unite = rows_unite as IDictionary<string, object>;
                                                                        UNITE_FONCTION_V = (string)fields_unite["FRANCAIS_M"];
                                                                    }
                                                                    var test_aff = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + AFFECTATION_V + @"'", Demande).ToList();
                                                                    foreach (var rows_aff in test_aff)
                                                                    {
                                                                        var fields_aff = rows_aff as IDictionary<string, object>;
                                                                        AFFECTATION_V = (string)fields_aff["FRANCAIS_M"];
                                                                    }
                                                                }
                                                                if (Demande.LANGUE == "ARABE")
                                                                {
                                                                    if ((string)SEXE == "F")
                                                                    {
                                                                        var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                                        foreach (var rows_grade in test_grade)
                                                                        {
                                                                            var fields_grade = rows_grade as IDictionary<string, object>;
                                                                            LIB_GRADE_V = (string)fields_grade["ARABE_F"];
                                                                        }
                                                                    }
                                                                    if ((string)SEXE == "M")
                                                                    {
                                                                        var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                                        foreach (var rows_grade in test_grade)
                                                                        {
                                                                            var fields_grade = rows_grade as IDictionary<string, object>;
                                                                            LIB_GRADE_V = (string)fields_grade["ARABE_M"];
                                                                        } 
                                                                    }
                                                                    if ((string)SEXE == "F")
                                                                    {
                                                                        var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                                        foreach (var rows_fonction in test_fonction)
                                                                        {
                                                                            var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                            FONCTION_V = (string)fields_fonction["ARABE_F"];
                                                                        }
                                                                    }
                                                                    if ((string)SEXE == "M")
                                                                    {
                                                                        var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                                        foreach (var rows_fonction in test_fonction)
                                                                        {
                                                                            var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                            FONCTION_V = (string)fields_fonction["ARABE_M"];
                                                                        }
                                                                    }
                                                                    var test_unite = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + UNITE_FONCTION_V + @"'", Demande).ToList();
                                                                    foreach (var rows_unite in test_unite)
                                                                    {
                                                                        var fields_unite = rows_unite as IDictionary<string, object>;
                                                                        UNITE_FONCTION_V = (string)fields_unite["ARABE_M"];
                                                                    }
                                                                    var test_aff = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + AFFECTATION_V + @"'", Demande).ToList();
                                                                    foreach (var rows_aff in test_aff)
                                                                    {
                                                                        var fields_aff = rows_aff as IDictionary<string, object>;
                                                                        AFFECTATION_V = (string)fields_aff["ARABE_M"];
                                                                    }
                                                                }
                                                                string query = "Insert into etat_travail_infos values  ('" + ORDRE_V + "', '" + DATE_V + "', '" + Demande.DDP + "', '" + NOM_PRENOM_V + "','" + CIN + "','" + SEXE + "','" + SITUATION_FAM + "','" + DATE_ENTREE_V + "','" + DATE_SORTIE_V + "','" + POSITION_ADM + "','" + LIB_GRADE_V + "','" + FONCTION_V + "','" + UNITE_FONCTION_V + "','" + AFFECTATION_V + "','" + Demande.LANGUE + "')";
                                                                var result = myCon.Query(query).ToList();
                                                            }
                                                            else
                                                            {
                                                                var test3 = myCongespers.Query("SELECT * FROM POSAD WHERE CODE='" + POSITION + @"'", Demande).ToList();
                                                                foreach (var rows3 in test3)
                                                                {
                                                                    var fields3 = rows3 as IDictionary<string, object>;
                                                                    var POSITION_ADM = fields3["POSADMIN"];
                                                                    if (POSITION_ADM == null)
                                                                    {
                                                                        POSITION_ADM = "";
                                                                    }
                                                                    if (Demande.LANGUE == "FRANCAIS")
                                                                    {
                                                                        if ((string)SEXE == "F")
                                                                        {
                                                                            var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                                            foreach (var rows_grade in test_grade)
                                                                            {
                                                                                var fields_grade = rows_grade as IDictionary<string, object>;
                                                                                LIB_GRADE_V = (string)fields_grade["FRANCAIS_F"];
                                                                            }
                                                                        }
                                                                        if ((string)SEXE == "M")
                                                                        {
                                                                            var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                                            foreach (var rows_grade in test_grade)
                                                                            {
                                                                                var fields_grade = rows_grade as IDictionary<string, object>;
                                                                                LIB_GRADE_V = (string)fields_grade["FRANCAIS_M"];
                                                                            }
                                                                        }
                                                                        if ((string)SEXE == "F")
                                                                        {
                                                                            var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                                            foreach (var rows_fonction in test_fonction)
                                                                            {
                                                                                var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                                FONCTION_V = (string)fields_fonction["FRANCAIS_F"];
                                                                            }
                                                                        }
                                                                        if ((string)SEXE == "M")
                                                                        {
                                                                            var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                                            foreach (var rows_fonction in test_fonction)
                                                                            {
                                                                                var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                                FONCTION_V = (string)fields_fonction["FRANCAIS_M"];
                                                                            }
                                                                        }
                                                                        var test_unite = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + UNITE_FONCTION_V + @"'", Demande).ToList();
                                                                        foreach (var rows_unite in test_unite)
                                                                        {
                                                                            var fields_unite = rows_unite as IDictionary<string, object>;
                                                                            UNITE_FONCTION_V = (string)fields_unite["FRANCAIS_M"];
                                                                        }
                                                                        var test_aff = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + AFFECTATION_V + @"'", Demande).ToList();
                                                                        foreach (var rows_aff in test_aff)
                                                                        {
                                                                            var fields_aff = rows_aff as IDictionary<string, object>;
                                                                            AFFECTATION_V = (string)fields_aff["FRANCAIS_M"];
                                                                        }
                                                                    }
                                                                    if (Demande.LANGUE == "ARABE")
                                                                    {
                                                                        if ((string)SEXE == "F")
                                                                        {
                                                                            var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                                            foreach (var rows_grade in test_grade)
                                                                            {
                                                                                var fields_grade = rows_grade as IDictionary<string, object>;
                                                                                LIB_GRADE_V = (string)fields_grade["ARABE_F"];
                                                                            }
                                                                        }
                                                                        if ((string)SEXE == "M")
                                                                        {
                                                                            var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE_V + @"'", Demande).ToList();
                                                                            foreach (var rows_grade in test_grade)
                                                                            {
                                                                                var fields_grade = rows_grade as IDictionary<string, object>;
                                                                                LIB_GRADE_V = (string)fields_grade["ARABE_M"];
                                                                            }
                                                                        }
                                                                        if ((string)SEXE == "F")
                                                                        {
                                                                            var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                                            foreach (var rows_fonction in test_fonction)
                                                                            {
                                                                                var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                                FONCTION_V = (string)fields_fonction["ARABE_F"];
                                                                            }
                                                                        }
                                                                        if ((string)SEXE == "M")
                                                                        {
                                                                            var test_fonction = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='FONCTION' AND FRANCAIS_GESPERS ='" + FONCTION_V + @"'", Demande).ToList();
                                                                            foreach (var rows_fonction in test_fonction)
                                                                            {
                                                                                var fields_fonction = rows_fonction as IDictionary<string, object>;
                                                                                FONCTION_V = (string)fields_fonction["ARABE_M"];
                                                                            }
                                                                        }
                                                                        var test_unite = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + UNITE_FONCTION_V + @"'", Demande).ToList();
                                                                        foreach (var rows_unite in test_unite)
                                                                        {
                                                                            var fields_unite = rows_unite as IDictionary<string, object>;
                                                                            UNITE_FONCTION_V = (string)fields_unite["ARABE_M"];
                                                                        }
                                                                        var test_aff = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='AFFECTATION' AND FRANCAIS_GESPERS ='" + AFFECTATION_V + @"'", Demande).ToList();
                                                                        foreach (var rows_aff in test_aff)
                                                                        {
                                                                            var fields_aff = rows_aff as IDictionary<string, object>;
                                                                            AFFECTATION_V = (string)fields_aff["ARABE_M"];
                                                                        }
                                                                    }
                                                                    string query = "Insert into etat_travail_infos values  ('" + ORDRE_V + "', '" + DATE_V + "', '" + Demande.DDP + "', '" + NOM_PRENOM_V + "','" + CIN + "','" + SEXE + "','" + SITUATION_FAM + "','" + DATE_ENTREE_V + "','" + DATE_SORTIE_V + "','" + POSITION_ADM + "','" + LIB_GRADE_V + "','" + FONCTION_V + "','" + UNITE_FONCTION_V + "','" + AFFECTATION_V + "','" + Demande.LANGUE + "')";
                                                                    var result = myCon.Query(query).ToList();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                string fileName = "";
                if (Demande.LANGUE == "FRANCAIS")
                {
                    fileName = "TRAVAIL_FR.docx";
                }
                if (Demande.LANGUE == "ARABE")
                {
                    fileName = "TRAVAIL_AR.docx";
                }
                string sourcePath = @".\Etats\";
                string targetPath = @".\Etats\SORTIE\ETAT_TRAVAIL";
                string fileoutput = (string)Demande.DDP + "_" + Demande.DATE.ToString().Substring(0, 10).Replace("/", "-") + "_" + (string)Demande.LANGUE.Substring(0, 2) + " (" + (string)Demande.DDP_DEMANDEUR + ")";
                string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                string destFile = System.IO.Path.Combine(targetPath, fileoutput + ".docx");
                System.IO.Directory.CreateDirectory(targetPath);
                System.IO.File.Copy(sourceFile, destFile, true);
                var test = myCon.Query("Select * from etat_travail_infos where DATE='" + DATE_V + @"' and LANGUE=@LANGUE and DDP=@DDP", Demande).ToList();
                foreach (var rows in test)
                {
                    var fields = rows as IDictionary<string, object>;
                    var DDP = fields["DDP"];
                    var INTERESSE = "";
                    var CIN = fields["CIN"];
                    var SEXE = fields["SEXE"];
                    var SITUATION_FAM = fields["SITUATION_FAM"];
                    var NOM_PRENOM = fields["NOM_PRENOM"];
                    var GRADE = fields["GRADE"];
                    NOM_PRENOM = NOM_PRENOM.ToString().Replace("_", "'");
                    if (Demande.LANGUE == "FRANCAIS")
                    {
                        if ((string)SEXE == "M")
                        {
                            NOM_PRENOM = "Monsieur " + NOM_PRENOM;
                            INTERESSE = "l'intéressé";
                        }
                        else
                        {
                            if ((string)SITUATION_FAM == "M")
                            {
                                NOM_PRENOM = "Madame " + NOM_PRENOM;
                                INTERESSE = "l'intéressée";
                            }
                            else
                            {
                                NOM_PRENOM = "Mademoiselle " + NOM_PRENOM;
                                INTERESSE = "l'intéressée";
                            }
                        }
                    }
                    var DEMANDEUR = "";
                    if (Demande.LANGUE == "ARABE")
                    {
                        if ((string)SEXE == "M")
                        {
                            NOM_PRENOM = "السيد " + NOM_PRENOM;
                            INTERESSE = "للمعني بالأمر";
                            DEMANDEUR = "منه";
                        }
                        else
                        {
                            if ((string)SITUATION_FAM == "M")
                            {
                                NOM_PRENOM = "السيدة " + NOM_PRENOM;
                                INTERESSE = "للمعنية بالأمر";
                                DEMANDEUR = "منها";
                            }
                            else
                            {
                                NOM_PRENOM = "الانسة " + NOM_PRENOM;
                                INTERESSE = "للمعنية بالأمر";
                                DEMANDEUR = "منها";
                            }
                        }
                    }
                    var MAINTIEN_RCAR = "";
                    int teste3 = myCongespers.ExecuteScalar<int>("SELECT count(*) FROM PERSONEL WHERE DOTI='" + Demande.DDP + @"' AND (Prolo1_du<>'' or Prolo2_du<>'')", Demande);
                    if (teste3 != 0)
                    {
                        MAINTIEN_RCAR = "OUI";
                    }
                    var DATE_ENTREE = fields["DATE_ENTREE"];
                    var DATE_SORTIE = fields["DATE_SORTIE"];
                    var MOTIF_SORTIE = fields["MOTIF_SORTIE"];
                    var FONCTION = fields["FONCTION"];
                    var UNITE_FONCTION = fields["UNITE_FONCTION"];
                    var AFFECTATION = fields["AFFECTATION"];
                    var TEXTE = "";
                    if (Demande.LANGUE == "FRANCAIS")
                    {
                        if ((string)DATE_SORTIE == "")
                        {
                            if ((string)FONCTION == "")
                            {
                                var test_texte = myCon.Query("Select * from etats_travail_corespondance_texte WHERE LANGUE='FR'", Demande).ToList();
                                foreach (var rows_texte in test_texte)
                                {
                                    var fields_texte = rows_texte as IDictionary<string, object>;
                                    TEXTE = (string)fields_texte["FONCTION_NON"];
                                    var test_unité = myCon.Query("Select * from etats_travail_corespondance_unité where LANGUE='FR' AND UNITE='" + AFFECTATION.ToString().Substring(0, 5) + "'", Demande).ToList();
                                    foreach (var rows_unité in test_unité)
                                    {
                                        var fields_unité = rows_unité as IDictionary<string, object>;
                                        var PREFIXE_UNITE = fields_unité["PREFIXE_UNITE"];
                                        TEXTE = TEXTE.ToString().Replace("#PREFIXE_UNITE#", PREFIXE_UNITE.ToString());
                                        TEXTE = TEXTE.ToString().Replace("#UNITE#", AFFECTATION.ToString().Replace("_", "'"));
                                        TEXTE = TEXTE.Trim();
                                    }
                                }
                            }
                            if ((string)FONCTION != "")
                            {
                                int test_cas_nbre = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etats_travail_corespondance_unité_cas_particuliers WHERE LANGUE='FR' AND UNITE='" + UNITE_FONCTION + "'", Demande);
                                if (test_cas_nbre != 0)
                                {
                                    var test_cas = myCon.Query("Select * from etats_travail_corespondance_unité_cas_particuliers WHERE LANGUE='FR' AND UNITE='" + UNITE_FONCTION + "'", Demande).ToList();
                                    foreach (var rows_cas in test_cas)
                                    {
                                        var fields_cas = rows_cas as IDictionary<string, object>;
                                        TEXTE = (string)fields_cas["TEXTE"];
                                    }
                                }
                                else
                                {
                                    var test_texte = myCon.Query("Select * from etats_travail_corespondance_texte WHERE LANGUE='FR'", Demande).ToList();
                                    foreach (var rows_texte in test_texte)
                                    {
                                        var fields_texte = rows_texte as IDictionary<string, object>;
                                        TEXTE = (string)fields_texte["FONCTION_OUI"];
                                        var test_fonction = myCon.Query("Select * from etats_travail_corespondance_fonction where LANGUE='FR' AND FONCTION='" + FONCTION.ToString().Replace("é", "e").ToUpper() + "'", Demande).ToList();
                                        foreach (var rows_fonction in test_fonction)
                                        {
                                            var fields_fonction = rows_fonction as IDictionary<string, object>;
                                            var PREFIXE_FCT = fields_fonction["PREFIXE_FCT"];
                                            var CAS_PARTICULIER = fields_fonction["CAS_PARTICULIER"];
                                            if ((string)CAS_PARTICULIER != null)
                                            {
                                                TEXTE = (string)CAS_PARTICULIER;
                                            }
                                            var test_unité = myCon.Query("Select * from etats_travail_corespondance_unité where LANGUE='FR' AND UNITE='" + AFFECTATION.ToString().Substring(0, 5) + "'", Demande).ToList();
                                            foreach (var rows_unité in test_unité)
                                            {
                                                var fields_unité = rows_unité as IDictionary<string, object>;
                                                var PREFIXE_UNITE = fields_unité["PREFIXE_UNITE"];
                                                TEXTE = TEXTE.ToString().Replace("#FONCTION#", FONCTION.ToString());
                                                if ((string)PREFIXE_FCT != null)
                                                {
                                                    int LONG = PREFIXE_FCT.ToString().Length;
                                                    if ((string)PREFIXE_FCT.ToString().Substring(LONG - 1, 1) == "'")
                                                    {
                                                        TEXTE = TEXTE.ToString().Replace("#PREFIXE_FCT# ", PREFIXE_FCT.ToString());
                                                    }
                                                    else
                                                    {
                                                        TEXTE = TEXTE.ToString().Replace("#PREFIXE_FCT#", PREFIXE_FCT.ToString());
                                                    }
                                                }
                                                TEXTE = TEXTE.ToString().Replace("#AFFECTATION#", UNITE_FONCTION.ToString().Replace("_", "'"));
                                                var AVEC_UNITE = fields_fonction["AVEC_UNITE"];
                                                if ((string)AVEC_UNITE == "OUI")
                                                {
                                                    TEXTE = TEXTE.ToString().Replace("#PREFIXE_UNITE#", PREFIXE_UNITE.ToString());
                                                    TEXTE = TEXTE.ToString().Replace("#UNITE#", AFFECTATION.ToString().Replace("_", "'"));
                                                    TEXTE = TEXTE.Trim();
                                                }
                                                else
                                                {
                                                    TEXTE = TEXTE.ToString().Replace("#PREFIXE_UNITE# ", "");
                                                    TEXTE = TEXTE.ToString().Replace("#UNITE#", "");
                                                    TEXTE = TEXTE.Trim();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            string DATE_SORTIE_PRINT = DATE_SORTIE.ToString().Substring(8, 2) + "/" + DATE_SORTIE.ToString().Substring(5, 2) + "/" + DATE_SORTIE.ToString().Substring(0, 4);
                            var test_text = myCon.Query("Select * from etats_travail_corespondance_fonction_sortants WHERE LANGUE='FR' AND MOTIF_SORTIE= '" + MOTIF_SORTIE + "'", Demande).ToList();
                            foreach (var rows_text in test_text)
                            {
                                var fields_text = rows_text as IDictionary<string, object>;
                                TEXTE = (string)fields_text["TEXTE"];
                                TEXTE = TEXTE.ToString().Replace("#DATE_SORTIE#", DATE_SORTIE_PRINT);
                                TEXTE = TEXTE.Trim();
                            }
                        }
                    }
                    if (Demande.LANGUE == "ARABE")
                    {
                        if ((string)DATE_SORTIE == "")
                        {
                            if ((string)FONCTION == "")
                            {
                                var test_texte = myCon.Query("Select * from etats_travail_corespondance_texte WHERE LANGUE='AR' AND SEXE='" + SEXE + "'", Demande).ToList();
                                foreach (var rows_texte in test_texte)
                                {
                                    var fields_texte = rows_texte as IDictionary<string, object>;
                                    TEXTE = (string)fields_texte["FONCTION_NON"];
                                    var test_unité = myCon.Query("Select * from etats_travail_corespondance_unité where LANGUE='AR' AND UNITE='" + AFFECTATION.ToString().Substring(0, 5) + "'", Demande).ToList();
                                    foreach (var rows_unité in test_unité)
                                    {
                                        var fields_unité = rows_unité as IDictionary<string, object>;
                                        var PREFIXE_UNITE = fields_unité["PREFIXE_UNITE"];
                                        TEXTE = TEXTE.ToString().Replace("#PREFIXE_UNITE# ", PREFIXE_UNITE.ToString());
                                        TEXTE = TEXTE.ToString().Replace("#UNITE#", AFFECTATION.ToString().Replace("_", "'"));
                                        TEXTE = TEXTE.Trim();
                                    }
                                }
                            }
                            if ((string)FONCTION != "")
                            {
                                int test_cas_nbre = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etats_travail_corespondance_unité_cas_particuliers WHERE LANGUE='AR' AND SEXE='" + SEXE + "' AND UNITE='" + UNITE_FONCTION + "'", Demande);
                                if (test_cas_nbre != 0)
                                {
                                    var test_cas = myCon.Query("Select * from etats_travail_corespondance_unité_cas_particuliers WHERE LANGUE='AR' AND SEXE='" + SEXE + "' AND UNITE='" + UNITE_FONCTION + "'", Demande).ToList();
                                    foreach (var rows_cas in test_cas)
                                    {
                                        var fields_cas = rows_cas as IDictionary<string, object>;
                                        TEXTE = (string)fields_cas["TEXTE"];
                                    }
                                }
                                else
                                {
                                    var test_texte = myCon.Query("Select * from etats_travail_corespondance_texte WHERE LANGUE='AR' AND SEXE='" + SEXE + "'", Demande).ToList();
                                    foreach (var rows_texte in test_texte)
                                    {
                                        var fields_texte = rows_texte as IDictionary<string, object>;
                                        TEXTE = (string)fields_texte["FONCTION_OUI"];
                                        var test_fonction = myCon.Query("Select * from etats_travail_corespondance_fonction where LANGUE='AR' AND SEXE='" + SEXE + "' AND FONCTION='" + FONCTION + "'", Demande).ToList();
                                        foreach (var rows_fonction in test_fonction)
                                        {
                                            var fields_fonction = rows_fonction as IDictionary<string, object>;
                                            var PREFIXE_FCT = fields_fonction["PREFIXE_FCT"];
                                            var CAS_PARTICULIER = fields_fonction["CAS_PARTICULIER"];
                                            if ((string)CAS_PARTICULIER != null)
                                            {
                                                TEXTE = (string)CAS_PARTICULIER;
                                            }
                                            var test_unité = myCon.Query("Select * from etats_travail_corespondance_unité where LANGUE='AR' AND UNITE='" + AFFECTATION.ToString().Substring(0, 5) + "'", Demande).ToList();
                                            foreach (var rows_unité in test_unité)
                                            {
                                                var fields_unité = rows_unité as IDictionary<string, object>;
                                                var PREFIXE_UNITE = fields_unité["PREFIXE_UNITE"];
                                                TEXTE = TEXTE.ToString().Replace("#FONCTION#", FONCTION.ToString());
                                                if ((string)PREFIXE_FCT != null)
                                                {
                                                    int LONG = PREFIXE_FCT.ToString().Length;
                                                    if ((string)PREFIXE_FCT.ToString().Substring(LONG - 1, 1) == "'")
                                                    {
                                                        TEXTE = TEXTE.ToString().Replace("#PREFIXE_FCT# ", PREFIXE_FCT.ToString());
                                                    }
                                                    else
                                                    {
                                                        TEXTE = TEXTE.ToString().Replace("#PREFIXE_FCT#", PREFIXE_FCT.ToString());
                                                    }
                                                }
                                                TEXTE = TEXTE.ToString().Replace("#AFFECTATION#", UNITE_FONCTION.ToString().Replace("_", "'"));
                                                var AVEC_UNITE = fields_fonction["AVEC_UNITE"];
                                                if ((string)AVEC_UNITE == "OUI")
                                                {
                                                    TEXTE = TEXTE.ToString().Replace("#PREFIXE_UNITE# ", PREFIXE_UNITE.ToString());
                                                    TEXTE = TEXTE.ToString().Replace("#UNITE#", AFFECTATION.ToString().Replace("_", "'"));
                                                    TEXTE = TEXTE.Trim();
                                                }
                                                else
                                                {
                                                    TEXTE = TEXTE.ToString().Replace("#PREFIXE_UNITE# ", "");
                                                    TEXTE = TEXTE.ToString().Replace("#UNITE#", "");
                                                    TEXTE = TEXTE.Trim();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            string DATE_SORTIE_PRINT = DATE_SORTIE.ToString().Substring(8, 2) + "/" + DATE_SORTIE.ToString().Substring(5, 2) + "/" + DATE_SORTIE.ToString().Substring(0, 4);
                            var test_text = myCon.Query("Select * from etats_travail_corespondance_fonction_sortants WHERE LANGUE='AR' AND SEXE = '" + SEXE + "' AND MOTIF_SORTIE= '" + MOTIF_SORTIE + "'", Demande).ToList();
                            foreach (var rows_text in test_text)
                            {
                                var fields_text = rows_text as IDictionary<string, object>;
                                TEXTE = (string)fields_text["TEXTE"];
                                TEXTE = TEXTE.ToString().Replace("#DATE_SORTIE#", DATE_SORTIE_PRINT);
                                TEXTE = TEXTE.Trim();
                            }
                        }
                    }
                    Document doc = new Document();
                    var DDP_PRINT = Convert.ToInt32(DDP);
                    doc.LoadFromFile(destFile);
                    if (Demande.LANGUE == "ARABE")
                    {
                        doc.Replace("<DATE>", (string)DateTime.Now.ToString("yyyy/MM/dd"), true, true);
                    }
                    else
                    {
                        doc.Replace("<DATE>", (string)DateTime.Now.ToString("dd/MM/yyyy"), true, true);
                    }
                    doc.Replace("<INTERESSE>", (string)INTERESSE, true, true);
                    if ((string)GRADE.ToString()!= "")
                    {
                        doc.Replace("<GRADE>", (string)GRADE.ToString().Replace("_", "'"), true, true);
                    }
                    {
                        doc.Replace("<GRADE>", "", true, true);
                    }
                    doc.Replace("<DDP>", DDP_PRINT.ToString(), true, true);
                    doc.Replace("<NOM_PRENOM>", (string)NOM_PRENOM.ToString().Replace("_", "'"), true, true);
                    doc.Replace("<CIN>", (string)CIN, true, true);
                    doc.Replace("<TEXTE>", TEXTE.ToString().Replace("_", "'"), true, true);
                    doc.Replace("<AFFECATATION>", (string)AFFECTATION, true, true);
                    doc.Replace("<DEMANDEUR>", (string)DEMANDEUR, true, true);
                    doc.Replace("<ORDRE>", ORDRE_V.ToString(), true, true);
                    doc.SaveToFile(destFile, FileFormat.Docx2013);
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
                    var filename = @".\Etats\SORTIE\ETAT_TRAVAIL\" + fileoutput + ".pdf";
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
                        int testsignature = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etats_signataires WHERE DDP='" + DDP + @"' AND DDP_SIGNATAIRE_OBLIGATOIRE<>''", Demande);
                        if (testsignature != 0)
                        {
                            var testsig = myCon.Query("SELECT * FROM etats_signataires WHERE DDP='" + DDP + @"'", Demande).ToList();
                            foreach (var rowssig in testsig)
                            {
                                var fieldssig = rowssig as IDictionary<string, object>;
                                var DDP_SIG = fieldssig["DDP_SIGNATAIRE_OBLIGATOIRE"];
                                if (Demande.LANGUE == "FRANCAIS")
                                {
                                    if ((String)FONCTION != "DIRECTEUR" && (String)FONCTION != "Directeur")
                                    {
                                        XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\FR\POUR_DIRECTEUR\" + DDP_SIG + ".jpg");
                                        gfx.DrawImage(imagesigne, 0, page.Height - 240, page.Width, 110);
                                    }
                                    else
                                    {
                                        XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\FR\SANS_POUR_DIRECTEUR\" + DDP_SIG + ".jpg");
                                        gfx.DrawImage(imagesigne, 0, page.Height - 240, page.Width, 110);
                                    }
                                }
                                else
                                {
                                    if ((String)FONCTION != "DIRECTEUR" && (String)FONCTION != "Directeur")
                                    {
                                        XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\AR\POUR_DIRECTEUR\" + DDP_SIG + ".jpg");
                                        gfx.DrawImage(imagesigne, 0, page.Height - 240, page.Width, 110);
                                    }
                                    else
                                    {
                                        XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\AR\SANS_POUR_DIRECTEUR\" + DDP_SIG + ".jpg");
                                        gfx.DrawImage(imagesigne, 0, page.Height - 240, page.Width, 110);
                                    }
                                }
                            }
                            XImage imagepied = XImage.FromFile(@".\Etats\PICS\PIED.jpg");
                            gfx.DrawImage(imagepied, 0, page.Height - 80, page.Width, 80);

                        }
                        else
                        {
                            var testsig = myCon.Query("SELECT * FROM etats_signataires WHERE ABSENCE_DU>'" + DATE_TODAY + @"' OR ABSENCE_AU<'" + DATE_TODAY + @"' OR ABSENCE_DU IS NULL ORDER BY ORDRE LIMIT 1", Demande).ToList();
                            foreach (var rowssig in testsig)
                            {
                                var fieldssig = rowssig as IDictionary<string, object>;
                                var DDP_SIG = fieldssig["DDP"];
                                if ((String)DDP_SIG == (String)DDP)
                                {
                                    var testsig2 = myCon.Query("SELECT * FROM etats_signataires WHERE DDP<>'" + DDP_SIG + "' AND (ABSENCE_DU>'" + DATE_TODAY + @"' OR ABSENCE_AU<'" + DATE_TODAY + @"' OR ABSENCE_DU IS NULL) ORDER BY ORDRE LIMIT 1", Demande).ToList();
                                    foreach (var rowssig2 in testsig2)
                                    {
                                        var fieldssig2 = rowssig2 as IDictionary<string, object>;
                                        DDP_SIG = fieldssig2["DDP"];
                                    }
                                }
                                if (Demande.LANGUE == "FRANCAIS")
                                {
                                    if ((String)FONCTION != "DIRECTEUR" && (String)FONCTION != "Directeur")
                                    {
                                        XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\FR\POUR_DIRECTEUR\" + DDP_SIG + ".jpg");
                                        gfx.DrawImage(imagesigne, 0, page.Height - 240, page.Width, 110);
                                    }
                                    else
                                    {
                                        XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\FR\SANS_POUR_DIRECTEUR\" + DDP_SIG + ".jpg");
                                        gfx.DrawImage(imagesigne, 0, page.Height - 240, page.Width, 110);
                                    }
                                }
                                else
                                {
                                    if ((String)FONCTION != "DIRECTEUR" && (String)FONCTION != "Directeur")
                                    {
                                        XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\AR\POUR_DIRECTEUR\" + DDP_SIG + ".jpg");
                                        gfx.DrawImage(imagesigne, 0, page.Height - 240, page.Width, 110);
                                    }
                                    else
                                    {
                                        XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\AR\SANS_POUR_DIRECTEUR\" + DDP_SIG + ".jpg");
                                        gfx.DrawImage(imagesigne, 0, page.Height - 240, page.Width, 110);
                                    }
                                }
                            }
                            XImage imagepied = XImage.FromFile(@".\Etats\PICS\PIED.jpg");
                            gfx.DrawImage(imagepied, 0, page.Height - 80, page.Width, 80);
                        }
                    }
                    inputDocument.Save(filename);
                    var result2 = myCon.Query("Insert into etat_travail_demandes values ('" + ORDRE_V + "', @DATE,@DDP,@NOM_PRENOM, @LANGUE, 'TRAITE',@DDP_DEMANDEUR,@NOM_PRENOM_DEMANDEUR, @CHEMIN)", new { Demande.DATE, Demande.DDP, Demande.NOM_PRENOM, Demande.LANGUE, Demande.DDP_DEMANDEUR, Demande.NOM_PRENOM_DEMANDEUR, CHEMIN }).ToList();
                    //myCon.close();
                    return Ok(result2);
                }
            }
        cleanup: return new JsonResult("Attestation de travail déjà éditée !");
        Sortir: return new JsonResult("");
        }
    }   
}
