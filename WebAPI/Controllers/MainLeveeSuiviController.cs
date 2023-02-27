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
using PdfSharp.Drawing;
using System.Data.SqlClient;
using Spire.Doc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Document = Spire.Doc.Document;
using DocXToPdfConverter;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainLeveeSuiviController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public MainLeveeSuiviController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // LISTE DES DEMANDES DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeDemandes")]
        public IActionResult ListeDemandes(MainLeveeSuivi Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if ((Demande.ETAT == "TOUS") && (Demande.DDP == "" || Demande.DDP == "-------" || Demande.DDP == null))
                {
                    var test = myCon.Query("SELECT * FROM etat_domiciliation_main_levée ORDER BY ETAT, DATE DESC", Demande).ToList();
                    return Ok(test);
                }
                if ((Demande.ETAT != "TOUS") && (Demande.DDP == "" || Demande.DDP == "-------" || Demande.DDP == null))
                {
                    var test = myCon.Query("SELECT * FROM etat_domiciliation_main_levée where ETAT=  @ETAT ORDER BY ETAT, DATE DESC", Demande).ToList();
                    return Ok(test);
                }
                if ((Demande.ETAT != "TOUS") && (Demande.DDP != "" || Demande.DDP != null))
                {
                    var test = myCon.Query("SELECT * FROM etat_domiciliation_main_levée where ETAT=  @ETAT AND DDP=  @DDP ORDER BY ETAT, DATE DESC", Demande).ToList();
                    return Ok(test);
                }
                if ((Demande.ETAT == "TOUS") && (Demande.DDP != "" && Demande.DDP != null))
                {
                    var test = myCon.Query("SELECT * FROM etat_domiciliation_main_levée where DDP=  @DDP ORDER BY ETAT, DATE DESC", Demande).ToList();
                    return Ok(test);
                }
                //myCon.close();
                return Ok();
            }
        }

        // LISTE DES AGENTS SELON STATUT
        [HttpPost]
        [Route("ListeAgents")]
        public IActionResult ListeAgents(MainLeveeSuivi Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Demande.ETAT == "TOUS" || Demande.ETAT == null)
                {
                    var test = myCon.Query("SELECT DDP, NOM_PRENOM FROM etat_domiciliation_main_levée GROUP BY DDP, NOM_PRENOM ORDER BY NOM_PRENOM", Demande).ToList();
                    test.Add(new { DDP = "-------", NOM_PRENOM = "TOUS" });
                    return Ok(test);
                }
                if (Demande.ETAT != "TOUS" || Demande.ETAT != null)
                {
                    var test = myCon.Query("SELECT DDP, NOM_PRENOM FROM etat_domiciliation_main_levée where ETAT=  @ETAT GROUP BY DDP, NOM_PRENOM  ORDER BY NOM_PRENOM", Demande).ToList();
                    test.Add(new { DDP = "-------", NOM_PRENOM = "TOUS" });
                    return Ok(test);
                }
                //myCon.close();
                return Ok();
            }
        }


        // VALIDATION/INVALIDATION DE LA MAINLEVEE
        [HttpPost]
        [Route("Ajout")]
        public IActionResult Post(MainLeveeSuivi Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string DATE_V = (string)Demande.DATE.ToString("yyyy-MM-dd");
                if (Demande.ETAT=="EN COURS")
                {
                    var t = myCon.Query(@"Update etat_domiciliation_main_levée set ETAT = 'TRAITE' where DDP=@DDP AND RIB=@RIB AND DATE='" + DATE_V + "'", Demande).ToList();
                    var t1 = myCon.Query(@"Update etat_domiciliation_demandes set ETAT = 'CLOTURE' where DDP=@DDP AND RIB=@RIB", Demande).ToList();
                }
                else
                {
                    var t = myCon.Query(@"Update etat_domiciliation_main_levée set ETAT = 'EN COURS' where DDP=@DDP AND RIB=@RIB AND DATE='" + DATE_V + "'", Demande).ToList();
                    var t1 = myCon.Query(@"Update etat_domiciliation_demandes set ETAT = 'EN COURS' where DDP=@DDP AND RIB=@RIB", Demande).ToList();
                }
                //myCon.close();
                return new JsonResult("Mise à jour effectuée !");
            }
            return Ok();
        }

        [HttpGet]
        [Route("file")]
        public IActionResult getfile(string nom_file)
        {
            var f = System.IO.File.ReadAllBytes(@nom_file);
            return Ok(f);
        }
    }
}
