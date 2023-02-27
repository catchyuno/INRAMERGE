using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using WebAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpDeskSuiviController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public HelpDeskSuiviController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // LISTE DES ANOMALIES SOULEVEES SELON STATUT SELECTIONNE
        [HttpPost]
        [Route("Liste")]
        public IActionResult Liste(HelpDeskSuivi Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Demande.STATUT=="TOUS")
                {
                    var test = myCon.Query("SELECT * FROM help_desk ORDER BY STATUT, DATE DESC", Demande).ToList();
                    //myCon.close();
                    return Ok(test);
                }
                else
                {
                    var test = myCon.Query("SELECT * FROM help_desk where STATUT=@STATUT ORDER BY DATE DESC", Demande).ToList();
                    //myCon.close();
                    return Ok(test);
                }
            }
        }

        // UPDATE DE L'INCIDENT
        [HttpPut]
        public JsonResult Put(HelpDeskSuivi Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if ((Demande.STATUT=="TRAITE") && (Demande.REPONSE == "" || Demande.REPONSE == null))
                {
                    return new JsonResult("Il faut saisir la réponse !");
                    goto Sortir;
                }
                var ttt = myCon.Query(@"Update help_desk set STATUT = '" + Demande.STATUT + @"', REPONSE = '" + Demande.REPONSE.Replace("'", "_") + @"' where INTITULE = '" + Demande.INTITULE_ANCIEN + @"'", Demande).ToList();
                //myCon.close();
                return new JsonResult("Mise à jour effectuée !");
            }
            Sortir: return new JsonResult("");
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
