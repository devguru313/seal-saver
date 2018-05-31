using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using Facebook.Unity;

public class Logout : MonoBehaviour {
    
    public FirebaseAuth auth;

    private void Start()
    {
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

    private void Update()
    {
        if (Register.registered == true && Login.loggedIn == true)
        {
            Launch();
        }
    }

    public void Launch()
    {
        string deviceModel = SystemInfo.deviceModel.ToLower();
        if (Login.loggedInEmail)
        {
            //Amazon Device check
            if (!deviceModel.Contains("amazon"))
            {
                auth.SignOut();
            }
            Login.loggedInEmail = false;
        }
        else if (Login.loggedInFacebook)
        {
            FB.LogOut();
            Login.loggedInFacebook = false;
        }
        Login.loggedIn = false;
        PlayerPrefs.DeleteAll();
    }

    public void GoBackMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
