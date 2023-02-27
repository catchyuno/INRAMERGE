using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Data.SqlClient;
using Spire.Doc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Document = Spire.Doc.Document;
using DocXToPdfConverter;
using PdfSharp.Drawing;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CINController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public CINController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // AJOUT DE CIN
        [HttpPost]
        [Route("Update_CIN")]
        public IActionResult Post(CINN Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string ADRESSE_V = Demande.ADRESSE.Replace("'", "_");
                string NOM_PRENOM_V = Demande.NOM_PRENOM.Replace("'", "_");

                //string targetPath = @".\Etats\CIN\" + Demande.DDP;
                //string fileoutput = (string)Demande.DDP;
                //string CHEMIN = targetPath + ".pdf";
                //string CHEMIN_V = CHEMIN.ToString().Replace("\\", "/");
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
                //int teste = myCon.ExecuteScalar<int>("Select count(*) from cin_maj where DDP='" + Demande.DDP + @"' AND VALIDE='OUI'", Demande);
                //if (teste != 0)
                //{
                //    goto cleanup;
                //}
                var t = myCon.Query(@"Delete from cin_maj where DDP = '" + Demande.DDP + "'", Demande).ToList();
                //System.IO.File.Delete(@".\Etats\CIN\" + Demande.DDP + ".pdf");
                string query = "Insert into cin_maj values  ('" + Demande.DDP + "', '" + NOM_PRENOM_V + "', '" + Demande.NOMAR + "', '" + Demande.CIN + "', '" + ADRESSE_V + "', 'OUI')";
                var result = myCon.Query(query).ToList();

                int DDP_V = Convert.ToInt32(Demande.DDP);
                string query1 = "Update PERSONEL set NOM='" + NOM_PRENOM_V + "', NOMAR = '" + Demande.NOMAR + "', CIN = '" + Demande.CIN + "', ADRESSE = '" + ADRESSE_V + "' where DOTI = '" + DDP_V + "'";
                var result1 = myCongespers.Query(query1).ToList();

                //var ttt = myCongespers.Query(@"Update PERSONEL set NOM='" + NOM_PRENOM_V + "', NOMAR = '" + Demande.NOMAR + "', CIN = '" + Demande.CIN + "', ADRESSE = '" + ADRESSE_V + "' where DOTI = '" + DDP_V + @"'", Demande).ToList();
                //myCon.close();
                return new JsonResult("Mise à jour effectuée !");


                //return new JsonResult("Ajout effectué !");
                
            }
        //cleanup: return new JsonResult("CIN déjà ajoutée et validée par les RH !");
        Sortir: return new JsonResult("");
        }

        // LISTE DES CINS
        [HttpPost]
        [Route("ListeDemandes")]
        public IActionResult ListeDemandes(CINN Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Demande.VALIDE == "TOUS")
                {
                    var test = myCon.Query("SELECT * FROM cin_maj ORDER BY DDP", Demande).ToList();
                    return Ok(test);
                }
                if (Demande.VALIDE == "VALIDES")
                {
                    var test = myCon.Query("SELECT * FROM cin_maj where VALIDE='OUI' ORDER BY DDP", Demande).ToList();
                    return Ok(test);
                }
                if (Demande.VALIDE == "NON VALIDES")
                {
                    var test = myCon.Query("SELECT * FROM cin_maj where VALIDE='NON' ORDER BY DDP", Demande).ToList();
                    return Ok(test);
                }
                //myCon.close();
                return Ok();
            }
        }

        // TELECHARGEMENT DE LA CIN
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
                return new JsonResult("CIN non mise en ligne par l'agent !");
            }
        }
    }
}
