using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using Facebook.Unity;
using System.Collections.Generic;

public class Login : MonoBehaviour {

    public InputField Username;
    public InputField Password;
    public Text errorTextSignIn;
    public Text errorTextFirstLoginMenu;
    public Text forgotPasswordText;
    public static bool loggedIn;
    public static bool loggedInEmail;
    public static bool loggedInFacebook;
    public static bool firstLoginAnalytics;
    public static string user;
    public static string userID;
    public static string email;
    public string password;
    public GameObject loadingScreen;
    public GameObject firstLoginMenu;
    public GameObject facebookButton;
    public Toggle termsAndConditions;
    public static bool subscribed;
    public static bool newUser;
    public FirebaseAuth auth;
    public string advertID;
    public string deviceModel;

    void Start()
    {
        firstLoginMenu.SetActive(false);
        facebookButton.SetActive(false);
        deviceModel = SystemInfo.deviceModel.ToLower();

        //Amazon Device check
        if (!deviceModel.Contains("amazon"))
        {
            //Debug.Log("NOT AMAZON");
            facebookButton.SetActive(true);
            InitializeFirebase();
        }

        #region Advertising Check
        bool adIDCheck;
        advertID = "iOS";
#if !UNITY_IOS
        adIDCheck = Application.RequestAdvertisingIdentifierAsync(
        (string advertisingId, bool trackingEnabled, string error) =>
            GetAdvertisingID(advertisingId)
        );
        if (!adIDCheck)
        {
            GetAdvertisingID("Error");
        }
#endif
#endregion

        #region Auto-Login
        if (PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password"))
        {
            loadingScreen.SetActive(true);
            Username.text = PlayerPrefs.GetString("Username");
            Password.text = PlayerPrefs.GetString("Password");
            Launch();
        }
        #endregion
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    private void Update()
    {
        #region Auto-Login after Register
        if (Register.registered == true && loggedIn == false)
        {
            //Deactivate flag first
            Register.registered = false;
            //Obtain valus from Playerprefs, which was saved while registering
            if (PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password"))
            {
                Username.text = PlayerPrefs.GetString("Username");
                Password.text = PlayerPrefs.GetString("Password");
                newUser = true;
                Launch();
            }
        }
        #endregion
    }

    #region Main Login and GetUID
    public void Launch()
    {
        loadingScreen.SetActive(true);
        errorTextSignIn.text = "";
        forgotPasswordText.color = new Color(0.7137255f, 0.6627451f, 0.7019608f);
        if (Username.text == "" || Password.text == "")
        {
            errorTextSignIn.text = "Please complete all fields";
            loadingScreen.SetActive(false);
            return;
        }
        //Debug.Log("Launch Login");
        email = Username.text.ToLower();
        password = Password.text;
        string deviceModel = SystemInfo.deviceModel.ToLower();
        //Amazon Device check
        if (!deviceModel.Contains("amazon"))
        {
            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    loadingScreen.SetActive(false);
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    // Show the error
                    if (SyncTables.internetLogin == false)
                    {
                        errorTextSignIn.text = "Not connected to Internet";
                    }
                    else
                    {
                        errorTextSignIn.text = "Oops, that's not the correct email/password combination.";
                        forgotPasswordText.color = Color.red;
                    }
                    loadingScreen.SetActive(false);
                    loggedIn = false;
                    return;
                }
                FirebaseUser newUser = task.Result;
                //Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                SyncTables.firebaseUID = newUser.UserId;
                user = email;
                PlayerPrefs.SetString("Username", email);
                PlayerPrefs.SetString("Password", password);
                loggedIn = true;
                loggedInEmail = true;
                firstLoginAnalytics = true;
                StartCoroutine(GetUID("Email"));
            });
        }
        else
        {
            StartCoroutine(SendDetails());
        }
    }

    IEnumerator GetUID(string loginMethod)
    {
        //Debug.Log("Getting UID");
        string findUIDURL = "https://edplus.net/findUID";
        var request = new UnityWebRequest(findUIDURL, "POST");
        //Debug.Log(SyncTables.firebaseUID);
        FindUIDJSON findUIDJSON = new FindUIDJSON()
        {
            Email = user,
            FirebaseUID = SyncTables.firebaseUID,
            FacebookUID = SyncTables.facebookUID,
            Method = loginMethod,
            ADID = advertID,
            DeviceModel = SystemInfo.deviceModel,
            DeviceOS = SystemInfo.operatingSystem
        };
        string json = JsonUtility.ToJson(findUIDJSON);
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        //Debug.Log("Response: " + request.downloadHandler.text);
        FindUIDJSONResponse findUIDJSONResponse = JsonUtility.FromJson<FindUIDJSONResponse>(request.downloadHandler.text);
        //Debug.Log(findUIDJSONResponse.data);
        if (request.downloadHandler.text == "" || request.downloadHandler.text == null)
        {
            loadingScreen.SetActive(false);
        }
        else
        {
            var cols = findUIDJSONResponse.data.Split('&');
            userID = cols[0];
            Branch.setIdentity(userID);
            if (cols[1] == "1")
            {
                subscribed = true;
            }
            else
            {
                subscribed = false;
            }
        }
        if (newUser)
        {
            newUser = false;
            SyncTables.playerData.Clear();
            SyncTables.playerCoins.Clear();
        }
        else
        {
            SyncTables.getStarsAndLevels = true;
        }

        #region Login Analytics
        if (!deviceModel.Contains("amazon"))
        {
            Firebase.Analytics.Parameter[] LoginParameters = { new Firebase.Analytics.Parameter("EmailID", user), new Firebase.Analytics.Parameter("UserID", userID) };
            Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLogin, LoginParameters);
        }
        #endregion

