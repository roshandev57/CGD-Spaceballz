using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Linq;

public class FirebaseManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    //User Data variables
    [Header("UserData")]
    public TMP_InputField usernameField;
    public TMP_InputField Scoretxt;
    public TMP_InputField Scoretxt1;

    public GameObject scoreElement;
    public Transform scoreboardContent;

    //[Header("Main Menu")]
    //public TMP_Text usernameText;




    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void ClearLoginFeilds()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }
    public void ClearRegisterFeilds()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    //public void MainMenuButton()
    //{
    //    usernameMainMenuText.text = "Welcome " + User.DisplayName;
    //}

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }
    //Function for the sign out button
    public void SignOutButton()
    {
        auth.SignOut();
        UIManager.instance.OpenLoginPanel();
        ClearRegisterFeilds();
        ClearLoginFeilds();
    }
    //Function for the save button
    public void SaveDataButton()
    {
        StartCoroutine(UpdateUsernameAuth(usernameField.text));
        StartCoroutine(UpdateUsernameDatabase(usernameField.text));

        //StartCoroutine(LoadUserScore());

        //StartCoroutine(UpdateUserScore(int.Parse(Scoretxt.text)));
        //StartCoroutine(UpdateUserScore1(int.Parse(Scoretxt1.text)));

        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }
    //Function for the scoreboard button
    //public void ScoreboardButton()
    //{
    //    StartCoroutine(LoadScoreboardData());
    //}

    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
           // StartCoroutine(LoadUserData());

            yield return new WaitForSeconds(2);

            //UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");


            usernameField.text = User.DisplayName;
            UIManager.instance.OpenUserDataPanel(); // Change to user data UI
            confirmLoginText.text = "";
            ClearLoginFeilds();
            ClearRegisterFeilds();
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        UIManager.instance.OpenLoginPanel();
                        warningRegisterText.text = "";
                        ClearRegisterFeilds();
                        ClearLoginFeilds();
                    }
                }
            }
        }
    }

    private IEnumerator UpdateUsernameAuth(string _username)
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //Call the Firebase auth update user profile function passing the profile with the username
        var ProfileTask = User.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth username is now updated
        }
    }

    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    //private IEnumerator LoadUserScore()
    //{
    //    var dbTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();
    //    yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

    //    if (dbTask.Exception != null)
    //    {
    //        Debug.LogWarning($"Failed to load user score: {dbTask.Exception}");
    //    }
    //    else
    //    {
    //        var snapshot = dbTask.Result;
    //        if (snapshot != null && snapshot.Exists)
    //        {
    //            // Check if the snapshot contains the "score" and "score1" fields
    //            if (snapshot.HasChild("score") && snapshot.HasChild("score1"))
    //            {
    //                // Get the user's scores from the snapshot
    //                int score = int.Parse(snapshot.Child("score").Value.ToString());
    //                int score1 = int.Parse(snapshot.Child("score1").Value.ToString());

    //                Scoretxt.text = score.ToString();
    //                // Do something with score1, e.g., display it
    //                Scoretxt1.text = score1.ToString();
    //            }
    //            else
    //            {
    //                Debug.LogWarning($"User scores not found.");
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogWarning($"User score not found.");
    //        }
    //    }
    //}

    //private IEnumerator UpdateUserScore(int score)
    //{
    //    var dbTask = DBreference.Child("users").Child(User.UserId).Child("score").SetValueAsync(score);
    //    yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

    //    if (dbTask.Exception != null)
    //    {
    //        Debug.LogWarning($"Failed to update user score: {dbTask.Exception}");
    //    }
    //    else
    //    {
    //        Debug.Log($"User score updated to {score}.");
    //    }
    //}

    //private IEnumerator UpdateUserScore1(int score1)
    //{
    //   var dbTask = DBreference.Child("users").Child(User.UserId).Child("score1").SetValueAsync(score1);
    //    yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

    //    if (dbTask.Exception != null)
    //    {
    //        Debug.LogWarning($"Failed to update user score: {dbTask.Exception}");
    //    }
    //    else
    //    {
    //        Debug.Log($"User score updated to {score1}.");
    //    }
    //}



    //private IEnumerator LoadUserData()
    //{
    //    //Get the currently logged in user data
    //    var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

    //    yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

    //    if (DBTask.Exception != null)
    //    {
    //        Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
    //    }
    //    else if (DBTask.Result.Value == null)
    //    {
    //        //No data exists yet
    //        //scoreField.text = "0";
    //    }
    //    else
    //    {
    //        //Data has been retrieved
    //        DataSnapshot snapshot = DBTask.Result;

    //        Scoretxt.text = snapshot.Child("score").Value.ToString();
    //        Scoretxt1.text = snapshot.Child("score1").Value.ToString();

    //    }
    //}

    //private IEnumerator LoadScoreboardData()
    //{
    //    var DBTask = DBreference.Child("users").OrderByChild("score").GetValueAsync();

    //    yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

    //    if (DBTask.Exception != null)
    //    {
    //        Debug.LogWarning($"Failed to load scoreboard data: {DBTask.Exception}");
    //    }
    //    else if (DBTask.Result == null || !DBTask.Result.Exists)
    //    {
    //        Debug.LogWarning("Scoreboard data not found.");
    //    }
    //    else
    //    {
    //        DataSnapshot snapshot = DBTask.Result;

    //        if (snapshot.ChildrenCount <= 0)
    //        {
    //            Debug.Log("No users found in the scoreboard.");
    //        }
    //        else
    //        {
    //            foreach (Transform child in scoreboardContent.transform)
    //            {
    //                Destroy(child.gameObject);
    //            }

    //            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
    //            {
    //                if (childSnapshot.HasChild("username") && childSnapshot.HasChild("score") && childSnapshot.HasChild("score1"))
    //                {
    //                    string username = childSnapshot.Child("username").Value.ToString();
    //                    int score = int.Parse(childSnapshot.Child("score").Value.ToString());
    //                    int score1 = int.Parse(childSnapshot.Child("score1").Value.ToString());

    //                    GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
    //                    scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username, score, score1);
    //                }
    //                else
    //                {
    //                    Debug.LogWarning("Incomplete user data found in the scoreboard.");
    //                }
    //            }

    //            UIManager.instance.OpenScoreboardPanel();
    //        }
    //    }

        //public void LoadUsername()
        //{
        //    // Get the user's ID from the Firebase Authentication service
        //    string userId = User.UserId;

        //    // Get a reference to the user's data in the Firebase Realtime Database
        //    DatabaseReference userRef = DBreference.Child("users").Child(userId);

        //    // Get the user's username from the database
        //    userRef.Child("username").GetValueAsync().ContinueWith(task =>
        //    {
        //        if (task.IsFaulted)
        //        {
        //            // Handle the error
        //            Debug.LogError("Failed to load username from database.");
        //            return;
        //        }

        //        // Update the UI with the user's username
        //        string username = (string)task.Result.Value;
        //        Debug.LogFormat("Username loaded successfully: {0}", username);
        //        UIManager.instance.usernameText.text = string.Format("Welcome {0}", username);
        //    });
        //}

    
}

