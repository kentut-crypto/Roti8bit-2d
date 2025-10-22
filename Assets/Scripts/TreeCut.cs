using UnityEngine;
using UnityEngine.UI;

public class TreeCut : Tool
{
    [Header("Tree Stats")]
    [SerializeField] int treeHealth = 25;
    [SerializeField] int damagePerHit = 10;

    [Header("Scoring")]
    [SerializeField] int pointsForCutting = 1;

    [Header("UI Visuals")]
    // We keep this public so we can see in the Inspector if it was found correctly,
    // but we no longer need to drag anything into it manually.
    public Slider healthBarSlider;

	[Header("Audio Effects")]
	[SerializeField] private AudioClip hitSound;
	[SerializeField] private float hitVolume = 0.5f;
	[SerializeField] private AudioClip munchSound;
	[SerializeField] private float munchVolume = 1.25f;

	private AudioSource audioSource;

	private int maxHealth;

    private void Awake()
    {
        // --- THIS IS THE NEW CODE ---
        // Automatically find the Slider component within this prefab's children.
        healthBarSlider = GetComponentInChildren<Slider>();

        // A quick check to make sure it was found, to prevent future errors.
        if (healthBarSlider == null)
        {
            Debug.LogError(gameObject.name + " could not find a Slider in its children! Make sure the health bar is part of the prefab.");
        }
		// --- END OF NEW CODE ---

		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = gameObject.AddComponent<AudioSource>();
		}
		audioSource.playOnAwake = false;

		// Store the maximum health so we can calculate the fill percentage later
		maxHealth = treeHealth;

        // You can now safely hide the health bar here if you want.
        healthBarSlider.gameObject.SetActive(false);
    }

    public override void Hit()
    {
		if (hitSound != null && audioSource != null)
		{
			audioSource.PlayOneShot(hitSound, hitVolume);
		}

		// Now that the slider is found automatically in Awake,
		// this Hit() function will work perfectly without any changes.

		if (healthBarSlider != null && !healthBarSlider.gameObject.activeInHierarchy)
        {
            healthBarSlider.gameObject.SetActive(true);
        }

        treeHealth -= damagePerHit;
        Debug.Log(gameObject.name + " was hit! Remaining health: " + treeHealth);

        if (healthBarSlider != null)
        {
            healthBarSlider.value = (float)treeHealth / maxHealth;
        }

        if (treeHealth <= 0)
        {
            // ... The rest of your code to destroy the tree ...
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(pointsForCutting);
            }

			if (munchSound != null)
			{
				AudioSource.PlayClipAtPoint(munchSound, transform.position, munchVolume);
			}

			Destroy(gameObject);
        }
    }
}