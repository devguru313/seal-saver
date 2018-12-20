using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScaler : MonoBehaviour {

    public float height;
    public float width;
    public float ratio;

    public RectTransform rect;

	void Start ()
    {
        rect = GetComponent<RectTransform>();
        height = Screen.height;
        width = Screen.width;
        ratio = height / width;
        transform.localScale = transform.localScale * (ratio) / 2.1653f;
        rect.anchoredPosition = rect.anchoredPosition * (ratio) / 2.1653f;
    }
}
