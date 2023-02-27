

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;
using WebAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonProfilController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public MonProfilController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // PROFIL DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("Profil")]
        public IActionResult Profil(MonProfil Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test1 = myCongespers.Query("SELECT a.*, b.libtret FROM PERSONEL a JOIN TYPERETRAITE b ON a.RETRAITE=b.tret WHERE DOTI=@DDP", Demande).ToList();
                //myCon.close();
                return Ok(test1);
            }
        }

        [HttpGet]
        [Route("pic")]
        public IActionResult GetPic(string ddp)
        {
            string path= @".\PICS_PROFIL\"+ddp+".jpg";
            if (System.IO.File.Exists(path))
            {
                var f = System.IO.File.ReadAllBytes(path);
                return Ok(f);
            }
            else
            {
                var f = System.IO.File.ReadAllBytes(@".\PICS_PROFIL\0000000.jpg");
                return Ok(f);
            }
        }


        // PROFIL DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("ProfilCIN")]
        public IActionResult ProfilCIN(MonProfil Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                int test = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM cin_maj WHERE DDP=@DDP", Demande);
                if (test != 0)
                {
                    var test1 = myCon.Query("SELECT * FROM cin_maj WHERE DDP=@DDP", Demande).ToList();
                    return Ok(test1);
                }
                else
                {
                    var test1 = myCongespers.Query("SELECT a.*, b.libtret FROM PERSONEL a JOIN TYPERETRAITE b ON a.RETRAITE=b.tret WHERE DOTI=@DDP", Demande).ToList();
                    return Ok(test1);
                }
                //myCon.close();
            }
        }

        [HttpPost]
        [Route("Motif_Sortie")]
        public IActionResult Motif_Sortie(MonProfil Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCongespers.Query("SELECT * FROM POSAD WHERE code=@MS", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        [HttpPut]
        public JsonResult Put(MonProfil Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                int DDP_V = Convert.ToInt32(Demande.DDP);
                var ttt = myCongespers.Query(@"Update PERSONEL set NOMAR = @NOMAR, CIN = @CIN, ADRESSE = @ADRESSE where DOTI = '" + DDP_V + @"'", Demande).ToList();
                //myCon.close();
                return new JsonResult("Mise à jour effectuée !");
            }
        }

        // GRADE/ECHELLE/ECHELON DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("ProfilGrade")]
        public IActionResult ProfilGrade(MonProfil Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var DIB_REQ = myCongespers.Query("SELECT * FROM PERSONEL WHERE DOTI=@DDP", Demande).ToList();
                foreach (var ligne_dib in DIB_REQ)
                {
                    var fields_mdib = ligne_dib as IDictionary<string, object>;
                    var DIB = fields_mdib["MDIB"];
                    var test = myCongespers.Query("SELECT a.GRADE, b.LIBGRADE, b.ECHELLE, a.ECHELON, a.DEFFET, a.DECHELON FROM AVANCEMENTS a JOIN GRADES b ON a.GRADE=b.GRADE WHERE MDIB='" + DIB + @"' AND a.DFIN IS null ORDER BY a.deffet", Demande).ToList();
                    //myCon.close();
                    return Ok(test);
                }
                return Ok();
            }
        }

        // GRADE/ECHELLE/ECHELON DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("ProfilGradeEffet")]
        public IActionResult ProfilGradeEffet(MonProfil Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var DIB_REQ = myCongespers.Query("SELECT * FROM PERSONEL WHERE DOTI=@DDP", Demande).ToList();
                foreach (var ligne_dib in DIB_REQ)
                {
                    var fields_mdib = ligne_dib as IDictionary<string, object>;
                    var DIB = fields_mdib["MDIB"];
                    var test = myCongespers.Query("SELECT TOP 1 b.LIBGRADE, b.ECHELLE, a.ECHELON, a.DEFFET, a.DECHELON FROM AVANCEMENTS a JOIN GRADES b ON a.GRADE=b.GRADE WHERE MDIB='" + DIB + @"' AND a.GRADE=@GRADE ORDER BY a.deffet", Demande).ToList();
                    //myCon.close();
                    return Ok(test);
                }
                return Ok();
            }
        }

        // AFFECTATION DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("ProfilAff")]
        public IActionResult ProfilAff(MonProfil Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var DIB_REQ = myCongespers.Query("SELECT * FROM PERSONEL WHERE DOTI=@DDP", Demande).ToList();
                foreach (var ligne_dib in DIB_REQ)
                {
                    var fields_mdib = ligne_dib as IDictionary<string, object>;
                    var DIB = fields_mdib["MDIB"];
                    var test = myCongespers.Query("SELECT b.LIBC, a.DEFFET, a.DFIN FROM AFFECTATIONS a JOIN UNITES b ON a.affectation=b.UNITE WHERE MDIB='" + DIB + @"' AND ORGANIGRAMME=2 AND a.DFIN IS null ORDER BY a.DEFFET", Demande).ToList();
                    return Ok(test);
                }
                //myCon.close();
                return Ok();
            }
        }

        // FONCTION DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("ProfilFonction")]
        public IActionResult ProfilFonction(MonProfil Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var DIB_REQ = myCongespers.Query("SELECT * FROM PERSONEL WHERE DOTI=@DDP", Demande).ToList();
                foreach (var ligne_dib in DIB_REQ)
                {
                    var fields_mdib = ligne_dib as IDictionary<string, object>;
                    var DIB = fields_mdib["MDIB"];
                    var test = myCongespers.Query("SELECT b.LIBFONCR, a.deffet FROM FONCRESPS a JOIN FONCR b ON a.foncresp=b.foncresp WHERE MDIB='" + DIB + @"' AND DFIN is null ORDER BY a.DEFFET", Demande).ToList();
                    return Ok(test);
                }
                //myCon.close();
                return Ok();
            }
        }

        // BANQUE/RIB DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("Banque")]
        public IActionResult Banque(MonProfil Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT b.BANQUE_FR, a.RIB FROM RIB a JOIN banque_codes b ON SUBSTRING(a.RIB,1,3)=b.CODE WHERE DDP=@DDP", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // HISTORIQUE DES GRADES DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("ListeGrades")]
        public IActionResult ListeGrades(MonProfil Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var DIB_REQ = myCongespers.Query("SELECT * FROM PERSONEL WHERE DOTI=@DDP", Demande).ToList();
                foreach (var ligne_dib in DIB_REQ)
                {
                    var fields_mdib = ligne_dib as IDictionary<string, object>;
                    var DIB = fields_mdib["MDIB"];
                    var test = myCongespers.Query("SELECT b.LIBGRADE, b.ECHELLE, a.ECHELON, a.DEFFET FROM AVANCEMENTS a JOIN GRADES b ON a.GRADE=b.GRADE WHERE MDIB='" + DIB + @"' ORDER BY a.deffet", Demande).ToList();
                    return Ok(test);
                }
                //myCon.close();
                return Ok();
            }
        }

        // HISTORIQUE DES AFFECTATIONS DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("ListeAff")]
        public IActionResult ListeAff(MonProfil Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var DIB_REQ = myCongespers.Query("SELECT * FROM PERSONEL WHERE DOTI=@DDP", Demande).ToList();
                foreach (var ligne_dib in DIB_REQ)
                {
                    var fields_mdib = ligne_dib as IDictionary<string, object>;
                    var DIB = fields_mdib["MDIB"];
                    var test = myCongespers.Query("SELECT b.LIBC, a.DEFFET, a.DFIN FROM AFFECTATIONS a JOIN UNITES b ON a.affectation=b.UNITE WHERE MDIB='" + DIB + @"' AND ORGANIGRAMME=2 ORDER BY a.DEFFET", Demande).ToList();
                    return Ok(test);
                }
                //myCon.close();
                return Ok();
            }
        }

        // HISTORIQUE DES FONCTIONS DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("ListeFonctions")]
        public IActionResult ListeFonctions(MonProfil Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var DIB_REQ = myCongespers.Query("SELECT * FROM PERSONEL WHERE DOTI=@DDP", Demande).ToList();
                foreach (var ligne_dib in DIB_REQ)
                {
                    var fields_mdib = ligne_dib as IDictionary<string, object>;
                    var DIB = fields_mdib["MDIB"];
                    var test = myCongespers.Query("SELECT c.LIBC, b.LIBFONCR, a.deffet FROM FONCRESPS a JOIN FONCR b ON a.foncresp=b.foncresp JOIN UNITES c ON a.poste=c.UNITE WHERE MDIB='" + DIB + @"' ORDER BY a.DEFFET", Demande).ToList();
                    return Ok(test);
                }
                //myCon.close();
                return Ok();
            }
        }

        // LISTE DES CONJOINTS DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("ListeConjoints")]
        public IActionResult ListeConjoints(MonProfil Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT distinct(nom_prenom_conj_enf) FROM situation_familiale WHERE DDP='" + Demande.DDP + @"' AND TYPE='F' ORDER BY nom_prenom_conj_enf", Demande).ToList();
                test.Clear();
                var test1 = myCon.Query("SELECT distinct(nom_prenom_conj_enf) FROM situation_familiale WHERE DDP='" + Demande.DDP + @"' AND TYPE='F' ORDER BY nom_prenom_conj_enf", Demande).ToList();
                foreach (var row in test1)
                {
                    var fields = row as IDictionary<string, object>;
                    var nom_prenom_conj_enf = fields["nom_prenom_conj_enf"].ToString().Replace("_", "'");
                    test.Add(new { nom_prenom_conj_enf = nom_prenom_conj_enf });
                }
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES ENFANTS PAR CONJOINT DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("ListeEnfants")]
        public IActionResult ListeEnfants(MonProfil Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Demande.SOURCE is null)
                {
                    var test = myCon.Query("SELECT nom_prenom_conj_enf FROM situation_familiale WHERE ddp='" + Demande.DDP + @"' AND SOURCE='" + Demande.SOURCE + @"' AND TYPE='E' ORDER BY nom_prenom_conj_enf", Demande).ToList();
                    test.Clear();
                    var test1 = myCon.Query("SELECT nom_prenom_conj_enf FROM situation_familiale WHERE ddp='" + Demande.DDP + @"' AND SOURCE='" + Demande.SOURCE + @"' AND TYPE='E' ORDER BY nom_prenom_conj_enf", Demande).ToList();
                    foreach (var row in test1)
                    {
                        var fields = row as IDictionary<string, object>;
                        var nom_prenom_conj_enf = fields["nom_prenom_conj_enf"].ToString().Replace("_", "'");
                        test.Add(new { nom_prenom_conj_enf = nom_prenom_conj_enf });
                    }
                    return Ok(test);
                }
                else
                {
                    var test = myCon.Query("SELECT nom_prenom_conj_enf FROM situation_familiale WHERE ddp='" + Demande.DDP + @"' AND SOURCE='" + Demande.SOURCE.ToString().Replace("'", "_") + @"' AND TYPE='E' ORDER BY nom_prenom_conj_enf", Demande).ToList();
                    test.Clear();
                    var test1 = myCon.Query("SELECT nom_prenom_conj_enf FROM situation_familiale WHERE ddp='" + Demande.DDP + @"' AND (SOURCE='" + Demande.SOURCE.ToString().Replace("'", "_") + @"' OR SOURCE='ADOPTION') AND TYPE='E' ORDER BY nom_prenom_conj_enf", Demande).ToList();
                    foreach (var row in test1)
                    {
                        var fields = row as IDictionary<string, object>;
                        var nom_prenom_conj_enf = fields["nom_prenom_conj_enf"].ToString().Replace("_", "'");
                        test.Add(new { nom_prenom_conj_enf = nom_prenom_conj_enf });
                    }
                    //myCon.close();
                    return Ok(test);
                }
                return Ok();
            }
        }

        //UPLOAD DU CIN
        [HttpPost]
        [Route("Upload_CIN")]
        public IActionResult getfile(MonProfil Demande)
        {
            Byte[] b;
            b = Convert.FromBase64String(Demande.nom_file);
            System.IO.File.WriteAllBytes(@".\Etats\CIN\" + Demande.DDP + ".pdf", b);
            return Ok();
        }

        // AJOUT DE CIN
        [HttpPost]
        [Route("Ajout_CIN")]
        public IActionResult Post(MonProfil Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string ADRESSE_V = Demande.ADRESSE.Replace("'", "_");
                string NOM_PRENOM_V = Demande.NOM_PRENOM.Replace("'", "_");
                string targetPath = @".\Etats\CIN\" + Demande.DDP;
                string CHEMIN = targetPath + ".pdf";
                string CHEMIN_V = CHEMIN.ToString().Replace("\\", "/");
                if (Demande.ADRESSE == "" || Demande.ADRESSE == null)
                {
                    return new JsonResult("Il faut saisir une adresse !");
                    goto Sortir;
                }
                if (Demande.NOMAR == "" || Demande.NOMAR == null)
                {
                    return new JsonResult("Il faut saisir le nom et prénom en arabe !");
                    goto Sortir;
                }
                int teste = myCon.ExecuteScalar<int>("Select count(*) from cin_maj where DDP='" + Demande.DDP + @"' AND VALIDE='OUI'", Demande);
                if (teste != 0)
                {
                    goto cleanup;
                }
                var t = myCon.Query(@"Delete from cin_maj where DDP = '" + Demande.DDP + "'", Demande).ToList();
                string query = "Insert into cin_maj values  ('" + Demande.DDP + "', '" + NOM_PRENOM_V + "', '" + Demande.NOMAR + "', '" + Demande.CIN + "', '" + ADRESSE_V + "', 'NON')";
                //myCon.close();
                var result = myCon.Query(query).ToList();
                return new JsonResult("Ajout effectué !");
            }
        cleanup: return new JsonResult("CIN déjà ajouté et validé par les RH !");
        Sortir: return new JsonResult("");
        }

        // DETAIL SITUATIONS DE L'ENFANT DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("ListeSituations")]
        public IActionResult ListeSituations(MonProfil Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT DATE_SITUATION, SITUATION FROM situation_familiale WHERE ddp='" + Demande.DDP + @"' AND nom_prenom_conj_enf='" + Demande.ENFANT + @"' AND (SOURCE='" + Demande.SOURCE + @"' OR SOURCE='ADOPTION') AND TYPE='E' ORDER BY DATE_SITUATION", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES AGENTS SUR LISTE DEROULANTE SELON DROITS DE L'AGENT CONNECTE (AGENT / HIERARCHIE / ADMIN / GESTIONNAIRE)
        [HttpPost]
        [Route("ListeAgents")]
        public IActionResult ListeAgents(MonProfil Demande)
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
                        return Ok(AGENT_REQ_TOTAL);
                    }
                }
                //myCon.close();
                return Ok();
            }
        }
    }
}
