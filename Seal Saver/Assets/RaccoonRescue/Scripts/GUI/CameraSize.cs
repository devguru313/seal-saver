﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class CameraSize : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("map"))
        {
            Application.targetFrameRate = 60;
            /*float aspect = (float)Screen.height / (float)Screen.width;
            aspect = (float)Math.Round(aspect, 2);
            if (aspect == 1.6f)
                GetComponent<Camera>().orthographicSize = 12.23f;                   //16:10
            else if (aspect == 1.78f)
                GetComponent<Camera>().orthographicSize = 13.6f;                    //16:9
            else if (aspect == 1.5f)
                GetComponent<Camera>().orthographicSize = 11.46f;                   //3:2
            else if (aspect == 1.33f)
                GetComponent<Camera>().orthographicSize = 10.16f;                   //4:3
            else if (aspect == 1.67f)
                GetComponent<Camera>().orthographicSize = 12.68f;                   //5:3
            else if (aspect == 1.25f)
                GetComponent<Camera>().orthographicSize = 9.5f;                     //5:4
            else if (aspect == 2.06f)
                GetComponent<Camera>().orthographicSize = 15.7f;                    //2960:1440
            else if (aspect == 1.71f)
                GetComponent<Camera>().orthographicSize = 13.05f;                    //1024:600
            else if (aspect >= 2.16f && aspect <= 2.17f)
                GetComponent<Camera>().orthographicSize = 16.45f;                    //iPhone X*/
            Camera.main.orthographicSize = 15.36f / Screen.width * Screen.height / 2f;
        }

        else if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("game"))
        {
            Application.targetFrameRate = 60;
            float aspect = (float)Screen.height / (float)Screen.width;
            aspect = (float)Math.Round(aspect, 2);
            if (aspect == 2.06f)
                GetComponent<Camera>().orthographicSize = 8f;                    //2960:1440
            else if (aspect >= 2.16f && aspect <= 2.17f)
                GetComponent<Camera>().orthographicSize = 8.4f;                    //iPhone X
            //Camera.main.orthographicSize = 7.7f / Screen.width * Screen.height / 2f;
        }
    }

}
