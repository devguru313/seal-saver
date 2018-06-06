using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour {
    
    public GameObject signInMenu;
    public GameObject signUpMenu;
    public GameObject signUpEmail;
    public GameObject signUpPassword;
    public GameObject signUpName;
    public GameObject signUpNext;
    public GameObject registerButton;
    public GameObject termsConditions;
    public Text signInError;
    public Text signUpError;
    public Text forgotText;
    public GameObject forgotPasswordMenu;

    public void OpenSignInMenu()
    {
        signUpMenu.SetActive(false);
        forgotPasswordMenu.SetActive(false);
        signInMenu.SetActive(true);
        signInError.text = "";
        forgotText.color = new Color(0.7137255f, 0.6627451f, 0.7019608f);
    }

    public void OpenSignUpMenu()
    {
        signInMenu.SetActive(false);
        forgotPasswordMenu.SetActive(false);
        signUpMenu.SetActive(true);
        signUpEmail.SetActive(false);
        signUpPassword.SetActive(false);
        signUpNext.SetActive(true);
        signUpName.SetActive(false);
        registerButton.SetActive(false);
        termsConditions.SetActive(false);
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