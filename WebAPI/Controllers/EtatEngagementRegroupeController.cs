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

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtatEngagementRegroupeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public EtatEngagementRegroupeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // AJOUT DE LA DEMANDE DE L'ETAT D'ENGAGEMENT REGROUPE DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("Ajout")]
        public IActionResult post(EtatEngagementRegroup Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string DU_V = "";
                string AU_V = "";
                string DATE_V = (string)Demande.DATE.ToString("yyyy-MM-dd");
                try
                {
                    DateTime dt = DateTime.ParseExact(Demande.DU, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    return new JsonResult("Il faut saisir une date début valide !");
                    goto Sortir;
                }
                try
                {
                    DateTime dt = DateTime.ParseExact(Demande.AU, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    return new JsonResult("Il faut saisir une date fin valide !");
                    goto Sortir;
                }
                if (Demande.DU != "" && Demande.AU != "")
                {
                    DU_V = Demande.DU.ToString().Substring(6, 4) + "-" + Demande.DU.ToString().Substring(3, 2) + "-" + Demande.DU.ToString().Substring(0, 2);
                    AU_V = Demande.AU.ToString().Substring(6, 4) + "-" + Demande.AU.ToString().Substring(3, 2) + "-" + Demande.AU.ToString().Substring(0, 2);
                }
                if (Demande.DU == "" || Demande.AU == "")
                {
                    return new JsonResult("Il faut saisir la date début et la date fin !");
                    goto Sortir;
                }
                int teste = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etat_engagement_demandes_periode_regroupée WHERE DATE='" + DATE_V + @"' AND DU='" + DU_V + @"' AND AU='" + AU_V + @"' AND DDP=@DDP  AND DDP_DEMANDEUR=@DDP_DEMANDEUR", Demande);
                if (teste != 0)
                {
                    goto cleanup;
                }
                if (Demande.DU != null && Demande.AU == null)
                {
                    return new JsonResult("Il faut saisir la date début !");
                    goto Sortir;
                }
                if (Demande.DU == null && Demande.AU != null)
                {
                    return new JsonResult("Il faut saisir la date fin !");
                    goto Sortir;
                }
                if (DateTime.Parse(Demande.DU) > DateTime.Parse(Demande.AU))
                {
                    return new JsonResult("Il faut saisir la date fin supérieure strictement à la date début !");
                    goto Sortir;
                }
                string targetPath = @".\Etats\SORTIE\ETAT_ENG_REGROUPE";
                string fileoutput = (string)Demande.DDP + "_" + Demande.DU.ToString().Substring(0, 10).Replace("/", "-") + "-" + Demande.AU.ToString().Substring(0, 10).Replace("/", "-") + "_" + Demande.DATE.ToString().Substring(0, 10).Replace("/", "-") + " (" + (string)Demande.DDP_DEMANDEUR + ")";
                string CHEMIN = targetPath + "\\" + fileoutput + ".pdf";
                string CHEMIN_V = CHEMIN.ToString().Replace("\\", "/");
                
                DU_V = Demande.DU.ToString().Substring(6, 4) + "-" + Demande.DU.ToString().Substring(3, 2) + "-" + Demande.DU.ToString().Substring(0, 2);
                AU_V = Demande.AU.ToString().Substring(6, 4) + "-" + Demande.AU.ToString().Substring(3, 2) + "-" + Demande.AU.ToString().Substring(0, 2);
                string query = "Insert into etat_engagement_demandes_periode_regroupée values  ('" + DATE_V + "', '" + Demande.DDP + "', '" + Demande.NOM_PRENOM + "', '" + DU_V + "', '" + AU_V + "', '" + Demande.STATUT + "', '" + Demande.DDP_DEMANDEUR + "', '" + Demande.NOM_PRENOM_DEMANDEUR + "', '" + CHEMIN_V + "')";
                var result = myCon.Query(query).ToList();
                //myCon.close();
                return new JsonResult("Ajout effectué !");
            }
        cleanup: return new JsonResult("Etat Engagemet déjà demandée pour ce jour !");
        Sortir: return new JsonResult("");
        }

        // LISTE DES DEMANDES DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeDemandes")]
        public IActionResult ListeDemandes(EtatEngagementRegroup Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * from etat_engagement_demandes_periode_regroupée where DDP_DEMANDEUR =  @DDP_DEMANDEUR AND DDP =  @DDP ORDER BY DATE DESC", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }


        // LISTE DES AGENTS SUR LISTE DEROULANTE SELON DROITS DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeAgents")]
        public IActionResult ListeAgents(EtatEngagementRegroup Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var catégorie = myCon.Query("SELECT distinct(CATEGORIE) from etats_droits where MODULE='ETAT_ENG_REGROUPE' AND DDP =  @DDP", Demande).ToList();
                foreach (var rows in catégorie)
                {
                    var fields = rows as IDictionary<string, object>;
                    var CATEGORIE = fields["CATEGORIE"];
                    if ((string)CATEGORIE == "AGENT")
                    {
                        var test = myCon.Query("SELECT a.DDP,a.NOM_PRENOM,b.CATEGORIE from etat_engagement_infos a JOIN etats_droits b ON a.DDP=b.DDP where a.DDP=@DDP AND b.MODULE='ETAT_ENG_REGROUPE' GROUP BY a.DDP,a.NOM_PRENOM,b.CATEGORIE order by a.NOM_PRENOM", Demande).ToList();
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
                                    var fields_aff = ligne_aff as IDictionary<string, object>;
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
                        var AGENT_REQ_TOTAL = myCon.Query("SELECT * from etats_droits_custom WHERE MODULE='ETAT_ENG_REGROUPE' AND DDP=@DDP", Demande).ToList();
                        AGENT_REQ_TOTAL.Clear();
                        var AFF_REQ = myCon.Query("SELECT * from etats_droits_custom WHERE MODULE='ETAT_ENG_REGROUPE' AND DDP=@DDP", Demande).ToList();
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
                return new JsonResult("Etat Engagement non encore généré !");
            }
        }

        // SUPPRESSION DE L'INCIDENT
        [HttpPost]
        [Route("Delete")]
        public JsonResult Delete(EtatEngagementRegroup Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string DATE_V = (string)Demande.DATE.ToString("yyyy-MM-dd");
                //string DU_V = (string)Demande.DU.ToString("yyyy-MM-dd");
                //string AU_V = (string)Demande.AU.ToString("yyyy-MM-dd");
                //string tt = Demande.DU.ToString().Substring(6, 4);
                string DU_V = Demande.DU.ToString().Substring(0, 10);// + "-" + Demande.DU.ToString().Substring(4, 2) + "-" + Demande.DU.ToString().Substring(0, 2);
                string AU_V = Demande.AU.ToString().Substring(0, 10);// + "-" + Demande.AU.ToString().Substring(3, 2) + "-" + Demande.AU.ToString().Substring(0, 2);
                var t = myCon.Query(@"Delete FROM etat_engagement_demandes_periode_regroupée WHERE DATE='" + DATE_V + @"' AND  DU='" + DU_V + @"' AND  AU='" + AU_V + @"' AND DDP=@DDP  AND DDP_DEMANDEUR=@DDP_DEMANDEUR", Demande).ToList();
                //myCon.close();
            }
            return new JsonResult("Suppression effectuée !");
        }
    }
}
