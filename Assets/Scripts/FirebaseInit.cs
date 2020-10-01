using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Extensions;
using System.Threading.Tasks;
public class FirebaseInit : MonoBehaviour
{
    [SerializeField] private GameObject initScreen;
    [SerializeField] private Text initText;
    protected Firebase.Auth.FirebaseAuth auth;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    public bool isAuthenticated;

    // When the app starts, check to make sure that we have
    // the required dependencies to use Firebase, and if not,
    // add them if possible.
    public virtual void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                isAuthenticated = true;
                initScreen.SetActive(false);
            }
            else
            {
                initText.text = "Could not resolve all Firebase dependencies: " + dependencyStatus;
            }
        });
    }

}
