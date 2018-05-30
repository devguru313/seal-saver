using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerDataManager : IDataManager {
    
    public static string highestLevel;
    public static int starCount;
    public static int levelCount;
    public Dictionary<string, int> starsDict = new Dictionary<string, int>();

    public void SetPlayerLevel(int level)
    {
        levelCount = level;
        SyncTables.setLevels = true;
    }
    
    public void GetPlayerLevel(Action<int> callback)
    {
        //Get from static string list SyncTables.playerData
        int currentPlayer = SyncTables.currentPlayerIndex;
        //Debug.Log("CURRENT PLAYER: " + currentPlayer);
        for(int i = 0; i < SyncTables.playerData.Count; i++)
        {
            //Debug.Log(SyncTables.playerData[i]);
            var values = SyncTables.playerData[i].Split(' ');
            if(values[0] == currentPlayer.ToString())
            {
                callback(int.Parse(values[1]));
            }
        }
    }

    public void SetStars(int stars, int level)
    {
        starCount = stars;
        levelCount = level;
        SyncTables.setStars = true;
    }

    public void GetStars(Action<Dictionary<string, int>> callback)
    {
        starsDict.Clear();
        //Get from static string list SyncTables.playerData
        int currentPlayer = SyncTables.currentPlayerIndex;
        for (int i = 0; i < SyncTables.playerData.Count; i++)
        {
            var values = SyncTables.playerData[i].Split(' ');
            if (values[0] == currentPlayer.ToString())
            {
                //values.Length - 1 to account for blank space at end
                for(int j = 2; j < values.Length - 1; j++)
                {
                    string item = (j - 1).ToString();
                    int val;
                    int.TryParse(values[j], out val);
                    starsDict.Add(item, val);
                    //Debug.Log(item + "&" + val + "&");
                }
            }
        }
        callback(starsDict);
    }

    public void SetPlayerScore(int level, int score)
    {
        //Code to store score in SQL
    }

    public void GetPlayerScore(Action<int> callback)
    {
        //Code to get score from SQL
    }

    public void SetTotalStars()
    {
        //Code to store total stars in SQL
    }

    public void SetBoosterData(Dictionary<string, string> dict)
    {
        //Code to store booster data in SQL
    }

    public void GetBoosterData(Action<Dictionary<string, int>> callback)
    {
        //Code to get booster data from SQL
    }

    public void Logout()
    {

    }
}
