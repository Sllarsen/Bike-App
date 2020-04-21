using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeVT.Models
{
    public class FirebaseHelper
    {

        FirebaseClient firebase = new FirebaseClient("https://bikemobilea.firebaseio.com/");

        public async Task<List<User>> GetAllUsers()
        {
            return (await firebase
                .Child("Users")
                .OnceAsync<User>()).Select(item => new User
                {
                    Id = item.Object.Id,
                    Name = item.Object.Name,
                    GivenName = item.Object.GivenName,
                    FamilyName = item.Object.FamilyName,
                    Email = item.Object.Email
                }).ToList();
        }

        public async Task AddUser(User curUser)
        {
            await firebase
                .Child("Users")
                .PostAsync(new User() { Id = curUser.Id, Email=curUser.Email, 
                                        FamilyName = curUser.FamilyName, GivenName = curUser.GivenName, 
                                        Name = curUser.Name });
        }

        public async Task<User> GetUser(string id)
        {
            var allUsers = await GetAllUsers();
            await firebase
                .Child("Users")
                .OnceAsync<User>();
            return allUsers.Where(a => a.Id == id).FirstOrDefault();
        }

    }
}
