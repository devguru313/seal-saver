using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SyncTables : MonoBehaviour {
        
    private string outputTablePath;
    private string inputTablePath;
    private string userID;
    private TextAsset tempTable;
    private string tempText;
    public GameObject internetMenu;
    public GameObject loadingScreen;
    public static bool syncUploadNow = false;
    public static bool syncOutputNow = false;
    public static bool checkTables = false;
    public static bool internetLogin = true;
    public static bool internet = true;

    private string outputQID;
    private string outputQuestion;
    private string outputResult;
    private string outputOptionSelected;
    private string outputTimeAsked;
    private string outputTimeTaken;
    private string outputUserID;
    private string outputQuestionSet;
    private string outputUQID;
    public string answerText;
    
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

    private void Start()
    {
        gameName = GameSpecificChanges.gameName;
        internetMenu.SetActive(false);
        if (/*SceneManager.GetActiveScene().name == "menu" || */SceneManager.GetActiveScene().name == "Login")
        {
            CheckInternet();
        }
        if (SceneManager.GetActiveScene().name == "menu")
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

        #region Flags
        if (syncUploadNow)
        {
            syncUploadNow = false;
            WriteCoinsSQL();
        }

        if (syncOutputNow)
        {
            syncOutputNow = false;
            ReadOutput();
            WriteOutputSQL();
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

    #region Output
    void ReadOutput()
    {
        answerText = "";
        var reader = new StreamReader(outputTablePath);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(',');
            outputUserID = values[0];
            outputQID = values[1];
            outputQuestion = values[2];
            outputResult = values[3];
            outputOptionSelected = values[4];
            outputTimeAsked = values[5];
            outputTimeTaken = values[6];
            outputQuestionSet = values[7];
            answerText = answerText + values[1] + "," + values[7] + "," + values[5] + "," + values[6] + "," + values[3] + "," + values[4] + "&";
            //outputUQID = values[8];
        }
        reader.Close();
    }

    void WriteOutputSQL()
    {
        string insertOutputURL = "https://edplus.net/insertOutput";
        var request = new UnityWebRequest(insertOutputURL, "POST");
        InsertOutputJSON insertOutputJSON = new InsertOutputJSON()
        {
            UserID = userID,
            Level = knowledgeLevel,
            App = gameName,
            DeviceModel = SystemInfo.deviceModel,
            DeviceOS = SystemInfo.operatingSystem,
            Company = Application.companyName,
            UpdateCT = 1,
            Answers = answerText
            /*,
            UQID = outputUQID*/
        };
        string json = JsonUtility.ToJson(insertOutputJSON);
        internet = CheckInternetPing(false);
        if (internet)
        {
            StartCoroutine(WaitForUnityWebRequest(request, json));
        }
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
        internet = CheckInternetPing(true);
        //Debug.Log("Response: " + request.downloadHandler.text);
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
            PlayerID = currentPlayerIndex
        };
        string jsonGetPlayerLevel = JsonUtility.ToJson(getPlayerLevelJSON);
        internet = CheckInternetPing(false);
        if (internet)
        {
            StartCoroutine(WaitForUnityWebRequestLevel(varGetPlayerLevelRequest, jsonGetPlayerLevel));
        }
    }

    IEnumerator WaitForUnityWebRequestLevel(UnityWebRequest request, string json)
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
        GetPlayerLevelJSONResponse getPlayerLevelJSONResponse = JsonUtility.FromJson<GetPlayerLevelJSONResponse>(request.downloadHandler.text);
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
        internet = CheckInternetPing(false);
        if (internet)
        {
            StartCoroutine(WaitForUnityWebRequestPlayerStars(varGetPlayerStarsRequest, jsonGetPlayerStars));
        }
    }

    IEnumerator WaitForUnityWebRequestPlayerStars(UnityWebRequest request, string json)
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
        GetPlayerStarsJSONResponse getPlayerStarsJSONResponse = JsonUtility.FromJson<GetPlayerStarsJSONResponse>(request.downloadHandler.text);
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
            var rows = text.Split('&');
            //rows.Length - 1 to account for blank row
            for(int i = 0; i < rows.Length - 1; i++)
            {
                //Split to separate coins
                var col = rows[i].Split('@');
                playerData.Add(col[0]);
                playerCoins.Add(rows[i]);
                //Debug.Log(rows[i]);
            }
        }
    }

    public void SetStars(int stars, int level)
    {
        //Code to store stars in SQL
        if (Login.subscribed || level < 2)
        {
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
            internet = CheckInternetPing(false);
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
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        //Debug.Log("SET STARS Response: " + request.downloadHandler.text);
        SetPlayerStarsJSONResponse setPlayerStarsJSONResponse = JsonUtility.FromJson<SetPlayerStarsJSONResponse>(request.downloadHandler.text);
        if (setPlayerStarsJSONResponse.status != "success")
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
            internet = CheckInternetPing(false);
            if (internet)
            {
                StartCoroutine(WaitForUnityWebRequestSetGameLevel(varSetGameLevelRequest, jsonSetGameLevel));
            }
        }
    }

    IEnumerator WaitForUnityWebRequestSetGameLevel(UnityWebRequest request, string json)
    {
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        //Debug.Log("SET LEVEL REQUEST");
        yield return request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        //Debug.Log("Response SET LEVEL: " + request.downloadHandler.text);
        SetGameLevelJSONResponse setGameLevelJSONResponse = JsonUtility.FromJson<SetGameLevelJSONResponse>(request.downloadHandler.text);
        if (setGameLevelJSONResponse.status != "success")
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
        internet = CheckInternetPing(false);
        if (internet)
        {
            StartCoroutine(WaitForUnityWebRequest(varSetCoinsRequest, jsonSetCoins));
        }
    }
    #endregion

    #region Internet
    public void CheckInternet()
    {
        StartCoroutine(CheckInternetPingShowMenu());
    }

    IEnumerator CheckInternetPingShowMenu()
    {
        //Edtopia website taken as URL link to check connectivity
        WWW wwwInternet = new WWW("https://www.google.com");
        yield return wwwInternet;
        if (wwwInternet.bytesDownloaded == 0)
        {
            Debug.Log("Not Connected to Internet");
            internetMenu.SetActive(true);
            internetLogin = false;
        }
        else
        {
            //Debug.Log("Connected to Internet");
            internetMenu.SetActive(false);
            internetLogin = true;
        }
    }

    bool CheckInternetPing(bool insertOutput)
    {
        string checkInternetURL = "https://edplus.net/checkServerAlive";
        var varCheckInternetRequest = new UnityWebRequest(checkInternetURL, "POST");
        StartCoroutine(WaitForServer(varCheckInternetRequest));
        System.Threading.Thread.Sleep(100);
        if (!internet)
        {
            Debug.Log("Not Connected to Internet");
        }
        else
        {
            //Clear Output Table after sending data
            if (insertOutput)
            {
                insertOutput = false;
                File.WriteAllText(outputTablePath, string.Empty);
            }
        }
        return internet;
    }

    IEnumerator WaitForServer(UnityWebRequest request)
    {
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes("{}");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        //Debug.Log("CHECK INTERNET Response: " + request.downloadHandler.text);
        ServerJSONResponse serverJSONResponse = JsonUtility.FromJson<ServerJSONResponse>(request.downloadHandler.text);
        if (serverJSONResponse.status == "success")
        {
            internet = true;
        }
        else
        {
            internet = false;
        }
    }

    public void CloseInternetMenu()
    {
        CheckInternet();
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