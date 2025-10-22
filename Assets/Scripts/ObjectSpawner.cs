using System.Collections;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
	public GameObject[] objectPrefabs;

	public float spawnInterval = 0.5f;
	public float horizontalSpawnRange = 20f;

	public float minFallSpeed = 2f;
	public float maxFallSpeed = 5f;
	public float minSpinSpeed = -50f;
	public float maxSpinSpeed = 50f;

	void Start()
    {
		StartCoroutine(SpawnObjects());
	}


	IEnumerator SpawnObjects()
	{
		while (true)
		{
			yield return new WaitForSeconds(spawnInterval);

			GameObject prefabToSpawn = objectPrefabs[Random.Range(0, objectPrefabs.Length)];

			float randomX = Random.Range(-horizontalSpawnRange / 2, horizontalSpawnRange / 2);
			Vector3 spawnPosition = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z);

			GameObject newObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

			FallingObject fallingScript = newObject.GetComponent<FallingObject>();
			if (fallingScript != null)
			{
				fallingScript.fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
				fallingScript.spinSpeed = Random.Range(minSpinSpeed, maxSpinSpeed);
			}

			SpriteRenderer renderer = newObject.GetComponent<SpriteRenderer>();
			if (renderer != null)
			{
				renderer.sortingOrder = Random.Range(-5, 5);
			}
		}
	}
}