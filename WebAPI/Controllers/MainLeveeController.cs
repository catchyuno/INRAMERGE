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
    public class MainLeveeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public MainLeveeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // LISTE DES UPLOADS DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeDemandes")]
        public IActionResult ListeDemandes(MainLevee Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM etat_domiciliation_main_levée where DDP_DEMANDEUR =  @DDP_DEMANDEUR AND DDP =  @DDP ORDER BY DATE DESC", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES AGENTS SUR LISTE DEROULANTE SELON DROITS DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeAgents")]
        public IActionResult ListeAgents(MainLevee Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var catégorie = myCon.Query("SELECT distinct(CATEGORIE) FROM etats_droits where MODULE='MAIN_LEVEE' AND DDP =  @DDP", Demande).ToList();
                foreach (var rows in catégorie)
                {
                    var fields = rows as IDictionary<string, object>;
                    var CATEGORIE = fields["CATEGORIE"];
                    if ((string)CATEGORIE == "AGENT")
                    {
                        var test = myCon.Query("SELECT a.DDP,a.NOM_PRENOM,b.CATEGORIE FROM etat_domiciliation_demandes a JOIN etats_droits b ON a.DDP=b.DDP where a.DDP=@DDP AND b.MODULE='MAIN_LEVEE' GROUP BY a.DDP,a.NOM_PRENOM,b.CATEGORIE order by a.NOM_PRENOM", Demande).ToList();
                        return Ok(test);
                    }
                    if ((string)CATEGORIE == "ADMIN")
                    {
                        var test = myCon.Query("SELECT DDP, NOM_PRENOM FROM etat_domiciliation_demandes WHERE ETAT='EN COURS' GROUP BY DDP, NOM_PRENOM ORDER BY NOM_PRENOM", Demande).ToList();
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
                                                int EXISTE_REVENU = myCon.ExecuteScalar<int>("SELECT count(*) FROM etat_domiciliation_demandes where ETAT='EN COURS' AND DDP='" + DDP + @"'", Demande);
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
                        var AGENT_REQ_TOTAL = myCon.Query("SELECT * FROM etats_droits_custom WHERE MODULE='MAIN_LEVEE' AND DDP=@DDP", Demande).ToList();
                        AGENT_REQ_TOTAL.Clear();
                        var AFF_REQ = myCon.Query("SELECT * FROM etats_droits_custom WHERE MODULE='MAIN_LEVEE' AND DDP=@DDP", Demande).ToList();
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
                                        int EXISTE_REVENU = myCon.ExecuteScalar<int>("SELECT count(*) FROM etat_domiciliation_demandes where ETAT='EN COURS' AND DDP='" + DDP + @"'", Demande);
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

        // AFFICHAGE DE LA BANQUE/RIB DE L'AGENT SELECTIONNE DEPUIS LA LISTE
        [HttpPost]
        [Route("RIB_BANQUE")]
        public IActionResult RIB_BANQUE(MainLevee Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT BANQUE, RIB FROM etat_domiciliation_demandes where ETAT='EN COURS' AND DDP =  @DDP GROUP BY BANQUE, RIB ", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // TELECHARGEMENT DE LA MAIN LEVEE EN FORMAT (PDF)
        [HttpGet]
        [Route("file")]
        public IActionResult getfile(string nom_file)
        {
            var f = System.IO.File.ReadAllBytes(@nom_file);
            return Ok(f);
        }

        // UPLOAD DE LA MAIN LEVEE
        [HttpPost]
        [Route("upload")]
        public IActionResult getfile(MainLevee Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string DATE_V = (string)Demande.DATE.ToString("yyyy-MM-dd");
                string targetPath = @".\Etats\SORTIE\MAINLEVEE";
                string fileoutput = (string)Demande.DDP + "_" + Demande.DATE.ToString().Substring(0, 10).Replace("/", "-") + " (" + (string)Demande.DDP_DEMANDEUR + ")";
                string CHEMIN = targetPath + "\\" + fileoutput + ".pdf";
                string CHEMIN_V = CHEMIN.ToString().Replace("\\", "/");
                int teste = myCon.ExecuteScalar<int>("Select count(*) from etat_domiciliation_main_levée where ETAT='EN COURS' AND DDP=@DDP AND DDP_DEMANDEUR=@DDP_DEMANDEUR AND RIB=@RIB", Demande);
                if (teste != 0)
                {
                    goto cleanup;
                }
                string query = "Insert into etat_domiciliation_main_levée values  ('" + DATE_V + "', '" + Demande.DDP + "', '" + Demande.NOM_PRENOM + "','" + Demande.BANQUE + "', '" + Demande.RIB + "', 'EN COURS', '" + Demande.DDP_DEMANDEUR + "', '" + Demande.NOM_PRENOM_DEMANDEUR + "', '" + CHEMIN_V + "')";
                var result = myCon.Query(query).ToList();
                Byte[] b;
                b = Convert.FromBase64String(Demande.nom_file);
                System.IO.File.WriteAllBytes(@".\Etats\SORTIE\MAINLEVEE\" + Demande.DDP + "_" + Demande.DATE.ToString().Substring(0, 10).Replace("/", "-") + " (" + Demande.DDP_DEMANDEUR + ")" + ".pdf", b);
                //Sortir: return new JsonResult("");
                //return new JsonResult("Ajout effectué !");
                //myCon.close();
            }
            cleanup: return new JsonResult("Main levée déjà inscrite pour ce RIB !");
            return Ok();
        }

        // SUPPRESSION DE L'INCIDENT
        [HttpPost]
        [Route("Delete")]
        public JsonResult Delete(MainLevee Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string targetPath = @".\Etats\SORTIE\MAINLEVEE";
                string fileoutput = (string)Demande.DDP + "_" + Demande.DATE.ToString().Substring(0, 10).Replace("/", "-") + " (" + (string)Demande.DDP_DEMANDEUR + ")";
                string CHEMIN = targetPath + "\\" + fileoutput + ".pdf";
                string CHEMIN_V = CHEMIN.ToString().Replace("\\", "/");
                string DATE_V = (string)Demande.DATE.ToString("yyyy-MM-dd");
                var t = myCon.Query(@"Delete from etat_domiciliation_main_levée where DATE='" + DATE_V + @"' AND DDP=@DDP AND DDP_DEMANDEUR=@DDP_DEMANDEUR", Demande).ToList();
                //myCon.close();
                System.IO.File.Delete(CHEMIN_V);
            }
            return new JsonResult("Suppression effectuée !");
        }

        // AJOUT DE LA MAIN LEVEE DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        //[HttpPost]
        //[Route("Ajout")]
        //public IActionResult Post(MainLevee Demande)
        //{
        //    string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
        //    using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
        //    {
        //        string DATE_V = (string)Demande.DATE.ToString("yyyy-MM-dd");
        //        string targetPath = @".\Etats\SORTIE\MAIN_LEVEE";
        //        string fileoutput = (string)Demande.DDP + "_" + Demande.DATE.ToString().Substring(0, 10).Replace("/", "-") + " (" + (string)Demande.DDP_DEMANDEUR + ")";
        //        string CHEMIN = targetPath + "\\" + fileoutput + ".pdf";
        //        string CHEMIN_V = CHEMIN.ToString().Replace("\\", "/");
        //        int teste = myCon.ExecuteScalar<int>("Select count(*) from etat_domiciliation_main_levée where ETAT='EN COURS' AND DATE='" + DATE_V + @"' AND DDP=@DDP AND DDP_DEMANDEUR=@DDP_DEMANDEUR AND RIB=@RIB", Demande);
        //        if (teste != 0)
        //        {
        //            goto cleanup;
        //        }
        //        string query = "Insert into etat_domiciliation_main_levée values  ('" + DATE_V + "', '" + Demande.DDP + "', '" + Demande.NOM_PRENOM + "','" + Demande.BANQUE + "', '" + Demande.RIB + "', 'EN COURS', '" + Demande.DDP_DEMANDEUR + "', '" + Demande.NOM_PRENOM_DEMANDEUR + "', '" + CHEMIN_V + "')";
        //        var result = myCon.Query(query).ToList();
        //        return new JsonResult("Ajout effectué !");
        //    }
        //cleanup: return new JsonResult("Main levée déjà inscrite pour ce RIB !");
        //Sortir: return new JsonResult("");
        //}
    }
}
