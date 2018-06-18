using System.Collections;
using UnityEngine;

public class CardAnimator : MonoBehaviour {

	public AnimationCurve fromDeckCurveX;
	public AnimationCurve fromDeckCurveY;
	public AnimationCurve fromDeckCurveZ;

	public float timeToMove;

	//public Vector3 startPos;
	//public Vector3 startAngles;

	public void MoveTo(Vector3 startPos, Vector3 targetPos, Vector3 startAngles, Vector3 targetAngles) {
		StopAllCoroutines();
		StartCoroutine(Move(startPos, targetPos, startAngles, targetAngles));

		Debug.Log("Card " + name);
		Debug.Log("Rotate from: " + startAngles + " to: " + targetAngles);
	}

	IEnumerator Move(Vector3 startPos, Vector3 targetPos, Vector3 startAngles, Vector3 targetAngles) {

		float timeStamp = Time.time;
		Vector3 posDelta = targetPos - startPos;
		Vector3 anglesDelta;
		anglesDelta.x = (targetAngles.x - startAngles.x + 360) % 360;
		anglesDelta.y = (targetAngles.y - startAngles.y + 360) % 360;
		anglesDelta.z = (targetAngles.z - startAngles.z + 360) % 360;

		Debug.Log(anglesDelta);

		while (Time.time - timeStamp <= timeToMove) {
			float time = (Time.time - timeStamp) / timeToMove;
			Vector3 newPos;
			Vector3 newAngl;

			Vector3 k;

			k.x = fromDeckCurveX.Evaluate(time);
			k.y = fromDeckCurveY.Evaluate(time);
			k.z = fromDeckCurveZ.Evaluate(time);

			newPos.x = startPos.x + (k.x * posDelta.x);
			newPos.y = startPos.y + (k.y * posDelta.y);
			newPos.z = startPos.z + (k.z * posDelta.z);

			newAngl.x = startAngles.x + (k.x * anglesDelta.x);
			newAngl.y = startAngles.y + (k.y * anglesDelta.y);
			newAngl.z = startAngles.z + (k.z * anglesDelta.z);

			transform.position = newPos;
			transform.eulerAngles = newAngl;

            yield return new WaitForFixedUpdate();
		}

		//GetComponent<CardMovment>().WriteNewPosition();
	}
}