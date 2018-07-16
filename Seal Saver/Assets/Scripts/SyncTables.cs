using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SyncTables : MonoBehaviour
{

    private string outputTablePath;
    private string inputTablePath;
    private string userID;
    private string deviceModel;
    private TextAsset tempTable;
    private string tempText;
    public GameObject internetMenu;
    public GameObject loadingScreen;
    public static bool syncUploadNow = false;
    public static bool syncOutputNow = false;
    public static bool checkTables = false;
    public static bool internetLogin = true;
    public static bool internet = true;

    public static bool getLevel = false;
    public static bool getStarsAndLevels = false;
    public static bool setStars = false;
    public static bool setLevels = false;
    public static List<string> playerData = new List<string>();
    public static List<string> playerCoins = new List<string>();
    public static int currentPlayerIndex;
    public static string knowledgeLevel;
    public static bool isLoggingIn;
    public static string gameName;
    public static string firebaseUID;
    public static string facebookUID;
    public bool internetLoginFlag;


    private void Start()
    {
        gameName = GameSpecificChanges.gameName;
        internetMenu.SetActive(false);
        deviceModel = SystemInfo.deviceModel.ToLower();
        if (SceneManager.GetActiveScene().name == "Login")
        {
            internetLoginFlag = true;
            CheckInternet();
        }
        if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
        {
            //To clear star data of all players before setting it up
            if (PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password"))
            {
                string temp1 = PlayerPrefs.GetString("Username");
                string temp2 = PlayerPrefs.GetString("Password");
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetString("Username", temp1);
                PlayerPrefs.SetString("Password", temp2);
            }
            else
            {
                PlayerPrefs.DeleteAll();
            }
        }
        if (SceneManager.GetActiveScene().buildIndex == 3 && isLoggingIn)
        {
            isLoggingIn = false;
            checkTables = true;
            getLevel = true;
        }

        #region Progress Analytics
        if(SceneManager.GetActiveScene().buildIndex == 0 && !deviceModel.Contains("amazon"))
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("GameProgress", "Percent", "0%");
        }
        if (SceneManager.GetActiveScene().buildIndex == 1 && !deviceModel.Contains("amazon"))
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("GameProgress", "Percent", "10%");
        }
        if (SceneManager.GetActiveScene().buildIndex == 2 && !deviceModel.Contains("amazon"))
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("GameProgress", "Percent", "20%");
        }
        if (SceneManager.GetActiveScene().buildIndex == 3 && !deviceModel.Contains("amazon"))
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("GameProgress", "Percent", "30%");
        }
        if(Login.firstLoginAnalytics && QuestionManager.isStart && !deviceModel.Contains("amazon"))
        {
            QuestionManager.isStart = false;
            Login.firstLoginAnalytics = false;
            Firebase.Analytics.FirebaseAnalytics.LogEvent("GameProgress", "Percent", "40%");
        }
        #endregion
    }

    private void Update()
    {
        if (Login.loggedIn)
        {
            userID = Login.userID + "_" + currentPlayerIndex.ToString();
        }
        else
        {
            //Does not matter since they will be replaced after logging in
            userID = "";
        }
        outputTablePath = GetApplicationPath() + userID + "_OutputTable.csv";
        inputTablePath = GetApplicationPath() + userID + "_InputTable.csv";
        //Debug.Log("CURRENT USER INDEX: " + userID);

        if (deviceModel.Contains("amazon"))
        {
            CheckInternetPing();
        }

        #region Flags
        if (syncUploadNow)
        {
            syncUploadNow = false;
            WriteCoinsSQL();
        }

        if (checkTables)
        {
            checkTables = false;
            CheckTablesExist();
        }

        if (getLevel)
        {
            getLevel = false;
            ReadLevelSQL();
        }

        if (getStarsAndLevels)
        {
            getStarsAndLevels = false;
            ReadStarsSQL();
        }

        if (setStars)
        {
            setStars = false;
            SetStars(PlayerDataManager.starCount, PlayerDataManager.levelCount);
        }

        if (setLevels)
        {
            setLevels = false;
            SetPlayerLevel(PlayerDataManager.levelCount);
        }
        #endregion
    }

    #region Application Pause and Quit
    private void OnApplicationPause(bool pause)
    {
        //Debug.Log("App Paused");
        if (Login.loggedIn)
        {
            syncUploadNow = true;
        }
    }

    private void OnApplicationQuit()
    {
        //Debug.Log("App Quit");
        syncUploadNow = true;
    }
    #endregion

    #region Check and Create Tables
    void CheckTablesExist()
    {
        //Debug.Log("Checking Tables if Exist");
        if (!File.Exists(inputTablePath))
        {
            tempTable = (TextAsset)Resources.Load(("inputTable"), typeof(TextAsset));
            tempText = tempTable.text;
            CreateTable(inputTablePath);
        }
        if (!File.Exists(outputTablePath))
        {
            tempTable = (TextAsset)Resources.Load(("outputTable"), typeof(TextAsset));
            tempText = tempTable.text;
            CreateTable(outputTablePath);
        }
    }

    void CreateTable(string path)
    {
        var writer = new StreamWriter(path);
        writer.Write(tempText);
        writer.Flush();
        writer.Close();
    }
    #endregion

    #region Knowledge Level
    void ReadLevelSQL()
    {
        string getPlayerLevelURL = "https://edplus.net/getPlayerLevel";
        var varGetPlayerLevelRequest = new UnityWebRequest(getPlayerLevelURL, "POST");
        GetPlayerLevelJSON getPlayerLevelJSON = new GetPlayerLevelJSON()
        {
            UserID = Login.userID,
            PlayerID = currentPlayerIndex,
            Email = Login.user,
            FirebaseUID = firebaseUID
        };
        string jsonGetPlayerLevel = JsonUtility.ToJson(getPlayerLevelJSON);
        internet = CheckInternetPing();
        if (internet)
        {
            StartCoroutine(WaitForUnityWebRequestLevel(varGetPlayerLevelRequest, jsonGetPlayerLevel));
        }
    }

    IEnumerator WaitForUnityWebRequestLevel(UnityWebRequest request, string json)
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
        GetPlayerLevelJSONResponse getPlayerLevelJSONResponse = JsonUtility.FromJson<GetPlayerLevelJSONResponse>(request.downloadHandler.text);
        if (request.downloadHandler.text == "" || request.downloadHandler.text == null)
        {
            knowledgeLevel = "1";
            ShowKnowledgeLevel.level = knowledgeLevel;
        }
        if (getPlayerLevelJSONResponse.status != "success")
        {
            Debug.Log(getPlayerLevelJSONResponse.data);
        }
        else
        {
            knowledgeLevel = getPlayerLevelJSONResponse.data;
            ShowKnowledgeLevel.level = knowledgeLevel;
            //Debug.Log("KNOWLEDGE: " + knowledgeLevel);
        }
    }
    #endregion

    #region Stars, Level and Coins
    void ReadStarsSQL()
    {
        string getPlayerStarsURL = "https://edplus.net/getPlayerStars";
        var varGetPlayerStarsRequest = new UnityWebRequest(getPlayerStarsURL, "POST");
        GetPlayerStarsJSON getPlayerStarsJSON = new GetPlayerStarsJSON()
        {
            UserID = Login.userID,
            Game = gameName
        };
        string jsonGetPlayerStars = JsonUtility.ToJson(getPlayerStarsJSON);
        internet = CheckInternetPing();
        if (internet)
        {
            StartCoroutine(WaitForUnityWebRequestPlayerStars(varGetPlayerStarsRequest, jsonGetPlayerStars));
        }
    }

    IEnumerator WaitForUnityWebRequestPlayerStars(UnityWebRequest request, string json)
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
        GetPlayerStarsJSONResponse getPlayerStarsJSONResponse = JsonUtility.FromJson<GetPlayerStarsJSONResponse>(request.downloadHandler.text);
        if (request.downloadHandler.text == "" || request.downloadHandler.text == null)
        {
            playerCoins.Clear();
            playerData.Clear();
        }
        if (getPlayerStarsJSONResponse.status != "success")
        {
            Debug.Log(getPlayerStarsJSONResponse.data);
        }
        else
        {
            playerData.Clear();
            playerCoins.Clear();
            string text;
            text = getPlayerStarsJSONResponse.data;
            if(text == "&")
            {
                playerCoins.Add("@10");
            }
            else
            {
                var rows = text.Split('&');
                //rows.Length - 1 to account for blank row
                for (int i = 0; i < rows.Length - 1; i++)
                {
                    //Split to separate coins
                    var col = rows[i].Split('@');
                    playerData.Add(col[0]);
                    //Debug.Log(col[0]);
                    playerCoins.Add(rows[i]);
                    //Debug.Log(rows[i]);
                }
            }
        }
        if (PlayerButtonController.fromPBController)
        {
            PlayerButtonController.fromPBController = false;
            SceneManager.LoadScene(3);
        }
    }

    public void SetStars(int stars, int level)
    {
        //Code to store stars in SQL
        if (Login.subscribed || level < 2)
        {
            //To update stars in playerData as well
            for (int i = 0; i < playerData.Count; i++)
            {
                var values = playerData[i].Split(' ');
                if (values[0] == currentPlayerIndex.ToString())
                {
                    playerData[i] += stars + " ";
                }
            }
            string setPlayerStarsURL = "https://edplus.net/setPlayerStars";
            var varSetPlayerStarsRequest = new UnityWebRequest(setPlayerStarsURL, "POST");
            SetPlayerStarsJSON setPlayerStarsJSON = new SetPlayerStarsJSON()
            {
                UserID = Login.userID,
                PlayerID = currentPlayerIndex,
                GameLevel = level,
                Stars = stars,
                Game = gameName
            };
            string jsonSetPlayerStars = JsonUtility.ToJson(setPlayerStarsJSON);
            //Debug.Log("SET STARS");
            internet = CheckInternetPing();
            if (internet)
            {
                StartCoroutine(WaitForUnityWebRequestSetPlayerStars(varSetPlayerStarsRequest, jsonSetPlayerStars));
            }
        }
    }

    IEnumerator WaitForUnityWebRequestSetPlayerStars(UnityWebRequest request, string json)
    {
        //Debug.Log("SET STARS REQUEST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        //Debug.Log("SET STARS Response: " + request.downloadHandler.text);
        SetPlayerStarsJSONResponse setPlayerStarsJSONResponse = JsonUtility.FromJson<SetPlayerStarsJSONResponse>(request.downloadHandler.text);
        if (request.downloadHandler.text == "" || request.downloadHandler.text == null)
        {
            Debug.Log("Connection Error");
        }
    }

    public void SetPlayerLevel(int level)
    {
        //Code to store level in SQL
        if (Login.subscribed || level < 3)
        {
            string setGameLevelURL = "https://edplus.net/setGameLevel";
            var varSetGameLevelRequest = new UnityWebRequest(setGameLevelURL, "POST");
            SetGameLevelJSON setGameLevelJSON = new SetGameLevelJSON()
            {
                UserID = Login.userID,
                PlayerID = currentPlayerIndex,
                GameLevel = level,
                Game = gameName
            };
            string jsonSetGameLevel = JsonUtility.ToJson(setGameLevelJSON);
            //Debug.Log("SET LEVEL");
            internet = CheckInternetPing();
            if (internet)
            {
                StartCoroutine(WaitForUnityWebRequestSetGameLevel(varSetGameLevelRequest, jsonSetGameLevel));
            }
        }
    }

    IEnumerator WaitForUnityWebRequestSetGameLevel(UnityWebRequest request, string json)
    {
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        //Debug.Log("SET LEVEL REQUEST");
        yield return request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        //Debug.Log("Response SET LEVEL: " + request.downloadHandler.text);
        SetGameLevelJSONResponse setGameLevelJSONResponse = JsonUtility.FromJson<SetGameLevelJSONResponse>(request.downloadHandler.text);
        if (request.downloadHandler.text == "" || request.downloadHandler.text == null)
        {
            Debug.Log("Connection Error");
        }
    }

    void WriteCoinsSQL()
    {
        string setCoinsURL = "https://edplus.net/setCoins";
        var varSetCoinsRequest = new UnityWebRequest(setCoinsURL, "POST");
        SetCoinsJSON setCoinsJSON = new SetCoinsJSON()
        {
            UserID = Login.userID,
            PlayerID = currentPlayerIndex,
            Coins = GameSpecificChanges.coins,
            Game = gameName
        };
        string jsonSetCoins = JsonUtility.ToJson(setCoinsJSON);
        internet = CheckInternetPing();
        if (internet)
        {
            StartCoroutine(WaitForUnityWebRequest(varSetCoinsRequest, jsonSetCoins));
        }
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
        internet = CheckInternetPing();
        //Debug.Log("Response: " + request.downloadHandler.text);
    }
    #endregion

    #region Internet
    public void CheckInternet()
    {
        internet = CheckInternetPing();
    }

    bool CheckInternetPing()
    {
        string checkInternetURL = "https://edplus.net/checkServerAlive";
        var varCheckInternetRequest = new UnityWebRequest(checkInternetURL, "POST");
        StartCoroutine(WaitForServer(varCheckInternetRequest));
        System.Threading.Thread.Sleep(50);
        if (!internet)
        {
            Debug.Log("Not Connected to Internet");
        }
        return internet;
    }

    IEnumerator WaitForServer(UnityWebRequest request)
    {
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes("{}");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        //Debug.Log("CHECK INTERNET Response: " + request.downloadHandler.text);
        ServerJSONResponse serverJSONResponse = JsonUtility.FromJson<ServerJSONResponse>(request.downloadHandler.text);
        if (request.downloadHandler.text == "" || request.downloadHandler.text == null)
        {
            internet = false;
            if (deviceModel.Contains("amazon") || internetLoginFlag)
            {
                internetLoginFlag = false;
                internetMenu.SetActive(true);
            }
        }
        else if (serverJSONResponse.status == "success")
        {
            internet = true;
            internetMenu.SetActive(false);
            if (deviceModel.Contains("amazon") || internetLoginFlag)
            {
                internetLoginFlag = false;
            }
        }
        else
        {
            internet = false;
            if (deviceModel.Contains("amazon") || internetLoginFlag)
            {
                internetLoginFlag = false;
                internetMenu.SetActive(true);
            }
        }
    }

    public void OpenTermsAndConditionsURL()
    {
        Application.OpenURL("https://edplus.io/privacy/");
    }

    public void OpenParentsDashboardURL()
    {
        string email = Login.user;
        string game = WWW.EscapeURL(GameSpecificChanges.gameName);
        string url = "https://edbit.app/?email=" + email + "&app=" + game;
        Application.OpenURL(url);
    }
    #endregion

    #region Get Path Functions
    private string GetApplicationPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/CSV/";
#elif UNITY_ANDROID
        return Application.persistentDataPath;
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/";
#else
        return Application.dataPath +"/";
#endif
    }
    #endregion

    #region Other Functions
    public void ShowLoading()
    {
        loadingScreen.SetActive(true);
    }
    #endregion
}