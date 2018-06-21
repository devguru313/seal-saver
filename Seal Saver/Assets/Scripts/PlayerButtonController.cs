using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerButtonController : MonoBehaviour
{
    public Text buttonText;
    public int playerIndex;
    //public static int currentPlayer;

    public void SetText(string text, int index)
    {
        buttonText.text = text;
        playerIndex = index;
    }

    public void OnClick()
    {
        //Debug.Log(playerIndex);
        SyncTables.currentPlayerIndex = playerIndex;
        var cols = SyncTables.playerCoins[playerIndex - 1].Split('@');
        int gems;
        int.TryParse(cols[1], out gems);
        PlayerPrefs.SetString("CurrentPlayerName", buttonText.text);
        PlayerPrefs.SetInt("Gems", gems);
        PlayerPrefs.Save();
        SyncTables.isLoggingIn = true;
        //SyncTables.syncDownloadNow = true;
        SceneManager.LoadScene(3);
    }
}