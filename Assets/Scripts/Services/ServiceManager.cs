using System;
using System.Collections.Generic;
using Data;
using Facebook.Unity;
using UnityEngine;

namespace Services
{
    public static class ServiceManager
    {

        public static UserData User { get; private set; }
        public static bool IsLoggedIn
        {
            get { return FB.IsLoggedIn; }
        }
        private static bool IsInitialized
        {
            get { return FB.IsInitialized; }
        }

        public static void Setup()
        {
            if (!IsInitialized) FB.Init(OnComplete);
        }

        private static void OnComplete()
        {
            Debug.Log("Facebook initialized...");
            FB.ActivateApp();
        }

        public static void Login(Action<bool> onResult)
        {
            if (IsLoggedIn || !FB.IsInitialized)
            {
                if (IsLoggedIn) LoadUser();
                onResult(FB.IsInitialized);
                return;
            }

            var permissions = new[] {"public_profile", "email", "user_friends"};
            FB.LogInWithReadPermissions(permissions, result =>
            {
                Debug.Log("Login : " + IsLoggedIn);
                if (IsLoggedIn) LoadUser(result.ResultDictionary);
                onResult(IsLoggedIn);
            });
        }

        private static void LoadUser(IDictionary<string, object> dictionary = null)
        {
            var token = AccessToken.CurrentAccessToken;
            foreach (var permission in token.Permissions) Debug.Log(permission);

            if (dictionary != null)
            {
                if (dictionary.ContainsKey("name")) Values.Name = dictionary["name"].ToString();
                if (dictionary.ContainsKey("first_name")) Values.FirstName = dictionary["first_name"].ToString();
                if (dictionary.ContainsKey("last_name")) Values.LastName = dictionary["last_name"].ToString();
                if (dictionary.ContainsKey("email")) Values.Email = dictionary["email"].ToString();
            }
            else
            {
                FB.API("/me?fields=name,first_name,last_name,email", HttpMethod.GET, result =>
                {
                    LoadUser(result.ResultDictionary);
                });
            }

            User = new UserData
            {
                Id = token.UserId,
                Name = Values.Name,
                FirstName = Values.FirstName,
                LastName = Values.LastName,
                Email = Values.Email,
            };
            Debug.Log(User);
        }
    }

    public class UserData
    {
        public string Id;
        public string Name;
        public string FirstName;
        public string LastName;
        public string Email;

        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}, FirstName: {2}, LastName: {3}, Email: {4}", Id, Name, FirstName,
                LastName, Email);
        }
    }
}