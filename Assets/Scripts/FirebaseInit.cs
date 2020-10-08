using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.Database;
using System.Threading.Tasks;


public class TestData
{
    public string name;
}

public class FirebaseInit : MonoBehaviour
{
    public static FirebaseInit instance;
    public string emailTest;
    public string passwordTest;

    [SerializeField] private GameObject initScreen = null;
    [SerializeField] private GameObject loginScreen = null;
    [SerializeField] private Text initText = null;
    [SerializeField] private Text loginText = null;
    [SerializeField] private Text dataText = null;
    protected Firebase.Auth.FirebaseAuth auth;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    public bool isAvailable;
    public bool isAuthenticated;

    private TestData data;


    // When the app starts, check to make sure that we have
    // the required dependencies to use Firebase, and if not,
    // add them if possible.
    public virtual void Start()
    {
        instance = this;
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                isAvailable = true;
                initScreen.SetActive(false);
                StartCoroutine(LoginUser(emailTest, passwordTest));
            }
            else
            {
                initScreen.SetActive(true);
                initText.text = "Could not resolve all Firebase dependencies: " + dependencyStatus;
            }
        });
    }
    private IEnumerator LoginUser(string email, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        // at this point, we either have a successful login or an issue, so lets handle both cases
        if (loginTask.Exception != null)
        {
            // do something to the UI with the failure.
            loginText.text = $"Failed to login user:\n{loginTask.Exception.Message}";
        }
        else
        {
            // successful auth, hide the login screen and grab the data
            loginScreen.SetActive(false);
            StartCoroutine(GetTestData());
        }
    }


    private IEnumerator GetTestData()
    {
        var loadTestDataTask = LoadData();// fire off the database call
        yield return new WaitUntil(() => loadTestDataTask.IsCompleted);
        if (loadTestDataTask.Exception != null)
            dataText.text = $"Data Exception:\n{loadTestDataTask.Exception.Message}";
        else
            dataText.text = $"Loaded Data\nPlayer Name: {loadTestDataTask.Result.name}";// new text field updated to show data loaded
    }

    public static async Task<TestData> LoadData()
    {
        var dbSnapshot = await FirebaseDatabase.DefaultInstance.GetReference("users/mcb").GetValueAsync();
        if (!dbSnapshot.Exists)
            return null;

        // save the loaded data right away into local data
        instance.data = JsonUtility.FromJson<TestData>(dbSnapshot.GetRawJsonValue());
        // return the resulting data
        return instance.data;
    }

    // called from the game
    public static void SavePlayer()
    {
        instance.data.name = "Beth";
        FirebaseDatabase.DefaultInstance.GetReference("users/mcb").SetRawJsonValueAsync(JsonUtility.ToJson(instance.data));
    }

}
