using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerManager : MonoBehaviour {

    public GameObject playerButtonTemplate;
    public GameObject newNameMenu;
    public int numPlayers;
    public int playerIndex;
    public InputField nameField;
    public string playerList;
    public Text currentUserText;
    public GameObject loadingScreen;
    public Text errorTextCreateUser;
    public Dropdown yearDropdown;

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
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
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
            numPlayers = getPlayerDataJSONResponse.numPlayers;
            //Debug.Log(numPlayers);
            var playerNames = playerText.Split(' ');
            for (int i = 0; i < numPlayers; i++)
            {
                playerList += playerNames[i] + " ";
                GameObject newButton = Instantiate(playerButtonTemplate) as GameObject;
                newButton.SetActive(true);
                newButton.GetComponent<PlayerButtonController>().SetText(playerNames[i], i+1);
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
        newButton.GetComponent<PlayerButtonController>().SetText(nameField.text, numPlayers);
        newButton.transform.SetParent(playerButtonTemplate.transform.parent, false);
        newNameMenu.SetActive(false);
        playerList += nameField.text + " ";
        SyncTables.playerCoins.Add(numPlayers + "@20");
        //Debug.Log(playerList + numPlayers);
        WritePlayerDataSQL(nameField.text, SetTextFromIndex.year);
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
            BirthYear = year
        };
        string jsonChangePlayerData = JsonUtility.ToJson(changePlayerDataJSON);
        //Debug.Log(jsonChangePlayerData);
        StartCoroutine(WaitForUnityWebRequest(varChangePlayerDataRequest, jsonChangePlayerData));
    }

    IEnumerator WaitForUnityWebRequest(UnityWebRequest request, string json)
    {
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
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

    public void ShowLoading()
    {
        loadingScreen.SetActive(true);
    }
}
