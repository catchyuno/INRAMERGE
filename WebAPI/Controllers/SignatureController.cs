using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System;
using WebAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Collections.Generic;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using Spire.Doc;
using Document = Spire.Doc.Document;
using DocXToPdfConverter;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignatureController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SignatureController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // LISTE DES AGENTS EN FONCTION
        [HttpPost]
        [Route("ListeAgents")]
        public IActionResult ListeAgents(Signature Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            string sqlDataSourcegespers = _configuration.GetConnectionString("gespersAppCon");
            SqlConnection myCongespers = new SqlConnection(sqlDataSourcegespers);
            //myCongespers.Open();
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                decimal DDP_CONNECTE = decimal.Parse(Demande.DDP.ToString());
                var test = myCongespers.Query("SELECT DOTI as DDP, NOM as NOM_PRENOM FROM PERSONEL WHERE DOTI is not null and dsortie is null ORDER BY NOM", Demande).ToList();
                var test_2 = myCongespers.Query("SELECT DOTI as DDP, NOM as NOM_PRENOM FROM PERSONEL WHERE DOTI is not null and dsortie is null ORDER BY NOM", Demande).ToList();
                test_2.Clear();
                foreach (var row_AGENT_REQ in test)
                {
                    var fields_AGENT_REQ = row_AGENT_REQ as IDictionary<string, object>;
                    var DDP = fields_AGENT_REQ["DDP"];
                    var NOM_PRENOM = fields_AGENT_REQ["NOM_PRENOM"];
                    if (DDP.ToString().Length == 7)
                    {
                        test_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM });
                    }
                    if (DDP.ToString().Length == 6)
                    {
                        DDP = "0" + DDP;
                        test_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM });
                    }
                    if (DDP.ToString().Length == 5)
                    {
                        DDP = "00" + DDP;
                        test_2.Add(new { DDP = DDP, NOM_PRENOM = NOM_PRENOM });
                    }
                }
                //myCon.close();
                return Ok(test_2);
            }
        }

        // LISTE DES AGENTS SIGNATAIRES
        [HttpPost]
        [Route("ListeAgentsSIG")]
        public IActionResult ListeAgentsSIG(Signature Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM etats_signataires ORDER BY ORDRE", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // LISTE DES SIGNATAIRES
        [HttpPost]
        [Route("Liste")]
        public IActionResult Liste(Signature Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var test = myCon.Query("SELECT * FROM etats_signataires ORDER BY ORDRE", Demande).ToList();
                //myCon.close();
                return Ok(test);
            }
        }

        // TELECHARGEMENT DE LA SIGNATURE EN FORMAT (JPG)
        [HttpGet]
        [Route("file")]
        public IActionResult Getfile(string nom_file)
        {
            Document doc = new Document();
            //var DDP_PRINT = Convert.ToInt32(DDP);
            string fileName = "SIGNATURE.docx";
            string sourcePath = @".\Etats\";
            string targetPath = @".\Etats\SORTIE\SIGNATURE";
            string fileoutput = (string)@nom_file;
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileoutput + ".docx");
            System.IO.Directory.CreateDirectory(targetPath);
            System.IO.File.Copy(sourceFile, destFile, true);
            doc.LoadFromFile(destFile);
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
            var filename = @".\Etats\SORTIE\SIGNATURE\" + fileoutput + ".pdf";
            PdfSharp.Pdf.PdfDocument inputDocument = PdfReader.Open(filename, PdfDocumentOpenMode.Modify);
            int count = inputDocument.PageCount;
            for (int idx = 0; idx < count; idx++)
            {
                PdfPage page = inputDocument.Pages[idx];
                XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);
                if (System.IO.File.Exists(@".\Etats\SIGNATURE\FR\POUR_DIRECTEUR\" + @nom_file + ".jpg"))
                {
                    XImage imageentete1 = XImage.FromFile(@".\Etats\SIGNATURE\FR\POUR_DIRECTEUR\" + @nom_file + ".jpg");
                    gfx.DrawImage(imageentete1, 0, 50, page.Width, 90);
                }
                if (System.IO.File.Exists(@".\Etats\SIGNATURE\FR\SANS_POUR_DIRECTEUR\" + @nom_file + ".jpg"))
                {
                    XImage imageentete2 = XImage.FromFile(@".\Etats\SIGNATURE\FR\SANS_POUR_DIRECTEUR\" + @nom_file + ".jpg");
                    gfx.DrawImage(imageentete2, 0, 150, page.Width, 90);
                }
                if (System.IO.File.Exists(@".\Etats\SIGNATURE\AR\POUR_DIRECTEUR\" + @nom_file + ".jpg"))
                {
                    XImage imageentete3 = XImage.FromFile(@".\Etats\SIGNATURE\AR\POUR_DIRECTEUR\" + @nom_file + ".jpg");
                    gfx.DrawImage(imageentete3, 0, 250, page.Width, 90);
                }
                if (System.IO.File.Exists(@".\Etats\SIGNATURE\AR\SANS_POUR_DIRECTEUR\" + @nom_file + ".jpg"))
                {
                    XImage imagesigne4 = XImage.FromFile(@".\Etats\SIGNATURE\AR\SANS_POUR_DIRECTEUR\" + @nom_file + ".jpg");
                    gfx.DrawImage(imagesigne4, 0, 350, page.Width, 90);
                }
            }
            inputDocument.Save(filename);
            //return Ok();
            if (System.IO.File.Exists(@".\\Etats\\SORTIE\\SIGNATURE\\" + @nom_file + ".pdf"))
            {
                var f = System.IO.File.ReadAllBytes(@".\\Etats\\SORTIE\\SIGNATURE\\" + @nom_file + ".pdf");
                return Ok(f);
            }
            else
            {
                return new JsonResult("Aucune signature n'est enregistrée pour cet agent !");
            }
        }

         // TELECHARGEMENT DE L'ENTETE EN FORMAT (JPG)
        [HttpGet]
        [Route("Entetefile")]
        public IActionResult getfile(string nom_file)
        {
            if (System.IO.File.Exists(@nom_file))
            {
                var f = System.IO.File.ReadAllBytes(@nom_file);
                return Ok(f);
            }
            else
            {
                return new JsonResult("Aucun entete de page n'est enregistré !");
            }
            //return Ok();
        }

        // TELECHARGEMENT DE L'ENTETE EN FORMAT (JPG)
        [HttpGet]
        [Route("Piedfile")]
        public IActionResult getfil(string nom_file)
        {
            if (System.IO.File.Exists(@nom_file))
            {
                var f = System.IO.File.ReadAllBytes(@nom_file);
                return Ok(f);
            }
            else
            {
                return new JsonResult("Aucun pied de page n'est enregistré !");
            }
            //return Ok();
        }

        // UPLOAD DE LA SIGNATURE
        [HttpPost]
        [Route("upload")]
        public IActionResult getfile(Signature Demande)
        {
            Byte[] b;
            b = Convert.FromBase64String(Demande.nom_file);
            if (Demande.TYPE=="POURFR")
            {
                System.IO.File.WriteAllBytes(@".\Etats\SIGNATURE\FR\POUR_DIRECTEUR\" + Demande.DDP + ".jpg", b);
                //return Ok();
            }
            if (Demande.TYPE == "SANSFR")
            {
                System.IO.File.WriteAllBytes(@".\Etats\SIGNATURE\FR\SANS_POUR_DIRECTEUR\" + Demande.DDP + ".jpg", b);
                //return Ok();
            }
            if (Demande.TYPE == "POURAR")
            {
                System.IO.File.WriteAllBytes(@".\Etats\SIGNATURE\AR\POUR_DIRECTEUR\" + Demande.DDP + ".jpg", b);
                //return Ok();
            }
            if (Demande.TYPE == "SANSAR")
            {
                System.IO.File.WriteAllBytes(@".\Etats\SIGNATURE\AR\SANS_POUR_DIRECTEUR\" + Demande.DDP + ".jpg", b);
                //return Ok();
            }
            return Ok();
        }

        // UPLOAD DE L'ENTETE
        [HttpPost]
        [Route("uploadEntete")]
        public IActionResult getfile(Entete Demande)
        {
            Byte[] b;
            b = Convert.FromBase64String(Demande.nom_file);
            System.IO.File.WriteAllBytes(@".\Etats\PICS\EN_TETE.jpg", b);
            return Ok();
        }


        // UPLOAD DU PIED
        [HttpPost]
        [Route("uploadPied")]
        public IActionResult getfile(Pied Demande)
        {
            Byte[] b;
            b = Convert.FromBase64String(Demande.nom_file);
            System.IO.File.WriteAllBytes(@".\Etats\PICS\PIED.jpg", b);
            return Ok();
        }

        // MODIFICATION DE LA SIGNATAIRE
        [HttpPut]
        public JsonResult Put(Signature Demande)
        {
            var ABSENCE_DU_V = "";
            var ABSENCE_AU_V = "";
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                if (Demande.ORDRE == "" || Demande.ORDRE == null)
                {
                    return new JsonResult("Il faut saisir le n° d'ordre !");
                    goto Sortir;
                }
                if (Demande.ABSENCE_DU == "" || Demande.ABSENCE_DU == null)
                {
                    ABSENCE_DU_V = null;
                }
                else
                {
                    try
                    {
                        DateTime dt = DateTime.ParseExact(Demande.ABSENCE_DU, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception e)
                    {
                        return new JsonResult("Il faut saisir une date début valide !");
                        goto Sortir;
                    }
                    ABSENCE_DU_V = Demande.ABSENCE_DU.ToString().Substring(6, 4) + "-" + Demande.ABSENCE_DU.ToString().Substring(3, 2) + "-" + Demande.ABSENCE_DU.ToString().Substring(0, 2);
                }
                if (Demande.ABSENCE_AU == "" || Demande.ABSENCE_AU == null)
                {
                    ABSENCE_AU_V = null;
                }
                else
                {
                    try
                    {
                        DateTime dt = DateTime.ParseExact(Demande.ABSENCE_AU, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception e)
                    {
                        return new JsonResult("Il faut saisir une date début valide !");
                        goto Sortir;
                    }
                    ABSENCE_AU_V = Demande.ABSENCE_AU.ToString().Substring(6, 4) + "-" + Demande.ABSENCE_AU.ToString().Substring(3, 2) + "-" + Demande.ABSENCE_AU.ToString().Substring(0, 2);
                }
                if (ABSENCE_DU_V != null && ABSENCE_AU_V == null)
                {
                    return new JsonResult("Il faut saisir la date de fin d'absence !");
                    goto Sortir;
                }
                if (ABSENCE_DU_V == null && ABSENCE_AU_V != null)
                {
                    return new JsonResult("Il faut saisir la date début d'absence !");
                    goto Sortir;
                }
                if (ABSENCE_DU_V != null && ABSENCE_AU_V != null && (Demande.MOTIF == "" || Demande.MOTIF == "null"))
                {
                    return new JsonResult("Il faut choisir le motif d'absence !");
                    goto Sortir;
                }
                if ((ABSENCE_DU_V == null && ABSENCE_AU_V == null) && (Demande.MOTIF != ""))
                {
                    return new JsonResult("Il faut saisir les dates début et de fin d'absence !");
                    goto Sortir;
                }
                if (ABSENCE_DU_V != null && ABSENCE_AU_V != null)
                {
                    if (DateTime.Parse(ABSENCE_DU_V) > DateTime.Parse(ABSENCE_AU_V))
                    {
                        return new JsonResult("Il faut saisir la date fin supérieure strictement à la date début !");
                        goto Sortir;
                    }
                }
                var test0 = myCon.Query("Select * from etats_signataires where DDP='" + Demande.DDP + @"'", Demande).ToList();
                foreach (var rows in test0)
                {
                    var fields = rows as IDictionary<string, object>;
                    int ANCIEN_ORDRE = int.Parse(fields["ORDRE"].ToString());
                    if (ANCIEN_ORDRE== int.Parse(Demande.ORDRE.ToString()))
                    {
                        if (Demande.ABSENCE_DU.ToString() == "" && Demande.ABSENCE_AU.ToString() == "")
                        {
                            var t = myCon.Query(@"Update etats_signataires set ABSENCE_DU = null, ABSENCE_AU = null, MOTIF='" + Demande.MOTIF + @"' where DDP = '" + Demande.DDP + @"'", Demande).ToList();
                        }
                        else
                        {
                            var t = myCon.Query(@"Update etats_signataires set ABSENCE_DU = '" + ABSENCE_DU_V + @"', ABSENCE_AU = '" + ABSENCE_AU_V + @"', MOTIF='" + Demande.MOTIF + @"' where DDP = '" + Demande.DDP + @"'", Demande).ToList();
                        }
                    }
                    else
                    {
                        var t = myCon.Query(@"Delete from etats_signataires where DDP = '" + Demande.DDP + @"'", Demande).ToList();
                        var test01 = myCon.Query("Select * from etats_signataires ORDER BY ORDRE", Demande).ToList();
                        int j = 1;
                        foreach (var rows01 in test01)
                        {
                            var fields01 = rows01 as IDictionary<string, object>;
                            int ANCIEN_ORDRE01 = int.Parse(fields01["ORDRE"].ToString());
                            var NEW_ORDRE =j;
                            var tt = myCon.Query(@"Update etats_signataires set ORDRE = '" + NEW_ORDRE + @"' where ORDRE = '" + ANCIEN_ORDRE01 + @"'", Demande).ToList();
                            j++;
                        }
                        int MAX_REQ = myCon.ExecuteScalar<int>("SELECT max(ORDRE) FROM etats_signataires", Demande);
                        if (int.Parse(Demande.ORDRE.ToString()) <= MAX_REQ)
                        {
                            int NBRE_REQ = myCon.ExecuteScalar<int>("SELECT count(*) FROM etats_signataires", Demande);
                            int ORDRE_V = int.Parse(Demande.ORDRE.ToString());
                            int i = NBRE_REQ + 1;
                            do
                            {
                                ORDRE_V = i - 1;
                                string VALEUR = (string)(ORDRE_V.ToString());
                                var test00 = myCon.Query("Select * from etats_signataires where ORDRE='" + VALEUR + @"'", Demande).ToList();
                                foreach (var rows00 in test00)
                                {
                                    var fields00 = rows00 as IDictionary<string, object>;
                                    int ANCIEN_ORDRE00 = int.Parse(fields00["ORDRE"].ToString());
                                    var NEW_ORDRE = ANCIEN_ORDRE00 + 1;
                                    var t00 = myCon.Query(@"Update etats_signataires set ORDRE = '" + NEW_ORDRE + @"' where ORDRE = '" + ANCIEN_ORDRE00 + @"'", Demande).ToList();
                                }
                                i--;
                            } while (i > int.Parse(Demande.ORDRE.ToString()));
                        }
                        if (ABSENCE_DU_V == null && ABSENCE_AU_V == null)
                        {
                            string query = "Insert into etats_signataires values  ('" + Demande.ORDRE + "', '" + Demande.DDP + "', '" + Demande.NOM_PRENOM + "',  '" + Demande.MOTIF + "', null, null)";
                            var result = myCon.Query(query).ToList();
                        }
                        else
                        {
                            string query = "Insert into etats_signataires values  ('" + Demande.ORDRE + "', '" + Demande.DDP + "', '" + Demande.NOM_PRENOM + "',  '" + Demande.MOTIF + "',  '" + ABSENCE_DU_V + "',  '" + ABSENCE_AU_V + "')";
                            var result = myCon.Query(query).ToList();
                        }
                        var test1 = myCon.Query("Select * from etats_signataires ORDER BY ORDRE", Demande).ToList();
                        int k = 1;
                        foreach (var rows1 in test1)
                        {
                            var fields1 = rows1 as IDictionary<string, object>;
                            int ANCIEN_ORDRE_K = int.Parse(fields1["ORDRE"].ToString());
                            var NEW_ORDRE = k;
                            var ttt = myCon.Query(@"Update etats_signataires set ORDRE = '" + NEW_ORDRE + @"' where ORDRE = '" + ANCIEN_ORDRE_K + @"'", Demande).ToList();
                            k++;
                        }
                    }
                }
                //myCon.close();
            }
            return new JsonResult("Mise à jour effectuée !");
            Sortir: return new JsonResult("");
        }

        // SUPPRESSION DU SIGNATAIRE
        [HttpPost]
        [Route("Delete")]
        public JsonResult Delete(Signature Demande)
        {
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                var t = myCon.Query(@"Delete from etats_signataires where DDP = '" + Demande.DDP + @"'", Demande).ToList();
                var test0 = myCon.Query("Select * from etats_signataires ORDER BY ORDRE", Demande).ToList();
                int i=1;
                foreach (var rows in test0)
                {
                    var fields = rows as IDictionary<string, object>;
                    int ANCIEN_ORDRE = int.Parse(fields["ORDRE"].ToString());
                    var NEW_ORDRE = i;
                    var tt = myCon.Query(@"Update etats_signataires set ORDRE = '" + NEW_ORDRE + @"' where ORDRE = '" + ANCIEN_ORDRE + @"'", Demande).ToList();
                    i++;
                }
                //myCon.close();
            }
            return new JsonResult("Suppression effectuée !");
        }

        // AJOUT DU SIGNATAIRE
        [HttpPost]
        [Route("Ajout")]
        public IActionResult Post(Signature Demande)
        {
            var ABSENCE_DU_V="";
            var ABSENCE_AU_V="";
            string sqlDataSource = _configuration.GetConnectionString("myinraAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                int teste = myCon.ExecuteScalar<int>("SELECT COUNT(*) FROM etats_signataires WHERE DDP='" + Demande.DDP + @"'", Demande);
                if (teste != 0)
                {
                    goto cleanup;
                }
                if (Demande.DDP == "")
                {
                    return new JsonResult("Il faut choisir un agent !");
                    goto Sortir;
                }
                if (Demande.ORDRE == "" || Demande.ORDRE == null)
                {
                    return new JsonResult("Il faut saisir le n° d'ordre !");
                    goto Sortir;
                }
                if (Demande.ABSENCE_DU != "")
                {
                    try
                    {
                        DateTime dt = DateTime.ParseExact(Demande.ABSENCE_DU, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception e)
                    {
                        return new JsonResult("Il faut saisir une date début valide !");
                        goto Sortir;
                    }
                }
                if (Demande.ABSENCE_AU != "")
                {
                    try
                    {
                        DateTime dt = DateTime.ParseExact(Demande.ABSENCE_AU, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception e)
                    {
                        return new JsonResult("Il faut saisir une date fin valide !");
                        goto Sortir;
                    }
                }
                if (Demande.ABSENCE_DU == "")
                {
                    ABSENCE_DU_V = null;
                }
                else
                {
                    ABSENCE_DU_V = Demande.ABSENCE_DU.ToString().Substring(6, 4) + "-" + Demande.ABSENCE_DU.ToString().Substring(3, 2) + "-" + Demande.ABSENCE_DU.ToString().Substring(0, 2);
                }
                if (Demande.ABSENCE_AU == "")
                {
                    ABSENCE_AU_V = null;
                }
                else
                {
                    ABSENCE_AU_V = Demande.ABSENCE_AU.ToString().Substring(6, 4) + "-" + Demande.ABSENCE_AU.ToString().Substring(3, 2) + "-" + Demande.ABSENCE_AU.ToString().Substring(0, 2);
                }
                if (ABSENCE_DU_V != null && ABSENCE_AU_V == null)
                {
                    return new JsonResult("Il faut saisir la date de fin d'absence !");
                    goto Sortir;
                }
                if (ABSENCE_DU_V == null && ABSENCE_AU_V != null)
                {
                    return new JsonResult("Il faut saisir la date début d'absence !");
                    goto Sortir;
                }
                if (ABSENCE_DU_V != null && ABSENCE_AU_V != null && (Demande.MOTIF=="" || Demande.MOTIF == "null"))
                {
                    return new JsonResult("Il faut choisir le motif d'absence !");
                    goto Sortir;
                }
                if ((ABSENCE_DU_V == null && ABSENCE_AU_V == null) && (Demande.MOTIF != ""))
                {
                    return new JsonResult("Il faut saisir les dates début et de fin d'absence !");
                    goto Sortir;
                }
                if (Demande.ABSENCE_DU != "" && Demande.ABSENCE_AU != "")
                {
                    if (DateTime.Parse(ABSENCE_DU_V) > DateTime.Parse(ABSENCE_AU_V))
                    {
                        return new JsonResult("Il faut saisir la date fin supérieure strictement à la date début !");
                        goto Sortir;
                    }
                }
                int MAX_REQ = myCon.ExecuteScalar<int>("SELECT max(ORDRE) FROM etats_signataires", Demande);
                if (int.Parse(Demande.ORDRE.ToString()) <= MAX_REQ)
                {
                    int NBRE_REQ = myCon.ExecuteScalar<int>("SELECT count(*) FROM etats_signataires", Demande);
                    int ORDRE_V = int.Parse(Demande.ORDRE.ToString());
                    int i = NBRE_REQ + 1;
                    do
                    {
                        ORDRE_V = i - 1;
                        string VALEUR = (string)(ORDRE_V.ToString());
                        var test0 = myCon.Query("Select * from etats_signataires where ORDRE='" + VALEUR + @"'", Demande).ToList();
                        foreach (var rows in test0)
                        {
                            var fields = rows as IDictionary<string, object>;
                            int ANCIEN_ORDRE = int.Parse(fields["ORDRE"].ToString());
                            var NEW_ORDRE = ANCIEN_ORDRE + 1;
                            var t = myCon.Query(@"Update etats_signataires set ORDRE = '" + NEW_ORDRE + @"' where ORDRE = '" + ANCIEN_ORDRE + @"'", Demande).ToList();
                        }
                        i--;
                    } while (i > int.Parse(Demande.ORDRE.ToString()));
                }
                if (Demande.ABSENCE_DU.ToString() == "" && Demande.ABSENCE_AU.ToString() == "")
                {
                    string query = "Insert into etats_signataires values  ('" + Demande.ORDRE + "', '" + Demande.DDP + "', '" + Demande.NOM_PRENOM + "',  '" + Demande.MOTIF + "', null, null, '" + Demande.DDPSIG + "', '" + Demande.NOM_PRENOMSIG + "')";
                    var result = myCon.Query(query).ToList();
                }
                else
                {
                    string query = "Insert into etats_signataires values  ('" + Demande.ORDRE + "', '" + Demande.DDP + "', '" + Demande.NOM_PRENOM + "',  '" + Demande.MOTIF + "',  '" + ABSENCE_DU_V + "',  '" + ABSENCE_AU_V + "', '" + Demande.DDPSIG + "', '" + Demande.NOM_PRENOMSIG + "')";
                    var result = myCon.Query(query).ToList();
                }
                var test1 = myCon.Query("Select * from etats_signataires ORDER BY ORDRE", Demande).ToList();
                int j = 1;
                foreach (var rows1 in test1)
                {
                    var fields1 = rows1 as IDictionary<string, object>;
                    int ANCIEN_ORDRE = int.Parse(fields1["ORDRE"].ToString());
                    var NEW_ORDRE = j;
                    var ttt = myCon.Query(@"Update etats_signataires set ORDRE = '" + NEW_ORDRE + @"' where ORDRE = '" + ANCIEN_ORDRE + @"'", Demande).ToList();
                    j++;
                }
                //myCon.close();
                return new JsonResult("Ajout effectué !");
            }
        cleanup: return new JsonResult("Signataire déjà inscrit !");
        Sortir: return new JsonResult("");
        }
    }
}
