using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EventHarbor.Class
{
    [Owned]
    public class User
    {
        /// <summary>
        /// Base user Object
        /// </summary>
        [Key]
        public int UserId { get; set; }
        public string UserName { get;  set; }
        
        public string UserHash { get;  set; } 
        [NotMapped]
        public string? UserPasswd { get;  set; }
        public static int lastId = 0;
        

        public User(string userName, string userPasswd)
        {
            UserId = lastId;
            lastId++;
            UserName = userName;

            UserHash = userPasswd;

        }

        public User()
        {

        }



    }
}
