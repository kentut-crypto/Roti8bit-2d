using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
	public GameObject pauseMenuPanel;

	public string mainMenuSceneName = "MainMenu";

	private bool isPaused = false;

	void Start()
	{
		pauseMenuPanel.SetActive(false);
	}

	void Update()
	{
		// check esc key
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (isPaused)
			{
				ResumeGame();
			}
			else
			{
				PauseGame();
			}
		}
	}

	public void ResumeGame()
	{
		pauseMenuPanel.SetActive(false);
		Time.timeScale = 1f;
		isPaused = false;
	}

	private void PauseGame()
	{
		pauseMenuPanel.SetActive(true);
		Time.timeScale = 0f;
		isPaused = true;
	}

	public void LoadMainMenu()
	{
        Time.timeScale = 1f; 
        SceneManager.LoadScene(mainMenuSceneName);
    }
}