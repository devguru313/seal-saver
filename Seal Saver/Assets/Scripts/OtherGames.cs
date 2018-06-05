using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherGames : MonoBehaviour {

    public GameObject otherGamesMenu;

	public void CloseMenu()
    {
        otherGamesMenu.SetActive(false);
    }
}
