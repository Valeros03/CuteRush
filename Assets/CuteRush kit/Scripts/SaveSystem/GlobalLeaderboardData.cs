using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GlobalScoreRecord
{
    public string playerName;
    public int score;
    public string difficulty;

    public GlobalScoreRecord(string name, int s, string diff)
    {
        playerName = name;
        score = s;
        difficulty = diff;
    }
}

[System.Serializable]
public class GlobalMapLeaderboard
{
    public string mapName;
    public List<GlobalScoreRecord> topScores;

    public GlobalMapLeaderboard(string name)
    {
        mapName = name;
        topScores = new List<GlobalScoreRecord>();
    }
}

[System.Serializable]
public class GlobalLeaderboardData
{
    public List<GlobalMapLeaderboard> maps;

    public GlobalLeaderboardData()
    {
        maps = new List<GlobalMapLeaderboard>();
    }
}
