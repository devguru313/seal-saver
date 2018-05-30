using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMode : MonoBehaviour {

    public GameObject questionsMenu;
    public GameObject syncMessage;
    public static bool debugMode = false;

	// Use this for initialization
	void Start ()
    {
        questionsMenu.SetActive(false);
        syncMessage.SetActive(false);
	}

    public void OpenQuestions()
    {
        questionsMenu.SetActive(true);
        debugMode = true;
    }

    public void CloseQuestions()
    {
        questionsMenu.SetActive(false);
        debugMode = false;
    }

    public void ShowSyncMessage()
    {
        syncMessage.SetActive(true);
        Invoke("HideSyncMessage", 1);
    }

    public void HideSyncMessage()
    {
        syncMessage.SetActive(false);
    }

}
