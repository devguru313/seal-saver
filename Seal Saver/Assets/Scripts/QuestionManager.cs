using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public class QuestionManager : MonoBehaviour
{
    public GameObject optionSprite1;
    public GameObject optionSprite2;
    public GameObject optionSprite3;
    public GameObject optionSprite4;
    public GameObject questionUI;
    public GameObject coinRewardUI;
    public Sprite defaultButton;
    public Sprite greenButton;
    public Sprite redButton;
    public string startTime;                                        //Time when game level is loaded
    public static DateTime askTime;                                 //Time when question is asked
    public DateTime answerTime;                                     //Time when answer button is pressed
    public TimeSpan timeDuration;                                   //Time taken to answer question
    public string userID;
    
    private Image optionImage1;
    private Image optionImage2;
    private Image optionImage3;
    private Image optionImage4;

    public string inputPath;
    public string outputPath;
    public string questionID;
    public string question;
    public string correctAns;
    public string wrong1;
    public string wrong2;
    public string wrong3;
    public string option1;
    public string option2;
    public string option3;
    public string option4;
    public string questionSet;
    public string result;
    public string answerSelected;
    public string timeAsked;
    public string timeTaken;
    public string uQID;
    
    public static bool changeQuestion = false;
    public bool isStart = true;

    public Text questionText;
    public Text optionText1;
    public Text optionText2;
    public Text optionText3;
    public Text optionText4;

    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    public AudioSource audioSource;
    public AudioClip audioCorrect;
    public AudioClip audioWrong;
    public AudioClip audioCoins;
    public AudioSource backMusic;

    public int currentPlayerIndex;
    private int questionCount;
    private int inputQuestionNo;
    private string downloadedInputText;
    private bool internet = true;
    private List<string> inputQID = new List<string>();
    private List<string> inputQuestion = new List<string>();
    private List<string> inputCorrectAns = new List<string>();
    private List<string> inputWrong1 = new List<string>();
    private List<string> inputWrong2 = new List<string>();
    private List<string> inputWrong3 = new List<string>();
    private List<string> inputQuestionSet = new List<string>();
    private List<string> shuffleTemp = new List<string>();

    void Start ()
    {
        optionImage1 = optionSprite1.GetComponent<Image>();
        optionImage2 = optionSprite2.GetComponent<Image>();
        optionImage3 = optionSprite3.GetComponent<Image>();
        optionImage4 = optionSprite4.GetComponent<Image>();
        startTime = DateTime.Now.ToString();
        //Debug.Log("Start Time: " + startTime);
        questionText.text = "start";
        optionText1.text = "start";
        optionText2.text = "start";
        optionText3.text = "start";
        optionText4.text = "start";
        questionCount = 0;
        backMusic.volume = 0.75f;
        currentPlayerIndex = SyncTables.currentPlayerIndex;
        if (Login.loggedIn)
        {
            userID = Login.userID + "_" + currentPlayerIndex.ToString();
        }
        else
        {
            userID = "guest_0";
        }
        inputPath = GetApplicationPath() + userID + "_InputTable.csv";
        outputPath = GetApplicationPath() + userID + "_OutputTable.csv";
        inputQuestionNo = 0;
        //SetQuestion();
        changeQuestion = true;
    }

    void Update()
    {
        if (Login.loggedIn)
        {
            userID = Login.userID + "_" + currentPlayerIndex.ToString();
        }
        else
        {
            userID = "guest";
        }
        outputPath = GetApplicationPath() + userID + "_OutputTable.csv";
        inputPath = GetApplicationPath() + userID + "_InputTable.csv";

        if (changeQuestion)
        {
            changeQuestion = false;
            ReadInputSQL();
        }
    }

    public void SetQuestion()
    {
        ReadInput();
        questionText.text = question;
        optionText1.text = option1;
        optionText2.text = option2;
        optionText3.text = option3;
        optionText4.text = option4;
    }

    public int CorrectAnsIndex()
    {
        if (optionText1.text == correctAns)
            return 0;
        else if (optionText2.text == correctAns)
            return 1;
        else if (optionText3.text == correctAns)
            return 2;
        else
            return 3;
    }

    #region On Option Select Functions
    public void OnOptionSelect1()
    {
        timeAsked = askTime.ToString();
        answerTime = DateTime.Now;                                                          //Time when button is pressed
        timeDuration = answerTime.Subtract(askTime);                                        //Subtract with time when question was asked
        timeTaken = ((float)timeDuration.TotalSeconds).ToString("F1");                      //Rounding off double to int and then converting to string
        button1.interactable = false;
        button2.interactable = false;
        button3.interactable = false;
        button4.interactable = false;
        if (CorrectAnsIndex() == 0)
        {
            optionImage1.sprite = greenButton;
            result = "1";
            answerSelected = option1;
            questionCount += 1;
            WriteOutput();
            //changeQuestion = true;
            //Get a coin if answered 3 questions correctly
            if (questionCount >= 3)
            {
                coinRewardUI.SetActive(true);
                audioSource.PlayOneShot(audioCoins);
                questionCount = 0;
                InitScriptName.InitScript.Gems += 1;
                questionUI.GetComponent<AnimationManager>().CloseMenu();
                Invoke("ResetOnCorrect", 2.5f);
            }
            else
            {
                audioSource.PlayOneShot(audioCorrect);
                Invoke("ResetOnCorrect", 1);
            }
        }
        else
        {
            result = "0";
            answerSelected = option1;
            optionImage1.sprite = redButton;
            audioSource.PlayOneShot(audioWrong);
            switch (CorrectAnsIndex())
            {
                case 1:
                    optionImage2.sprite = greenButton;
                    button3.gameObject.SetActive(false);
                    button4.gameObject.SetActive(false);
                    break;
                case 2:
                    optionImage3.sprite = greenButton;
                    button2.gameObject.SetActive(false);
                    button4.gameObject.SetActive(false);
                    break;
                case 3:
                    optionImage4.sprite = greenButton;
                    button2.gameObject.SetActive(false);
                    button3.gameObject.SetActive(false);
                    break;
            }
            WriteOutput();
            //changeQuestion = true;
            Invoke("ResetOnWrong", 1.5f);
        }
    }

    public void OnOptionSelect2()
    {
        timeAsked = askTime.ToString();
        answerTime = DateTime.Now;
        timeDuration = answerTime.Subtract(askTime);
        timeTaken = ((float)timeDuration.TotalSeconds).ToString("F1");
        button1.interactable = false;
        button2.interactable = false;
        button3.interactable = false;
        button4.interactable = false;
        if (CorrectAnsIndex() == 1)
        {
            optionImage2.sprite = greenButton;
            result = "1";
            answerSelected = option2;
            questionCount += 1;
            WriteOutput();
            //changeQuestion = true;
            //Get a coin if answered 3 questions correctly
            if (questionCount >= 3)
            {
                coinRewardUI.SetActive(true);
                audioSource.PlayOneShot(audioCoins);
                questionCount = 0;
                InitScriptName.InitScript.Gems += 1;
                questionUI.GetComponent<AnimationManager>().CloseMenu();
                Invoke("ResetOnCorrect", 2.5f);
            }
            else
            {
                audioSource.PlayOneShot(audioCorrect);
                Invoke("ResetOnCorrect", 1);
            }
        }
        else
        {
            result = "0";
            answerSelected = option2;
            optionImage2.sprite = redButton;
            audioSource.PlayOneShot(audioWrong);
            switch (CorrectAnsIndex())
            {
                case 0:
                    optionImage1.sprite = greenButton;
                    button3.gameObject.SetActive(false);
                    button4.gameObject.SetActive(false);
                    break;
                case 2:
                    optionImage3.sprite = greenButton;
                    button1.gameObject.SetActive(false);
                    button4.gameObject.SetActive(false);
                    break;
                case 3:
                    optionImage4.sprite = greenButton;
                    button1.gameObject.SetActive(false);
                    button3.gameObject.SetActive(false);
                    break;
            }
            WriteOutput();
            //changeQuestion = true;
            Invoke("ResetOnWrong", 1.5f);
        }
    }

    public void OnOptionSelect3()
    {
        timeAsked = askTime.ToString();
        answerTime = DateTime.Now;
        timeDuration = answerTime.Subtract(askTime);
        timeTaken = ((float)timeDuration.TotalSeconds).ToString("F1");
        button1.interactable = false;
        button2.interactable = false;
        button3.interactable = false;
        button4.interactable = false;
        if (CorrectAnsIndex() == 2)
        {
            optionImage3.sprite = greenButton;
            result = "1";
            answerSelected = option3;
            questionCount += 1;
            WriteOutput();
            //changeQuestion = true;
            //Get a coin if answered 3 questions correctly
            if (questionCount >= 3)
            {
                coinRewardUI.SetActive(true);
                audioSource.PlayOneShot(audioCoins);
                questionCount = 0;
                InitScriptName.InitScript.Gems += 1;
                questionUI.GetComponent<AnimationManager>().CloseMenu();
                Invoke("ResetOnCorrect", 2.5f);
            }
            else
            {
                audioSource.PlayOneShot(audioCorrect);
                Invoke("ResetOnCorrect", 1);
            }
        }
        else
        {
            result = "0";
            answerSelected = option3;
            optionImage3.sprite = redButton;
            audioSource.PlayOneShot(audioWrong);
            switch (CorrectAnsIndex())
            {
                case 0:
                    optionImage1.sprite = greenButton;
                    button2.gameObject.SetActive(false);
                    button4.gameObject.SetActive(false);
                    break;
                case 1:
                    optionImage2.sprite = greenButton;
                    button1.gameObject.SetActive(false);
                    button4.gameObject.SetActive(false);
                    break;
                case 3:
                    optionImage4.sprite = greenButton;
                    button1.gameObject.SetActive(false);
                    button2.gameObject.SetActive(false);
                    break;
            }
            WriteOutput();
            //changeQuestion = true;
            Invoke("ResetOnWrong", 1.5f);
        }
    }

    public void OnOptionSelect4()
    {
        timeAsked = askTime.ToString();
        answerTime = DateTime.Now;
        timeDuration = answerTime.Subtract(askTime);
        timeTaken = ((float)timeDuration.TotalSeconds).ToString("F1");
        button1.interactable = false;
        button2.interactable = false;
        button3.interactable = false;
        button4.interactable = false;
        if (CorrectAnsIndex() == 3)
        {
            optionImage4.sprite = greenButton;
            result = "1";
            answerSelected = option4;
            questionCount += 1;
            WriteOutput();
            //changeQuestion = true;
            //Get a coin if answered 3 questions correctly
            if (questionCount >= 3)
            {
                coinRewardUI.SetActive(true);
                audioSource.PlayOneShot(audioCoins);
                questionCount = 0;
                InitScriptName.InitScript.Gems += 1;
                questionUI.GetComponent<AnimationManager>().CloseMenu();
                Invoke("ResetOnCorrect", 2.5f);
            }
            else
            {
                audioSource.PlayOneShot(audioCorrect);
                Invoke("ResetOnCorrect", 1);
            }
        }
        else
        {
            result = "0";
            answerSelected = option4;
            optionImage4.sprite = redButton;
            audioSource.PlayOneShot(audioWrong);
            switch (CorrectAnsIndex())
            {
                case 0:
                    optionImage1.sprite = greenButton;
                    button2.gameObject.SetActive(false);
                    button3.gameObject.SetActive(false);
                    break;
                case 1:
                    optionImage2.sprite = greenButton;
                    button1.gameObject.SetActive(false);
                    button3.gameObject.SetActive(false);
                    break;
                case 2:
                    optionImage3.sprite = greenButton;
                    button1.gameObject.SetActive(false);
                    button2.gameObject.SetActive(false);
                    break;
            }
            WriteOutput();
            //changeQuestion = true;
            Invoke("ResetOnWrong", 1.5f);
        }
    }
#endregion

    public void ResetOnCorrect()
    {
        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(true);
        button3.gameObject.SetActive(true);
        button4.gameObject.SetActive(true);
        coinRewardUI.SetActive(false);
        questionUI.GetComponent<AnimationManager>().CloseMenu();
        SyncTables.syncOutputNow = true;
        changeQuestion = true;
        SetQuestion();
        optionImage1.sprite = defaultButton;
        optionImage2.sprite = defaultButton;
        optionImage3.sprite = defaultButton;
        optionImage4.sprite = defaultButton;
        button1.interactable = true;
        button2.interactable = true;
        button3.interactable = true;
        button4.interactable = true;
    }

    public void ResetOnWrong()
    {
        questionCount = 0;
        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(true);
        button3.gameObject.SetActive(true);
        button4.gameObject.SetActive(true);
        questionUI.GetComponent<AnimationManager>().CloseMenu();
        SyncTables.syncOutputNow = true;
        changeQuestion = true;
        SetQuestion();
        GameEvent.ShowQuestion();
        optionImage1.sprite = defaultButton;
        optionImage2.sprite = defaultButton;
        optionImage3.sprite = defaultButton;
        optionImage4.sprite = defaultButton;
        button1.interactable = true;
        button2.interactable = true;
        button3.interactable = true;
        button4.interactable = true;
    }

    #region Read/Write Functions
    void ReadInput()
    {
        //Debug.Log("ReadInput called");
        //StartCoroutine("CheckInternetPing");
        internet = CheckInternetPing();
        if (internet)
        {
            inputQuestionNo = 0;
        }
        else
        {
            inputQuestionNo++;
        }
        var reader = new StreamReader(inputPath);
        for (int i = 0; i <= inputQuestionNo; i++)
        {
            var line = reader.ReadLine();
            var values = line.Split(',');
            questionID = values[0];
            question = values[1];
            correctAns = values[2];
            wrong1 = values[3];
            wrong2 = values[4];
            wrong3 = values[5];
            questionSet = values[6];
            //uQID = values[7];
        }
        reader.Close();
        shuffleTemp.Clear();
        shuffleTemp.Add(correctAns);
        shuffleTemp.Add(wrong1);
        shuffleTemp.Add(wrong2);
        shuffleTemp.Add(wrong3);
        Shuffle(shuffleTemp);
        option1 = shuffleTemp[0];
        option2 = shuffleTemp[1];
        option3 = shuffleTemp[2];
        option4 = shuffleTemp[3];
        //Debug.Log(question);
    }

    void ReadInputSQL()
    {
        //StartCoroutine("CheckInternetPing");
        internet = CheckInternetPing();
        //Debug.Log("CHECKING INTERNET: " + internet);
        if (internet)
        {
            inputQID.Clear();
            inputQuestion.Clear();
            inputCorrectAns.Clear();
            inputWrong1.Clear();
            inputWrong2.Clear();
            inputWrong3.Clear();
            inputQuestionSet.Clear();
            inputQuestionNo = 0;
            string readInputURL = "https://edplus.net/getNextQuestions";
            var request = new UnityWebRequest(readInputURL, "POST");
            ReadInputJSON readInputJSON = new ReadInputJSON()
            {
                UserID = Login.userID,
                PlayerID = currentPlayerIndex,
                previousFailures = 0
            };
            string json = JsonUtility.ToJson(readInputJSON);
            //Debug.Log(json);
            //Debug.Log("CALLING READ INPUT SQL");
            StartCoroutine(WaitForUnityWebRequestReadInput(request, json));
        }
    }

    IEnumerator WaitForUnityWebRequestReadInput(UnityWebRequest request, string json)
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
        ReadInputJSONResponse readInputJSONResponse = JsonUtility.FromJson<ReadInputJSONResponse>(request.downloadHandler.text);
        if (readInputJSONResponse.status != "success")
        {
            Debug.Log(readInputJSONResponse.data);
        }
        else
        {
            //Debug.Log(downloadCentralJSONResponse.data);
            downloadedInputText = readInputJSONResponse.data;
            var lines = downloadedInputText.Split('&');
            for (int i = 0; i < lines.Length - 1; i++)
            {
                //Debug.Log(lines[i]);
                var values = lines[i].Split(',');
                inputQID.Add(values[0]);
                inputQuestion.Add(values[1]);
                inputCorrectAns.Add(values[2]);
                inputWrong1.Add(values[3]);
                inputWrong2.Add(values[4]);
                inputWrong3.Add(values[5]);
                inputQuestionSet.Add(values[6]);
                //centralUQID.Add(values[7]);              //SEND AS STRING INSTEAD OF INT
            }
        }
        WriteInput(inputPath);
    }

    void WriteInput(string path)
    {
        var writer = new StreamWriter(path);
        for (int i = 0; i < inputQID.Count; i++)
        {
            writer.WriteLine(inputQID[i] + "," + inputQuestion[i] + "," + inputCorrectAns[i] + "," + inputWrong1[i] + "," + inputWrong2[i] + "," + inputWrong3[i] + "," + inputQuestionSet[i]/* + "," + centralUQID[i]*/);
        }
        writer.Flush();
        writer.Close();
        if (isStart)
        {
            isStart = false;
            SetQuestion();
        }
    }

    void WriteOutput()
    {
        var writer = new StreamWriter(outputPath, true);
        writer.WriteLine(userID + "," + questionID + "," + question + "," + result + "," + answerSelected + "," + timeAsked + "," + timeTaken + "," + questionSet/* + "," + uQID*/);
        writer.Flush();
        writer.Close();
    }
    #endregion

    #region GetPath Functions
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

    void Shuffle(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            string temp = list[i];
            int random = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[random];
            list[random] = temp;
        }
    }

    bool CheckInternetPing()
    {
        string checkInternetURL = "https://edplus.net/checkServerAlive";
        var varCheckInternetRequest = new UnityWebRequest(checkInternetURL, "POST");
        StartCoroutine(WaitForServer(varCheckInternetRequest));
        System.Threading.Thread.Sleep(100);
        if (!internet)
        {
            Debug.Log("Not Connected to Internet");
        }
        //Debug.Log(internet);
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
}
