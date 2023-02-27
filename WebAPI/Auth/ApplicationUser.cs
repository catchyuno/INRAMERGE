using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI
{
    public class ApplicationUser: IdentityUser
    {
        public string Role { get; set; }
        public string FullName { get; set; }
        public string DDP { get; set; }
        public int? Unite { get; set; }
        public bool Enabled { get; set; }
        public int? Class { get; set; }

        private string Access;

        public string[] GetAccess()
        {
            if (Access == null) return null;
            return (string[])JsonConvert.DeserializeObject(Access);
        }

        public void SetAccess(string[] value)
        {
            Access = JsonConvert.SerializeObject(value);
        }
    }
}
