using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoginFlags : MonoBehaviour {
    
    public void OnSignInPress()
    {
        SceneManager.LoadScene("Login");
    }

    public void QuitApp()
    {
#if !UNITY_IPHONE
        Application.Quit();
#endif
    }

}
