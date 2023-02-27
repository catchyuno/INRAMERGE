

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using WebAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpDeskController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public HelpDeskController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // LISTE DES ANOMALIES SOULEVEES PAR L'AGENT CONNECTE
        [HttpPost]
        [Route("Liste")]
        public IActionResult Liste(HelpDesk Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM help_desk where DDP=@DDP AND VOLET=  @VOLET AND MENU=  @MENU ORDER BY DATE DESC", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES VOLETS
        [HttpPost]
        [Route("VoletListe")]
        public IActionResult VoletListe(HelpDesk Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT DISTINCT(VOLET) FROM etats_droits ORDER BY VOLET", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES MENUS
        [HttpPost]
        [Route("MenuListe")]
        public IActionResult MenuListe(HelpDesk Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT DISTINCT(MENU) FROM etats_droits WHERE VOLET=@VOLET ORDER BY MENU", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // AJOUT DE L'INCIDENT
        [HttpPost]
        [Route("Ajout")]
        public IActionResult Post(HelpDesk Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Demande.INTITULE == "" || Demande.INTITULE == null)
                {
                    return new JsonResult("Il faut saisir l'intitulé !");
                    goto Sortir;
                }
                if (Demande.DESCRIPTION == "" || Demande.DESCRIPTION == null)
                {
                    return new JsonResult("Il faut saisir la description !");
                    goto Sortir;
                }
                string DATE_V = (string)Demande.DATE.ToString("yyyy-MM-dd");
                int teste = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM help_desk WHERE DATE='" + DATE_V + @"' AND DDP=@DDP AND VOLET=@VOLET AND MENU=@MENU AND INTITULE=@INTITULE", Demande);
                if (teste != 0)
                {
                    goto cleanup;
                }
                string targetPath = @".\Etats\HELP_DESK";
                string fileoutput = (string)Demande.DDP + "_" + Demande.DATE.ToString().Substring(0, 10).Replace("/", "-") + "_" + Demande.VOLET + "_" + Demande.MENU + " (" + Demande.INTITULE + ")";
                string CHEMIN = targetPath + "\\" + fileoutput + ".jpg";
                string CHEMIN_V = CHEMIN.ToString().Replace("\\", "/");
                string DESCRIPTION_V = Demande.DESCRIPTION.ToString().Replace("'", "_");
                string query = "Insert into help_desk values  ('" + Demande.DDP + "', '" + Demande.NOM_PRENOM.ToString().Replace("'", "_") + "', '" + DATE_V + "', '" + Demande.VOLET + "', '" + Demande.MENU + "', '" + Demande.INTITULE + "', '" + DESCRIPTION_V + "',  '', 'EN COURS', '" + CHEMIN_V + "')";
                var result = myCon.Query(query).ToList();
                //myCon.close();
                return new JsonResult("Ajout effectué !");
            }
        cleanup: return new JsonResult("Incidence déjà signalée pour ce jour !");
        Sortir: return new JsonResult("");
        }

        // SUPPRESSION DE L'INCIDENT
        [HttpPost]
        [Route("Delete")]
        public JsonResult Delete(HelpDesk Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string targetPath = @".\Etats\HELP_DESK";
                string fileoutput = (string)Demande.DDP + "_" + Demande.DATE.ToString().Substring(0, 10).Replace("/", "-") + "_" + Demande.VOLET + "_" + Demande.MENU + " (" + Demande.INTITULE + ")";
                string CHEMIN = targetPath + "\\" + fileoutput + ".jpg";
                string CHEMIN_V = CHEMIN.ToString().Replace("\\", "/");
                string DATE_V = (string)Demande.DATE.ToString("yyyy-MM-dd");
                var t = myCon.Query(@"Delete from help_desk where DATE='" + DATE_V + @"' AND DDP=@DDP AND VOLET=@VOLET AND MENU=@MENU AND INTITULE=@INTITULE", Demande).ToList();
                System.IO.File.Delete(CHEMIN_V);
                //myCon.close();
            }
            return new JsonResult("Suppression effectuée !");
        }

        // UPDATE DE L'INCIDENT
        [HttpPut]
        public JsonResult Put(HelpDesk Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if ((Demande.DESCRIPTION!= Demande.DESCRIPTION_ANCIEN) && (Demande.INTITULE == Demande.INTITULE_ANCIEN))
                {
                    var ttt = myCon.Query(@"Update help_desk set DESCRIPTION = '" + Demande.DESCRIPTION.Replace("'", "_") + @"' where INTITULE = '" + Demande.INTITULE_ANCIEN + @"'", Demande).ToList();
                }
                if ((Demande.DESCRIPTION != Demande.DESCRIPTION_ANCIEN) && (Demande.INTITULE != Demande.INTITULE_ANCIEN))
                {
                    int teste = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM help_desk WHERE INTITULE='" + Demande.INTITULE + @"'", Demande);
                    if (teste != 0)
                    {
                        goto cleanup;
                    }
                    var ttt = myCon.Query(@"Update help_desk set INTITULE = '" + Demande.INTITULE + @"', DESCRIPTION = '" + Demande.DESCRIPTION.Replace("'", "_") + @"' where INTITULE = '" + Demande.INTITULE_ANCIEN + @"'", Demande).ToList();
                }
                if ((Demande.DESCRIPTION == Demande.DESCRIPTION_ANCIEN) && (Demande.INTITULE != Demande.INTITULE_ANCIEN))
                {
                    int teste = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM help_desk WHERE INTITULE='" + Demande.INTITULE + @"'", Demande);
                    if (teste != 0)
                    {
                        goto cleanup;
                    }
                    var ttt = myCon.Query(@"Update help_desk set INTITULE = '" + Demande.INTITULE + @"' where INTITULE = '" + Demande.INTITULE_ANCIEN + @"'", Demande).ToList();
                }
                //myCon.close();
                return new JsonResult("Mise à jour effectuée !");
            }
            cleanup: return new JsonResult("Incident déjà signalé pour ce jour !");
        }

        // UPLOAD DE LA CAPTURE ECRAN (ERREUR)
        [HttpPost]
        [Route("upload")]
        public IActionResult getfile(HelpDesk Demande)
        {
            Byte[] b;
            b = Convert.FromBase64String(Demande.nom_file);
            System.IO.File.WriteAllBytes(@".\Etats\HELP_DESK\" + Demande.DDP + "_" + Demande.DATE.ToString().Substring(0, 10).Replace("/", "-") + "_" + Demande.VOLET + "_" + Demande.MENU + " (" + Demande.INTITULE + ")" + ".jpg", b);
            return Ok();
        }

        // TELECHARGEMENT DE LA CAPTURE ECRAN (ERREUR)
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
                return new JsonResult("Capture d'incident non éxistante !");
            }
        }
    }
}
