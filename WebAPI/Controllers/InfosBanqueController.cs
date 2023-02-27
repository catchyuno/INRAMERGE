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
using System.Globalization;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfosBanqueController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public InfosBanqueController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // EDITION DE LA LETTRE D'INFORMATION BANCAIRE
        [HttpPost]
        [Route("Ajout")]
        public IActionResult Post(InfosBanque Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string fileName = "";
                fileName = "INFOS_BANQUE.docx";
                string sourcePath = @".\Etats\";
                string targetPath = @".\Etats\SORTIE\INFOS_BANQUE";
                string fileoutput = (string)Demande.DDP;
                //string fileoutput = (string)Demande.DDP + "_" + (string)Demande.ANNEE + "_" + (string)Demande.PRIME + "_" + Demande.DATE.ToString().Substring(0, 10).Replace("/", "-") + " (" + (string)Demande.DDP_DEMANDEUR + ")";
                string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                string destFile = System.IO.Path.Combine(targetPath, fileoutput + ".docx");
                System.IO.Directory.CreateDirectory(targetPath);
                System.IO.File.Copy(sourceFile, destFile, true);
                var test = myCon.Query("Select * from etat_domiciliation_infos where DDP=@DDP", Demande).ToList();
                foreach (var rows in test)
                {
                    var fields = rows as IDictionary<string, object>;
                    var BANQUE = fields["BANQUE"];
                    var DATE = fields["DATE"];
                    var RIB = fields["RIB"];
                    var DDP = fields["DDP"];
                    var NOM_PRENOM = fields["NOM_PRENOM"].ToString().Replace("_", "'");
                    var CIN = fields["CIN"];
                    var MOTIF_SORTIE = fields["MOTIF_SORTIE"];
                    if ((string)MOTIF_SORTIE == "DECES" || (string)MOTIF_SORTIE == "LICENCIEMENT" || (string)MOTIF_SORTIE == "ABONDAN")
                    {
                        MOTIF_SORTIE = "son " + MOTIF_SORTIE;
                    }
                    else
                    {
                        MOTIF_SORTIE = "sa " + MOTIF_SORTIE;
                    }
                    var SEXE = fields["SEXE"];
                    if ((string)SEXE == "M")
                    {
                        NOM_PRENOM = "Monsieur " + NOM_PRENOM;
                    }
                    else
                    {
                        NOM_PRENOM = "Madame " + NOM_PRENOM;
                    }
                    Document doc = new Document();
                    doc.LoadFromFile(destFile);
                    doc.Replace("<DATE_TODAY>", (string)DateTime.Now.ToString("dd/MM/yyyy"), true, true);
                    doc.Replace("<BANQUE>", (string)BANQUE, true, true);
                    doc.Replace("<RIB>", (string)RIB, true, true);
                    doc.Replace("<DDP>", (string)DDP, true, true);
                    doc.Replace("<MOTIF_SORTIE>", (string)MOTIF_SORTIE.ToString().ToLower(), true, true);
                    //DateTime dt = DateTime.ParseExact(DATE, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    DATE= (String)DATE.ToString().Substring(0, 2) + "/" + (String)DATE.ToString().Substring(3, 2) + "/" + (String)DATE.ToString().Substring(6,4);
                    doc.Replace("<DATE>", (string)DATE, true, true);
                    doc.Replace("<NOM_PRENOM>", (string)NOM_PRENOM, true, true);
                    if (CIN != null)
                    {
                        doc.Replace("<CIN>", (string)CIN, true, true);
                    }
                    else
                    {
                        doc.Replace("<CIN>", "", true, true);
                    }
                    doc.SaveToFile(destFile, FileFormat.Docx2013);
                }
                using (WordprocessingDocument doc2 = WordprocessingDocument.Open(destFile, true))
                {
                    var body = doc2.MainDocumentPart.Document.Body;
                    foreach (var text in body.Descendants<Text>())
                    {
                        if (text.Text.Contains("Evaluation Warning: The document was created with Spire.Doc for .NET."))
                        {
                            text.Text = text.Text.Replace("Evaluation Warning: The document was created with Spire.Doc for .NET.", "");
                        }
                    }
                }
                string locationOfLibreOfficeSoffice = sourcePath + "\\" + "LibreOfficePortable\\App\\libreoffice\\program\\soffice.exe";
                var tes = new ReportGenerator(locationOfLibreOfficeSoffice);
                tes.Convert(destFile, targetPath + "\\" + fileoutput + ".pdf");
                System.IO.File.Delete(destFile);
                string CHEMIN = targetPath + "\\" + fileoutput + ".pdf";
                PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
                var filename = @".\Etats\SORTIE\INFOS_BANQUE\" + fileoutput + ".pdf";
                PdfSharp.Pdf.PdfDocument inputDocument = PdfReader.Open(filename, PdfDocumentOpenMode.Modify);
                int count = inputDocument.PageCount;
                for (int idx = 0; idx < count; idx++)
                {
                    PdfPage page = inputDocument.Pages[idx];
                    XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);
                    XImage imageentete = XImage.FromFile(@".\Etats\PICS\EN_TETE.jpg");
                    gfx.DrawImage(imageentete, 0, 0, page.Width, 90);
                    DateTime thisDay = DateTime.Today;
                    string DATE_TODAY = thisDay.ToString("yyyy-MM-dd");
                    int testsignature = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etats_signataires WHERE DDP='" + Demande.DDP + @"' AND DDP_SIGNATAIRE_OBLIGATOIRE<>''", Demande);
                    if (testsignature != 0)
                    {
                        var testsig = myCon.Query("SELECT * FROM etats_signataires WHERE DDP='" + Demande.DDP + @"'", Demande).ToList();
                        foreach (var rows in testsig)
                        {
                            var fields = rows as IDictionary<string, object>;
                            var DDP = fields["DDP_SIGNATAIRE_OBLIGATOIRE"];
                            XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\FR\POUR_DIRECTEUR\" + DDP + ".jpg");
                            gfx.DrawImage(imagesigne, 0, page.Height - 240, page.Width, 110);
                        }
                        XImage imagepied = XImage.FromFile(@".\Etats\PICS\PIED.jpg");
                        gfx.DrawImage(imagepied, 0, page.Height - 80, page.Width, 80);
                    }
                    else
                    {
                        var testsig = myCon.Query("SELECT * FROM etats_signataires WHERE ABSENCE_DU>'" + DATE_TODAY + @"' OR ABSENCE_AU<'" + DATE_TODAY + @"' OR ABSENCE_DU IS NULL ORDER BY ORDRE LIMIT 1", Demande).ToList();
                        foreach (var rows in testsig)
                        {
                            var fields = rows as IDictionary<string, object>;
                            var DDP = fields["DDP"];
                            XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\FR\POUR_DIRECTEUR\" + DDP + ".jpg");
                            gfx.DrawImage(imagesigne, 0, page.Height - 240, page.Width, 110);
                        }
                        XImage imagepied = XImage.FromFile(@".\Etats\PICS\PIED.jpg");
                        gfx.DrawImage(imagepied, 0, page.Height - 80, page.Width, 80);
                    }
                }
                inputDocument.Save(filename);

                string CHEMIN_V = CHEMIN.ToString().Replace("\\", "/");
                // UPADTE TABLE DOM (INFOS=OUI & STATUT=CLOTURE)
                // UPDATE TABLE INFOS (STATUT=CLOTURE & UPDATE ALL AVEC DDP)
                //string query = "UPDATE etat_domiciliation_infos values SET STATUT='TRAITE', NOM_FILE='" + CHEMIN + "' WHERE DDP='" + Demande.DDP + "'";
                //var result = myCon.Query(query).ToList();
                var t = myCon.Query(@"Update etat_domiciliation_infos set STATUT = 'TRAITE', NOM_FILE='" + CHEMIN_V + "' where DDP=@DDP", Demande).ToList();
                var t1 = myCon.Query(@"Update etat_domiciliation_demandes set ETAT = 'CLOTURE', INFOS_BANQUE='OUI' where DDP=@DDP", Demande).ToList();

                //myCon.close();
                //string query1 = "Insert into etat_domiciliation_demandes SET STATUT='CLOTURE', INFOS_BANQUE='OUI' WHERE DDP='" + Demande.DDP + "')";
                //var result1 = myCon.Query(query1).ToList();

                //var result = myCon.Query("Insert into etat_prime_demandes values (@DATE,@DDP,@NOM_PRENOM, @ANNEE, @PRIME, 'TRAITE', @DDP_DEMANDEUR,@NOM_PRENOM_DEMANDEUR, @CHEMIN)", new { Demande.DATE, Demande.DDP, Demande.NOM_PRENOM, Demande.ANNEE, Demande.PRIME, Demande.DDP_DEMANDEUR, Demande.NOM_PRENOM_DEMANDEUR, CHEMIN }).ToList();
                //return Ok(result);
            }
            // cleanup: return new JsonResult("Etat de prime déjà éditée !");
            
            return Ok();
        }
        // }



        [HttpGet]
        [Route("file")]
        public IActionResult getfile(string nom_file)
        {
            //if (System.IO.File.Exists(@nom_file))
            //{
                var f = System.IO.File.ReadAllBytes(@nom_file);
                return Ok(f);
            //}
            //else
            //{
            //    return new JsonResult("Etat Engagement non encore généré !");
            //}
        }

        // LISTE DES INFOS
        [HttpPost]
        [Route("ListeAgents")]
        public IActionResult ListeAgents(InfosBanque Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Demande.STATUT == "TOUS")
                {
                    var test = myCon.Query("SELECT * FROM etat_domiciliation_infos ORDER BY DDP", Demande).ToList();
                    return Ok(test);
                }
                if (Demande.STATUT == "EN COURS")
                {
                    var test = myCon.Query("SELECT * FROM etat_domiciliation_infos where STATUT='EN COURS' ORDER BY DDP", Demande).ToList();
                    return Ok(test);
                }
                if (Demande.STATUT == "TRAITE")
                {
                    var test = myCon.Query("SELECT * FROM etat_domiciliation_infos where STATUT='TRAITE' ORDER BY DDP", Demande).ToList();
                    return Ok(test);
                }
                //myCon.close();
                return Ok();
            }
        }

        // LISTE DES AGENTS
        [HttpPost]
        [Route("MAJ_InfosBanque")]
        public IActionResult MAJ_InfosBanque(InfosBanque Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                //var AGENT_REQ_TOTAL = myCon.Query("SELECT * FROM etat_domiciliation_demandes WHERE ETAT='EN COURS'", Demande).ToList();
                //AGENT_REQ_TOTAL.Clear();
                var test = myCon.Query("SELECT * FROM etat_domiciliation_demandes WHERE ETAT='EN COURS'", Demande).ToList();
                foreach (var row in test)
                {
                    var fields = row as IDictionary<string, object>;
                    var DATE = fields["DATE"];
                    var DDP = fields["DDP"];
                    var NOM_PRENOM = fields["NOM_PRENOM"];
                    var SEXE = fields["SEXE"];
                    var CIN = fields["CIN"];
                    var BANQUE = fields["BANQUE"];
                    var MOTIF_SORTIE = fields["MOTIF_SORTIE"];
                    var RIB = fields["RIB"];
                    int test_infos = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etat_domiciliation_infos WHERE DDP='" + DDP + @"'", Demande);
                    if (test_infos != 0)
                    {
                        goto Sortir;
                    }
                    // var test_sortie = myCon.Query("SELECT * FROM etat_domiciliation_infos WHERE DDP='" + DDP + @"'", Demande).ToList();
                    DateTime thisDay = DateTime.Today;
                    string DATE_TODAY = thisDay.ToString("yyyy-MM-dd");
                    // string DATE_REFERENCE = thisDay.ToString("yyyy") + "-01-01";
                    decimal DDP_CONNECTE = decimal.Parse(DDP.ToString());
                    var test1 = myCongespers.Query("SELECT MDIB, DOTI, POSADMIN, DSORTIE FROM dbo.personel WHERE DOTI='" + DDP_CONNECTE + @"'", Demande).ToList();
                    foreach (var rows1 in test1)
                    {
                        var fields1 = rows1 as IDictionary<string, object>;
                        var DATE_SORTIE_V = fields1["DSORTIE"];
                        var POSITION = fields1["POSADMIN"];
                        CultureInfo culture = new CultureInfo("en-US");
                        DateTime tempDate = Convert.ToDateTime(DATE_SORTIE_V, culture);

                        //DateTime DATE_SORTIE_VV = DateTime.Parse((string)DATE_SORTIE_V);
                        if (DATE_SORTIE_V != null)
                        {
                            if (tempDate <= thisDay)
                            {
                                string DATE_V = DATE.ToString().Substring(6, 4) + "-" + DATE.ToString().Substring(3, 2) + "-" + DATE.ToString().Substring(0, 2);
                                var test3 = myCongespers.Query("SELECT * FROM POSAD WHERE CODE='" + POSITION + @"'", Demande).ToList();
                                foreach (var rows3 in test3)
                                {
                                    var fields3 = rows3 as IDictionary<string, object>;
                                    var POSITION_ADM = fields3["POSADMIN"];
                                    if (POSITION_ADM == null)
                                    {
                                        POSITION_ADM = "";
                                    }
                                    string query = "Insert into etat_domiciliation_infos values  ('" + DATE_V + "', '" + DDP + "', '" + NOM_PRENOM + "','" + POSITION_ADM + "', '" + SEXE + "', '" + CIN + "','" + BANQUE + "','" + RIB + "','EN COURS', '')";
                                    var result = myCon.Query(query).ToList();
                                }
                            }
                        }
                    }
                Sortir:;
                }
                //myCon.close();
            }
            return Ok();
        }
    }
}
