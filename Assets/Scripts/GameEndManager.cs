using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameEndManager : MonoBehaviour
{
	[Header("Game Conditions")]
	[Tooltip("The score threshold to differentiate between endings.")]
	public int scoreThresholdForGoodEnding = 40;

	[Header("Scene Transitions")]
	[Tooltip("Build index for the scene to load if score > scoreThresholdForGoodEnding.")]
	public int goodEndingSceneIndex = 3;
	[Tooltip("Build index for the scene to load if score <= scoreThresholdForGoodEnding.")]
	public int badEndingSceneIndex = 4;

	[Header("Fade Settings")]
	[Tooltip("The UI Image to use for fading in the Inspector.")]
	public Image fadePanel;
	[Tooltip("How long the fade-in at the start should take in seconds.")]
	public float fadeInDuration = 1.5f;  // NEW: Specific duration for fade-in
	[Tooltip("How long the fade-out at the end should take in seconds.")]
	public float fadeOutDuration = 2f; // RENAMED: from fadeDuration for clarity

	private bool conditionsHaveBeenMet = false;
	private ScoreManager scoreManagerInstance;

	private LeaderboardManager leaderboardManagerInstance;
	private int finalScore;
	private int targetSceneIndex;

	void Start()
	{
		scoreManagerInstance = ScoreManager.Instance;
		leaderboardManagerInstance = FindObjectOfType<LeaderboardManager>();

		if (scoreManagerInstance == null)
		{
			Debug.LogError("GameEndManager: ScoreManager.Instance not found!");
			enabled = false;
			return;
		}

		if (fadePanel == null)
		{
			Debug.LogError("GameEndManager: Fade Panel has not been assigned in the Inspector!");
			enabled = false;
			return;
		}

		// CHANGED: Instead of making the panel transparent, we start the fade-in coroutine.
		StartCoroutine(FadeInScene());
	}

	// NEW: This entire coroutine handles the fade-in at the beginning of the scene.
	IEnumerator FadeInScene()
	{
		// Ensure the panel is active and fully opaque (black) at the very start.
		fadePanel.gameObject.SetActive(true);
		fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 1f);

		float elapsedTime = 0f;

		while (elapsedTime < fadeInDuration)
		{
			elapsedTime += Time.deltaTime;
			// Animate the alpha value from 1 (opaque) down to 0 (transparent).
			float newAlpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeInDuration);
			fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, newAlpha);
			yield return null;
		}

		// Ensure it's fully transparent at the end.
		fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 0f);
		// Good practice: Deactivate the panel so it doesn't block clicks on other UI elements.
		fadePanel.gameObject.SetActive(false);
	}

	void Update()
	{
		if (conditionsHaveBeenMet || scoreManagerInstance == null)
		{
			return;
		}

		bool timeIsOver = !scoreManagerInstance.timerIsRunning && scoreManagerInstance.timeRemaining <= 0;

		if (timeIsOver)
		{
			conditionsHaveBeenMet = true;
			finalScore = scoreManagerInstance.currentScore;

			targetSceneIndex = (finalScore > scoreThresholdForGoodEnding)
				? goodEndingSceneIndex
				: badEndingSceneIndex;

			ProcessScoreAndTransition();
		}
	}

	private void ProcessScoreAndTransition()
	{
		long currentHighscore = 0;

		if (leaderboardManagerInstance != null)
		{
			currentHighscore = leaderboardManagerInstance.GetHighScore();
		}

		bool isNewHighscore = finalScore > currentHighscore;

		GameDataTransfer.FinalScore = finalScore;
		GameDataTransfer.HighScore = isNewHighscore ? finalScore : currentHighscore;
		GameDataTransfer.IsNewHighscore = isNewHighscore;

		if (leaderboardManagerInstance != null)
		{
			leaderboardManagerInstance.SaveScore(finalScore);
		}

		fadePanel.gameObject.SetActive(true);
		StartCoroutine(PerformFadeAndLoadScene(targetSceneIndex));
	}

	IEnumerator PerformFadeAndLoadScene(int targetSceneIndex)
	{
		if (Time.timeScale == 0f)
		{
			Time.timeScale = 1f;
		}

		float elapsedTime = 0f;
		Color panelColor = fadePanel.color;

		// Use the new fadeOutDuration variable here.
		while (elapsedTime < fadeOutDuration)
		{
			elapsedTime += Time.deltaTime;
			float newAlpha = Mathf.Clamp01(elapsedTime / fadeOutDuration);
			fadePanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, newAlpha);
			yield return null;
		}

		fadePanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, 1f);
		SceneManager.LoadScene(targetSceneIndex);
	}
}