using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using WebAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanqueController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public BanqueController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // LISTE DES BANQUES
        [HttpPost]
        [Route("Liste")]
        public IActionResult Liste(Banque Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM banque ORDER BY BANQUE_FR", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        [HttpPut]
        public JsonResult Put(Banque Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string BANQUE_FR_V = Demande.BANQUE_FR.ToString().Replace("'", "_");
                string BANQUE_FR_ANCIEN_V = Demande.BANQUE_FR_ANCIEN.ToString().Replace("'", "_");
                int teste = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM banque WHERE BANQUE_FR='" + BANQUE_FR_V + @"'", Demande);
                if (teste != 0)
                {
                    goto cleanup;
                }
                var t1 = myCon.Query(@"Update Banque set BANQUE_FR = '" + BANQUE_FR_V + @"', BANQUE_AR = '" + Demande.BANQUE_AR + @"' where BANQUE_FR = '" + BANQUE_FR_ANCIEN_V + @"'", Demande).ToList();
                var t2 = myCon.Query(@"Update banque_codes set BANQUE_FR = '" + BANQUE_FR_V + @"' where BANQUE_FR = '" + BANQUE_FR_ANCIEN_V + @"'", Demande).ToList();
                //myCon.close();
                return new JsonResult("Mise à jour effectuée !");
            }
        cleanup: return new JsonResult("Banque déjà inscrite !");
        }

        // SUPPRESSION DE LA BANQUE
        [HttpPost]
        [Route("Delete")]
        public JsonResult Delete(Banque Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string BANQUE_FR_V = Demande.BANQUE_FR.ToString().Replace("'", "_");
                int teste = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM banque_codes WHERE BANQUE_FR='" + BANQUE_FR_V + @"'", Demande);
                if (teste != 0)
                {
                    goto cleanup;
                }
                else
                {
                    var t = myCon.Query(@"Delete from Banque where BANQUE_FR = '" + BANQUE_FR_V + @"'", Demande).ToList();
                }
                //myCon.close();
            }
            return new JsonResult("Suppression effectuée !");
            cleanup: return new JsonResult("Suppression impossile, cette banque dispose dèjà de code banque !");
        }

        // AJOUT DE BANQUE
        [HttpPost]
        [Route("Ajout")]
        public IActionResult Post(Banque Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Demande.BANQUE_FR == "")
                {
                    return new JsonResult("Il faut saisir l'intitulé de la banque en francais !");
                    goto Sortir;
                }
                if (Demande.BANQUE_AR == "")
                {
                    return new JsonResult("Il faut saisir l'intitulé de la banque en arabe !");
                    goto Sortir;
                }
                string BANQUE_FR_V = Demande.BANQUE_FR.ToString().Replace("'", "_");
                int teste = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM banque WHERE BANQUE_FR='" + BANQUE_FR_V + @"'", Demande);
                if (teste != 0)
                {
                    goto cleanup;
                }
                string query = "Insert into banque values  ('" + BANQUE_FR_V + "', '" + Demande.BANQUE_AR + "')";
                var result = myCon.Query(query).ToList();
                return new JsonResult("Ajout effectué !");
                //myCon.close();
            }
        cleanup: return new JsonResult("Banque déjà inscrite !");
        Sortir: return new JsonResult("");
        }
    }
}