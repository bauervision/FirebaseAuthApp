using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Extensions;
using Firebase.Database;
using System.Threading.Tasks;


public class TestData
{
    public string name;
}

public class FirebaseInit : MonoBehaviour
{
    public static FirebaseInit instance;

    [SerializeField] private GameObject initScreen = null;
    [SerializeField] private Text initText = null;
    [SerializeField] private Text dataText = null;
    protected Firebase.Auth.FirebaseAuth auth;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

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
                isAuthenticated = true;
                initScreen.SetActive(false);
                StartCoroutine(GetTestData());
            }
            else
            {
                initScreen.SetActive(true);
                initText.text = "Could not resolve all Firebase dependencies: " + dependencyStatus;
            }
        });
    }


    private IEnumerator GetTestData()
    {
        var loadTestDataTask = LoadData();// fire off the database call
        yield return new WaitUntil(() => loadTestDataTask.IsCompleted);
        dataText.text = $"Loaded Data\nPlayer Name: {data.name}";// new text field updated to show data loaded
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
