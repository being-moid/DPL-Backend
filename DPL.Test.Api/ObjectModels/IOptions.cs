using System;
using Microsoft.Extensions.Options;

namespace DPL.Test.Api.ObjectModels
{

	public class AppSettings
	{
        public UserStore UserStore { get; set; } = null!;
	}
	public class UserStore
	{
        public UserStore()
        {
            if (this.Users == null)
            {
                this.Users = new List<AppUser>();
            }
        }
        public List<AppUser> Users { get; set; }
    }
    public class AppUser
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
       

    }
    public class StoredUsers
    {
        private static List<AppUser> users;
        private StoredUsers()
        {
            users = new List<AppUser>()
            {
                new AppUser(){Email="abc@dpl.com",Password="123"}
            };
        }
        public static List<AppUser> GetAppUsers()
        {
            return users;
        }
    }

}

