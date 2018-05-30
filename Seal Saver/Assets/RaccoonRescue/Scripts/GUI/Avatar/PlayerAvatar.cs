using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using InitScriptName;

public class PlayerAvatar : MonoBehaviour, IAvatarLoader {
	//public Image image;
    public SpriteRenderer image;
    public string playerName;
    public Sprite a;
    public Sprite b;
    public Sprite c;
    public Sprite d;
    public Sprite e;
    public Sprite f;
    public Sprite g;
    public Sprite h;
    public Sprite i;
    public Sprite j;
    public Sprite k;
    public Sprite l;
    public Sprite m;
    public Sprite n;
    public Sprite o;
    public Sprite p;
    public Sprite q;
    public Sprite r;
    public Sprite s;
    public Sprite t;
    public Sprite u;
    public Sprite v;
    public Sprite w;
    public Sprite x;
    public Sprite y;
    public Sprite z;

    void Start()
    {
        if(this.gameObject.name == "FriendCharacter")
        {
            playerName = "test";
        }
        else
        {
            playerName = PlayerPrefs.GetString("CurrentPlayerName");
        }
        switch (playerName.Substring(0, 1).ToUpper())
        {
            case "A":
                image.sprite = a;
                break;
            case "B":
                image.sprite = b;
                break;
            case "C":
                image.sprite = c;
                break;
            case "D":
                image.sprite = d;
                break;
            case "E":
                image.sprite = e;
                break;
            case "F":
                image.sprite = f;
                break;
            case "G":
                image.sprite = g;
                break;
            case "H":
                image.sprite = h;
                break;
            case "I":
                image.sprite = i;
                break;
            case "J":
                image.sprite = j;
                break;
            case "K":
                image.sprite = k;
                break;
            case "L":
                image.sprite = l;
                break;
            case "M":
                image.sprite = m;
                break;
            case "N":
                image.sprite = n;
                break;
            case "O":
                image.sprite = o;
                break;
            case "P":
                image.sprite = p;
                break;
            case "Q":
                image.sprite = q;
                break;
            case "R":
                image.sprite = r;
                break;
            case "S":
                image.sprite = s;
                break;
            case "T":
                image.sprite = t;
                break;
            case "U":
                image.sprite = u;
                break;
            case "V":
                image.sprite = v;
                break;
            case "W":
                image.sprite = w;
                break;
            case "X":
                image.sprite = x;
                break;
            case "Y":
                image.sprite = y;
                break;
            case "Z":
                image.sprite = z;
                break;
        }
    }

    public void ShowPicture () {
        
		//image.enabled = true;
	}

}
