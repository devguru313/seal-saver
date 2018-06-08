using UnityEngine;

public class GameSpecificChanges : MonoBehaviour {

    //Name of the Game
    public static string gameName;
    //In-game currency value
    public static string coins;
    public static bool getCoins;
    public static bool setCoins;

    private void Awake()
    {
        gameName = "Seal Saver";
    }

    private void Update()
    {
        if (getCoins)
        {
            getCoins = false;
            coins = InitScriptName.InitScript.Gems.ToString();
        }
        if (setCoins)
        {
            setCoins = false;
            InitScriptName.InitScript.Gems += 1;
        }
    }

}
