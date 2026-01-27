using UnityEngine;
using UnityEngine.Events;

public class PublicCollisionEvents : MonoBehaviour
{

    public UnityEvent<Collision2D> collisionEnter2D;
    public UnityEvent<Collision2D> collisionExit2D;
    public UnityEvent<Collider2D> triggerEnter2D;
    public UnityEvent<Collider2D> triggerExit2D;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		collisionEnter2D.Invoke(collision);
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		collisionExit2D.Invoke(collision);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		triggerEnter2D.Invoke(collision);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		triggerExit2D.Invoke(collision);
	}
}
