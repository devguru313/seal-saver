using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class OtherGames : MonoBehaviour {

    public GameObject otherGamesMenu;
    public Text versionText;

    private void Start()
    {
        versionText.text = GameSpecificChanges.version;
    }

    public void SealSaver()
    {
#if UNITY_ANDROID
        if (!SystemInfo.deviceModel.ToLower().Contains("amazon"))
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.edplus.sealsaver");
        }
        else
        {
            Application.OpenURL("amzn://apps/android?p=com.edplus.sealsaver");
        }
#elif UNITY_IOS
        Application.OpenURL("itms://itunes.apple.com");
#endif
    }

    public void PlanetZorg()
    {
#if UNITY_ANDROID
        if (!SystemInfo.deviceModel.ToLower().Contains("amazon"))
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.edplus.planetzorg");
        }
        else
        {
            Application.OpenURL("amzn://apps/android?p=com.edplus.planetzorg");
        }
#elif UNITY_IOS
        Application.OpenURL("itms://itunes.apple.com");
#endif
    }

    public void MatchShake()
    {
#if UNITY_ANDROID
        if (!SystemInfo.deviceModel.ToLower().Contains("amazon"))
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.edplus.matchshake");
        }
        else
        {
            Application.OpenURL("amzn://apps/android?p=com.edplus.matchshake");
        }
#elif UNITY_IOS
        Application.OpenURL("itms://itunes.apple.com");
#endif
    }

    public void CloseMenu()
    {
        otherGamesMenu.SetActive(false);
    }
}
