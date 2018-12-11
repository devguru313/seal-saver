using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerButtonController : MonoBehaviour
{
    public Image avatar;
    public Sprite[] avatars = new Sprite[50];
    public Text buttonText;
    public int playerIndex;
    public int avatarIndex;
    public static bool fromPBController;

    public void SetText(string text, int index, int avatIndex)
    {
        buttonText.text = text;
        playerIndex = index;
        avatarIndex = avatIndex;
        avatar.sprite = avatars[avatarIndex];
    }

    public void OnClick()
    {
        //Debug.Log(playerIndex);
        SyncTables.currentPlayerIndex = playerIndex;
        AvatarSelection.currentAvatar = avatarIndex;
        var cols = SyncTables.playerCoins[playerIndex - 1].Split('@');
        int gems;
        int.TryParse(cols[1], out gems);
        PlayerPrefs.SetString("CurrentPlayerName", buttonText.text);
        PlayerPrefs.SetInt("Gems", gems);
        PlayerPrefs.Save();
        SyncTables.isLoggingIn = true;
        //SyncTables.syncDownloadNow = true;
        fromPBController = true;
        SyncTables.getStarsAndLevels = true;
        //SceneManager.LoadScene(3);
    }
}