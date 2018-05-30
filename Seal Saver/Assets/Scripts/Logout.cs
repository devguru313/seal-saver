using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;

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
        //Amazon Device check
        if (!deviceModel.Contains("amazon"))
        {
            auth.SignOut();
        }
        Login.loggedIn = false;
        PlayerPrefs.DeleteAll();
    }

    public void GoBackMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
