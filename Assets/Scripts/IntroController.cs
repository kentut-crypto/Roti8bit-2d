using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Make sure to include this for TextMeshPro

public class IntroController : MonoBehaviour
{
    // Drag your TextMeshPro object here in the Unity Inspector
    public TextMeshProUGUI introText;

    // The name of your main game scene
    public string mainGameSceneName = "MainScene";

    // Adjust these values in the Inspector
    [Header("Timing Settings")]
    public float timeBetweenChars = 0.05f; // How fast the text types out
    public float timeAfterParagraph = 2.0f; // How long to wait after a paragraph is finished

    // Optional: Add a sound effect for typing
    [Header("Audio Settings")]
    public AudioSource typingAudioSource; // Assign an AudioSource component

	private bool skipRequested = false;

	// The four paragraphs for your intro
	private string[] introParagraphs = new string[]
    {
		"In a small town filled with the aroma of freshly baked bread, you—an ordinary traveler—find yourself wandering the streets with an empty stomach and no money in your pocket. The sun is setting, your stomach growls louder with every step, and the only thing on your mind is one simple wish: to eat some bread.",
		"Then, you stumble upon a cozy little bakery called Roti 8 Bit, famous for its warm loaves and pixel-perfect patterns. The baker isn’t around—but the shelves are full, and time is ticking. You realize this might be your only chance to finally satisfy your hunger.",
		"You have only one minute to grab and eat as much bread as you can. Every loaf brings you closer to fullness—but be careful. Some breads are fresh and delicious, while others are burnt, moldy, or might slow you down.",
		"Every choice counts. Will you eat wisely and fill your stomach before time runs out? Or will your hunger get the best of you?",
		"The clock is ticking. The smell of bread fills the air. And only you can decide: starvation… or satisfaction."
	};

    void Start()
    {
        // Ensure the text is empty at the start
        introText.text = "";
        StartCoroutine(PlayIntroSequence());
    }

	void Update()
	{
        // check input spasi
		if (Input.GetKeyDown(KeyCode.Space))
		{
			skipRequested = true;
		}
	}

	IEnumerator PlayIntroSequence()
    {
        // Loop through each paragraph
        foreach (string paragraph in introParagraphs)
        {
            // Call the typing coroutine for the current paragraph
            yield return StartCoroutine(TypeText(paragraph));

            // check tiap frame
			float waitTimer = 0f;
			while (waitTimer < timeAfterParagraph)
            {
				if (skipRequested)
				{
					skipRequested = false;
					break;
				}
				waitTimer += Time.deltaTime;
				yield return null;
			}

			introText.text = "";
			skipRequested = false;
		}

        // After the last paragraph, load the main game
        SceneManager.LoadScene(mainGameSceneName);
    }

    IEnumerator TypeText(string textToType)
    {
        // 'i' will be our character counter
        int i = 0;

        while (i < textToType.Length)
        {
            if (skipRequested)
            {
				introText.text = textToType; // langsung show
				skipRequested = false;
				if (typingAudioSource != null)
                {
					typingAudioSource.Stop();
				}
                break;
			}

            introText.text += textToType[i];
            i++;

            // Play a typing sound, if one is assigned
            if (typingAudioSource != null)
            {
                typingAudioSource.Play();
            }

            // Wait a short moment before typing the next character
            yield return new WaitForSeconds(timeBetweenChars);
        }
    }
}