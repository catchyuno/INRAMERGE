using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FOINRA
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FactureController : ControllerBase
    {
        public FOINRAContext dbContext { get; set; }
        public UserManager<ApplicationUser> mng { get; set; }
        public IConfiguration Configuration { get; set; }
        public FactureController(FOINRAContext db,IConfiguration configuration,UserManager<ApplicationUser> usrmng)
        {
            dbContext = db;
            Configuration = configuration;
            mng = usrmng;
        }
        public bool hasrole(string Role, int re)
        {
            var role = JsonConvert.DeserializeObject<Array>(Role);
            foreach (int r in role)
            {
                if (re == r) return true;
            }
            return false;
        }
        public void notifications(processModel pm)
        {
            var f = dbContext.Facture.Include(k => k.IdMarcheNavigation).ThenInclude(m => m.ServiceMarche).Where(k => k.IdFacture == pm.f.IdFacture).First();
            var userlist = mng.Users.Select(u => new { u.Id, u.Email, u.FullName, u.UserName, u.Role, u.Unite, u.Class });
            bool mail = false;var user = userlist.Where(u=>u.UserName=="  ");var Role = -1;
            switch (f.Status)
            {
                case "A rectifier": mail = true;Role=0 ; user = userlist.Where(u => u.Id == f.UserSaisie); break;
                case "Réceptionné": mail = true; Role = 4; user = userlist.Where(u => f.IdMarcheNavigation.ServiceMarche.Where(s =>s.IdService == u.Unite).Count() != 0); break;
                case "Certifié": mail = true; Role = 1; user = userlist.Where(u => u.Class == f.IdMarcheNavigation.Ordonnateur); break;
                case "verifié": mail = true; Role = 2; user = userlist.Where(u => u.Class == f.IdMarcheNavigation.Ordonnateur ); break;
                case "Envoyé au TP": mail = true; Role = 3; user = userlist.Where(u => u.Class == f.IdMarcheNavigation.Ordonnateur); break;
                case "Dossier visé": mail = true; Role = 2; user = userlist.Where(u => u.Class == f.IdMarcheNavigation.Ordonnateur ); break;
                default: mail = false;break;
            }
            if (mail)
            {
                Console.WriteLine("mail");

                
                    user.ToList().ForEach(u =>
                    {
                        if(Role!=0 && Role != -1 && hasrole(u.Role, Role))
                        {
                            if (u != null && u.Email != null && u.Email != "")
                            {
                                if (u.Id != pm.t.UserSaisie)
                                {
                                    var message = "Madame,Monsieur " + new string(u.FullName) + "," + "<br><br> La facture N° " + f.NumFacture.ToString() + " demande votre intervention.<br> Vous pouvez traiter la facture sur le lien : " + Configuration.GetValue<string>("mail:site");
                                    new Mail(message, "FOINRA,Alerte Facture N°" + f.NumFacture.ToString(), u.Email, Configuration).toMail();
                                }
                            }

                        }else if (Role == 0)
                        {
                            if (u != null && u.Email != null && u.Email != "")
                            {
                                if (u.Id != pm.t.UserSaisie)
                                {
                                    var message = "Madame,Monsieur " + new string(u.FullName) + "," + "<br><br> La facture N° " + f.NumFacture.ToString() + " demande votre intervention.<br> Vous pouvez traiter la facture sur le lien : " + Configuration.GetValue<string>("mail:site");
                                    new Mail(message, "FOINRA,Alerte Facture N°" + f.NumFacture.ToString(), u.Email, Configuration).toMail();
                                }
                            }
                        }
                        
                    });

            }
        }
        [Authorize]
        [HttpGet]
        [Route("get")]
        public IActionResult Get(String id, int classe, int service,int all)
        {
            try
            {
                var result = dbContext.Facture.Include(f => f.IdMarcheNavigation).ThenInclude(m => m.ServiceMarche)
                .Where(f => (f.UserSaisie == id || f.IdMarcheNavigation.IceNavigation.IdUser == id || f.IdMarcheNavigation.Ordonnateur == classe || f.IdMarcheNavigation.ServiceMarche.Where(s => s.IdService == service).Count() != 0) || all == 1)
                .Include(f => f.TrajetFacture).OrderByDescending(f => f.IdFacture).ToList();

                return Ok(result);
            }
            catch (Exception e)
            {

                throw e;
            }

        }        
        [Authorize]
        [HttpPost]
        [Route("add")]
        public IActionResult Add(factureModel fm)
        {
            try
            {
                var name = Fileproc.GetSeq(Configuration).ToString();
                Byte[] b;
                b = Convert.FromBase64String(fm.file);
                System.IO.File.WriteAllBytes(@".\Attach\" + "facture" + name + ".pdf", b);
                fm.f.Fichier = name;
                dbContext.Facture.Add(fm.f);
                dbContext.SaveChanges();
                return Ok(true);

            }
            catch (Exception )
            {
                return Ok(false);
            }
        }
        [Authorize]
        [HttpGet]
        [Route("getfile")]
        public IActionResult File(String name)
        {
            try
            {
                Byte[] b;
                b = System.IO.File.ReadAllBytes(@".\Attach\"+name+".pdf");
                return Ok(b);
            }
            catch (Exception )
            {
                return Ok(null);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("edit")]
        public IActionResult Edit(factureModel fm)
        {
            try
            {
                if (fm.file != null)
                {
                    var name = Fileproc.GetSeq(Configuration).ToString();
                    Byte[] b;
                    b = Convert.FromBase64String(fm.file);
                    System.IO.File.WriteAllBytes(@".\Attach\" +"facture"+name + ".pdf", b);
                    fm.f.Fichier = name;
                }
                dbContext.Facture.Update(fm.f);
                dbContext.SaveChanges();
                return Ok(true);
            }
            catch (Exception )
            {
                return Ok(false);
            }
            
        }
        [Authorize]
        [HttpPost]
        [Route("remove")]
        public IActionResult Remove(Facture f)
        {
            try
            {
                var nb = dbContext.TrajetFacture.Where(t => t.IdFacture == f.IdFacture);
                if (nb.Count() != 0)
                {
                    dbContext.TrajetFacture.RemoveRange(nb.ToList());
                }
                dbContext.Facture.Remove(f);
                dbContext.SaveChanges();
                return Ok(true);
                
            }
            catch (Exception )
            {
                return Ok(false);
            }

        }
        [Authorize]
        [HttpPost]
        [Route("res")]
        public IActionResult Res([FromBody] int id)
        {
            try
            {
                using (var cnx = new SqlConnection(Configuration.GetConnectionString("cnx").ToString()))
                {
                    cnx.Query("delete from TrajetFacture where idFacture=@id;update facture set status = 'Déposé' where idFacture = @id",new {id });
                    return Ok(true);
                }

            }
            catch (Exception )
            {
                return Ok(false);
            }

        }
        [Route("getfournisseur")]
        [HttpGet]
        public IActionResult Getfournisseur(int classe, bool isadmin)
        {
            using (var cnx = new SqlConnection(Configuration.GetConnectionString("gespers").ToString()))
            {
                var date = DateTime.Today;
                var classes = cnx.Query("select * from classes where organigramme=2 and ord=1");
                var unites = cnx.Query("select * from unites where organigramme=2");
                var fournisseurs = dbContext.Fournissseur.ToList();
                var marchebc = dbContext.MarcheBc.Include(m=>m.ServiceMarche).Include(m=>m.Facture).Where(m => m.Ordonnateur == classe || isadmin ).ToList();
                return Ok(new { classes, unites,fournisseurs,marchebc,date });
            }
        }
        [HttpPost]
        [Route("addfournisseur")]
        public IActionResult AddFournisseur(Fournissseur f)
        {
            try
            {
                dbContext.Fournissseur.Add(f);
                dbContext.SaveChanges();
                return Ok(true);
            }
            catch (Exception )
            {
                return Ok(false);
            }
        }
        [HttpPost]
        [Route("editfournisseur")]
        public IActionResult EditFournisseur(Fournissseur f)
        {
            try
            {
                dbContext.Fournissseur.Update(f);
                dbContext.SaveChanges();
                return Ok(true);

            }
            catch (Exception)
            {
                return Ok(false);
            }
        }
        [HttpPost]
        [Route("deletefournisseur")]
        public IActionResult removeFourniseur(Fournissseur f)
        {
            try
            {
                var theris = dbContext.MarcheBc.Where(fc => fc.Ice == f.Ice).ToList();
                if (theris.Count > 0)
                {
                    return Ok(false);
                }
                else
                {
                    dbContext.Fournissseur.Remove(f);
                    dbContext.SaveChanges();
                    return Ok(true);
                }
                

            }
            catch (Exception )
            {
                return Ok(false);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("process")]
        public IActionResult Process(processModel pm)
        {
            try
            {
                
                if (pm.file != null)
                {
                    var name = Fileproc.GetSeq(Configuration).ToString();
                    Byte[] b;
                    b = Convert.FromBase64String(pm.file);
                    System.IO.File.WriteAllBytes(@".\Attach\" + "process" + name + ".pdf", b);
                    pm.t.Fichier = name;
                }
                dbContext.TrajetFacture.Add(pm.t);
                dbContext.Facture.Update(pm.f);
                dbContext.SaveChanges();
                return Ok(true);

            }
            catch (Exception)
            {
                return Ok(false);
            }
        }
        [Route("getcalcul")]
        [HttpGet]
        public IActionResult GetCalcul(int id)
        {
            using (var cnx = new SqlConnection(Configuration.GetConnectionString("cnx").ToString()))
            {
                var classes = cnx.Query("select * from gethistorique(null, null, 0, @id,0,0,0,0,0,0,0)", new { id }).ToList();
                return Ok(classes);

            }
        }
        [Route("getsuivi")]
        [HttpGet]
        public ActionResult GetEtat(string param)
        {
            try
            {
                string d = DateTime.Today.Day.ToString() + '-' + DateTime.Today.Month.ToString() + '-' + DateTime.Today.Year.ToString();
                string link = Configuration.GetValue<string>("report:base") + "suivifacture" + "&rs:format=excel" + param;
                Uri rl = new Uri(link);
                Byte[] b;
                Console.WriteLine(link);
                using (var client = new WebClient())
                {
                    client.UseDefaultCredentials = true;
                    client.Credentials = new NetworkCredential(Configuration.GetValue<string>("report:username"), Configuration.GetValue<string>("report:password"));
                    client.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                    b = client.DownloadData(rl);
                }
                return File(b, "application/octet-stream");
            }
            catch (Exception e)
            {
                return Ok(false);
            }
            
        }
        #region marchebc
        [Authorize]
        [HttpGet]
        [Route("getmb")]
        public async Task<IActionResult> GetmbAsync(int set)
        {
            var user = await mng.FindByIdAsync(User.FindFirst("jti")?.Value);
            if (set == 0)
            {
                var res = dbContext.MarcheBc.Include(m=>m.ServiceMarche).Include(m => m.IceNavigation).Where(m=> m.ServiceMarche.Where(s => s.IdService == user.Unite).Count() != 0 && m.Status==true).ToList();
                return Ok(res);
            }
            else
            {
                var res = dbContext.MarcheBc.Include(m => m.ServiceMarche).Include(m => m.IceNavigation).Where(m => (m.Ordonnateur==user.Class || m.UserSaisie==user.Id) && m.Status==true ).ToList();
                return Ok(res);
            }
            

        }
        [Authorize]
        [HttpPost]
        [Route("addmb")]
        public IActionResult Addmb(mbcModel m)
        {
            try
            {
                if (m.file != null)
                {
                    var name = Fileproc.GetSeq(Configuration).ToString();
                    Byte[] b;
                    b = Convert.FromBase64String(m.file);
                    System.IO.File.WriteAllBytes(@".\Attach\" + "MarcheBc" + name + ".pdf", b);
                    m.mb.Fichier = name;
                }
                dbContext.MarcheBc.Add(m.mb);
                dbContext.SaveChanges();
                return Ok(true);
            }
            catch (Exception)
            {
                return Ok(false);
            }
            
        }
        [Authorize]
        [HttpPost]
        [Route("editmb")]
        public IActionResult Editmb(mbcModel m)
        {
            try
            {
                if (m.file != null)
                {
                    var name = Fileproc.GetSeq(Configuration).ToString();
                    Byte[] b;
                    b = Convert.FromBase64String(m.file);
                    System.IO.File.WriteAllBytes(@".\Attach\" + "MarcheBc" + name + ".pdf", b);
                    m.mb.Fichier = name;
                }
                dbContext.MarcheBc.Update(m.mb);
                dbContext.SaveChanges();
                return Ok(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return Ok(false);
            }
            
        }
        [Authorize]
        [HttpPost]
        [Route("editsm")]
        public IActionResult Editsm(ServiceMarche sm)
        {
            try
            {
                dbContext.ServiceMarche.Update(sm);
                dbContext.SaveChanges();
                return Ok(true);
            }
            catch (Exception)
            {
                return Ok(false);
            }

        }
        [Authorize]
        [HttpPost]
        [Route("deletesm")]
        public IActionResult Deletesm(ServiceMarche sm)
        {
            try
            {
                dbContext.ServiceMarche.Remove(sm);
                dbContext.SaveChanges();
                return Ok(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return Ok(false);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("deletemb")]
        public IActionResult Deletemb(MarcheBc mb)
        {
            try
            {
                dbContext.MarcheBc.Remove(mb);
                dbContext.SaveChanges();
                return Ok(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return Ok(false);
            }
        }
        #endregion
    }
    public class processModel
    {
        public Boolean notif { get; set; }
        public Facture f { get; set; }
        public string file { get; set; }
        public TrajetFacture t { get; set; }
    }
    public class factureModel
    {
        public Facture f { get; set; }
        public string file { get; set; }
    }
    public partial class mbcModel
    {
        public MarcheBc mb { get; set; }
        public string file { get; set; }
    }
}