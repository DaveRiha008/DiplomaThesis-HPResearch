using UnityEngine;

public class DetectionAreaScript : MonoBehaviour
{
	public int myIndex = 0;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			DataCollectionManager.AreaEntered(myIndex);
		}
	}
}
