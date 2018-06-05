using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SendFeedback : MonoBehaviour {

    public Toggle happyToggle;
    public Toggle sadToggle;
    public GameObject feedbackMenu;
    public GameObject confirmationMenu;
    public InputField feedbackField;
    public string emotion;
    public Text errorText;

    private void Start()
    {
        confirmationMenu.SetActive(false);
        errorText.text = "";
    }

    public void SendFeedbackToServer()
    {
        if(feedbackField.text == "")
        {
            errorText.text = "Please add your feedback.";
            return;
        }
        if (happyToggle.isOn)
        {
            emotion = "Positive";
        }
        else if (sadToggle.isOn)
        {
            emotion = "Negative";
        }
        else
        {
            emotion = "Neutral";
        }
        string sendFeedbackURL = "https://edplus.net/sendFeedback";
        var varSendFeedbackRequest = new UnityWebRequest(sendFeedbackURL, "POST");
        SendFeedbackJSON sendFeedbackJSON = new SendFeedbackJSON()
        {
            Email = Login.user,
            UserID = Login.userID,
            PlayerID = SyncTables.currentPlayerIndex.ToString(),
            Emotion = emotion,
            Feedback = feedbackField.text,
            TimeFeedbackProvided = DateTime.Now.ToString(),
            App = SyncTables.gameName,
            DeviceModel = SystemInfo.deviceModel,
            DeviceOS = SystemInfo.operatingSystem,
            Company = Application.companyName
    };
        string jsonSendFeedback = JsonUtility.ToJson(sendFeedbackJSON);
        //Debug.Log(jsonSendFeedback);
        StartCoroutine(WaitForUnityWebRequest(varSendFeedbackRequest, jsonSendFeedback));
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
        SendFeedbackJSONResponse sendFeedbackJSONResponse = JsonUtility.FromJson<SendFeedbackJSONResponse>(request.downloadHandler.text);
        if (sendFeedbackJSONResponse.status == "success")
        {
            confirmationMenu.SetActive(true);
        }
    }

    public void OnHappyToggle()
    {
        if (happyToggle.isOn)
        {
            sadToggle.isOn = false;
        }
    }

    public void OnSadToggle()
    {
        if (sadToggle.isOn)
        {
            happyToggle.isOn = false;
        }
    }

    public void CloseMenu()
    {
        confirmationMenu.SetActive(false);
        feedbackMenu.SetActive(false);
    }
}
