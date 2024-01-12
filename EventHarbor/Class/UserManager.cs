using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace EventHarbor.Class
{
    internal class UserManager
    {
        public UserManager() { }


        public bool AddUser(string name,string passwd)
        {
            using (DatabaseContextManager context = new DatabaseContextManager())
            {
                User user = new User(name, passwd);
                context.Users.Add(user);
                context.SaveChanges();
                return true;
            }
           
            
            
           
        }

        public void RemoveUser(User user)
        {
            using (var context = new DatabaseContextManager())
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }

        public void UpdateUser(User user)
        {
            using (var context = new DatabaseContextManager())
            {
                context.Users.Update(user);
                context.SaveChanges();
            }
        }

        public void FindUser(User user)
        {
            using (var context = new DatabaseContextManager())
            {
                context.Users.Find(user);
            }
        }




    }
}
