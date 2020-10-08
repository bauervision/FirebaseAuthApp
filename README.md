# FirebaseAuthApp

This project is about figuring out the procedure for getting an android app, built in Unity, to connect to Firebase for authentication, and database access.

## Steps to Deploy

- Key Requirements: Unity 2020.1.7f1 minimum, Firebase SDK 6.16.0

### Setup Firebase

- First thing first, we need a Google Firebase account, so create one, and then create a new Project.
- Now add a new 'Unity' App to your project.
- This will lead to Registering your app with Firebase, so set the same package name that you will use in your Project Settings / Player Settings. For example, `com.BauerVision.AuthApp`, and provide a nickname as well.
- Download the provided **google_services.json** file, and as directed, drag this into your **Assets folder**.
- Enable Authentication for your Firebase Project ( Email & Password ), and Realtime Database

### Setup Unity

- Make sure you convert your Unity file over to Android
- In Unity, Edit / Build Settings / Player Settings / Publishing Settings -> Set up your keystore
- Add the following code to the bottom of manifest.json to enable installing the Firebase sdk inside Unity
  ```
  "scopedRegistries": [
    {
      "name": "Game Package Registry by Google",
      "url": "https://unityregistry-pa.googleapis.com",
      "scopes": [
        "com.google"
      ]
    }
  ]
  ```
- Using the Unity Package Manager, go ahead and install Firebase App (Core) first, and then Auth, and Database

### Associate your Unity App with Firebase

- With your keystore in place, you need to **generate the SHA1**.
- You don't need to install any Java to do this, Unity already installed when you needed.
  `C:\Program Files\Unity\Hub\Editor\2020.1.5f1\Editor\Data\PlaybackEngines\AndroidPlayer\OpenJDK\bin` is where keytool.exe is located.

- Open command prompt, change to the above directory and run, as an example:
  `keytool -exportcert -alias "authApp" -keystore "C:\MyFolder\MyGame\Unity\FireBase\AuthApp\AuthAppKey.keystore" -list -v`

Note: `authApp` and `C:\MyFolder\MyGame\Unity\FireBase\AuthApp\AuthAppKey.keystore` will need to be replaced with the alias you chose, and the path to where your keystore file was saved.

You should be prompted to enter your password, enter it, and copy the SHA1 key from the output.

- Back in your Firebase console, go to your Project's Settings, scroll down to Your apps, select your App and **Add Fingerprint**, and then paste your copied SHA1 code.

## Test Deploy

Now let's make sure that you can even successfully build an android app. Before you push a build, make sure you have some kind of a scene saved, and added to your Build.

- Connect your device via USB
- File / Build Settings
- For Run Device, select your device from the list
- Build and Run. This will ask you for where you want to save the apk, I like to put it inside of an `APK` folder, but it doesn't matter where it goes.

If all goes well, you should see you app load to your device.

## Test Firebase Setup

First thing we need to check is that Firebase has been properly setup and can connect to its servers

Within the Scripts folder, I have a FirebaseInit class, which is assigned to an empty game object in my scene. Essentially, the UI has 3 screens, an Initializer which will display if there are any errors from Firebase, a "Login" which displays while we wait for the authentication to return, and finally a Loaded screen which only displays if Firebase is good to go.

The key code here, taken from the Firebase example, is this:

```
public virtual void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                isAvailable = true;// signal to any other classes that we were successful
                initScreen.SetActive(false);// and hide the init screen
            }
            else
            {
                // otherwise display this message and its details on the UI
                initText.text = "Could not resolve all Firebase dependencies: " + dependencyStatus;
            }
        });
    }
```

So with this added, **test Play in your editor** and verify that you can get to the 2nd screen, proving that Firebase was setup successfully.

## Test Authentication

With a successful build, it's time to make sure we can auth our users.

Running this IEnumerator will process the authentication and handle the error, as well as the completed state.

```
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
```

Lets' push what we have to your device and verify all things are as they should be.

### Enable Debugging

- Before pushing this change, let's make sure we have debugging enabled. In VSCode, Debugger you should see
  `To customize Run and Debug create a launch.json file`
  Go ahead and click that to do so, and select **Unity Debugger**.

- Now Hit run on the Debugger in VsCode, and back in Unity -> Build Settings check Development Build and Script Debugging
- Now Build and Run

You should see the same result as in the editor, and checking the console for logs, you should see no errors.

## Test Database

Now that we can successfully access Firebase with an authenticated user, let's make sure we can read and write to our database.

Here are the two key methods

```
private IEnumerator GetTestData()
    {
        var loadTestDataTask = LoadData();// fire off the database call
        yield return new WaitUntil(() => loadTestDataTask.IsCompleted);
        dataText.text = $"Loaded Data\nPlayer Name: {data.name}";// new text field updated to show data loaded
    }

    public static async Task<TestData> LoadData()
    {
      // "users/mcb" is the realtime database location of my test data
        var dbSnapshot = await FirebaseDatabase.DefaultInstance.GetReference("users/mcb").GetValueAsync();
        if (!dbSnapshot.Exists)
        return null;

        // save the loaded data right away into local data
        instance.data = JsonUtility.FromJson<TestData>(dbSnapshot.GetRawJsonValue());
        // return the resulting data
        return instance.data;
    }
```

And outside of the FirebaseInit class, here is my simple data class

```
public class TestData
{
    public string name;
}
```

I added a new text field on the seconds screen which displays the data loaded form firebase.

With this, you now have a simple project that can auth a user, and load data from firebase.

## Save to Database

To push an update to the database

```
    public static void SavePlayer()
    {
        instance.data.name = "Beth";// change the data to push
        // and push the change
        FirebaseDatabase.DefaultInstance.GetReference("users/mcb").SetRawJsonValueAsync(JsonUtility.ToJson(instance.data));
    }
```
