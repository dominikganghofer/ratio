using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

namespace PCAD.Backend
{
    public class FirebaseConnection
    {
        DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
        private FirebaseApp _app;

        public void Initialize()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError(
                        "Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }

        protected virtual void InitializeFirebase()
        {
            _app = FirebaseApp.Create();
            StartListener();
        }

        public void AddScore(string json)
        {
            DatabaseReference reference =
                FirebaseDatabase.GetInstance(_app, "https://drawsynced-default-rtdb.europe-west1.firebasedatabase.app/")
                    .RootReference;
            reference.Child("pcad").SetRawJsonValueAsync(json).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Debug.Log("Faulted..");
                }

                if (t.IsCanceled)
                {
                    Debug.Log("Cancelled..");
                }

                if (t.IsCompleted)
                {
                    Debug.Log("complete");
                }
            });
        }

        protected void StartListener()
        {
            FirebaseDatabase.GetInstance(_app, "https://drawsynced-default-rtdb.europe-west1.firebasedatabase.app/")
                .RootReference.Child("pcad")
                .ValueChanged += (sender2, e2) =>
            {
                if (e2.DatabaseError != null)
                {
                    Debug.LogError(e2.DatabaseError.Message);
                    return;
                }

                if (e2.Snapshot == null || e2.Snapshot.ChildrenCount <= 0)
                    return;

                Debug.Log(e2.Snapshot.Value);
            };
        }
    }
}