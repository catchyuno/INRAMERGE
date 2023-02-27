using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using WebAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodeBanqueController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public CodeBanqueController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // LISTE DES CODES DE BANQUES
        [HttpPost]
        [Route("Liste")]
        public IActionResult Liste(CodeBanque Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var ret = myCon.Query("SELECT substring(rib, 1, 3) as RIB FROM rib GROUP BY substring(rib, 1, 3)", Demande).ToList();
                foreach (var rowsret in ret)
                {
                    var fieldsret = rowsret as IDictionary<string, object>;
                    var RIB = fieldsret["RIB"];
                    int teste = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM banque_codes WHERE CODE='" + RIB + @"'", Demande);
                    if (teste == 0)
                    {
                        string query = "Insert into banque_codes values  ('','', '" + RIB + "')";
                        var result = myCon.Query(query).ToList();
                    }
                }
                if (Demande.BANQUE_FR != null)
                {
                    string BANQUE_FR_V = Demande.BANQUE_FR;
                    var test = myCon.Query("Select * from banque_codes where BANQUE_FR='" + BANQUE_FR_V + @"' ORDER BY CODE", Demande).ToList();
                    //myCon.close();
                    return Ok(test);
                }
                else
                {
                    var test = myCon.Query("SELECT * FROM banque_codes ORDER BY BANQUE_FR, CODE", Demande).ToList();
                    //myCon.close();
                    return Ok(test);
                }
            }
        }

        [HttpPost]
        [Route("Liste_sansmaj")]
        public IActionResult Liste_sansmaj(CodeBanque Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Demande.BANQUE_FR != null)
                {
                    string BANQUE_FR_V = Demande.BANQUE_FR;
                    var test = myCon.Query("Select * from banque_codes where BANQUE_FR='" + BANQUE_FR_V + @"' ORDER BY CODE", Demande).ToList();
                    //myCon.close();
                    return Ok(test);
                }
                else
                {
                    var test = myCon.Query("SELECT * FROM banque_codes ORDER BY BANQUE_FR, CODE", Demande).ToList();
                    //myCon.close();
                    return Ok(test);
                }
            }
        }

        // LISTE DES BANQUES
        [HttpPost]
        [Route("ListeBanque")]
        public IActionResult ListeBanque(CodeBanque Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM banque_codes where BANQUE_FR<>'' ORDER BY BANQUE_FR", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // MISE A JOUR DU CODE DE BANQUE
        [HttpPut]
        public JsonResult Put(CodeBanque Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Demande.BANQUE_FR == "")
                {
                    return new JsonResult("Il faut choisir une banque !");
                    goto Sortir;
                }
                string BANQUE_FR_V = Demande.BANQUE_FR.ToString().Replace("'", "_");
                var ttt = myCon.Query(@"Update banque_codes set BANQUE_FR = '" + BANQUE_FR_V + @"', BANQUE_AR = '" + Demande.BANQUE_AR + @"' where CODE = '" + Demande.CODE + @"'", Demande).ToList();
                //myCon.close();
                return new JsonResult("Mise à jour effectuée !");
                Sortir: return new JsonResult("");
            }
        }

        // AFFICHAGE DE LA BANQUE SELECTIONNEE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("Banque")]
        public IActionResult Banque(CodeBanque Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Demande.BANQUE_FR != null)
                {
                    string BANQUE_FR_V = Demande.BANQUE_FR;
                    var test = myCon.Query("Select * from banque_codes where BANQUE_FR='" + BANQUE_FR_V + @"' ORDER BY CODE", Demande).ToList();
                    //myCon.close();
                    return Ok(test);
                }
                else
                {
                    var test = myCon.Query("SELECT * FROM banque_codes ORDER BY BANQUE_FR, CODE", Demande).ToList();
                    //myCon.close();
                    return Ok(test);
                }
            }
        }

        // AFFICHAGE DE LA BANQUE ARABE SUR LE MODAL
        [HttpPost]
        [Route("BanqueAR")]
        public IActionResult BanqueAR(CodeBanque Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Demande.BANQUE_FR != null)
                {
                    var test = myCon.Query("Select * from banque where BANQUE_FR=@BANQUE_FR", Demande).ToList();
                    //myCon.close();
                    return Ok(test);
                }
                else
                {
                    var test = myCon.Query("SELECT * FROM banque ORDER BY BANQUE_FR, CODE", Demande).ToList();
                    //myCon.close();
                    return Ok(test);
                }

            }
        }
    }
}