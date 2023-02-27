using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArabeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ArabeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // MISE A JOUR DES AGENTS SORTANTS (ACTIF=NON)
        [HttpPost]
        [Route("MAJ_ACTIFS")]
        public IActionResult MAJ_ACTIFS(Arabe Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                DateTime thisDay = DateTime.Today;
                string DATE_TODAY = thisDay.ToString("yyyy-MM-dd");
                string DATE_REFERENCE = thisDay.ToString("yyyy") + "-01-01";
                var test1 = myCongespers.Query("SELECT DOTI, DSORTIE FROM dbo.personel WHERE dsortie < '" + DATE_TODAY + "'  AND dsortie>'" + DATE_REFERENCE + "'", Demande).ToList();
                foreach (var rows1 in test1)
                {
                    var fields1 = rows1 as IDictionary<string, object>;
                    var DDP_V = fields1["DOTI"];
                    if (DDP_V.ToString().Length == 6)
                    {
                        DDP_V = "0" + DDP_V;
                    }
                    if (DDP_V.ToString().Length == 5)
                    {
                        DDP_V = "00" + DDP_V;
                    }
                    string query = "update etats_droits set ACTIF='NON' WHERE DDP='" + DDP_V + "'";
                    var result = myCon.Query(query).ToList();
                }
                //myCon.close();
            }
            return Ok();
        }


        // MISE A JOUR DES AGENTS SORTANTS (ACTIF=NON)
        [HttpPost]
        [Route("MAJ_RUBS")]
        public IActionResult MAJ_RUBS(Arabe Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcepaie= _configuration.GetConnectionString("paieAppCon");
            MySqlConnection myConpaie= new MySqlConnection(sqlDataSourcepaie);
            myConpaie.Open();
            //MySqlConnection myConpaie = new MySqlConnection(sqlDataSourcepaie)
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                //DateTime thisDay = DateTime.Today;
                //string DATE_TODAY = thisDay.ToString("yyyy-MM-dd");
                //string DATE_REFERENCE = thisDay.ToString("yyyy") + "-01-01";
                var test1 = myConpaie.Query("SELECT * FROM rubriques order by Rubrique", Demande).ToList();
                foreach (var rows1 in test1)
                {
                    var fields1 = rows1 as IDictionary<string, object>;
                    var CODE_V = fields1["Rubrique"];
                    var LIBELLE_V = fields1["LibellÃ©_Rubrique"];
                    var ABBR_V = fields1["LibellÃ©_Abb"];
                    var CAT_V = fields1["CatÃ©gorie"];
                    int test = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM salaire_rubriques where CODE_RUBRIQUE = '" + CODE_V + "'", Demande);
                    if (test == 0)
                    {
                        string query = "Insert into salaire_rubriques values  ('" + CODE_V + "', '" + LIBELLE_V + "', '" + ABBR_V + "','" + CAT_V + "')";
                        var result = myCon.Query(query).ToList();
                    }
                    //string query = "update etats_droits set ACTIF='NON' WHERE DDP='" + DDP_V + "'";
                    //var result = myCon.Query(query).ToList();
                }
                //myCon.close();
            }
            return Ok();
        }

        // MISE A JOUR DE LA TABLE DEPUIS GESPERS
        [HttpPost]
        [Route("MAJ")]
        public IActionResult MAJ(Arabe Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test1 = myCongespers.Query("SELECT LIBGRADE FROM GRADES WHERE LIBGRADE<>'SANS GRADE'", Demande).ToList();
                foreach (var rows1 in test1)
                {
                    var fields1 = rows1 as IDictionary<string, object>;
                    var LIB_GRADE = fields1["LIBGRADE"];
                    var LIB_GRADE_V = LIB_GRADE.ToString().Replace("'", "_");
                    int testgrade = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etat_travail_langue where CATEGORIE = 'GRADE' AND FRANCAIS_GESPERS='" + LIB_GRADE_V + @"'", Demande);
                    if (testgrade == 0)
                    {
                        string query = "Insert into etat_travail_langue values  ('GRADE', '" + LIB_GRADE_V + "', '', '','','')";
                        var result = myCon.Query(query).ToList();
                    }
                }
                var test2 = myCongespers.Query("SELECT LIBC FROM UNITES WHERE ORGANIGRAMME=2", Demande).ToList();
                foreach (var rows2 in test2)
                {
                    var fields2 = rows2 as IDictionary<string, object>;
                    var LIB_AFF = fields2["LIBC"];
                    var LIB_AFF_V = LIB_AFF.ToString().Replace("'", "_");
                    int testaffectation = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etat_travail_langue where CATEGORIE = 'AFFECTATION' AND FRANCAIS_GESPERS='" + LIB_AFF_V + @"'", Demande);
                    if (testaffectation == 0)
                    {
                        string query = "Insert into etat_travail_langue values  ('AFFECTATION', '" + LIB_AFF_V + "', '', '','','')";
                        var result = myCon.Query(query).ToList();
                    }
                }
                var test3 = myCongespers.Query("SELECT LIBFONCR FROM FONCR WHERE LIBFONCR<>'FIN DE FONCTION'", Demande).ToList();
                foreach (var rows3 in test3)
                {
                    var fields3 = rows3 as IDictionary<string, object>;
                    var LIB_FCT = fields3["LIBFONCR"];
                    var LIB_FCT_V = LIB_FCT.ToString().Replace("'", "_");
                    int testfonction = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etat_travail_langue where CATEGORIE = 'FONCTION' AND FRANCAIS_GESPERS='" + LIB_FCT_V + @"'", Demande);
                    if (testfonction == 0)
                    {
                        string query = "Insert into etat_travail_langue values  ('FONCTION', '" + LIB_FCT_V + "', '', '','','')";
                        var result = myCon.Query(query).ToList();
                    }
                }
                //myCon.close();
            }
            return Ok();
        }

        // LISTE DES LIBELLES SELON CATEGORIE
        [HttpPost]
        [Route("Liste")]
        public IActionResult Liste(Arabe Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM etat_travail_langue where CATEGORIE = '" + Demande.CATEGORIE + @"' ORDER BY FRANCAIS_GESPERS", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // BANQUE CHOISIE
        [HttpPost]
        [Route("Listecategories")]
        public IActionResult Listecategories(Arabe Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT distinct(CATEGORIE) FROM etat_travail_langue order by CATEGORIE", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        [HttpPut]
        public JsonResult Put(Arabe Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string FRANCAIS_M_V = Demande.FRANCAIS_M.ToString().Replace("'", "_");
                string FRANCAIS_F_V = Demande.FRANCAIS_F.ToString().Replace("'", "_");
                if (Demande.FRANCAIS_M == "")
                {
                    return new JsonResult("Il faut saisir l'intitulé masculin en francais !");
                    goto Sortir;
                }
                if (Demande.ARABE_M == "" )
                {
                    return new JsonResult("Il faut saisir l'intitulé masculin en arabe !");
                    goto Sortir;
                }
                if (Demande.ARABE_F == "" && Demande.CATEGORIE != "AFFECTATION")
                {
                    return new JsonResult("Il faut saisir l'intitulé féminin en arabe !");
                    goto Sortir;
                }
                if (Demande.FRANCAIS_F == "" && Demande.CATEGORIE != "AFFECTATION")
                {
                    return new JsonResult("Il faut saisir l'intitulé féminin en francais !");
                    goto Sortir;
                }
                var t = myCon.Query(@"Update etat_travail_langue set FRANCAIS_M = '" + FRANCAIS_M_V + @"', ARABE_M = '" + Demande.ARABE_M + @"', FRANCAIS_F = '" + FRANCAIS_F_V + @"', ARABE_F = '" + Demande.ARABE_F + @"' where CATEGORIE = '" + Demande.CATEGORIE + @"' AND FRANCAIS_GESPERS = '" + Demande.FRANCAIS_GESPERS + @"'", Demande).ToList();
                //myCon.close();
                return new JsonResult("Mise à jour effectuée !");
            }
            Sortir: return new JsonResult("");
        }
    }
}