using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowKnowledgeLevel : MonoBehaviour {

    public static string level;
    public Text levelGUI;

	private void Update()
	{
        if(Login.loggedIn)
        {
            levelGUI.text = level;
            //Debug.Log(levelGUI.text);
        }
        else
        {
            levelGUI.text = "1";
        }
	}
}
