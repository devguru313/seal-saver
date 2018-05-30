using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using UnityEngine.Networking;

public class Register : MonoBehaviour {

    public InputField Username;
    public InputField Mail;
    public InputField Password;
    public Text errorTextSignUp;
    public static bool registered = false;
    public GameObject emailField;
    public GameObject passwordField;
    public GameObject nameField;
    public GameObject nextButton1;
    public GameObject nextButton2;
    public GameObject signupButton;
    public GameObject backButton;
    public string email;
    public string password;
    public string userName;
    public FirebaseAuth auth;
    public GameObject loadingScreen;

    private void Start()
    {
        emailField.SetActive(true);
        nextButton1.SetActive(true);
        passwordField.SetActive(false);
        nextButton2.SetActive(false);
        nameField.SetActive(false);
        signupButton.SetActive(false);
        backButton.SetActive(false);
        errorTextSignUp.text = "";
        string deviceModel = SystemInfo.deviceModel.ToLower();
        //Amazon Device check
        if (!deviceModel.Contains("amazon"))
        {
            InitializeFirebase();
        }
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void GoSignUpPage1()
    {
        errorTextSignUp.text = "";
        emailField.SetActive(true);
        nextButton1.SetActive(true);
        passwordField.SetActive(false);
        nextButton2.SetActive(false);
        nameField.SetActive(false);
        signupButton.SetActive(false);
        backButton.SetActive(false);
    }

    public void GoSignUpPage2()
    {
        if (Mail.text == "")
        {
            errorTextSignUp.text = "Please complete all fields";
        }
        else if (Mail.text.Length < 6 || !Mail.text.Contains("@") || Mail.text[0].Equals('@') || Mail.text[0].Equals("."))
        {
            errorTextSignUp.text = "Invalid Email";
        }
        else
        {
            email = Mail.text.ToLower();
            emailField.SetActive(false);
            nextButton1.SetActive(false);
            passwordField.SetActive(true);
            nextButton2.SetActive(true);
            nameField.SetActive(false);
            signupButton.SetActive(false);
            backButton.SetActive(true);
            errorTextSignUp.text = "";
        }
    }

    public void GoSignUpPage3()
    {
        if (Password.text == "")
        {
            errorTextSignUp.text = "Please complete all fields";
        }
        else if (Password.text.Length < 6)
        {
            errorTextSignUp.text = "Password should contain at least 6 characters";
        }
        else
        {
            password = Password.text;
            emailField.SetActive(false);
            nextButton1.SetActive(false);
            passwordField.SetActive(false);
            nextButton2.SetActive(false);
            nameField.SetActive(true);
            signupButton.SetActive(true);
            backButton.SetActive(true);
            errorTextSignUp.text = "";
        }
    }

    public void Launch()
    {
        loadingScreen.SetActive(true);
        errorTextSignUp.text = "";
        // Check that all field are set
        if (Username.text == "")
        {
            errorTextSignUp.text = "Please complete all fields";
            loadingScreen.SetActive(false);
            return;
        }
        userName = Username.text;
        Debug.Log("Registration launched.");
        //Debug.Log(email);
        string deviceModel = SystemInfo.deviceModel.ToLower();
        //Amazon Device check
        if (!deviceModel.Contains("amazon"))
        {
            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    loadingScreen.SetActive(false);
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    loadingScreen.SetActive(false);
                    errorTextSignUp.text = "Email already exists";
                    return;
                }

                //Firebase user has been created.
                Firebase.Auth.FirebaseUser newUser = task.Result;
                //Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                SyncTables.firebaseUID = newUser.UserId;
                //Save information in playerPrefs
                PlayerPrefs.SetString("Username", email);
                PlayerPrefs.SetString("Mail", email);
                PlayerPrefs.SetString("Password", password);
                Debug.Log("Registration succeeded.");
                registered = true;
                StartCoroutine(SendFirebaseUID());
            });
        }
        else
        {
            StartCoroutine(SendDetails());
        }
    }

    IEnumerator SendFirebaseUID()
    {
        string sendUIDURL = "https://edplus.net/createUser";
        var request = new UnityWebRequest(sendUIDURL, "POST");
        SendFirebaseUIDJSON sendUIDJSON = new SendFirebaseUIDJSON()
        {
            FirebaseUID = SyncTables.firebaseUID,
            Mail = email
        };
        string json = JsonUtility.ToJson(sendUIDJSON);
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        //Debug.Log("Response: " + request.downloadHandler.text);
    }

    IEnumerator SendDetails()
    {
        //Debug.Log("SEND DETAILS");
        string registerUserURL = "http://35.177.197.153/registerUser";
        var request = new UnityWebRequest(registerUserURL, "POST");
        SendAuthDetailsJSON sendAuthDetailsJSON = new SendAuthDetailsJSON()
        {
            UserID = email,
            Password = password
        };
        string json = JsonUtility.ToJson(sendAuthDetailsJSON);
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        //Debug.Log("Response: " + request.downloadHandler.text);
        SendAuthDetailsJSONResponse sendAuthDetailsJSONResponse = JsonUtility.FromJson<SendAuthDetailsJSONResponse>(request.downloadHandler.text);
        if (sendAuthDetailsJSONResponse.status != "success")
        {
            //Debug.Log(sendAuthDetailsJSONResponse.data);
            if(sendAuthDetailsJSONResponse.data == "EMAIL_EXISTS")
            {
                errorTextSignUp.text = "Email already exists";
                registered = false;
                yield return null;
            }
            else
            {
                errorTextSignUp.text = "Please try again";
                yield return null;
            }
        }
        else
        {
            SyncTables.firebaseUID = sendAuthDetailsJSONResponse.data;
            PlayerPrefs.SetString("Username", email);
            PlayerPrefs.SetString("Mail", email);
            PlayerPrefs.SetString("Password", password);
            //Debug.Log("Registration succeeded.");
            registered = true;
            StartCoroutine(SendFirebaseUID());
        }
    }
}
