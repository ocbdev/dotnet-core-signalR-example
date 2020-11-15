using System;
namespace SignalR.API.Models
{
    public class User
    {
        public User()
        {
          
        }

        public int Id { get; set; }

        public string name { get; set; }

        public virtual Team Team { get; set; }
    }
}
