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
    public class EtatSuiviPrecompteController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public EtatSuiviPrecompteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // LISTE DES DEMANDES DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeDemandes")]
        public IActionResult ListeDemandes(EtatSuiviPrecompte Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if ((Demande.STATUT == "TOUS") && (Demande.DDP == "" || Demande.DDP == "-------" || Demande.DDP == null))
                {
                    var test = myCon.Query("SELECT * FROM etat_precompte_demandes ORDER BY STATUT, DATE DESC", Demande).ToList();
                    return Ok(test);
                }
                if ((Demande.STATUT != "TOUS") && (Demande.DDP == "" || Demande.DDP == "-------" || Demande.DDP == null))
                {
                    var test = myCon.Query("SELECT * FROM etat_precompte_demandes where STATUT=  @STATUT ORDER BY STATUT, DATE DESC", Demande).ToList();
                    return Ok(test);
                }
                if ((Demande.STATUT != "TOUS") && (Demande.DDP != "" || Demande.DDP != null))
                {
                    var test = myCon.Query("SELECT * FROM etat_precompte_demandes where STATUT=  @STATUT AND DDP=  @DDP ORDER BY STATUT, DATE DESC", Demande).ToList();
                    return Ok(test);
                }
                if ((Demande.STATUT == "TOUS") && (Demande.DDP != "" && Demande.DDP != null))
                {
                    var test = myCon.Query("SELECT * FROM etat_precompte_demandes where DDP=  @DDP ORDER BY STATUT, DATE DESC", Demande).ToList();
                    return Ok(test);
                }
                //myCon.close();
                return Ok();
            }
        }

        // LISTE DES AGENTS SELON STATUT
        [HttpPost]
        [Route("ListeAgents")]
        public IActionResult ListeAgents(EtatSuiviPrecompte Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Demande.STATUT == "TOUS" || Demande.STATUT == null)
                {
                    var test = myCon.Query("SELECT DDP, NOM_PRENOM FROM etat_precompte_demandes GROUP BY DDP, NOM_PRENOM ORDER BY NOM_PRENOM", Demande).ToList();
                    test.Add(new { DDP = "-------", NOM_PRENOM="TOUS" });
                    return Ok(test);
                }
                if (Demande.STATUT != "TOUS" || Demande.STATUT != null)
                {
                    var test = myCon.Query("SELECT DDP, NOM_PRENOM FROM etat_precompte_demandes where STATUT=  @STATUT GROUP BY DDP, NOM_PRENOM  ORDER BY NOM_PRENOM", Demande).ToList();
                    test.Add(new { DDP = "-------", NOM_PRENOM = "TOUS" });
                    return Ok(test);
                }
                //myCon.close();
                return Ok();
            }
        }

        // UPLOAD DU PRECOMPTE
        [HttpPost]
        [Route("upload")]
        public IActionResult getfile(EtatSuiviPrecompte Demande)
        {
            //string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");

            Byte[] b;
            b = Convert.FromBase64String(Demande.nom_file);
            string fileoutput = (string)Demande.DDP + "_" + Demande.DATE.ToString().Substring(0, 10).Replace("/", "-") + "_" + Demande.TYPE + "_" + Demande.CODE_RUBRIQUE + " (" + (string)Demande.DDP_DEMANDEUR + ")";
            System.IO.File.WriteAllBytes(@".\Etats\SORTIE\ETAT_PRECOMPTE\" + fileoutput + ".pdf", b);
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string DATE_V = (string)Demande.DATE.ToString("yyyy-MM-dd");
                var t = myCon.Query(@"Update etat_precompte_demandes set STATUT = 'TRAITE' where TYPE=@TYPE AND CODE_RUBRIQUE=@CODE_RUBRIQUE AND DATE='" + DATE_V + @"' AND DDP=@DDP  AND DDP_DEMANDEUR=@DDP_DEMANDEUR", Demande).ToList();
            }
            PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
            var filename = @".\Etats\SORTIE\ETAT_PRECOMPTE\" + fileoutput + ".pdf";
            PdfSharp.Pdf.PdfDocument inputDocument = PdfReader.Open(filename, PdfDocumentOpenMode.Modify);
            int count = inputDocument.PageCount;
            for (int idx = 0; idx < count; idx++)
            {
                PdfPage page = inputDocument.Pages[idx];
                XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);
                XImage imageentete = XImage.FromFile(@".\Etats\PICS\EN_TETE.jpg");
                gfx.DrawImage(imageentete, 0, 0, page.Width, 90);
                XImage imagepied = XImage.FromFile(@".\Etats\PICS\PIED.jpg");
                gfx.DrawImage(imagepied, 0, page.Height-110, page.Width, 90);
                using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
                {
                    DateTime thisDay = DateTime.Today;
                    string DATE_TODAY = thisDay.ToString("yyyy-MM-dd");
                    int testsignature = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etats_signataires WHERE DDP='" + Demande.DDP + @"' AND DDP_SIGNATAIRE_OBLIGATOIRE<>''", Demande);
                    if (testsignature != 0)
                    {
                        var test = myCon.Query("SELECT * FROM etats_signataires WHERE DDP='" + Demande.DDP + @"'", Demande).ToList();
                        foreach (var rows in test)
                        {
                            var fields = rows as IDictionary<string, object>;
                            var DDP = fields["DDP_SIGNATAIRE_OBLIGATOIRE"];
                            XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\FR\POUR_DIRECTEUR\" + DDP + ".jpg");
                            gfx.DrawImage(imagesigne, 0, page.Height - 250, page.Width, 90);
                        }
                    }
                    else
                    {
                        var test = myCon.Query("SELECT * FROM etats_signataires WHERE ABSENCE_DU>'" + DATE_TODAY + @"' OR ABSENCE_AU<'" + DATE_TODAY + @"' OR ABSENCE_DU IS NULL ORDER BY ORDRE LIMIT 1", Demande).ToList();
                        foreach (var rows in test)
                        {
                            var fields = rows as IDictionary<string, object>;
                            var DDP = fields["DDP"];
                            XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\FR\POUR_DIRECTEUR\" + DDP + ".jpg");
                            gfx.DrawImage(imagesigne, 0, page.Height - 250, page.Width, 90);
                        }
                    }
                }
            }
            inputDocument.Save(filename);
            return Ok();
        }

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
                return new JsonResult("Etat Précompte non encore généré !");
            }
        }
    }
}
