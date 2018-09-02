using System.Collections;
using UnityEngine;

public class CardAnimator: MonoBehaviour {

	[HideInInspector] public CardMovment movment;

	private void Awake() {
		movment = GetComponent<CardMovment>();
	}

	public void MoveTo(Vector3 targetPos, Vector3 targetAngles, float time) {
		movment.moving = true;

		transform.eulerAngles = targetAngles;

		StopAllCoroutines();
		StartCoroutine(MoveToPos(targetPos, targetAngles, time, true));
	}
	public void PlayCard() {
		Vector3 pos = transform.position;
		pos.z -= 2 * Mathf.Sign(pos.z);
		pos.y = 2;

		Vector3 angl = transform.eulerAngles + Vector3.forward * 180;
		transform.eulerAngles = angl;

		StopAllCoroutines();
		StartCoroutine(MoveToPos(pos, transform.eulerAngles, 1.5f, true));
		StartCoroutine(DestroyCard(1.5f));
	}
	public void CardToPile() {
		movment.moving = true;
		movment.ResetPosition();

		Vector3 pos = new Vector3(-12, 0.51f, 0);

		StopAllCoroutines();
		StartCoroutine(MoveToPos(pos, transform.eulerAngles, 2f, true));
		StartCoroutine(DestroyCard(2f));
	}
	public void HoverCard(Vector3 basePosition) {
		float zLimit = 1;
		float xLimit = 4;
		float distanceToCamera = 3;

		float k = distanceToCamera / (Camera.main.transform.position.y - basePosition.y);

		Vector3 hoverPosition;
		hoverPosition.x = Mathf.Min(Mathf.Max(k * basePosition.x, -xLimit), xLimit);
		hoverPosition.y = Camera.main.transform.position.y - distanceToCamera;
		hoverPosition.z = Mathf.Min(Mathf.Max(k * transform.position.z, -zLimit), zLimit);

		transform.eulerAngles = Vector3.zero;

		StopAllCoroutines();
		StartCoroutine(MoveToPos(hoverPosition, Vector3.zero, 0.1f, false));
	}
	
	private IEnumerator MoveToPos(Vector3 targetPosition, Vector3 targetAngles, float time, bool locked) {
		Vector3 velocity = Vector3.zero;
		float t = 0;

		while (t < time) {
			transform.position = transform.position = Vector3.Lerp(transform.position, targetPosition, (t / time));

			t += Time.deltaTime;

			yield return new WaitForFixedUpdate();
		}

		movment.moving = false;
	}
	private IEnumerator DestroyCard(float t) {
		yield return new WaitForSeconds(t);
		Destroy(gameObject);
	}
}