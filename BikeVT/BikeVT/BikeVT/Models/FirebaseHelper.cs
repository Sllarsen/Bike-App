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

        public async Task AddGPSData(User user, Trip t) 
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
                .PostAsync(new GPS { Time = "6", Value = "7" });
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

            Console.WriteLine("ADDING ACEL DATA ===========================================================");

            await firebase
                .Child("Users")
                .Child(curUser.Key)
                .Child("_Trips")
                .Child(curTrip.Key)
                .Child("Data")
                .Child("Acel")
                .PostAsync(new Acel { Time = DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm:ss.fff"), Value = data });
        }

        public async Task AddGyrolData(User user, Trip t)
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
                .PostAsync(new Gyro { Time = "6", Value = "7" });
        }

        public async Task AddDataToTrip(User user, Trip t)
        {
            var curUser = (await firebase
               .Child("Users")
               .OnceAsync<User>()).Where(a => a.Object.Id == user.Id).FirstOrDefault();
            var curTrip = (await firebase
                .Child("Users")
                .Child(curUser.Key)
                .Child("_Trips")
                .OnceAsync<Trip>()).Where(a => a.Object.StartTime == t.StartTime).FirstOrDefault();
            //create dummy values so the Data child populates
            List<Acel> aa = new List<Acel>();
            Acel temp = new Acel();
            temp.Time = "0";
            temp.Value = "0";
            aa.Add(temp);

            List<Gyro> gg = new List<Gyro>();
            Gyro tg = new Gyro();
            tg.Time = "0";
            tg.Value = "0";
            gg.Add(tg);

            List<GPS> g = new List<GPS>();
            GPS tempg = new GPS();
            tempg.Time = "0";
            tempg.Value = "0";
            g.Add(tempg);

            await firebase
                .Child("Users")
                .Child(curUser.Key)
                .Child("_Trips")
                .Child(curTrip.Key)
                .Child("Data")
                .PostAsync(new Data()
                {
                    acel = aa,
                    gyro = gg,
                    gps = g
                });
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
