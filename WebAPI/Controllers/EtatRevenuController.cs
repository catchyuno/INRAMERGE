using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;
using Spire.Doc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Document = Spire.Doc.Document;
using DocXToPdfConverter;
using System.Data.SqlClient;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Microsoft.Office.Interop.Excel;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtatRevenuController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public EtatRevenuController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // LISTE DES DEMANDES DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeDemandes")]
        public IActionResult ListeDemandes(EtatRevenu Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM etat_revenu_demandes where DDP_DEMANDEUR =  @DDP_DEMANDEUR AND DDP =  @DDP ORDER BY DATE DESC", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES AGENTS SUR LISTE DEROULANTE SELON DROITS DE L'AGENT CONNECTE (AGENT / HIERARCHIE / ADMIN / GESTIONNAIRE)
        [HttpPost]
        [Route("ListeAgents")]
        public IActionResult ListeAgents(EtatRevenu Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var catégorie = myCon.Query("SELECT distinct(CATEGORIE) FROM etats_droits where MODULE='ETAT_REVENU' AND DDP =  @DDP", Demande).ToList();
                foreach (var rows in catégorie)
                {
                    var fields = rows as IDictionary<string, object>;
                    var CATEGORIE = fields["CATEGORIE"];
                    if ((string)CATEGORIE == "AGENT")
                    {
                        var test = myCon.Query("SELECT b.DDP,b.NOM_PRENOM,b.CATEGORIE FROM etat_revenu_infos a RIGHT JOIN etats_droits b ON a.DDP=b.DDP where b.DDP=@DDP AND b.MODULE='ETAT_REVENU' GROUP BY a.DDP,a.NOM_PRENOM,b.CATEGORIE order by a.NOM_PRENOM", Demande).ToList();
                        return Ok(test);
                    }
                    if ((string)CATEGORIE == "ADMIN")
                    {
                        var test = myCon.Query("SELECT DDP, NOM_PRENOM FROM etat_revenu_infos GROUP BY DDP, NOM_PRENOM ORDER BY NOM_PRENOM", Demande).ToList();
                        return Ok(test);
                    }
                    if ((string)CATEGORIE == "HIERARCHIE")
                    {
                        var DIB_REQ = myCongespers.Query("SELECT * FROM PERSONEL WHERE DOTI=@DDP", Demande).ToList();
                        foreach (var ligne_dib in DIB_REQ)
                        {
                            var fields_mdib = ligne_dib as IDictionary<string, object>;
                            var DIB = fields_mdib["MDIB"];
                            int RESP_REQ = myCongespers.ExecuteScalar<int>("SELECT * FROM FONCRESPS WHERE MDIB='" + DIB + @"' AND DFIN IS NULL", Demande);
                            if (RESP_REQ != 0)
                            {
                                var AFF_REQ = myCongespers.Query("SELECT * FROM AFFECTATIONS WHERE MDIB='" + DIB + @"' AND DFIN IS NULL", Demande).ToList();
                                foreach (var ligne_aff in AFF_REQ)
                                {
                                    var fields_aff = ligne_aff as IDictionary<string, object>;
                                    var AFF = fields_aff["AFFECTATION"];
                                    var AGENT_REQ = myCongespers.Query("SELECT a.MDIB,a.DOTI as DDP,a.NOM as NOM_PRENOM FROM PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' GROUP BY a.MDIB,a.DOTI,a.NOM order by a.NOM", Demande).ToList();
                                    var AGENT_REQ_2 = myCongespers.Query("SELECT a.MDIB,a.DOTI as DDP,a.NOM as NOM_PRENOM FROM PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' GROUP BY a.MDIB,a.DOTI,a.NOM order by a.NOM", Demande).ToList();
                                    AGENT_REQ_2.Clear();
                                    foreach (var row_AGENT_REQ in AGENT_REQ)
                                    {
                                        var fields_AGENT_REQ = row_AGENT_REQ as IDictionary<string, object>;
                                        var DDP = fields_AGENT_REQ["DDP"];
                                        var NOM_PRENOM = fields_AGENT_REQ["NOM_PRENOM"];
                                        var DIB_V = fields_AGENT_REQ["MDIB"];
                                        var AGENT_REQ_3 = myCongespers.Query("SELECT TOP 1 AFFECTATION as AFF FROM AFFECTATIONS WHERE MDIB='" + DIB_V + @"' AND AFFECTATION<>'255' ORDER BY nligne DESC", Demande).ToList();
                                        foreach (var row_AGENT_REQ_3 in AGENT_REQ_3)
                                        {
                                            var fields_AGENT_REQ_3 = row_AGENT_REQ_3 as IDictionary<string, object>;
                                            var AFF_2 = fields_AGENT_REQ_3["AFF"];
                                            decimal AFF_2_V = decimal.Parse(AFF_2.ToString());
                                            decimal AFF_V = decimal.Parse(AFF.ToString());
                                            decimal DDP_CONNECTE = decimal.Parse(Demande.DDP.ToString());
                                            decimal DDP_V = decimal.Parse(DDP.ToString());
                                            if (AFF_2_V == AFF_V )
                                            {
                                                if (DDP.ToString().Length == 6)
                                                {
                                                    DDP = "0" + DDP;
                                                }
                                                if (DDP.ToString().Length == 5)
                                                {
                                                    DDP = "00" + DDP;
                                                }
                                                int EXISTE_REVENU = myCon.ExecuteScalar<int>("SELECT * FROM etat_revenu_infos where DDP='" + DDP + @"'", Demande);
                                                if (EXISTE_REVENU != 0)
                                                {
                                                    if (DDP.ToString().Length == 7)
                                                    {
                                                        AGENT_REQ_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM });
                                                    }
                                                    if (DDP.ToString().Length == 6)
                                                    {
                                                        DDP = "0" + DDP;
                                                        AGENT_REQ_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM });
                                                    }
                                                    if (DDP.ToString().Length == 5)
                                                    {
                                                        DDP = "00" + DDP;
                                                        AGENT_REQ_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    return Ok(AGENT_REQ_2);
                                }
                            }
                        }
                    }
                        if ((string)CATEGORIE == "CUSTOM")
                        {
                            var AGENT_REQ_TOTAL = myCon.Query("SELECT * FROM etats_droits_custom WHERE MODULE='ETAT_REVENU' AND DDP=@DDP", Demande).ToList();
                            AGENT_REQ_TOTAL.Clear();
                            var AFF_REQ = myCon.Query("SELECT * FROM etats_droits_custom WHERE MODULE='ETAT_REVENU' AND DDP=@DDP", Demande).ToList();
                            foreach (var ligne_aff in AFF_REQ)
                            {
                                var fields_aff = ligne_aff as IDictionary<string, object>;
                                var AFF = fields_aff["CODE_AFF"];
                                var AGENT_REQ = myCongespers.Query("SELECT a.MDIB,a.DOTI as DDP,a.NOM as NOM_PRENOM FROM PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' GROUP BY a.MDIB,a.DOTI,a.NOM order by a.NOM", Demande).ToList();
                                var AGENT_REQ_2 = myCongespers.Query("SELECT a.MDIB,a.DOTI as DDP,a.NOM as NOM_PRENOM FROM PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' GROUP BY a.MDIB,a.DOTI,a.NOM order by a.NOM", Demande).ToList();
                                AGENT_REQ_2.Clear();
                                foreach (var row_AGENT_REQ in AGENT_REQ)
                                {
                                    var fields_AGENT_REQ = row_AGENT_REQ as IDictionary<string, object>;
                                    var DDP = fields_AGENT_REQ["DDP"];
                                    var NOM_PRENOM = fields_AGENT_REQ["NOM_PRENOM"];
                                    var DIB_V = fields_AGENT_REQ["MDIB"];
                                    var AGENT_REQ_3 = myCongespers.Query("SELECT TOP 1 AFFECTATION as AFF FROM AFFECTATIONS WHERE MDIB='" + DIB_V + @"' AND AFFECTATION<>'255' ORDER BY nligne DESC", Demande).ToList();
                                    foreach (var row_AGENT_REQ_3 in AGENT_REQ_3)
                                    {
                                        var fields_AGENT_REQ_3 = row_AGENT_REQ_3 as IDictionary<string, object>;
                                        var AFF_2 = fields_AGENT_REQ_3["AFF"];
                                        decimal AFF_2_V = decimal.Parse(AFF_2.ToString());
                                        decimal AFF_V = decimal.Parse(AFF.ToString());
                                        decimal DDP_CONNECTE = decimal.Parse(Demande.DDP.ToString());
                                        decimal DDP_V = decimal.Parse(DDP.ToString());
                                        if (AFF_2_V == AFF_V )
                                        {
                                            if (DDP.ToString().Length == 6)
                                            {
                                                DDP = "0" + DDP;
                                            }
                                            if (DDP.ToString().Length == 5)
                                            {
                                                DDP = "00" + DDP;
                                            }
                                            int EXISTE_REVENU = myCon.ExecuteScalar<int>("SELECT * FROM etat_revenu_infos where DDP='" + DDP + @"'", Demande);
                                            if (EXISTE_REVENU != 0)
                                            {
                                                if (DDP.ToString().Length == 7)
                                                {
                                                    AGENT_REQ_TOTAL.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM });
                                                }
                                                if (DDP.ToString().Length == 6)
                                                {
                                                    DDP = "0" + DDP;
                                                    AGENT_REQ_TOTAL.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM });
                                                }
                                                if (DDP.ToString().Length == 5)
                                                {
                                                    DDP = "00" + DDP;
                                                    AGENT_REQ_TOTAL.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM });
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            return Ok(AGENT_REQ_TOTAL);
                        }
                    }
                //myCon.close();
                return Ok();
            }
        }

        // LISTE DES ANNEES INSCRITES SUR LA BASE POUR L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("Annee")]
        public IActionResult ListeAnnees(EtatRevenu Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT distinct(ANNEE) FROM etat_revenu_infos where DDP =  @DDP order by ANNEE DESC", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // TELECHARGEMENT DE L'ETAT DE REVENU EN FORMAT (PDF)
        [HttpGet]
        [Route("file")]
        public IActionResult getfile(string nom_file)
        {
            var f = System.IO.File.ReadAllBytes(@nom_file);
            return Ok(f);
        }

        // AJOUT DE LA DEMANDE DE L'ETAT DE REVENU DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE ET GENERATION DE L'ETAT DE REVENU
        [HttpPost]
        [Route("Ajout")]
        public IActionResult Post(EtatRevenu Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Demande.ANNEE == "")
                {
                    return new JsonResult("Il faut choisir l'année !");
                    goto Sortir;
                }
                string DATE_V = (string)Demande.DATE.ToString("yyyy-MM-dd");
                int teste = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etat_revenu_demandes WHERE DATE='" + DATE_V + @"' AND DDP=@DDP  AND DDP_DEMANDEUR=@DDP_DEMANDEUR  AND ANNEE =  @ANNEE", Demande);
                if (teste != 0)
                {
                    goto cleanup;
                }
                string fileName = "REVENU.docx";
                string sourcePath = @".\Etats\";
                string targetPath = @".\Etats\SORTIE\ETAT_REVENU";
                string fileoutput = (string)Demande.DDP + "_" + (string)Demande.ANNEE + "_" + Demande.DATE.ToString().Substring(0, 10).Replace("/", "-") + " (" + (string)Demande.DDP_DEMANDEUR + ")";
                string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                string destFile = System.IO.Path.Combine(targetPath, fileoutput + ".docx");
                System.IO.Directory.CreateDirectory(targetPath);
                System.IO.File.Copy(sourceFile, destFile, true);
                
                var test = myCon.Query("Select * from etat_revenu_infos where ANNEE=@ANNEE and DDP=@DDP", Demande).ToList();
                foreach (var rows in test)
                {
                    int DDP_V = Convert.ToInt32(Demande.DDP);
                    var test0 = myCongespers.Query("SELECT * FROM PERSONEL WHERE DOTI='" + DDP_V + @"'", Demande).ToList();
                    foreach (var rows0 in test0)
                    {
                        var fields0 = rows0 as IDictionary<string, object>;
                        var DIB = fields0["MDIB"];
                        var CIN = fields0["CIN"];
                        var SEXE = fields0["SEXE"];
                       // var NOM_PRENOM = fields["NOM"];
                        var test7 = myCongespers.Query("SELECT GRADE FROM AVANCEMENTS where MDIB='" + DIB + @"' AND DFIN IS NULL", Demande).ToList();
                        foreach (var rows7 in test7)
                        {
                            var fields7 = rows7 as IDictionary<string, object>;
                            var GRADE = fields7["GRADE"];
                            var test8 = myCongespers.Query("SELECT LIBGRADE FROM GRADES WHERE GRADE='" + GRADE + @"'", Demande).ToList();
                            foreach (var rows8 in test8)
                            {
                                var fields8 = rows8 as IDictionary<string, object>;
                                var LIB_GRADE = fields8["LIBGRADE"];

                                if ((string)SEXE == "F")
                                {
                                    var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE + @"'", Demande).ToList();
                                    foreach (var rows_grade in test_grade)
                                    {
                                        var fields_grade = rows_grade as IDictionary<string, object>;
                                        LIB_GRADE = fields_grade["FRANCAIS_F"];
                                    }
                                }
                                if ((string)SEXE == "M")
                                {
                                    var test_grade = myCon.Query("SELECT * FROM etat_travail_langue WHERE CATEGORIE='GRADE' AND FRANCAIS_GESPERS ='" + LIB_GRADE + @"'", Demande).ToList();
                                    foreach (var rows_grade in test_grade)
                                    {
                                        var fields_grade = rows_grade as IDictionary<string, object>;
                                        LIB_GRADE = fields_grade["FRANCAIS_M"];
                                    }
                                }

                                //var LIB_GRADE_V = LIB_GRADE.ToString().Replace("'", "_");
                                var fields = rows as IDictionary<string, object>;
                                var ANNEE = fields["ANNEE"];
                                //var MOIS = fields["MOIS"];
                                var DDP = fields["DDP"];
                                var NOM_PRENOM = fields["NOM_PRENOM"];
                               // var CIN = fields["CIN"];
                               // var SEXE = fields["SEXE"];
                               // var GRADE = fields["GRADE"];
                                GRADE = LIB_GRADE.ToString().Substring(0, 1).ToUpper() + LIB_GRADE.ToString().Substring(1, LIB_GRADE.ToString().Length - 1).ToLower();
                                var INTERESSE = "";
                                
                                var IR = fields["IR"];
                                var NET = fields["NET"];
                                var BRUT_ANNUEL = fields["BRUT_ANNUEL"];
                                var BRUT_IMPOSABLE = fields["BRUT_IMPOSABLE"];
                                var RETENUES_PENSION_MUTUELLE = fields["RETENUES_PENSION_MUTUELLE"];
                                var NET_IMPOSABLE = fields["NET_IMPOSABLE"];

                                var testsf = myCongespers.Query("SELECT SITFAMILLE FROM PERSONEL WHERE DOTI='" + DDP + @"'", Demande).ToList();
                                foreach (var rowssf in testsf)
                                {
                                    var fieldssf = rowssf as IDictionary<string, object>;
                                    var SITUATION_FAM = fieldssf["SITFAMILLE"];
                                    Document doc = new Document();
                                    doc.LoadFromFile(destFile);
                                    doc.Replace("<DATE>", (string)DateTime.Now.ToString("dd/MM/yyyy"), true, true);
                                    doc.Replace("<ANNEE>", (string)ANNEE, true, true);
                                    doc.Replace("<DDP>", (string)DDP, true, true);
                                    doc.Replace("<CIN>", (string)CIN, true, true);
                                    if ((string)SEXE == "M")
                                    {
                                        NOM_PRENOM = "Monsieur " + NOM_PRENOM;
                                        INTERESSE = "l'intéressé";
                                    }
                                    else
                                    {
                                        if ((string)SITUATION_FAM == "M")
                                        {
                                            NOM_PRENOM = "Madame " + NOM_PRENOM;
                                            INTERESSE = "l'intéressée";
                                        }
                                        else
                                        {
                                            NOM_PRENOM = "Mademoiselle " + NOM_PRENOM;
                                            INTERESSE = "l'intéressée";
                                        }
                                    }
                                    decimal BRUT_ANNUEL_ARRONDI = decimal.Parse(BRUT_ANNUEL.ToString());
                                    decimal BRUT_IMPOSABLE_ARRONDI = decimal.Parse(BRUT_IMPOSABLE.ToString());
                                    decimal RETENUES_PENSION_MUTUELLE_ARRONDI = decimal.Parse(RETENUES_PENSION_MUTUELLE.ToString());
                                    decimal NET_IMPOSABLE_ARRONDI = decimal.Parse(NET_IMPOSABLE.ToString());
                                    decimal IR_ARRONDI = decimal.Parse(IR.ToString());
                                    decimal NET_ARRONDI = decimal.Parse(NET.ToString());
                                    doc.Replace("<BRUT_ANNUEL>", BRUT_ANNUEL_ARRONDI.ToString("#,0.00"), true, true);
                                    doc.Replace("<BRUT_IMPOSABLE>", BRUT_IMPOSABLE_ARRONDI.ToString("#,0.00"), true, true);
                                    doc.Replace("<RETENUES>", RETENUES_PENSION_MUTUELLE_ARRONDI.ToString("#,0.00"), true, true);
                                    doc.Replace("<NET_IMPOSABLE>", NET_IMPOSABLE_ARRONDI.ToString("#,0.00"), true, true);
                                    doc.Replace("<IR>", IR_ARRONDI.ToString("#,0.00"), true, true);
                                    doc.Replace("<NET>", NET_ARRONDI.ToString("#,0.00"), true, true);
                                    doc.Replace("<GRADE>", (string)GRADE, true, true);
                                    doc.Replace("<INTERESSE>", (string)INTERESSE, true, true);
                                    doc.Replace("<NOM_PRENOM>", (string)NOM_PRENOM, true, true);
                                    //doc.Replace("<ECHELON>", ECHELON.ToString(), true, true);
                                    //doc.Replace("<DP>", DP.ToString(), true, true);
                                    //doc.Replace("<FONCTION>", (string)FONCTION, true, true);
                                    //doc.Replace("<DF>", DF.ToString(), true, true);
                                    //doc.Replace("<NOTE>", NOTE.ToString(), true, true);
                                    doc.SaveToFile(destFile, FileFormat.Docx2013);
                                }
                            }
                        }
                    }
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
                var filename = @".\Etats\SORTIE\ETAT_REVENU\" + fileoutput + ".pdf";
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
                var result = myCon.Query("Insert into etat_revenu_demandes values (@DATE,@DDP,@NOM_PRENOM, @ANNEE, 'TRAITE', @DDP_DEMANDEUR,@NOM_PRENOM_DEMANDEUR, @CHEMIN)", new { Demande.DATE, Demande.DDP, Demande.NOM_PRENOM, Demande.ANNEE, Demande.DDP_DEMANDEUR, Demande.NOM_PRENOM_DEMANDEUR, CHEMIN }).ToList();
                //myCon.close();
                return Ok(result);
            }
        cleanup: return new JsonResult("Etat de revenu déjà éditée !");
        Sortir: return new JsonResult("");
        }
    }
}
