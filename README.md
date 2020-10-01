# FirebaseAuthApp

This project is about figuring out the procedure for getting an android app, built in Unity, to connect to Firebase for authentication, and database access.

## Steps to Deploy

### Setup Firebase

- First thing first, we need a Google Firebase account, so create one, and then create a new Project.
- Now add a new 'Unity' App to your project.
- This will lead to Registering your app with Firebase, so set the same package name that you will use in your Project Settings / Player Settings. For example, `com.BauerVision.AuthApp`, and provide a nickname as well.
- Download the provided google_services.json file, and as directed, drag this into your Assets folder.
- Download the Firebase SDK as directed.
- Enable Authentication for your Firebase Project ( Email & Password ), and Realtime Database

### Setup Unity

- Make sure you convert your Unity file over to Android
- In Unity, add the firebase sdk, Assets/Import Package/Custom Package and browse to the
  `\firebase_unity_sdk.6.15.2\dotnet4\FirebaseAuth.unitypackage`
  that you download and import it. This will enabled the Scoped Registry which will enable access to all of the other features available.
- Edit/Preferences and at the bottom, un-check each of the checked SDks for Android, then re-check them. This will fix an error about Unity not knowing where `JAVA_HOME` is located.
- Edit / Build Settings / Player Settings / Publishing Settings -> Set up your keystore
- Under Build options, check `Custom Gradle Properties Template`
- Assets/ External Dependency Manager / Force Resolve to make sure all the t's get crossed.

### Associate your Unity App with Firebase

- With your keystore in place, you need to generate the SHA1.
- You don't need to install any Java to do this, Unity already installed when you needed.
  `C:\Program Files\Unity\Hub\Editor\2020.1.5f1\Editor\Data\PlaybackEngines\AndroidPlayer\OpenJDK\bin` is where keytool.exe is located.

- Open command prompt, change to the above directory and run, as an example:
  `keytool -exportcert -alias "authApp" -keystore "C:\Bauer\Dev\My Courses\Unity\FireBase\AuthApp\AuthAppKey.keystore" -list -v`

Note: `authApp` and `C:\Bauer\Dev\My Courses\Unity\FireBase\AuthApp\AuthAppKey.keystore` will need to be replaced with the alias you chose, and the path to where your keystore file was saved.

You should be prompted to enter your password, enter it, and copy the SHA1 key from the output.

- Back in your Firebase console, go to your Project's Settings, scroll down to Your apps, select your App and add Fingerprint, and then paste your copied SHA1 code.

## Test Deploy

Now let's make sure that you can even successfully build an android app. Before you push a build, make sure you have some kind of a scene saved, and added to your Build.

- Connect your device via USB
- File / Build Settings
- For Run Device, select your device from the list
- Build and Run. This will ask you for where you want to save the apk, I like to put it inside of an `APK` folder, but it doesn't matter where it goes.

If all goes well, you should see you app load to your device.

## Test Authentication

With a successful build, it's time to make sure we can auth our users. Within the Scripts folder, I have a FirebaseInit class, which is assigned to an empty game object in my scene. Essentially, the UI has 2 screens, an Initializer which will display if there are any errors from Firebase, and then a Loaded screen which only displays if Firebase connection and setup wsa successful.

The key code here, taken from the Firebase example, is this:

```
public virtual void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                isAuthenticated = true;// signal to any other classes that we were successful
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

## Test Database

Now that we can successfully access Firebase with an authenticated user, let's make sure we can read and write to our database.
````
