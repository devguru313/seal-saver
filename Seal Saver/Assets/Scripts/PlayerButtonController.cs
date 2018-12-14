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
    public GameObject updatePlayerMenu;
    public Image updateMenuAvatar;
    public int birthYear;
    public string subject;
    public Text placeholderNameText;

    public void SetText(string text, int index, int avatIndex, string year, string topic)
    {
        buttonText.text = text;
        playerIndex = index;
        avatarIndex = avatIndex;
        //Debug.Log(avatarIndex);
        avatar.sprite = avatars[avatarIndex];
        int.TryParse(year, out birthYear);
        subject = topic;
    }

    public void OnClick()
    {
        SyncTables.currentPlayerIndex = playerIndex;
        AvatarSelection.currentAvatar = avatarIndex;
        if (PlayerManager.parentZoneActive)
        {
            updatePlayerMenu.SetActive(true);
            placeholderNameText.text = buttonText.text;
            updateMenuAvatar.sprite = avatars[avatarIndex];
            YearSubjectSelection.currentYear = birthYear;
            switch (subject)
            {
                case "Maths":
                    YearSubjectSelection.currentSubject = 0;
                    break;
                case "Science":
                    YearSubjectSelection.currentSubject = 1;
                    break;
                case "Verbal Reasoning":
                    YearSubjectSelection.currentSubject = 2;
                    break;
                case "Latin":
                    YearSubjectSelection.currentSubject = 3;
                    break;
                default:
                    YearSubjectSelection.currentSubject = 0;
                    break;
            }
        }
        else
        {
            //Debug.Log(playerIndex);
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
}