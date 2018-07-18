using System.Collections;
using UnityEngine;

public class CardAnimator: MonoBehaviour {

	public void Animate(Vector3 targetPos, Vector3 targetAngles, float time) {
		transform.eulerAngles = targetAngles;

		StopAllCoroutines();
		StartCoroutine(MoveToPos(targetPos, targetAngles, time));
	}

	private IEnumerator MoveToPos(Vector3 targetPosition, Vector3 targetAngles, float time) {
		Vector3 velocity = Vector3.zero;
		float t = 0;

		while (t < time) {
			transform.position = transform.position = Vector3.Lerp(transform.position, targetPosition, (t / time));

			t += Time.deltaTime;

			yield return new WaitForFixedUpdate();
		}
	}
}