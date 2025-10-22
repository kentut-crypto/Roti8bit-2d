using UnityEngine;

public class FallingObject : MonoBehaviour
{
	public float fallSpeed;
	public float spinSpeed;

	public float destroyYPosition = -15f;

	void Update()
	{
		transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
		transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
		if (transform.position.y < destroyYPosition)
		{
			Destroy(gameObject);
		}
	}
}
