using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerManager : MonoBehaviour {

    public GameObject playerButtonTemplate;
    public GameObject newNameMenu;
    public GameObject addUserButton;
    public GameObject parentZoneBackButton;
    public GameObject parentZoneButton;
    public GameObject logoutButton;
    public GameObject parentGatewayMenu;
    public int numPlayers;
    public int playerIndex;
    public InputField nameField;
    public InputField nameFieldUpdate;
    public string playerList;
    public Text currentUserText;
    public GameObject loadingScreen;
    public Text errorTextCreateUser;
    public Text yearText;
    public Text subjectText;
    public Text yearTextUpdate;
    public Text subjectTextUpdate;
    public Text parentGatewayPinText;
    public Text parentGatewayErrorText;
    public Text sceneTitle;
    public static bool parentZoneActive;
    public static bool openParentZone = false;
    public Sprite[] avatars = new Sprite[50];
    public InputField[] parentGatewayFields = new InputField[5];
    public string[] parentGatewayDigits = new string[5];

    private void Start()
    {
        newNameMenu.SetActive(false);
        loadingScreen.SetActive(true);
        numPlayers = 0;
        playerList = "";
        currentUserText.text = Login.user;
        parentZoneActive = false;
        ReadPlayerDataSQL();
    }

    public void ReadPlayerDataSQL()
    {
        //Debug.Log("Get Player Data");
        string getPlayerDataURL = "https://edplus.net/getPlayerData";
        var varGetPlayerDataRequest = new UnityWebRequest(getPlayerDataURL, "POST");
        GetPlayerDataJSON getPlayerDataJSON = new GetPlayerDataJSON()
        {
            LoggedInUserID = Login.userID
        };
        string jsonGetPlayerData = JsonUtility.ToJson(getPlayerDataJSON);
        //Debug.Log(jsonGetPlayerData);
        StartCoroutine(WaitForUnityWebRequestPlayerData(varGetPlayerDataRequest, jsonGetPlayerData));
    }

    IEnumerator WaitForUnityWebRequestPlayerData(UnityWebRequest request, string json)
    {
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        //Debug.Log("Response: " + request.downloadHandler.text);
        GetPlayerDataJSONResponse getPlayerDataJSONResponse = JsonUtility.FromJson<GetPlayerDataJSONResponse>(request.downloadHandler.text);
        if (getPlayerDataJSONResponse.status != "success")
        {
            Debug.Log(getPlayerDataJSONResponse.data);
        }
        else
        {
            string playerText = getPlayerDataJSONResponse.data;
            //Debug.Log(playerText);
            numPlayers = getPlayerDataJSONResponse.numPlayers;
            //Debug.Log(numPlayers);
            var playerNames = playerText.Split(',');
            string avatarText = getPlayerDataJSONResponse.playerAvatars;
            var playerAvatars = avatarText.Split(',');
            string birthYearList = getPlayerDataJSONResponse.playerBirthYears;
            var birthYears = birthYearList.Split(',');
            string subjectList = getPlayerDataJSONResponse.playerLevels;
            var subjects = subjectList.Split(',');
            for (int i = 0; i < numPlayers; i++)
            {
                playerList += playerNames[i] + " ";
                int avatarIndex;
                int.TryParse(playerAvatars[i], out avatarIndex);
                GameObject newButton = Instantiate(playerButtonTemplate) as GameObject;
                newButton.SetActive(true);
                newButton.GetComponent<PlayerButtonController>().SetText(playerNames[i], i+1, avatarIndex - 1, birthYears[i], subjects[i]);
                newButton.transform.SetParent(playerButtonTemplate.transform.parent, false);
            }
        }
        if(numPlayers == 0)
        {
            newNameMenu.SetActive(true);
        }
        if (openParentZone)
        {
            openParentZone = false;
            OpenParentZone();
        }
        loadingScreen.SetActive(false);
    }

    public void CreateUser()
    {
        if(nameField.text == "")
        {
            errorTextCreateUser.text = "Please complete all fields before continuing";
            return;
        }
        numPlayers += 1;
        GameObject newButton = Instantiate(playerButtonTemplate) as GameObject;
        newButton.SetActive(true);
        newButton.GetComponent<PlayerButtonController>().SetText(nameField.text, numPlayers, AvatarSelection.currentAvatar, yearText.text, subjectText.text);
        newButton.transform.SetParent(playerButtonTemplate.transform.parent, false);
        newNameMenu.SetActive(false);
        playerList += nameField.text + " ";
        SyncTables.playerCoins.Add(numPlayers + "@10");
        //Debug.Log(playerList + numPlayers);
        WritePlayerDataSQL(nameField.text, yearText.text, subjectText.text);
    }

    public void WritePlayerDataSQL(string playerName, string year, string subject)
    {
        string changePlayerDataURL = "https://edplus.net/createPlayer";
        var varChangePlayerDataRequest = new UnityWebRequest(changePlayerDataURL, "POST");
        ChangePlayerDataJSON changePlayerDataJSON = new ChangePlayerDataJSON()
        {
            LoggedInUserID = Login.userID,
            NumPlayers = numPlayers.ToString(),
            NewPlayer = playerName,
            BirthYear = year,
            PlayerAvatar = AvatarSelection.currentAvatar,
            Topic = subject
        };
        string jsonChangePlayerData = JsonUtility.ToJson(changePlayerDataJSON);
        //Debug.Log(jsonChangePlayerData);
        StartCoroutine(WaitForUnityWebRequest(varChangePlayerDataRequest, jsonChangePlayerData));
    }

    IEnumerator WaitForUnityWebRequest(UnityWebRequest request, string json)
    {
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        //Debug.Log("Response: " + request.downloadHandler.text);
        GetPlayerDataJSONResponse getPlayerDataJSONResponse = JsonUtility.FromJson<GetPlayerDataJSONResponse>(request.downloadHandler.text);
    }

    public void OpenNewNameMenu()
    {
        YearSubjectSelection.currentYear = 2012;
        YearSubjectSelection.currentSubject = 0;
        newNameMenu.SetActive(true);
    }

    public void OpenParentGateway()
    {
        string[] words = new string[5];
        for(int j = 0; j < parentGatewayDigits.Length; j++)
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
        if(fieldNo > 0 && fieldNo < 5)
        {
            parentGatewayFields[fieldNo].ActivateInputField();
        }
        else
        {
            for(int i = 0; i < parentGatewayFields.Length; i++)
            {
                if(parentGatewayDigits[i] != parentGatewayFields[i].text)
                {
                    parentGatewayErrorText.text = "Wrong pin. Try again.";
                    parentGatewayFields[0].ActivateInputField();
                    return;
                }
            }
            parentGatewayMenu.SetActive(false);
            OpenParentZone();
        }
    }

    public void OpenParentZone()
    {
        logoutButton.SetActive(false);
        parentZoneButton.SetActive(false);
        parentZoneBackButton.SetActive(true);
        addUserButton.SetActive(true);
        sceneTitle.text = "Parent Zone";
        parentZoneActive = true;
        var editButtons = GameObject.FindGameObjectsWithTag("PlayerEditButton");
        if(editButtons != null)
        {
            for(int i = 0; i < editButtons.Length; i++)
            {
                Image currentButton = editButtons[i].GetComponent<Image>();
                var tempColor = currentButton.color;
                tempColor.a = 1f;
                currentButton.color = tempColor;
            }
        }
    }

    public void CloseParentZone()
    {
        parentZoneBackButton.SetActive(false);
        addUserButton.SetActive(false);
        logoutButton.SetActive(true);
        parentZoneButton.SetActive(true);
        sceneTitle.text = "Select your name";
        parentZoneActive = false;
        var editButtons = GameObject.FindGameObjectsWithTag("PlayerEditButton");
        if (editButtons != null)
        {
            for (int i = 0; i < editButtons.Length; i++)
            {
                Image currentButton = editButtons[i].GetComponent<Image>();
                var tempColor = currentButton.color;
                tempColor.a = 0f;
                currentButton.color = tempColor;
            }
        }
    }

    public void UpdatePlayerDetails()
    {
        CloseParentZone();
        var playerButtons = GameObject.FindGameObjectsWithTag("PlayerButton");
        var playerAvatars = GameObject.FindGameObjectsWithTag("PlayerAvatar");
        string newName;
        if (nameFieldUpdate.text != "" && nameFieldUpdate.text != null)
        {
            playerButtons[SyncTables.currentPlayerIndex - 1].GetComponentInChildren<Text>().text = nameFieldUpdate.text;
            newName = nameFieldUpdate.text;
        }
        else
        {
            newName = playerButtons[SyncTables.currentPlayerIndex - 1].GetComponentInChildren<Text>().text;
        }
        playerAvatars[SyncTables.currentPlayerIndex - 1].GetComponent<Image>().sprite = avatars[AvatarSelection.currentAvatar];
        //Debug.Log("Get Player Data");
        string updatePlayerDataURL = "https://edplus.net/updatePlayer";
        var varUpdatePlayerDataRequest = new UnityWebRequest(updatePlayerDataURL, "POST");
        UpdatePlayerJSON updatePlayerDataJSON = new UpdatePlayerJSON()
        {
            UserID = Login.userID,
            PlayerID = SyncTables.currentPlayerIndex.ToString(),
            PlayerName = newName,
            PlayerAvatar = AvatarSelection.currentAvatar + 1,
            BirthYear = yearTextUpdate.text,
            Topic = subjectTextUpdate.text
        };
        string jsonUpdatePlayerData = JsonUtility.ToJson(updatePlayerDataJSON);
        //Debug.Log(jsonUpdatePlayerData);
        StartCoroutine(WaitForUnityWebRequestUpdatePlayerData(varUpdatePlayerDataRequest, jsonUpdatePlayerData));
    }

    IEnumerator WaitForUnityWebRequestUpdatePlayerData(UnityWebRequest request, string json)
    {
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        //Debug.Log("Response: " + request.downloadHandler.text);
        UpdatePlayerJSONResponse updatePlayerDataJSONResponse = JsonUtility.FromJson<UpdatePlayerJSONResponse>(request.downloadHandler.text);
        if (updatePlayerDataJSONResponse.status != "success")
        {
            Debug.Log(updatePlayerDataJSONResponse.data);
        }
        else
        {
            //Debug.Log(updatePlayerDataJSONResponse.data);
        }
    }

    public void ShowLoading()
    {
        if (!parentZoneActive)
        {
            loadingScreen.SetActive(true);
        }
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
