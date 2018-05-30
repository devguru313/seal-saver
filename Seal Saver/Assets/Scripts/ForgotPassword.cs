using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class ForgotPassword : MonoBehaviour {
    
    public InputField Mail;
    public FirebaseAuth auth;
    public Text errorText;
    public GameObject instructionsButton;

    void Start()
    {
        string deviceModel = SystemInfo.deviceModel.ToLower();
        //Amazon Device check
        if (!deviceModel.Contains("amazon"))
        {
            InitializeFirebase();
        }
        LoadPlayerPrefs();
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void Launch()
    {
        if(Mail.text == "")
        {
            errorText.text = "Please complete all fields";
            return;
        }
        //Debug.Log("Forgot Password launched.");
        string deviceModel = SystemInfo.deviceModel.ToLower();
        //Amazon Device check
        if (!deviceModel.Contains("amazon"))
        {
            auth.SendPasswordResetEmailAsync(Mail.text).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("Password reset email sent successfully.");
                instructionsButton.SetActive(true);
            });
        }
        else
        {
            StartCoroutine(SendDetails());
        }
    }

    public void GoHome()
    {
        instructionsButton.SetActive(false);
        SceneManager.LoadScene(0);
    }

    public void LoadPlayerPrefs()
    {
        // Prefill fields with saved datas
        if (PlayerPrefs.HasKey("Mail"))
        {
            Mail.text = PlayerPrefs.GetString("Mail");
        }
    }

    IEnumerator SendDetails()
    {
        //Debug.Log("SEND DETAILS");
        string forgotPasswordURL = "https://edplus.net/sendPasswordResetEmail";
        var request = new UnityWebRequest(forgotPasswordURL, "POST");
        ForgotPasswordJSON forgotPasswordJSON = new ForgotPasswordJSON()
        {
            Email = Mail.text
        };
        string json = JsonUtility.ToJson(forgotPasswordJSON);
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
        SendAuthDetailsJSONResponse sendAuthDetailsJSONResponse = JsonUtility.FromJson<SendAuthDetailsJSONResponse>(request.downloadHandler.text);
        if (sendAuthDetailsJSONResponse.status != "success")
        {
            //Debug.Log(sendAuthDetailsJSONResponse.data);
            if (sendAuthDetailsJSONResponse.data == "EMAIL_NOT_FOUND")
            {
                errorText.text = "This Email is not associated with any account. Please check the email entered and try again.";
                yield return null;
            }
            else
            {
                errorText.text = "Please try again";
                yield return null;
            }
        }
        else
        {
            Debug.Log(sendAuthDetailsJSONResponse.data);
            instructionsButton.SetActive(true);
        }
    }
}
