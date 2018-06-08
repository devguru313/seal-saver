using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

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
