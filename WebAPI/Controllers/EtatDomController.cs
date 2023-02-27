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
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtatDomController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public EtatDomController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // LISTE DES DEMANDES DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeDemandes")]
        public IActionResult ListeDemandes(EtatDom Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM etat_domiciliation_demandes where DDP_DEMANDEUR =  @DDP_DEMANDEUR AND DDP =  @DDP ORDER BY DATE DESC", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES DEMANDES DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeDemandesGest")]
        public IActionResult ListeDemandesGest(EtatDom Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM etat_domiciliation_demandes ORDER BY DATE DESC", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES AGENTS SUR LISTE DEROULANTE SELON DROITS DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeAgents")]
        public IActionResult ListeAgents(EtatDom Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var catégorie = myCon.Query("SELECT distinct(CATEGORIE) FROM etats_droits where MODULE='ETAT_DOM' AND DDP =  @DDP", Demande).ToList();
                foreach (var rows in catégorie)
                {
                    var fields = rows as IDictionary<string, object>;
                    var CATEGORIE = fields["CATEGORIE"];
                    if ((string)CATEGORIE == "AGENT")
                    {
                        var test = myCon.Query("SELECT a.DDP,a.NOM_PRENOM,b.CATEGORIE FROM rib a JOIN etats_droits b ON a.DDP=b.DDP where a.DDP=@DDP AND b.MODULE='ETAT_DOM' GROUP BY a.DDP,a.NOM_PRENOM,b.CATEGORIE order by a.NOM_PRENOM", Demande).ToList();
                        return Ok(test);
                    }
                    if ((string)CATEGORIE == "ADMIN" || (string)CATEGORIE == "GESTIONNAIRE")
                    {
                        var test = myCongespers.Query("SELECT DOTI as DDP, NOM as NOM_PRENOM FROM PERSONEL WHERE DOTI is not null and DSORTIE IS NULL ORDER BY NOM", Demande).ToList();
                        var test_2 = myCongespers.Query("SELECT DOTI as DDP, NOM as NOM_PRENOM FROM PERSONEL WHERE DOTI is not null and DSORTIE IS NULL ORDER BY NOM", Demande).ToList();
                        test_2.Clear();
                        foreach (var row_AGENT_REQ in test)
                        {
                            var fields_AGENT_REQ = row_AGENT_REQ as IDictionary<string, object>;
                            var DDP = fields_AGENT_REQ["DDP"];
                            var NOM_PRENOM = fields_AGENT_REQ["NOM_PRENOM"];
                            if (DDP.ToString().Length == 7)
                            {
                                test_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM, CATEGORIE = CATEGORIE });
                            }
                            if (DDP.ToString().Length == 6)
                            {
                                DDP = "0" + DDP;
                                test_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM, CATEGORIE = CATEGORIE });
                            }
                            if (DDP.ToString().Length == 5)
                            {
                                DDP = "00" + DDP;
                                test_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM, CATEGORIE = CATEGORIE });
                            }
                        }
                        return Ok(test_2);
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
                                    var AGENT_REQ = myCongespers.Query("SELECT a.MDIB, a.DOTI as DDP,a.NOM as NOM_PRENOM FROM PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' and DSORTIE IS NULL GROUP BY a.MDIB, a.DOTI,a.NOM order by a.NOM", Demande).ToList();
                                    var AGENT_REQ_2 = myCongespers.Query("SELECT a.MDIB, a.DOTI as DDP,a.NOM as NOM_PRENOM FROM PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' and DSORTIE IS NULL GROUP BY a.MDIB, a.DOTI,a.NOM order by a.NOM", Demande).ToList();
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
                                            if (AFF_2_V == AFF_V)
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
                                    return Ok(AGENT_REQ_2);
                                }
                            }
                        }
                    }
                    if ((string)CATEGORIE == "CUSTOM")
                    {
                        var AGENT_REQ_TOTAL = myCon.Query("SELECT * FROM etats_droits_custom WHERE MODULE='ETAT_DOM' AND DDP=@DDP", Demande).ToList();
                        AGENT_REQ_TOTAL.Clear();
                        var AFF_REQ = myCon.Query("SELECT * FROM etats_droits_custom WHERE MODULE='ETAT_DOM' AND DDP=@DDP", Demande).ToList();
                        foreach (var ligne_aff in AFF_REQ)
                        {
                            var fields_aff = ligne_aff as IDictionary<string, object>;
                            var AFF = fields_aff["CODE_AFF"];
                            var AGENT_REQ = myCongespers.Query("SELECT a.MDIB, a.DOTI as DDP,a.NOM as NOM_PRENOM FROM PERSONEL a JOIN AFFECTATIONS b ON a.MDIB=b.MDIB where b.affectation='" + AFF + @"' and DSORTIE IS NULL GROUP BY a.MDIB, a.DOTI,a.NOM order by a.NOM", Demande).ToList();
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
                                    if (AFF_2_V == AFF_V)
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
                            //return Ok(AGENT_REQ_2);
                        }
                        return Ok(AGENT_REQ_TOTAL);
                    }
                }
                //myCon.close();
                return Ok();
            }
        }

        // TELECHARGEMENT DE L'ATTESTATION DE DOMICILIATION EN FORMAT (PDF)
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
                return new JsonResult("Etat de domiciliation non éxistante !");
            }
        }

        // AFFICHAGE DE LA BANQUE/RIB DE L'AGENT SELECTIONNE DEPUIS LA LISTE
        [HttpPost]
        [Route("RIB_BANQUE")]
        public IActionResult RIB_BANQUE(EtatDom Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test_req = myCon.Query("Select * from rib where DDP=@DDP", Demande).ToList();
                test_req.Clear();
                var test = myCon.Query("Select * from rib where DDP=@DDP", Demande).ToList();
                foreach (var rows in test)
                {
                    var fields = rows as IDictionary<string, object>;
                    var RIB = fields["RIB"];
                    var CODE_BQ = RIB.ToString().Substring(0, 3);
                    var test1 = myCon.Query("Select * from banque_codes where CODE='" + CODE_BQ + @"'", Demande).ToList();
                    foreach (var rows1 in test1)
                    {
                        var fields1 = rows1 as IDictionary<string, object>;
                        var BANQUE = fields1["BANQUE_FR"].ToString().Replace("_", "'"); ;
                        test_req.Add(new { RIB = RIB, BANQUE = BANQUE });
                    }
                }
                //myCon.close();
                return Ok(test_req);
            }
        }

        // AJOUT DE LA DEMANDE DE L'ATTESTATION DE DOMICILIATION DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE ET GENERATION DE L'ATTESTATION DE DIMICILIATION
        [HttpPost]
        [Route("Ajout")]
        public IActionResult Post(EtatDom Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Demande.BANQUE == "")
                {
                    return new JsonResult("Il faut paramétrer la banque avant de valider !");
                    goto Sortir;
                }
                string DATE_V = (string)Demande.DATE.ToString("yyyy-MM-dd");
                int teste = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etat_domiciliation_demandes WHERE DATE='" + DATE_V + @"' AND DDP=@DDP AND DDP_DEMANDEUR=@DDP_DEMANDEUR", Demande);
                if (teste != 0)
                {
                    goto cleanup;
                }
                string fileName = "DOMICILIATION.docx";
                string sourcePath = @".\Etats\";
                string targetPath = @".\Etats\SORTIE\ETAT_DOMICILIATION";
                string fileoutput = (string)Demande.DDP + "_" + Demande.DATE.ToString().Substring(0, 10).Replace("/", "-") + " (" + (string)Demande.DDP_DEMANDEUR + ")";
                string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                string destFile = System.IO.Path.Combine(targetPath, fileoutput + ".docx");
                System.IO.Directory.CreateDirectory(targetPath);
                System.IO.File.Copy(sourceFile, destFile, true);
                Document doc = new Document();
                doc.LoadFromFile(destFile);
                int test_existe = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etat_domiciliation_demandes WHERE ETAT='EN COURS' AND DDP=@DDP", Demande);
                if (test_existe != 0)
                {
                    var req_existe = myCon.Query("Select * from etat_domiciliation_demandes where ETAT='EN COURS' AND DDP=@DDP", Demande).ToList();
                    foreach (var rows_existe in req_existe)
                    {
                        var fields_existe = rows_existe as IDictionary<string, object>;
                        var RIB = fields_existe["RIB"];
                        var NOM_PRENOM = fields_existe["NOM_PRENOM"];
                        var NOM_PRENOM_V = NOM_PRENOM.ToString();
                        var CIN = fields_existe["CIN"];
                        var SEXE = fields_existe["SEXE"];
                        var BANQUE = fields_existe["BANQUE"];
                        var BANQUE_V = BANQUE.ToString();
                        //var LIB_GRADE = fields_existe["GRADE"];
                        //var LIB_GRADE_V = LIB_GRADE.ToString();
                        doc.Replace("<DATE>", (string)DateTime.Now.ToString("dd/MM/yyyy"), true, true);
                        doc.Replace("<CIN>", (string)CIN, true, true);
                        //doc.Replace("<GRADE>", (string)LIB_GRADE_V, true, true);
                        doc.Replace("<DDP>", Demande.DDP, true, true);
                        doc.Replace("<NOM_PRENOM>", (string)NOM_PRENOM_V, true, true);
                        doc.Replace("<BANQUE>", (string)BANQUE_V, true, true);
                        doc.Replace("<RIB>", (string)RIB, true, true);
                        if ((String)SEXE == "M")
                        {
                            doc.Replace("<MR>", "Monsieur", true, true);
                            doc.Replace("<EMPLOYE>", "l'employé", true, true);
                        }
                        else
                        {
                            doc.Replace("<EMPLOYE>", "l'employée", true, true);
                        }
                        doc.SaveToFile(destFile, FileFormat.Docx2013);
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
                        string CHEMIN_V = CHEMIN.ToString().Replace("\\", "/");
                        PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
                        var filename = @".\Etats\SORTIE\ETAT_DOMICILIATION\" + fileoutput + ".pdf";
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
                                foreach (var rowssig in testsig)
                                {
                                    var fieldssig = rowssig as IDictionary<string, object>;
                                    var DDP = fieldssig["DDP_SIGNATAIRE_OBLIGATOIRE"];
                                    XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\FR\POUR_DIRECTEUR\" + DDP + ".jpg");
                                    gfx.DrawImage(imagesigne, 0, page.Height - 240, page.Width, 110);
                                }
                                XImage imagepied = XImage.FromFile(@".\Etats\PICS\PIED.jpg");
                                gfx.DrawImage(imagepied, 0, page.Height - 80, page.Width, 80);
                            }
                            else
                            {
                                var testsig = myCon.Query("SELECT * FROM etats_signataires WHERE ABSENCE_DU>'" + DATE_TODAY + @"' OR ABSENCE_AU<'" + DATE_TODAY + @"' OR ABSENCE_DU IS NULL ORDER BY ORDRE LIMIT 1", Demande).ToList();
                                foreach (var rowssig in testsig)
                                {
                                    var fieldssig = rowssig as IDictionary<string, object>;
                                    var DDP = fieldssig["DDP"];
                                    XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\FR\POUR_DIRECTEUR\" + DDP + ".jpg");
                                    gfx.DrawImage(imagesigne, 0, page.Height - 240, page.Width, 110);
                                }
                                XImage imagepied = XImage.FromFile(@".\Etats\PICS\PIED.jpg");
                                gfx.DrawImage(imagepied, 0, page.Height - 80, page.Width, 80);
                            }
                        }
                        inputDocument.Save(filename);
                        string query = "Insert into etat_domiciliation_demandes values  ('" + DATE_V + "', '" + Demande.DDP + "', '" + Demande.NOM_PRENOM + "', '" + SEXE + "', '" + CIN + "', '" + BANQUE + "', '" + RIB + "', 'EN COURS', 'TRAITE', '" + Demande.DDP_DEMANDEUR + "', '" + Demande.NOM_PRENOM_DEMANDEUR + "', '" + CHEMIN_V + "','' )";
                        var result = myCon.Query(query).ToList();
                        return Ok(result);
                    }
                }
                var test = myCon.Query("Select * from rib where DDP=@DDP", Demande).ToList();
                foreach (var rows in test)
                {
                    var fields = rows as IDictionary<string, object>;
                    var RIB = fields["RIB"];
                    var NOM_PRENOM = fields["NOM_PRENOM"];
                    var NOM_PRENOM_V = NOM_PRENOM.ToString().Replace("_", "'");
                    var CODE_BQ = RIB.ToString().Substring(0, 3);
                    var test1 = myCon.Query("Select * from banque_codes where CODE='" + CODE_BQ + @"'", Demande).ToList();
                    foreach (var rows1 in test1)
                    {
                        var fields1 = rows1 as IDictionary<string, object>;
                        var BANQUE = fields1["BANQUE_FR"];
                        var BANQUE_V = BANQUE.ToString().Replace("_", "'");
                        decimal DDP_CONNECTE = decimal.Parse(Demande.DDP.ToString());
                        var test0 = myCongespers.Query("SELECT MDIB, CIN, NOM, SEXE FROM PERSONEL WHERE DOTI='" + DDP_CONNECTE + @"'", Demande).ToList();
                        foreach (var rows0 in test0)
                        {
                            var fields0 = rows0 as IDictionary<string, object>;
                            var DIB = fields0["MDIB"];
                            var CIN = fields0["CIN"];
                            var SEXE = fields0["SEXE"];
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
                                    var LIB_GRADE_V = LIB_GRADE.ToString().Replace("'", "_");
                                    LIB_GRADE_V = LIB_GRADE_V.ToString().Substring(0, 1).ToUpper() + LIB_GRADE_V.ToString().Substring(1, LIB_GRADE_V.ToString().Length - 1).ToLower();
                                    doc.Replace("<DATE>", (string)DateTime.Now.ToString("dd/MM/yyyy"), true, true);
                                    doc.Replace("<CIN>", (string)CIN, true, true);
                                    doc.Replace("<GRADE>", (string)LIB_GRADE_V.Replace("_", "'"), true, true);
                                    doc.Replace("<DDP>", Demande.DDP, true, true);
                                    doc.Replace("<NOM_PRENOM>", (string)NOM_PRENOM_V, true, true);
                                    doc.Replace("<BANQUE>", (string)BANQUE_V, true, true);
                                    doc.Replace("<RIB>", (string)RIB, true, true);
                                    if ((String)SEXE == "M")
                                    {
                                        doc.Replace("<MR>", "Monsieur", true, true);
                                        doc.Replace("<EMPLOYE>", "l'employé", true, true);
                                    }
                                    else
                                    {
                                        doc.Replace("<EMPLOYE>", "l'employée", true, true);
                                    }
                                    doc.SaveToFile(destFile, FileFormat.Docx2013);
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
                                    string CHEMIN_V = CHEMIN.ToString().Replace("\\", "/");
                                    PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
                                    var filename = @".\Etats\SORTIE\ETAT_DOMICILIATION\" + fileoutput + ".pdf";
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
                                            foreach (var rowssig in testsig)
                                            {
                                                var fieldssig = rowssig as IDictionary<string, object>;
                                                var DDP = fieldssig["DDP_SIGNATAIRE_OBLIGATOIRE"];
                                                XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\FR\POUR_DIRECTEUR\" + DDP + ".jpg");
                                                gfx.DrawImage(imagesigne, 0, page.Height - 240, page.Width, 110);
                                            }
                                            XImage imagepied = XImage.FromFile(@".\Etats\PICS\PIED.jpg");
                                            gfx.DrawImage(imagepied, 0, page.Height - 80, page.Width, 80);
                                        }
                                        else
                                        {
                                            var testsig = myCon.Query("SELECT * FROM etats_signataires WHERE ABSENCE_DU>'" + DATE_TODAY + @"' OR ABSENCE_AU<'" + DATE_TODAY + @"' OR ABSENCE_DU IS NULL ORDER BY ORDRE LIMIT 1", Demande).ToList();
                                            foreach (var rowssig in testsig)
                                            {
                                                var fieldssig = rowssig as IDictionary<string, object>;
                                                var DDP = fieldssig["DDP"];
                                                XImage imagesigne = XImage.FromFile(@".\Etats\SIGNATURE\FR\POUR_DIRECTEUR\" + DDP + ".jpg");
                                                gfx.DrawImage(imagesigne, 0, page.Height - 240, page.Width, 110);
                                            }
                                            XImage imagepied = XImage.FromFile(@".\Etats\PICS\PIED.jpg");
                                            gfx.DrawImage(imagepied, 0, page.Height - 80, page.Width, 80);
                                        }
                                    }
                                    inputDocument.Save(filename);

                                    string query = "Insert into etat_domiciliation_demandes values  ('" + DATE_V + "', '" + Demande.DDP + "', '" + Demande.NOM_PRENOM + "', '" + SEXE + "', '" + CIN + "', '" + BANQUE + "', '" + RIB + "', 'EN COURS', 'TRAITE', '" + Demande.DDP_DEMANDEUR + "', '" + Demande.NOM_PRENOM_DEMANDEUR + "', '" + CHEMIN_V + "','')";
                                    var result = myCon.Query(query).ToList();
                                    return Ok(result);
                                }
                            }
                        }
                    }
                }
                //myCon.close();
            }
        cleanup: return new JsonResult("Domiciliation de salaire déjà éditée !");
        Sortir: return new JsonResult("");
        }
    }
}