        loadingScreen.SetActive(false);
        if (findUIDJSONResponse.firstTimeLogin == 1)
        {
            firstLoginMenu.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("Hub");
        }
    }
    #endregion

    #region Login API for Amazon Devices
    IEnumerator SendDetails()
    {
        //Debug.Log("SEND DETAILS");
        string loginUserURL = "https://edplus.net/loginUser";
        var request = new UnityWebRequest(loginUserURL, "POST");
        SendAuthDetailsJSON sendAuthDetailsJSON = new SendAuthDetailsJSON()
        {
            UserID = email,
            Password = password
        };
        string json = JsonUtility.ToJson(sendAuthDetailsJSON);
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        //Debug.Log("Response: " + request.downloadHandler.text);
        SendAuthDetailsJSONResponse sendAuthDetailsJSONResponse = JsonUtility.FromJson<SendAuthDetailsJSONResponse>(request.downloadHandler.text);
        if (request.downloadHandler.text == "" || request.downloadHandler.text == null)
        {
            loadingScreen.SetActive(false);
        }
        if (sendAuthDetailsJSONResponse.status != "success")
        {
            //Debug.Log(sendAuthDetailsJSONResponse.data);
            if (sendAuthDetailsJSONResponse.data == "EMAIL_NOT_FOUND" || sendAuthDetailsJSONResponse.data == "INVALID_PASSWORD")
            {
                errorTextSignIn.text = "Oops, that's not the correct email/password combination.";
                forgotPasswordText.color = Color.red;
                yield return null;
            }
            else if (SyncTables.internetLogin == false)
            {
                errorTextSignIn.text = "Not connected to Internet";
            }
            else
            {
                errorTextSignIn.text = "PLease try again";
                yield return null;
            }
            loadingScreen.SetActive(false);
        }
        else
        {
            //Debug.Log(sendAuthDetailsJSONResponse.data);
            SyncTables.firebaseUID = sendAuthDetailsJSONResponse.data;
            user = email;
            PlayerPrefs.SetString("Username", email);
            PlayerPrefs.SetString("Password", password);
            loggedIn = true;
            loggedInEmail = true;
            StartCoroutine(GetUID("Email"));
        }
    }
    #endregion

    #region Facebook Login
    public void FacebookLogin()
    {
        loadingScreen.SetActive(true);
        errorTextSignIn.text = "";
        var permissions = new List<string>() {"public_profile", "email", "user_location" };
        FB.LogInWithReadPermissions(permissions, AuthCallback);
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            SyncTables.facebookUID = aToken.UserId;
            //Debug.Log(aToken);
            Credential credential = FacebookAuthProvider.GetCredential(aToken.TokenString);
            auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    loadingScreen.SetActive(false);
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    errorTextSignIn.text = "You have to accept all permissions on Facebook to login using Facebook";
                    loadingScreen.SetActive(false);
                    return;
                }
                FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                SyncTables.firebaseUID = newUser.UserId;
            });
            //Debug.Log("User ID: " + SyncTables.facebookUID);
            FB.API("/me?fields=name,email", HttpMethod.GET, FetchProfileCallback, new Dictionary<string, string>() { });
        }
        else
        {
            Debug.Log("User cancelled login");
            loadingScreen.SetActive(false);
            loggedIn = false;
            return;
        }
    }

    private void FetchProfileCallback(IResult result)
    {
        if (FB.IsLoggedIn)
        {
            user = (string)result.ResultDictionary["email"];
            //Debug.Log("Email: " + user);
            //Debug.Log("Name: " + result.ResultDictionary["name"]);
            loggedIn = true;
            loggedInFacebook = true;
            StartCoroutine(GetUID("Facebook"));
        }
        else
        {
            loadingScreen.SetActive(false);
            loggedIn = false;
            return;
        }
    }
    #endregion

    #region Other Functions
    public void LoadPlayerPrefs()
    {
        // Prefill fields with saved datas
        if (PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password"))
        {
            Username.text = PlayerPrefs.GetString("Username");
            Password.text = PlayerPrefs.GetString("Password");
        }
    }

    public void ShowFirstLoginScreen()
    {
        if (!termsAndConditions.isOn)
        {
            errorTextFirstLoginMenu.text = "You have to accept the Terms and Conditions before continuing";
            return;
        }
        else
        {
            SceneManager.LoadScene("Hub");
        }
    }

    public void GetAdvertisingID(string adID)
    {
        advertID = adID;
    }
    #endregion
}
