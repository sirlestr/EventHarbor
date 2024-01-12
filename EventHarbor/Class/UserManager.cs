﻿namespace EventHarbor.Class
{
    internal class UserManager
    {
        public UserManager() { }

        /// <summary>
        /// Add new user to Db
        /// </summary>
        /// <param name="name">User name</param>
        /// <param name="passwd">User Password</param>
        /// <returns>true if user has been added</returns>
        public bool AddUser(string name, string passwd)
        {
            using (DatabaseContextManager context = new DatabaseContextManager())
            {
                User user = new User(name, passwd);
                context.Users.Add(user);
                context.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// Retunr if user exists, and if password is correct
        /// </summary>
        /// <param name="name">User name</param>
        /// <param name="passwd">User password</param>
        /// <returns>-1 user not found, 0 password not correct, 1 Login credentials Ok</returns>
        public int IsRegistered(string name, string passwd)
        {
            //-1 user not found, 0 password not correct, 1 login credentials Ok
            int result;
            using (DatabaseContextManager context = new DatabaseContextManager())
            {
                User user = context.Users.ToList().Find(u => u.UserName == name);

                if (user == null)
                { //not found
                    return result = -1;
                }
                else if (user.getHashEncryption(passwd) != user.UserHash)
                {
                    return result = 0; //wrong password
                }
                else
                {
                    return result = 1; // login success
                }
            }
        }

        /// <summary>
        /// Reset user password
        /// </summary>
        /// <param name="name">User name</param>
        /// <param name="passwd">New password</param>
        /// <returns>true if operation is ok, false if user not found</returns>
        public bool ResetPassword(string name, string passwd)
        {
            using (DatabaseContextManager context = new DatabaseContextManager())
            {
                User user = context.Users.ToList().Find(u => u.UserName == name);
                if (user == null)
                {
                    //user not found
                    return false;
                }
                else
                {
                    //user found, reset ok
                    user.UserHash = user.getHashEncryption(passwd);
                    context.SaveChanges();
                    return true;
                }

            }
        }


        //to do classes
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
