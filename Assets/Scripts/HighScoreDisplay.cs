using UnityEngine;
using UnityEngine.UI;

public class HighScoreDisplay : MonoBehaviour
{
	public Text highScoreTextElement;
	private LeaderboardManager leaderboardManagerInstance;

	void Start()
	{
		leaderboardManagerInstance = FindObjectOfType<LeaderboardManager>();

		if (highScoreTextElement == null)
		{
			enabled = false;
			return;
		}

		if (leaderboardManagerInstance == null)
		{
			highScoreTextElement.text = "High Score: ???";
			return;
		}

		long currentHighScore = leaderboardManagerInstance.GetHighScore();

		highScoreTextElement.text = $"High Score: {currentHighScore}";
	}
}
