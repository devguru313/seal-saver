using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSelection : MonoBehaviour {

    public Image avatarImage;

    public Sprite[] avatarSprites = new Sprite[50];

    public static int currentAvatar;

    private void Start()
    {
        avatarImage.sprite = avatarSprites[currentAvatar];
    }

    public void OnLeftClick()
    {
        if(currentAvatar == 0)
        {
            currentAvatar = avatarSprites.Length - 1;
        }
        else
        {
            currentAvatar--;
        }
        avatarImage.sprite = avatarSprites[currentAvatar];
    }

    public void OnRightClick()
    {
        if (currentAvatar == avatarSprites.Length - 1)
        {
            currentAvatar = 0;
        }
        else
        {
            currentAvatar++;
        }
        avatarImage.sprite = avatarSprites[currentAvatar];
    }
}
