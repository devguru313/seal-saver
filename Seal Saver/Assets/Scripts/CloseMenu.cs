using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMenu : MonoBehaviour {

    public GameObject menu;

    public void OnClick()
    {
        menu.SetActive(false);
    }
}
