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
    public class SalaireRubriqueController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SalaireRubriqueController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // LISTE DES RUBRIQUES
        [HttpPost]
        [Route("ListeRubrique")]
        public IActionResult ListeRubrique(SalaireRubrique Rubrique)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM salaire_rubriques ORDER BY CATEGORIE, RUBRIQUE", Rubrique).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // AFFICHAGE DE LA RUBRIQUE SELECTIONNEE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("Rubriqu")]
        public IActionResult Rubriqu(SalaireRubrique Rubrique)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Rubrique.CODE_RUBRIQUE != null)
                {
                    var test = myCon.Query("Select * from salaire_rubriques where CODE_RUBRIQUE=@CODE_RUBRIQUE", Rubrique).ToList();
                    //myCon.close();
                    return Ok(test);
                }
                else
                {
                    var test = myCon.Query("SELECT * FROM salaire_rubriques ORDER BY CATEGORIE, RUBRIQUE", Rubrique).ToList();
                    //myCon.close();
                    return Ok(test);
                }
            }
        }

        // AFFICHAGE DES RUBRIQUES SELON LA CATEGORIE CHOISIE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("Categorie")]
        public IActionResult Categorie(SalaireRubrique Rubrique)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Rubrique.CATEGORIE == null || Rubrique.CATEGORIE == "" && Rubrique.CATEGORIE != "TOUS")
                {
                    var test = myCon.Query("Select * from salaire_rubriques where CATEGORIE='' or CATEGORIE is null ORDER BY RUBRIQUE", Rubrique).ToList();
                    //myCon.close();
                    return Ok(test);
                }
                else if (Rubrique.CATEGORIE != null && Rubrique.CATEGORIE != "" && Rubrique.CATEGORIE != "TOUS")
                {
                    var test = myCon.Query("SELECT * FROM salaire_rubriques where CATEGORIE=@CATEGORIE ORDER BY RUBRIQUE, RUBRIQUE", Rubrique).ToList();
                    //myCon.close();
                    return Ok(test);
                }
                else
                {
                    var test = myCon.Query("SELECT * FROM salaire_rubriques ORDER BY RUBRIQUE, RUBRIQUE", Rubrique).ToList();
                    //myCon.close();
                    return Ok(test);
                }
            }

        }
        // MISE A JOUR DE LA RUBRIQUE
        [HttpPut]
        public JsonResult Put(SalaireRubrique Rubrique)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Rubrique.RUBRIQUE_ABBREVEE == "")
                {
                    return new JsonResult("Il faut saisir une abbréviation de la rubrique !");
                    goto Sortir;
                }
                string RUBRIQUE_ABBREVEE_V = Rubrique.RUBRIQUE_ABBREVEE.ToString().Replace("'", "_");
                var test = myCon.Query("Update salaire_rubriques set RUBRIQUE_ABBREVEE='" + RUBRIQUE_ABBREVEE_V + @"', CATEGORIE='" + Rubrique.CATEGORIE + @"' where CODE_RUBRIQUE=@CODE_RUBRIQUE", Rubrique).ToList();
                //myCon.close();
                return new JsonResult("Mise à jour effectuée !");
                Sortir: return new JsonResult("");
            }
        }
    }
}
