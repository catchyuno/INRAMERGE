using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtatPrecompteController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public EtatPrecompteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // LISTE DES DEMANDES DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeDemandes")]
        public IActionResult ListeDemandes(EtatPrecompte Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM etat_precompte_demandes where DDP_DEMANDEUR =  @DDP_DEMANDEUR AND DDP =  @DDP ORDER BY DATE DESC", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES RUBRIQUES (PRET/RETENUE) DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeRubrique")]
        public IActionResult ListeRubrique(EtatPrecompte Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string TYPE_V = "";
                if (Demande.TYPE=="PRET")
                {
                    TYPE_V = "CRE";
                }
                else
                {
                    TYPE_V = "RET";
                }
                var test = myCon.Query("SELECT a.code_rubrique as CODE_RUBRIQUE, b.RUBRIQUE_ABBREVEE FROM etat_engagement_rubriques a JOIN salaire_rubriques b ON a.CODE_RUBRIQUE=b.CODE_RUBRIQUE WHERE DDP=@DDP AND b.CATEGORIE='" + TYPE_V + @"' AND b.CODE_RUBRIQUE<>'4905' GROUP BY a.code_rubrique ORDER BY RUBRIQUE_ABBREVEE", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES AGENTS SUR LISTE DEROULANTE SELON DROITS DE L'AGENT CONNECTE (AGENT / HIERARCHIE / ADMIN / GESTIONNAIRE)
        [HttpPost]
        [Route("ListeAgents")]
        public IActionResult ListeAgents(EtatPrecompte Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var catégorie = myCon.Query("SELECT distinct(CATEGORIE) FROM etats_droits where MODULE='ETAT_PRECOMPTE' AND DDP =  @DDP", Demande).ToList();
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
                        var AGENT_REQ_TOTAL = myCon.Query("SELECT * FROM etats_droits_custom WHERE MODULE='ETAT_PRECOMPTE' AND DDP=@DDP", Demande).ToList();
                        AGENT_REQ_TOTAL.Clear();
                        var AFF_REQ = myCon.Query("SELECT * FROM etats_droits_custom WHERE MODULE='ETAT_PRECOMPTE' AND DDP=@DDP", Demande).ToList();
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
                return new JsonResult("Etat Précompte non encore généré !");
            }
        }

        // SUPPRESSION DE LA DEMANDE D'ETAT DE PRECOMPTE
        [HttpPost]
        [Route("Delete")]
        public JsonResult Delete(EtatPrecompte Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string DATE_V = (string)Demande.DATE.ToString("yyyy-MM-dd");
                var t = myCon.Query(@"Delete FROM etat_precompte_demandes WHERE TYPE=@TYPE AND CODE_RUBRIQUE=@CODE_RUBRIQUE AND DATE='" + DATE_V + @"' AND DDP=@DDP  AND DDP_DEMANDEUR=@DDP_DEMANDEUR", Demande).ToList();
                //myCon.close();
            }
            return new JsonResult("Suppression effectuée !");
        }

        // AJOUT DE LA DEMANDE DE L'ETAT DE PRECOMPTE DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("Ajout")]
        public IActionResult Post(EtatPrecompte Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string DATE_V = (string)Demande.DATE.ToString("yyyy-MM-dd");
                int teste = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etat_precompte_demandes WHERE TYPE=@TYPE AND CODE_RUBRIQUE=@CODE_RUBRIQUE AND DATE='" + DATE_V + @"' AND DDP=@DDP  AND DDP_DEMANDEUR=@DDP_DEMANDEUR", Demande);
                if (teste != 0)
                {
                    goto cleanup;
                }
                if (Demande.CODE_RUBRIQUE == "")
                {
                    return new JsonResult("Il faut choisir une rubrique !");
                    goto Sortir;
                }
                string targetPath = @".\Etats\SORTIE\ETAT_PRECOMPTE";
                string fileoutput = (string)Demande.DDP + "_" + Demande.DATE.ToString().Substring(0, 10).Replace("/", "-") + "_" + Demande.TYPE + "_" + Demande.CODE_RUBRIQUE + " (" + (string)Demande.DDP_DEMANDEUR + ")";
                string CHEMIN = targetPath + "\\" + fileoutput + ".pdf";
                string CHEMIN_V = CHEMIN.ToString().Replace("\\", "/");
                string query = "Insert into etat_precompte_demandes values  ('" + DATE_V + "', '" + Demande.TYPE + "', '" + Demande.CODE_RUBRIQUE + "', '" + Demande.RUBRIQUE_ABBREVEE + "', '" + Demande.DDP + "', '" + Demande.NOM_PRENOM + "', '" + Demande.STATUT + "', '" + Demande.DDP_DEMANDEUR + "', '" + Demande.NOM_PRENOM_DEMANDEUR + "', '" + CHEMIN_V + "')";
                var result = myCon.Query(query).ToList();
                //myCon.close();
                return new JsonResult("Ajout effectué !");
            }
        cleanup: return new JsonResult("Etat Précompte déjà demandée pour ce jour !");
        Sortir: return new JsonResult("");
        }
    }
}
