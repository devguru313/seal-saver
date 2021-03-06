﻿using UnityEngine;
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
    public string hint;
    public string opID1;
    public string opID2;
    public string opID3;
    public string opID4;
    public string questionSet;
    public string result;
    public string answerSelected;
    public string timeAsked;
    public string timeTaken;
    public string uQID;
    public string answerText;
    
    public static bool isStart;
    public bool afterWrong;

    public Text questionText;
    public Text optionText1;
    public Text optionText2;
    public Text optionText3;
    public Text optionText4;
    public Text hintText;

    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    public AudioSource audioSource;
    public AudioClip audioCorrect;
    public AudioClip audioWrong;
    public AudioClip audioCoins;
    
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
    private List<string> inputHint = new List<string>();
    private List<string> shuffleTemp = new List<string>();

    public Text topicText;

    public static bool hintsOn = true;
    public Image hintButtonImage;
    public Sprite hintOnImage;
    public Sprite hintOffImage;

    void Start()
    {
        isStart = true;
        ReadInputSQL();
        optionImage1 = optionSprite1.GetComponent<Image>();
        optionImage2 = optionSprite2.GetComponent<Image>();
        optionImage3 = optionSprite3.GetComponent<Image>();
        optionImage4 = optionSprite4.GetComponent<Image>();
        startTime = DateTime.Now.ToString();
        //Debug.Log("Start Time: " + startTime);
        questionText.text = "Please click to continue";
        optionText1.text = "";
        optionText2.text = "";
        optionText3.text = "";
        optionText4.text = "";
        questionCount = 0;
        if (Login.loggedIn)
        {
            userID = Login.userID + "_" + SyncTables.currentPlayerIndex.ToString();
        }
        else
        {
            userID = "guest_0";
        }
        inputPath = GetApplicationPath() + userID + "_InputTable.csv";
        outputPath = GetApplicationPath() + userID + "_OutputTable.csv";
        inputQuestionNo = 0;
        if (hintsOn)
        {
            hintButtonImage.sprite = hintOnImage;
        }
        else
        {
            hintButtonImage.sprite = hintOffImage;
        }
    }

    void Update()
    {
        if (Login.loggedIn)
        {
            userID = Login.userID + "_" + SyncTables.currentPlayerIndex.ToString();
        }
        else
        {
            userID = "guest";
        }
        outputPath = GetApplicationPath() + userID + "_OutputTable.csv";
        inputPath = GetApplicationPath() + userID + "_InputTable.csv";
        topicText.text = SyncTables.knowledgeLevel;
    }

    #region On Option Select Functions
    public void OnOptionSelect1()
    {
        //Debug.Log("Option 1 Selected");
        afterWrong = true;
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
            answerSelected = opID1;
            questionCount += 1;
            WriteOutputSQL();
            //Get a coin if answered 3 questions correctly
            if (questionCount >= 3)
            {
                coinRewardUI.SetActive(true);
                audioSource.PlayOneShot(audioCoins);
                questionCount = 0;
                GameSpecificChanges.setCoins = true;
                questionUI.SetActive(false);
                Invoke("ResetOnCorrect", 2.5f);
            }
            else
            {
                audioSource.PlayOneShot(audioCorrect);
                Invoke("ResetOnCorrect", 0.9f);
            }
        }
        else
        {
            result = "0";
            answerSelected = opID1;
            WriteOutputSQL();
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
            Invoke("ResetOnWrong", 2.5f);
        }
    }

    public void OnOptionSelect2()
    {
        //Debug.Log("Option 2 Selected");
        afterWrong = true;
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
            answerSelected = opID2;
            questionCount += 1;
            WriteOutputSQL();
            //Get a coin if answered 3 questions correctly
            if (questionCount >= 3)
            {
                coinRewardUI.SetActive(true);
                audioSource.PlayOneShot(audioCoins);
                questionCount = 0;
                GameSpecificChanges.setCoins = true;
                questionUI.SetActive(false);
                Invoke("ResetOnCorrect", 2.5f);
            }
            else
            {
                audioSource.PlayOneShot(audioCorrect);
                Invoke("ResetOnCorrect", 0.9f);
            }
        }
        else
        {
            result = "0";
            answerSelected = opID2;
            WriteOutputSQL();
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
            Invoke("ResetOnWrong", 2.5f);
        }
    }

    public void OnOptionSelect3()
    {
        //Debug.Log("Option 3 Selected");
        afterWrong = true;
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
            answerSelected = opID3;
            questionCount += 1;
            WriteOutputSQL();
            //Get a coin if answered 3 questions correctly
            if (questionCount >= 3)
            {
                coinRewardUI.SetActive(true);
                audioSource.PlayOneShot(audioCoins);
                questionCount = 0;
                GameSpecificChanges.setCoins = true;
                questionUI.SetActive(false);
                Invoke("ResetOnCorrect", 2.5f);
            }
            else
            {
                audioSource.PlayOneShot(audioCorrect);
                Invoke("ResetOnCorrect", 0.9f);
            }
        }
        else
        {
            result = "0";
            answerSelected = opID3;
            WriteOutputSQL();
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
            Invoke("ResetOnWrong", 2.5f);
        }
    }

    public void OnOptionSelect4()
    {
        //Debug.Log("Option 4 Selected");
        afterWrong = true;
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
            answerSelected = opID4;
            questionCount += 1;
            WriteOutputSQL();
            //Get a coin if answered 3 questions correctly
            if (questionCount >= 3)
            {
                coinRewardUI.SetActive(true);
                audioSource.PlayOneShot(audioCoins);
                questionCount = 0;
                GameSpecificChanges.setCoins = true;
                questionUI.SetActive(false);
                Invoke("ResetOnCorrect", 2.5f);
            }
            else
            {
                audioSource.PlayOneShot(audioCorrect);
                Invoke("ResetOnCorrect", 0.9f);
            }
        }
        else
        {
            result = "0";
            answerSelected = opID4;
            WriteOutputSQL();
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
            Invoke("ResetOnWrong", 2.5f);
        }
    }
    #endregion

    #region Reset Question Menu Functions
    public void ResetOnCorrect()
    {
        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(true);
        button3.gameObject.SetActive(true);
        button4.gameObject.SetActive(true);
        coinRewardUI.SetActive(false);
        GameSpecificChanges.getCoins = true;
        questionUI.SetActive(false);
        optionImage1.sprite = defaultButton;
        optionImage2.sprite = defaultButton;
        optionImage3.sprite = defaultButton;
        optionImage4.sprite = defaultButton;
        button1.interactable = true;
        button2.interactable = true;
        button3.interactable = true;
        button4.interactable = true;
        afterWrong = false;
        SetQuestion();
    }

    public void ResetOnWrong()
    {
        questionCount = 0;
        GameSpecificChanges.getCoins = true;
        questionUI.SetActive(false);
        optionImage1.sprite = defaultButton;
        optionImage2.sprite = defaultButton;
        optionImage3.sprite = defaultButton;
        optionImage4.sprite = defaultButton;
        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(true);
        button3.gameObject.SetActive(true);
        button4.gameObject.SetActive(true);
        button1.interactable = true;
        button2.interactable = true;
        button3.interactable = true;
        button4.interactable = true;
        SetQuestion();
        afterWrong = false;
        askTime = DateTime.Now;
        //Debug.Log("Menu ON");
        questionUI.SetActive(true);
    }
    #endregion

    #region Input
    void ReadInputSQL()
    {
        internet = CheckInternetPing();
        if (internet)
        {
            inputQID.Clear();
            inputQuestion.Clear();
            inputCorrectAns.Clear();
            inputWrong1.Clear();
            inputWrong2.Clear();
            inputWrong3.Clear();
            inputQuestionSet.Clear();
            inputHint.Clear();
            inputQuestionNo = 0;
            string readInputURL = "https://edplus.net/getNextQuestions";
            var request = new UnityWebRequest(readInputURL, "POST");
            //Debug.Log(SyncTables.currentPlayerIndex);
            ReadInputJSON readInputJSON = new ReadInputJSON()
            {
                UserID = Login.userID,
                PlayerID = SyncTables.currentPlayerIndex,
                previousFailures = 0
            };
            string json = JsonUtility.ToJson(readInputJSON);
            //Debug.Log("getNextQuestion");
            StartCoroutine(WaitForUnityWebRequestReadInput(request, json));
        }
        else
        {
            ReadInput();
            if (!afterWrong)
            {
                SetQuestion();
            }
        }
    }

    IEnumerator WaitForUnityWebRequestReadInput(UnityWebRequest request, string json)
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
        ReadInputJSONResponse readInputJSONResponse = JsonUtility.FromJson<ReadInputJSONResponse>(request.downloadHandler.text);
        if (request.downloadHandler.text == "" || request.downloadHandler.text == null)
        {
            ReadInput();
            if (!afterWrong)
            {
                SetQuestion();
            }
        }
        else if (readInputJSONResponse.status != "success")
        {
            ReadInput();
            if (!afterWrong)
            {
                SetQuestion();
            }
        }
        else
        {
            downloadedInputText = readInputJSONResponse.data;
            var lines = downloadedInputText.Split('&');
            var cols = lines[0].Split(',');
            questionID = cols[0];
            question = cols[1];
            correctAns = cols[2];
            wrong1 = cols[3];
            wrong2 = cols[4];
            wrong3 = cols[5];
            questionSet = cols[6];
            hint = readInputJSONResponse.Hints[0].Hint;
            if (!afterWrong)
            {
                SetQuestion();
            }
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
                inputHint.Add(readInputJSONResponse.Hints[0].Hint);
            }
            WriteInput(inputPath);
        }
    }

    void WriteInput(string path)
    {
        var writer = new StreamWriter(path);
        for (int i = 0; i < inputQID.Count; i++)
        {
            writer.WriteLine(inputQID[i] + "," + inputQuestion[i] + "," + inputCorrectAns[i] + "," + inputWrong1[i] + "," + inputWrong2[i] + "," + inputWrong3[i] + "," + inputQuestionSet[i] + "," + inputHint[i]);
        }
        writer.Flush();
        writer.Close();
    }

    public void SetQuestion()
    {
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
        questionText.text = question;
        if (hintsOn)
        {
            hintText.text = hint;
        }
        else
        {
            hintText.text = "";
        }
        var temp = option1.Split('@');
        opID1 = temp[0];
        optionText1.text = temp[1];
        temp = option2.Split('@');
        opID2 = temp[0];
        optionText2.text = temp[1];
        temp = option3.Split('@');
        opID3 = temp[0];
        optionText3.text = temp[1];
        temp = option4.Split('@');
        opID4 = temp[0];
        optionText4.text = temp[1];
    }

    void ReadInput()
    {
        //Debug.Log("ReadInput called");
        int lineCount = 0;
        var countReader = new StreamReader(inputPath);
        while (!countReader.EndOfStream)
        {
            var line = countReader.ReadLine();
            lineCount++;
        }
        countReader.Close();
        //Debug.Log(lineCount);
        inputQuestionNo++;
        if(inputQuestionNo >= lineCount)
        {
            inputQuestionNo = 0;
        }
        var reader = new StreamReader(inputPath);
        for (int i = 0; i <= inputQuestionNo; i++)
        {
            var line = reader.ReadLine();
            //Debug.Log(line);
            var values = line.Split(',');
            questionID = values[0];
            question = values[1];
            correctAns = values[2];
            wrong1 = values[3];
            wrong2 = values[4];
            wrong3 = values[5];
            questionSet = values[6];
            hint = values[7];
        }
        reader.Close();
    }
    #endregion

    #region Output
    void WriteOutput()
    {
        var writer = new StreamWriter(outputPath, true);
        writer.WriteLine(userID + "," + questionID + "," + question + "," + result + "," + answerSelected + "," + timeAsked + "," + timeTaken + "," + questionSet/* + "," + uQID*/);
        writer.Flush();
        writer.Close();
        ReadInputSQL();
    }

    void ReadOutput()
    {
        answerText = "";
        var reader = new StreamReader(outputPath);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(',');
            answerText = answerText + values[1] + "," + values[7] + "," + values[5] + "," + values[6] + "," + values[3] + "," + values[4] + "&";
            //outputUQID = values[8];
        }
        reader.Close();
    }

    void WriteOutputSQL()
    {
        string insertOutputURL = "https://edplus.net/insertOutput";
        var request = new UnityWebRequest(insertOutputURL, "POST");
        answerText = questionID + "," + questionSet + "," + timeAsked + "," + timeTaken + "," + result + "," + answerSelected + "&";
        InsertOutputJSON insertOutputJSON = new InsertOutputJSON()
        {
            UserID = userID,
            Level = SyncTables.knowledgeLevel,
            App = SyncTables.gameName,
            DeviceModel = SystemInfo.deviceModel,
            DeviceOS = SystemInfo.operatingSystem,
            Company = Application.companyName,
            UpdateCT = 1,
            Answers = answerText
            /*,
            UQID = outputUQID*/
        };
        string json = JsonUtility.ToJson(insertOutputJSON);
        internet = CheckInternetPing();
        if (internet)
        {
            string fileContents = File.ReadAllText(outputPath);
            if(fileContents == "" || fileContents == null)
            {
                StartCoroutine(WaitForUnityWebRequest(request, json));
            }
            else
            {
                ReadOutput();
                insertOutputJSON.Answers = answerText + insertOutputJSON.Answers;
                json = JsonUtility.ToJson(insertOutputJSON);
                File.WriteAllText(outputPath, string.Empty);
                StartCoroutine(WaitForUnityWebRequest(request, json));
            }
        }
        else
        {
            WriteOutput();
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
        if(request.downloadHandler.text == "" || request.downloadHandler.text == null)
        {
            WriteOutput();
        }
        else
        {
            ReadInputSQL();
        }
        //Debug.Log("Response: " + request.downloadHandler.text);
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

    #region Other Functions
    public int CorrectAnsIndex()
    {
        if ("1@" + optionText1.text == correctAns)
            return 0;
        else if ("1@" + optionText2.text == correctAns)
            return 1;
        else if ("1@" + optionText3.text == correctAns)
            return 2;
        else
            return 3;
    }

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

    public void HintButton()
    {
        if (hintsOn)
        {
            hintsOn = false;
            hintText.text = "";
            hintButtonImage.sprite = hintOffImage;
        }
        else
        {
            hintsOn = true;
            hintText.text = hint;
            hintButtonImage.sprite = hintOnImage;
        }
    }
    #endregion

    #region Internet Check
    bool CheckInternetPing()
    {
        //internet = false;
        string checkInternetURL = "https://edplus.net/checkServerAlive";
        var varCheckInternetRequest = new UnityWebRequest(checkInternetURL, "POST");
        Coroutine internetCoroutine = StartCoroutine(WaitForServer(varCheckInternetRequest));
        StartCoroutine(WaitForTime(1f, internetCoroutine));
        /*System.Threading.Thread.Sleep(100);
        if (!internet)
        {
            Debug.Log("Not Connected to Internet");
        }*/
        //Debug.Log(internet);
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
        if(request.downloadHandler.text == "" || request.downloadHandler.text == null)
        {
            internet = false;
        }
        else if (serverJSONResponse.status == "success")
        {
            internet = true;
        }
        else
        {
            internet = false;
        }
    }

    IEnumerator WaitForTime(float time, Coroutine cor)
    {
        yield return new WaitForSeconds(time);
        if (!internet)
        {
            Debug.Log("No/Slow Internet");
            internet = false;
            StopCoroutine(cor);
        }
    }
    #endregion
}