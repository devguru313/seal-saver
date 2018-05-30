using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShowUserInSettings : MonoBehaviour {

    public Text welcomeText;
    private string tempText;

    private void Update()
    {
        if (Login.loggedIn)
        {
            tempText = "Welcome " + PlayerPrefs.GetString("CurrentPlayerName");
            welcomeText.text = tempText;
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
}
