using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour {

    public Text userText;
    public GameObject MenuFeedback;
    public GameObject MenuOtherGames;
    public GameObject MenuSettings;
    public GameObject MenuChangeTopic;
    public GameObject parentGatewayMenu;
    public Text parentGatewayPinText;
    public Text parentGatewayErrorText;
    public InputField[] parentGatewayFields = new InputField[5];
    public string[] parentGatewayDigits = new string[5];

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

    public void OpenParentGateway()
    {
        string[] words = new string[5];
        for (int j = 0; j < parentGatewayDigits.Length; j++)
        {
            int rand = Random.Range(0, 10);
            parentGatewayDigits[j] = rand.ToString();
            words[j] = IntToWord(rand);
        }
        parentGatewayPinText.text = words[0] + ", " + words[1] + ", " + words[2] + ", " + words[3] + ", " + words[4];
        for (int i = 0; i < parentGatewayFields.Length; i++)
        {
            parentGatewayFields[i].textComponent.text = "";
        }
        parentGatewayMenu.SetActive(true);
        parentGatewayFields[0].ActivateInputField();
    }

    public void ParentGatewayCheckField(int fieldNo)
    {
        if (fieldNo > 0 && fieldNo < 5)
        {
            parentGatewayFields[fieldNo].ActivateInputField();
        }
        else
        {
            for (int i = 0; i < parentGatewayFields.Length; i++)
            {
                if (parentGatewayDigits[i] != parentGatewayFields[i].text)
                {
                    parentGatewayErrorText.text = "Wrong pin. Try again.";
                    parentGatewayFields[0].ActivateInputField();
                    return;
                }
            }
            parentGatewayMenu.SetActive(false);
            GoToParentZone();
        }
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

    public string IntToWord(int integer)
    {
        switch (integer)
        {
            case 0:
                return "ZERO";
            case 1:
                return "ONE";
            case 2:
                return "TWO";
            case 3:
                return "THREE";
            case 4:
                return "FOUR";
            case 5:
                return "FIVE";
            case 6:
                return "SIX";
            case 7:
                return "SEVEN";
            case 8:
                return "EIGHT";
            case 9:
                return "NINE";
            default:
                return "ZERO";
        }
    }
}
