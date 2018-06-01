using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour {
    
    public GameObject signInMenu;
    public GameObject signUpMenu;
    public GameObject signUpEmail;
    public GameObject signUpPassword;
    public GameObject signUpName;
    public GameObject signUpNext1;
    public GameObject signUpNext2;
    public GameObject signUpButton;
    public GameObject clickToReset;
    public Text signInError;
    public Text signUpError;
    public GameObject forgotPasswordMenu;

    public void OpenSignInMenu()
    {
        signUpMenu.SetActive(false);
        forgotPasswordMenu.SetActive(false);
        signInMenu.SetActive(true);
        signInError.text = "";
        clickToReset.SetActive(false);
    }

    public void OpenSignUpMenu()
    {
        signInMenu.SetActive(false);
        forgotPasswordMenu.SetActive(false);
        signUpMenu.SetActive(true);
        signUpEmail.SetActive(true);
        signUpNext1.SetActive(true);
        signUpPassword.SetActive(false);
        signUpNext2.SetActive(false);
        signUpName.SetActive(false);
        signUpButton.SetActive(false);
        signUpError.text = "";
    }

    public void OpenForgotPasswordMenu()
    {
        signUpMenu.SetActive(false);
        signInMenu.SetActive(false);
        forgotPasswordMenu.SetActive(true);
    }

    public void CloseAllMenus()
    {
        signInMenu.SetActive(false);
        signUpMenu.SetActive(false);
        forgotPasswordMenu.SetActive(false);
        SceneManager.LoadScene(0);
    }

    public void GoToHub()
    {
        SceneManager.LoadScene("Hub");
    }
    
}