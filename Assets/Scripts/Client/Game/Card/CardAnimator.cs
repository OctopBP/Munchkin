using System.Collections;
using UnityEngine;

public class CardAnimator: MonoBehaviour {

	public bool moving = false;

	public void MoveTo(Vector3 targetPos, Vector3 targetAngles, float time) {
		transform.eulerAngles = targetAngles;

		StopAllCoroutines();
		StartCoroutine(MoveToPos(targetPos, targetAngles, time));
	}
	public void PlayCard() {
		Vector3 pos = transform.position;
		pos.z -= 2 * Mathf.Sign(pos.z);
		pos.y = 2;

		Vector3 angl = transform.eulerAngles;
		angl.z += 180;
		transform.eulerAngles = angl;

		StopAllCoroutines();
		StartCoroutine(MoveToPos(pos, transform.eulerAngles, 1.5f));
		StartCoroutine(DestroyCard(1.5f));
	}
	public void CardToPile() {
		Vector3 pos = new Vector3(-12, 0.51f, 0);

		StopAllCoroutines();
		StartCoroutine(MoveToPos(pos, transform.eulerAngles, 2f));
		StartCoroutine(DestroyCard(2f));
	}

	private IEnumerator MoveToPos(Vector3 targetPosition, Vector3 targetAngles, float time) {
		moving = true;

		Vector3 velocity = Vector3.zero;
		float t = 0;

		while (t < time) {
			transform.position = transform.position = Vector3.Lerp(transform.position, targetPosition, (t / time));

			t += Time.deltaTime;

			yield return new WaitForFixedUpdate();
		}
		moving = false;
	}
	private IEnumerator DestroyCard(float t) {
		yield return new WaitForSeconds(t);
		Destroy(gameObject);
	}
}