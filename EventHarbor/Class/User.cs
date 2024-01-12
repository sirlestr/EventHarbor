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
        /// Definition User Object
        /// </summary>
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserHash { get; set; }
        [NotMapped]
        public string? UserPasswd { get; set; }
        public static int lastId = 0;

        public User(){}

        public User(string userName, string userPasswd)
        {
            
            UserName = userName;
            UserHash = getHashEncryption(userPasswd);
            

        }

        /// <summary>
        /// Basic hash function for password
        /// </summary>
        /// <param name="userPasswd">input password from form or other sources</param>
        /// <returns>hashed password</returns>
        public string getHashEncryption(string userPasswd)
        {
            string hash = "";
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(userPasswd));
                hash = Convert.ToBase64String(bytes);
            }

            return  hash;
        }

       


    }
}
