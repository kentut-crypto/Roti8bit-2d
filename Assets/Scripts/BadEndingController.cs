using UnityEngine;
using UnityEngine.UI;         // Required for UI Image
using UnityEngine.SceneManagement; // Required for scene management
using System.Collections;     // Required for Coroutines
using TMPro;

public class BadEndingSceneController : MonoBehaviour
{
	[Header("Fade Settings")]
	public Image fadePanel;
	public float fadeInDuration = 1.5f;
	public float fadeToBlackDuration = 1f;
	public float cinematicDuration = 2.0f;
	public float fadeOutDuration = 1.5f;

	[Header("Score Display UI")]
	public TextMeshProUGUI finalScoreText;
	public TextMeshProUGUI highscoreMessageText;

	[Header("Background Elements")]
	[Tooltip("The parent object containing the Map, Water, and all visual scene elements.")]
	public GameObject sceneBackgroundParent;

	[Header("Scene Transition")]
	public int sceneIndexToLoad = 0;

	void Start()
	{
		if (fadePanel == null)
		{
			Debug.LogError("BadEndingSceneController: Fade Panel is not assigned in the Inspector!");
			// Optionally, try to find it if not assigned, or disable the script
			// For simplicity, we'll just log an error. The scene won't fade correctly.
			enabled = false;
			return;
		}

		// Ensure Time.timeScale is 1 when this scene starts, in case it was set to 0 previously.
		if (Time.timeScale != 1f)
		{
			Time.timeScale = 1f;
			Debug.Log("BadEndingSceneController: Time.timeScale was not 1, resetting to 1.");
		}

		SetScoreUIActive(false);
		SetBackgroundActive(true);

		// Start the sequence
		StartCoroutine(SceneSequenceCoroutine());
	}

	void SetBackgroundActive(bool active)
	{
		if (sceneBackgroundParent != null)
		{
			sceneBackgroundParent.SetActive(active);
		}
	}

	void SetScoreUIActive(bool active)
	{
		if (finalScoreText != null) finalScoreText.gameObject.SetActive(active);
		if (highscoreMessageText != null) highscoreMessageText.gameObject.SetActive(active);
	}

	void DisplayFinalScore()
	{
		if (finalScoreText != null)
		{
			finalScoreText.text = $"Final Score: {GameDataTransfer.FinalScore}";
		}
		if (highscoreMessageText != null)
		{
			string message = GameDataTransfer.IsNewHighscore
				? "NEW HIGH RECORD!"
				: $"High Score: {GameDataTransfer.HighScore}";
			highscoreMessageText.text = message;
		}
	}

	IEnumerator SceneSequenceCoroutine()
	{
		// Start with the panel fully opaque (if it's not already, e.g., from a previous scene's fade-out)
		//    and then fade it in (alpha from 1 to 0).
		fadePanel.gameObject.SetActive(true);
		Color baseColor = fadePanel.color;
		yield return StartCoroutine(Fade(1f, 0f, fadeInDuration, baseColor)); // Fade In

		//  Wait for the specified duration
		yield return new WaitForSeconds(cinematicDuration);


		// fade to black screen
		yield return StartCoroutine(Fade(0f, 1f, fadeToBlackDuration, baseColor));

		// remove bg and display score
		SetBackgroundActive(false);
		DisplayFinalScore();
		SetScoreUIActive(true);
		Debug.Log("Scores displayed. Waiting for Spacebar input...");

		// wait until input
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

		SetScoreUIActive(false);
		SceneManager.LoadScene(sceneIndexToLoad);
	}

	IEnumerator Fade(float startAlpha, float endAlpha, float duration, Color color)
	{
		float elapsedTime = 0f;

		fadePanel.color = new Color(color.r, color.g, color.b, startAlpha);

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
			fadePanel.color = new Color(color.r, color.g, color.b, newAlpha);
			yield return null;
		}
		fadePanel.color = new Color(color.r, color.g, color.b, endAlpha);
	}
}