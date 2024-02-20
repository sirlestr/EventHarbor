namespace EventHarbor.Class
{
    public class UserManager
    {

        public UserManager manager;
        public int LoggedUserId { get; private set; }
        public string LoggedUserName { get; private set; }

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
        /// Return if user exists, and if password is correct
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
                    LoggedUserId = user.UserId;
                    LoggedUserName = user.UserName;
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
    }
}
