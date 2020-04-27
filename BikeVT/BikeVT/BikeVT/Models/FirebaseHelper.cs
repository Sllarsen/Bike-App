using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeVT.Models
{
    public class FirebaseHelper
    {

        public FirebaseClient firebase = new FirebaseClient("https://bikemobilea.firebaseio.com/");

        public async Task<List<User>> GetAllUsers()
        {
            return (await firebase
                .Child("Users")
                .OnceAsync<User>()).Select(item => new User
                {
                    Id = item.Object.Id,
                    GivenName = item.Object.GivenName,
                    FamilyName = item.Object.FamilyName,
                    Email = item.Object.Email,
                    Age = item.Object.Age,
                    Gender = item.Object.Gender,
                    Weight = item.Object.Weight,
                    BikerStatus = item.Object.BikerStatus
                }).ToList();
        }

        public async Task AddUser(User curUser)
        {
            await firebase
                .Child("Users")
                .PostAsync(new User() 
                { 
                    Id = curUser.Id, 
                    Email=curUser.Email, 
                    FamilyName = curUser.FamilyName, 
                    GivenName = curUser.GivenName,
                    Age = 0,
                    Gender = "N/A",
                    Weight = 0,
                    BikerStatus = "N/A"
                });
        }

        public async Task UpdateUserInfo(User user)
        {
            var curUser = (await firebase
                .Child("Users")
                .OnceAsync<User>()).Where(a => a.Object.Id == user.Id).FirstOrDefault();

            await firebase
                .Child("Users")
                .Child(curUser.Key)
                .PatchAsync(new User()
                {
                    Id = user.Id,
                    Email = user.Email,
                    FamilyName = user.FamilyName,
                    GivenName = user.GivenName,
                    Age = user.Age,
                    Gender = user.Gender,
                    Weight = user.Weight,
                    BikerStatus = user.BikerStatus
                });
        }

        public async Task AddTripToUser(User user, Trip t) 
        {
            var curUser = (await firebase
                .Child("Users")
                .OnceAsync<User>()).Where(a => a.Object.Id == user.Id).FirstOrDefault();

            await firebase
                .Child("Users")
                .Child(curUser.Key)
                .Child("_Trips")
                .PostAsync(new Trip()
                { 
                    StartTime = t.StartTime,
                    StartLocation = t.StartLocation,
                    WeatherData = t.WeatherData,
                    EndTime = "",
                    EndLocation = "",
                    acel = t.acel
                });
        }

     

        public async Task AddAcelData(User user, Trip t, string data)
        {
            var curUser = (await firebase
               .Child("Users")
               .OnceAsync<User>()).Where(a => a.Object.Id == user.Id).FirstOrDefault();
            var curTrip = (await firebase
                .Child("Users")
                .Child(curUser.Key)
                .Child("_Trips")
                .OnceAsync<Trip>()).Where(a => a.Object.StartTime == t.StartTime).FirstOrDefault();

            await firebase
                .Child("Users")
                .Child(curUser.Key)
                .Child("_Trips")
                .Child(curTrip.Key)
                .Child("Data")
                .Child("Acel")
                .PostAsync(new Acel { Values = data });
        }

        //GPS DATA UPLOAD
        public async Task AddGPSData(User user, Trip t, string data)
        {
            var curUser = (await firebase
               .Child("Users")
               .OnceAsync<User>()).Where(a => a.Object.Id == user.Id).FirstOrDefault();
            var curTrip = (await firebase
                .Child("Users")
                .Child(curUser.Key)
                .Child("_Trips")
                .OnceAsync<Trip>()).Where(a => a.Object.StartTime == t.StartTime).FirstOrDefault();

            await firebase
                .Child("Users")
                .Child(curUser.Key)
                .Child("_Trips")
                .Child(curTrip.Key)
                .Child("Data")
                .Child("GPS")
                .PostAsync(new GPS { Values = data });
        }



        public async Task AddGyroData(User user, Trip t, string data)
        {
            var curUser = (await firebase
               .Child("Users")
               .OnceAsync<User>()).Where(a => a.Object.Id == user.Id).FirstOrDefault();
            var curTrip = (await firebase
                .Child("Users")
                .Child(curUser.Key)
                .Child("_Trips")
                .OnceAsync<Trip>()).Where(a => a.Object.StartTime == t.StartTime).FirstOrDefault();

            await firebase
                .Child("Users")
                .Child(curUser.Key)
                .Child("_Trips")
                .Child(curTrip.Key)
                .Child("Data")
                .Child("Gyro")
                .PostAsync(new Gyro { Values = data });
        }

        public async Task UpdateTripToUser(User user, Trip t)
        {
            var curUser = (await firebase
               .Child("Users")
               .OnceAsync<User>()).Where(a => a.Object.Id == user.Id).FirstOrDefault();
            var curTrip = (await firebase
                .Child("Users")
                .Child(curUser.Key)
                .Child("_Trips")
                .OnceAsync<Trip>()).Where(a => a.Object.StartTime == t.StartTime).FirstOrDefault();

            await firebase
                .Child("Users")
                .Child(curUser.Key)
                .Child("_Trips")
                .Child(curTrip.Key)
                .PatchAsync(new Trip()
                {
                    StartTime = t.StartTime,
                    WeatherData = t.WeatherData,
                    StartLocation = t.StartLocation,
                    EndTime = t.EndTime,
                    EndLocation = t.EndLocation
                });

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
