using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject loadingScreen;

    public void Start()
    {
        loadingScreen.SetActive(false);
    }

    public void OnSignInPress()
    {
        loadingScreen.SetActive(true);
        SceneManager.LoadScene("Login");
    }

    public void QuitApp()
    {
#if !UNITY_IPHONE
        Application.Quit();
#endif
    }
}
