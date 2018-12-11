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
    public int numPlayers;
    public int playerIndex;
    public InputField nameField;
    public string playerList;
    public Text currentUserText;
    public GameObject loadingScreen;
    public Text errorTextCreateUser;
    public Text yearText;

    private void Start()
    {
        newNameMenu.SetActive(false);
        loadingScreen.SetActive(true);
        numPlayers = 0;
        playerList = "";
        currentUserText.text = Login.user;
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
            for (int i = 0; i < numPlayers; i++)
            {
                playerList += playerNames[i] + " ";
                int avatarIndex;
                int.TryParse(playerAvatars[i], out avatarIndex);
                GameObject newButton = Instantiate(playerButtonTemplate) as GameObject;
                newButton.SetActive(true);
                newButton.GetComponent<PlayerButtonController>().SetText(playerNames[i], i+1, avatarIndex);
                newButton.transform.SetParent(playerButtonTemplate.transform.parent, false);
            }
        }
        if(numPlayers == 0)
        {
            newNameMenu.SetActive(true);
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
        newButton.GetComponent<PlayerButtonController>().SetText(nameField.text, numPlayers, AvatarSelection.currentAvatar);
        newButton.transform.SetParent(playerButtonTemplate.transform.parent, false);
        newNameMenu.SetActive(false);
        playerList += nameField.text + " ";
        SyncTables.playerCoins.Add(numPlayers + "@10");
        //Debug.Log(playerList + numPlayers);
        WritePlayerDataSQL(nameField.text, yearText.text);
    }

    public void WritePlayerDataSQL(string playerName, string year)
    {
        string changePlayerDataURL = "https://edplus.net/changePlayerData";
        var varChangePlayerDataRequest = new UnityWebRequest(changePlayerDataURL, "POST");
        ChangePlayerDataJSON changePlayerDataJSON = new ChangePlayerDataJSON()
        {
            LoggedInUserID = Login.userID,
            NumPlayers = numPlayers.ToString(),
            NewPlayer = playerName,
            BirthYear = year,
            PlayerAvatar = AvatarSelection.currentAvatar
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
        newNameMenu.SetActive(true);
    }

    public void OpenParentZone()
    {
        logoutButton.SetActive(false);
        parentZoneButton.SetActive(false);
        parentZoneBackButton.SetActive(true);
        addUserButton.SetActive(true);
    }

    public void CloseParentZone()
    {
        parentZoneBackButton.SetActive(false);
        addUserButton.SetActive(false);
        logoutButton.SetActive(true);
        parentZoneButton.SetActive(true);
    }

    public void ShowLoading()
    {
        loadingScreen.SetActive(true);
    }
}
