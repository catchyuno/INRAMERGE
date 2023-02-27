using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
namespace FOINRA
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public IHubContext<Hub> hub { get; set; }
        public static int test = 0;
        private FOINRAContext gespers { get; }
        public ValuesController(FOINRAContext gespers)
        {
            this.gespers = gespers;
        }
        // GET api/values
        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            /* Uri rl = new Uri("http://desktop-c0rrk1s/ReportServer?/reports/grilledesalaire&rs:format=excel&deffet=01/01/2018");
             Byte[] b;
             using(var client=new WebClient())
             {
                 client.UseDefaultCredentials = true;
                 b = client.DownloadData(rl);
             }
             */
            /*IDbConnection db = conx.init();
            db.Open();
            var p =new DynamicParameters();
            p.Add("deffet", new DateTime(2018, 1, 1),direction:ParameterDirection.Input);
            var result=await db.QueryAsync("select * from PERSONEL a left join [SPECIALITES CHERCHEUR] b on a.MDIB=b.mdib");
            */
            //return File(b, "application/octet-stream","GrilleDeSalaire.pdf");
            test++;
            return Ok(test);
        }

        // GET api/values/5
        [Route("ping")]
        [HttpGet]
        public ActionResult<string> Get()
        {
            return Ok("ok");
        }

        
    }
}
