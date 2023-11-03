using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wits.Classes
{
    public class UserSingleton
    {
        private static UserSingleton instance;
        public string Username { get; private set; }

        private UserSingleton() { }

        public static UserSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UserSingleton();
                }
                return instance;
            }
        }

        public void SetUsername(string username)
        {
            Username = username;
        }

        public void ClearUsername()
        {
            Username = null;
        }
    }
}
