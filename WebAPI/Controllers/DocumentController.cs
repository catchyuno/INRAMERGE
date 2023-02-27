using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using WebAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public DocumentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // LISTE DES CATEGORIES
        [HttpPost]
        [Route("ListeCategories")]
        public IActionResult ListeCategories(Document Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM documents_catégories ORDER BY CATEGORIE", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // NOM DE LA CATEGORIE SUR LE MENU
        [HttpPost]
        [Route("CategorieNom")]
        public IActionResult CategorieNom(Document Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM documents_catégories WHERE LINK='" + Demande.LINK + @"'", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES DOCUMENTS
        [HttpPost]
        [Route("Liste")]
        public IActionResult Liste(Document Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM documents WHERE CATEGORIE='" + Demande.CATEGORIE + @"' ORDER BY INTITULE", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }


        // LISTE DE TOUS LES DOCUMENT
        [HttpPost]
        [Route("ListeAll")]
        public IActionResult ListeAll(Document Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM documents ORDER BY CATEGORIE, INTITULE", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }


        // LISTE DES DOCUMENTS PAR CATEGORIE
        [HttpPost]
        [Route("ListeCat")]
        public IActionResult ListeCat(Document Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM documents WHERE CATEGORIE='" + Demande.CATEGORIE + "' ORDER BY INTITULE", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES CATEGORIES (AFFICHAGE DOCS)
        [HttpPost]
        [Route("ListeCatégories")]
        public IActionResult ListeCatégories(Document Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT distinct(CATEGORIE) FROM documents ORDER BY CATEGORIE", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // TELECHARGEMENT DU DOCUMENT EN FORMAT (PDF)
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
                return new JsonResult("Aucun document n'est enregistrée !");
            }
        }

        //UPLOAD DU DOCUMENT
        [HttpPost]
        [Route("upload")]
        public IActionResult getfile(Document Demande)
        {
            if (Demande.INTITULE == "")
            {
                return new JsonResult("Il faut saisir l'intitulé du document !");
                goto Sortir;
            }
            Byte[] b;
            b = Convert.FromBase64String(Demande.nom_file);
            System.IO.File.WriteAllBytes(@".\Etats\DOCUMENTS\" + Demande.CATEGORIE + "\\" + Demande.INTITULE + ".pdf", b);
            return Ok();
            Sortir: return new JsonResult("");
        }

        // MODIFICATION DU DOCUMENT
        [HttpPut]
        public JsonResult Put(Document Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string INTITULE_V = Demande.INTITULE.Replace("'", "_");
                string INTITULE_ANCIEN_V = Demande.INTITULE_ANCIEN.ToString().Replace("'", "_");
                if (Demande.INTITULE == "" || Demande.INTITULE == null)
                {
                    return new JsonResult("Il faut saisir un intitulé du document !");
                    goto Sortir;
                }
                int teste = myCon.ExecuteScalar<int>("Select COUNT(*) from documents where CATEGORIE='" + Demande.CATEGORIE + @"' AND INTITULE='" + INTITULE_V + @"'", Demande);
                if (teste != 0)
                {
                    goto cleanup;
                }
                //string INTITULE_V = Demande.INTITULE.Replace("'", "_");
                string targetPath = @".\Etats\DOCUMENTS\" + Demande.CATEGORIE;
                string fileoutput = (string)Demande.INTITULE.Replace("'", "_");
                string CHEMIN = targetPath + "\\" + fileoutput + ".pdf";
                string CHEMIN_V = CHEMIN.ToString().Replace("\\", "/");
                var ttt = myCon.Query(@"Update documents set INTITULE = '" + INTITULE_V + @"', NOM_FILE = '" + CHEMIN_V + @"' where CATEGORIE='" + Demande.CATEGORIE + @"' AND INTITULE = '" + INTITULE_ANCIEN_V + @"'", Demande).ToList();
               // System.IO.File.Delete(@".\Etats\DOCUMENTS\" + Demande.CATEGORIE + "\\" + Demande.INTITULE + ".pdf");
                System.IO.File.Move(@".\Etats\DOCUMENTS\" + Demande.CATEGORIE + "\\" + Demande.INTITULE_ANCIEN + ".pdf", @".\Etats\DOCUMENTS\" + Demande.CATEGORIE + "\\" + Demande.INTITULE + ".pdf");
                return new JsonResult("Mise à jour effectuée !");
            }
        cleanup: return new JsonResult("Intitulé déjà inscrit !");
        Sortir: return new JsonResult("");
        }

        // SUPPRESSION DU DOCUMENT
        [HttpPost]
        [Route("Delete")]
        public JsonResult Delete(Document Demande)
        {
            var INTITULE_V = Demande.INTITULE.Replace("'", "_");
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var t = myCon.Query(@"Delete from documents where CATEGORIE = '" + Demande.CATEGORIE + @"' AND INTITULE = '" + INTITULE_V + @"'", Demande).ToList();
                System.IO.File.Delete(@".\Etats\DOCUMENTS\" + Demande.CATEGORIE + "\\" + Demande.INTITULE.Replace("_","'") + ".pdf");
            }
            return new JsonResult("Suppression effectuée !");
        }

        // AJOUT DE DOCUMENT
        [HttpPost]
        [Route("Ajout")]
        public IActionResult Post(Document Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string INTITULE_V = Demande.INTITULE.Replace("'", "_");
                string targetPath = @".\Etats\DOCUMENTS\" + Demande.CATEGORIE;
                string fileoutput = (string)Demande.INTITULE.Replace("'","_");
                string CHEMIN = targetPath + "\\" + fileoutput + ".pdf";
                string CHEMIN_V = CHEMIN.ToString().Replace("\\", "/");
                if (Demande.INTITULE == "" || Demande.INTITULE == null)
                {
                    return new JsonResult("Il faut saisir un intitulé du document !");
                    goto Sortir;
                }
                int teste = myCon.ExecuteScalar<int>("Select count(*) from documents where CATEGORIE='" + Demande.CATEGORIE + @"' AND INTITULE='" + INTITULE_V + @"'", Demande);
                if (teste != 0)
                {
                    goto cleanup;
                }
                string query = "Insert into documents values  ('" + Demande.CATEGORIE + "', '" + INTITULE_V + "', '" + CHEMIN_V + "')";
                var result = myCon.Query(query).ToList();
                return new JsonResult("Ajout effectué !");
            }
        cleanup: return new JsonResult("Intitulé déjà inscrit !");
        Sortir: return new JsonResult("");
        }
    }
}
