using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour {

    public Text userText;
    public GameObject MenuFeedback;
    public GameObject MenuOtherGames;
    public GameObject MenuSettings;
    public GameObject MenuChangeTopic;

    private void Update()
    {
        if (Login.loggedIn)
        {
            userText.text = "Hello " +  PlayerPrefs.GetString("CurrentPlayerName");
        }
    }

    public void HomeButton()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToHub()
    {
        //Clear player data before getting another player
        if (PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password"))
        {
            string temp1 = PlayerPrefs.GetString("Username");
            string temp2 = PlayerPrefs.GetString("Password");
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("Username", temp1);
            PlayerPrefs.SetString("Password", temp2);
            PlayerPrefs.SetInt("Lauched", 1);
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.DeleteAll();
        }
        SceneManager.LoadScene("Hub");
    }

    public void GoToParentZone()
    {
        PlayerManager.openParentZone = true;
        //Clear player data before getting another player
        if (PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password"))
        {
            string temp1 = PlayerPrefs.GetString("Username");
            string temp2 = PlayerPrefs.GetString("Password");
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("Username", temp1);
            PlayerPrefs.SetString("Password", temp2);
            PlayerPrefs.SetInt("Lauched", 1);
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.DeleteAll();
        }
        SceneManager.LoadScene("Hub");
    }

    public void ShowSettings()
    {
        MenuSettings.SetActive(true);
    }

    public void ShowFeedbackForm()
    {
        MenuFeedback.SetActive(true);
    }

    public void ShowOtherGames()
    {
        MenuOtherGames.SetActive(true);
    }

    public void ShowChangeTopic()
    {
        MenuChangeTopic.SetActive(true);
    }

    public void CloseMenu()
    {
        MenuSettings.SetActive(false);
    }
}
