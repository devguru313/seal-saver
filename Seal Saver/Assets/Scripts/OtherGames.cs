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

    public void CloseMenu()
    {
        otherGamesMenu.SetActive(false);
    }
}
