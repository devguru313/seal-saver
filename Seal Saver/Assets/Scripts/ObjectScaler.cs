using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScaler : MonoBehaviour {

    public float height;
    public float width;
    public float ratio;
    public float scaleRatio;
    public float buffer;
    public const float benchmarkRatio = 2.1653f;

    public RectTransform rect;

	void Start ()
    {
        rect = GetComponent<RectTransform>();
        height = Screen.height;
        width = Screen.width;
        ratio = height / width;
        scaleRatio = ratio / benchmarkRatio;
        if(scaleRatio <= 0.6f)
        {
            buffer = (1 - scaleRatio) * 0.25f;
        }
        else if (scaleRatio <= 0.7f)
        {
            buffer = (1 - scaleRatio) * 0.2f;
        }
        else if (scaleRatio <= 0.8f)
        {
            buffer = (1 - scaleRatio) * 0.15f;
        }
        else if(scaleRatio <= 0.9f)
        {
            buffer = (1 - scaleRatio) * 0.1f;
        }
        else
        {
            buffer = 0;
        }
        transform.localScale = transform.localScale * (scaleRatio + buffer);
        rect.anchoredPosition = rect.anchoredPosition * (scaleRatio + buffer);
    }
}
