using Firebase.Database;
using Unity.Entities;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Game.ECS.System
{
    [UpdateAfter(typeof(FirebaseRegistrationSystem))]
    public class FirebasePushDataBaseSystem : UpdateSystem
    {
        private DatabaseReference reference;
        
        protected override void OnCreate()
        {
            // Get the root reference location of the database.
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        }

        protected override void OnUpdate()
        {
            throw new NotImplementedException();
        }
        
        private void writeNewUser(string userId, string name, string email) {
            UserStateNetwork user = new UserStateNetwork(name, email);
            string json = JsonUtility.ToJson(user);

            reference.Child("users").Child(userId).SetRawJsonValueAsync(json);
        }
    }
}