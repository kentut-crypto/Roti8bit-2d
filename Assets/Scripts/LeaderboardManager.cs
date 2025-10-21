using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ScoreEntry
{
	public long score;
	public string date;

	public ScoreEntry(long s)
	{
		score = s;
		date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
	}
}

[System.Serializable]
public class ScoreListWrapper
{
	public List<ScoreEntry> scores = new List<ScoreEntry>();
}

public class LeaderboardManager : MonoBehaviour
{
	private const string LeaderboardKey = "HighScores";
	private ScoreListWrapper scoreWrapper = new ScoreListWrapper();
	private const int MaxEntries = 10;

	void Awake()
	{
		LoadLocalScores();
	}

	private void LoadLocalScores()
	{
		if (PlayerPrefs.HasKey(LeaderboardKey))
		{
			string json = PlayerPrefs.GetString(LeaderboardKey);
			scoreWrapper = JsonUtility.FromJson<ScoreListWrapper>(json);

			scoreWrapper.scores = scoreWrapper.scores
				.OrderByDescending(s => s.score)
				.Take(MaxEntries)
				.ToList();
		}
		else
		{
			scoreWrapper.scores = new List<ScoreEntry>();
		}
	}

	private void SaveLocalScores()
	{
		scoreWrapper.scores = scoreWrapper.scores
			.OrderByDescending(s => s.score)
			.Take(MaxEntries)
			.ToList();

		string json = JsonUtility.ToJson(scoreWrapper);
		PlayerPrefs.SetString(LeaderboardKey, json);
		PlayerPrefs.Save();
	}

	public void SaveScore(long score)
	{
		Debug.Log("--- ATTEMPTING PLAYERPREFS SAVE: Score " + score + " ---");
		ScoreEntry entry = new ScoreEntry(score);
		scoreWrapper.scores.Add(entry);
		SaveLocalScores();
	}

	public long GetHighScore()
	{
		return scoreWrapper.scores.Count > 0 ? scoreWrapper.scores[0].score : 0;
	}
}