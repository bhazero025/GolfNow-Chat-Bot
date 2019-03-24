using System;

namespace GolfNowAPI
{
    public class User
    {
        private string EMail = "";
        private string Password = "";
        private string FirstName = "";
        private string LastName = "";
        private string Token = "";
        private string Location = "";
        private int ZipCode;

        public string getEmail()
        {
            return this.EMail;
        }

        public void setEmail(string EMail)
        {
            this.EMail = EMail;
        }

        public string getPassword()
        {
            return this.Password;
        }

        public void setPassword(string Password)
        {
            this.Password = Password;
        }

        public string getFirstname()
        {
            return this.FirstName;
        }

        public void setFirstname(string FirstName)
        {
            this.FirstName = FirstName;
        }

        public string getLastname()
        {
            return this.LastName;
        }

        public void setLastname(string LastName)
        {
            this.LastName = LastName;
        }

        public int getZipcode()
        {
            return this.ZipCode;
        }

        public void setZipcode(int ZipCode)
        {
            this.ZipCode = ZipCode;
        }

        public string getToken()
        {
            return this.Token;
        }

        public void setToken(string Token)
        {
            this.Token = Token;
        }

        public string getLocation()
        {
            return this.Location;
        }

        public void setLocation(string Location)
        {
            this.Location = Location;
        }

        public User()
        {
            EMail = "";
            Password = "";
        }

        public User(string EMail, string Password)
        {
            this.EMail = EMail;
            this.Password = Password;
        }


        public string ToJSON()
        {
            string json = String.Format("{{ \"EMail\" : \"{0}\", \"Password\" : \"{1}\"}}", EMail, Password);
            return json;
        }
    }

}