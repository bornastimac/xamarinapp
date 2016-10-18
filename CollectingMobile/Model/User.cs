using System;

namespace CollectingMobile
{
    [Serializable]
    public class User
    {
        public readonly string Name;
        public readonly string Password;

        public User(User userToClone)
        {
            this.Name = userToClone.Name;
            this.Password = userToClone.Password;
        }

        public User(string username, string password)
        {
            this.Name = username;
            this.Password = password;
        }
    }
}