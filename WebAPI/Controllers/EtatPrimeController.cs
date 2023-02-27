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

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtatPrimeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public EtatPrimeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // LISTE DES DEMANDES DE L'AGENT CONNECTE
        [HttpPost]
        [Route("ListeDemandes")]
        public IActionResult ListeDemandes(EtatPrime Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM etat_prime_demandes where DDP_DEMANDEUR =  @DDP_DEMANDEUR AND DDP =  @DDP ORDER BY DATE DESC", Demande).ToList();
                //////myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES AGENTS SUR LISTE DEROULANTE SELON DROITS DE L'AGENT CONNECTE (AGENT / HIERARCHIE / ADMIN / GESTIONNAIRE)
        [HttpPost]
        [Route("ListeAgents")]
        public IActionResult ListeAgents(EtatPrime Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var catégorie = myCon.Query("SELECT distinct(CATEGORIE) FROM etats_droits where MODULE='ETAT_PRIME' AND DDP =  @DDP", Demande).ToList();
                foreach (var rows in catégorie)
                {
                    var fields = rows as IDictionary<string, object>;
                    var CATEGORIE = fields["CATEGORIE"];
                    if ((string)CATEGORIE == "AGENT")
                    {
                        var test = myCon.Query("SELECT a.DDP,a.NOM_PRENOM,b.CATEGORIE FROM etat_Prime_infos a JOIN etats_droits b ON a.DDP=b.DDP where a.DDP=@DDP AND b.MODULE='ETAT_PRIME' GROUP BY a.DDP,a.NOM_PRENOM,b.CATEGORIE order by a.NOM_PRENOM", Demande).ToList();
                        return Ok(test);
                    }
                    if ((string)CATEGORIE == "ADMIN")
                    {
                        var test = myCon.Query("SELECT DDP, NOM_PRENOM FROM etat_Prime_infos GROUP BY DDP, NOM_PRENOM ORDER BY NOM_PRENOM", Demande).ToList();
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
                                            //decimal DDP_CONNECTE = decimal.Parse(Demande.DDP.ToString());
                                            //decimal DDP_V = decimal.Parse(DDP.ToString());
                                            if (AFF_2_V == AFF_V)
                                            {
                                                if (DDP.ToString().Length == 6)
                                                {
                                                    DDP = "0" + DDP;
                                                }
                                                if (DDP.ToString().Length == 5)
                                                {
                                                    DDP = "00" + DDP;
                                                }
                                                int EXISTE_REVENU = myCon.ExecuteScalar<int>("SELECT * FROM etat_Prime_infos where DDP='" + DDP + @"'", Demande);
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
                        //var DIB_REQ = myCongespers.Query("SELECT * FROM PERSONEL WHERE DOTI=@DDP", Demande).ToList();
                        //foreach (var ligne_dib in DIB_REQ)
                        //{
                        //var fields_mdib = ligne_dib as IDictionary<string, object>;
                        //var DIB = fields_mdib["MDIB"];
                        //int RESP_REQ = myCongespers.ExecuteScalar<int>("SELECT * FROM FONCRESPS WHERE MDIB='" + DIB + @"' AND DFIN IS NULL", Demande);
                        //if (RESP_REQ != 0)
                        //{

                        var AGENT_REQ_TOTAL = myCon.Query("SELECT * FROM etats_droits_custom WHERE MODULE='ETAT_PRIME' AND DDP=@DDP", Demande).ToList();
                        AGENT_REQ_TOTAL.Clear();
                        var AFF_REQ = myCon.Query("SELECT * FROM etats_droits_custom WHERE MODULE='ETAT_PRIME' AND DDP=@DDP", Demande).ToList();
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
                                    if (AFF_2_V == AFF_V)
                                    {
                                        if (DDP.ToString().Length == 6)
                                        {
                                            DDP = "0" + DDP;
                                        }
                                        if (DDP.ToString().Length == 5)
                                        {
                                            DDP = "00" + DDP;
                                        }
                                        int EXISTE_REVENU = myCon.ExecuteScalar<int>("SELECT * FROM etat_Prime_infos where DDP='" + DDP + @"'", Demande);
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

                            //  }
                            //  }

                        }
                        return Ok(AGENT_REQ_TOTAL);
                    }
                }
                //////myCon.close();
                return Ok();
            }
        }

        // LISTE DES ANNEES INSCRITES SUR LA BASE POUR L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE
        [HttpPost]
        [Route("Annee")]
        public IActionResult ListeAnnees(EtatPrime Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT distinct(ANNEE) FROM etat_Prime_infos where DDP =  @DDP order by ANNEE DESC", Demande).ToList();
                //////myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES PRIMES INSCRITES SUR LA BASE POUR L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE ET L'ANNEE SELECTIONNEE
        [HttpPost]
        [Route("Prime")]
        public IActionResult ListePrime(EtatPrime Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("Select distinct(PRIME) from etat_Prime_infos where ANNEE=@ANNEE and DDP=@DDP order by PRIME desc", Demande).ToList();
                ////myCon.close();
                return Ok(test);
            }
        }

        // TELECHARGEMENT DE L'ETAT DE PRIME EN FORMAT (PDF)
        [HttpGet]
        [Route("file")]
        public IActionResult getfile(string nom_file)
        {
            var f = System.IO.File.ReadAllBytes(@nom_file);
            return Ok(f);
        }

        // AJOUT DE LA DEMANDE DE L'ETAT DE PRIME DE L'AGENT SELECTIONNE SUR LA LISTE DEROULANTE ET GENERATION DE L'ETAT DE PRIME
        [HttpPost]
        [Route("Ajout")]
        public IActionResult Post(EtatPrime Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                string DATE_V = (string)Demande.DATE.ToString("yyyy-MM-dd");
                int teste = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etat_Prime_demandes WHERE DATE='" + DATE_V + @"' AND DDP=@DDP  AND DDP_DEMANDEUR=@DDP_DEMANDEUR  AND ANNEE =  @ANNEE AND PRIME =  @PRIME", Demande);
                if (teste != 0)
                {
                    goto cleanup;
                }
                string fileName = "";
                //string r = Demande.PRIME.Substring(0, 3);
                if (Demande.PRIME.Substring(0, 3) != "INF")
                {
                    fileName = "PRIME.docx";
                }
                else
                {
                    fileName = "PRIME_INFORMATIQUE.docx";
                }
                string sourcePath = @".\Etats\";
                string targetPath = @".\Etats\SORTIE\ETAT_PRIME";
                string fileoutput = (string)Demande.DDP + "_" + (string)Demande.ANNEE + "_" + (string)Demande.PRIME + "_" + Demande.DATE.ToString().Substring(0, 10).Replace("/", "-") + " (" + (string)Demande.DDP_DEMANDEUR + ")";
                string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                string destFile = System.IO.Path.Combine(targetPath, fileoutput + ".docx");
                System.IO.Directory.CreateDirectory(targetPath);
                System.IO.File.Copy(sourceFile, destFile, true);
                var test = myCon.Query("Select * from etat_Prime_infos where ANNEE=@ANNEE and PRIME=@PRIME and DDP=@DDP", Demande).ToList();
                foreach (var rows in test)
                {
                    var fields = rows as IDictionary<string, object>;
                    var ANNEE = fields["ANNEE"];
                    var MOIS = fields["MOIS"];
                    var DDP = fields["DDP"];
                    var NOM_PRENOM = fields["NOM_PRENOM"].ToString().Replace("_", "'"); ;
                  //  NOM_PRENOM = NOM_PRENOM.ToString().Replace("_", "'");
                    var CIN = fields["CIN"];
                    var PRIME = fields["PRIME"];
                    var PRIME_V = PRIME + " AU TITRE DE L'EXERCICE : " + ANNEE;
                    var BRUT = fields["BRUT"];
                    var IR = fields["IR"];
                    var NET = fields["NET"];
                    var GRADE = fields["GRADE"];
                    var FONCTION = fields["FONCTION"];
                    var ECHELLE = fields["ECHELLE"];
                    var ECHELON = fields["ECHELON"];
                    var DP = fields["DUREE_PRESENCE"];
                    var DF = fields["DUREE_FONCTION"];
                    var NOTE = fields["NOTE"];
                    Document doc = new Document();
                    doc.LoadFromFile(destFile);
                    doc.Replace("<DATE>", (string)DateTime.Now.ToString("dd/MM/yyyy"), true, true);
                    doc.Replace("<PRIME>", (string)PRIME_V, true, true);
                    doc.Replace("<MOIS>", (string)MOIS, true, true);
                    doc.Replace("<DDP>", (string)DDP, true, true);
                    doc.Replace("<NOM_PRENOM>", (string)NOM_PRENOM, true, true);
                    if (CIN!=null)
                    {
                        doc.Replace("<CIN>", (string)CIN, true, true);
                    }
                    else
                    {
                        doc.Replace("<CIN>", "",true,true);
                    }
                    decimal IR_ARRONDI = decimal.Parse(IR.ToString());
                    decimal BRUT_MEN_ARRONDI = decimal.Parse(BRUT.ToString());
                    decimal NET_MENSUEL_ARRONDI = decimal.Parse(NET.ToString());
                    doc.Replace("<IR>", IR_ARRONDI.ToString("#,0.00"), true, true);
                    doc.Replace("<BRUT>", BRUT_MEN_ARRONDI.ToString("#,0.00"), true, true);
                    doc.Replace("<NET>", NET_MENSUEL_ARRONDI.ToString("#,0.00"), true, true);
                    doc.Replace("<GRADE>", (string)GRADE, true, true);
                    if (ECHELLE != null)
                    {
                        doc.Replace("<ECHELLE>", (string)ECHELLE, true, true);
                    }
                    else
                    {
                        doc.Replace("<ECHELLE>", "", true, true);
                    }
                    
                    doc.Replace("<ECHELON>", ECHELON.ToString(), true, true);
                    if ((string)FONCTION is null)
                    {
                        doc.Replace("<FONCTION>", "", true, true);
                    }
                    else
                    {
                        doc.Replace("<FONCTION>", (string)FONCTION, true, true);
                    }    
                    if (Demande.PRIME.Substring(0, 3) != "INF")
                    {
                        doc.Replace("<DF>", DF.ToString(), true, true);
                        doc.Replace("<DP>", DP.ToString(), true, true);
                        doc.Replace("<NOTE>", NOTE.ToString(), true, true);
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
                var filename = @".\Etats\SORTIE\ETAT_PRIME\" + fileoutput + ".pdf";
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
                var result = myCon.Query("Insert into etat_prime_demandes values (@DATE,@DDP,@NOM_PRENOM, @ANNEE, @PRIME, 'TRAITE', @DDP_DEMANDEUR,@NOM_PRENOM_DEMANDEUR, @CHEMIN)", new { Demande.DATE, Demande.DDP, Demande.NOM_PRENOM, Demande.ANNEE, Demande.PRIME, Demande.DDP_DEMANDEUR, Demande.NOM_PRENOM_DEMANDEUR, CHEMIN }).ToList();
                ////myCon.close();
                return Ok(result);
            }
        cleanup: return new JsonResult("Etat de prime déjà éditée !");
        }
    }
}
