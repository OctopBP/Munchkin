using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WarTableSide : MonoBehaviour {

	public WarTable warTable;
	public TextMeshPro dmgText;
	public Vector3 plasePos;
	public float SPASE;

	public List<CardInfo> cards;
	public int dmg = 0;

	void Awake() {
		cards = new List<CardInfo>();
	}

	public void AddCard(CardInfo card) {
		card.transform.parent = transform;
		//card.transform.eulerAngles = new Vector3(0, Random.Range(-10, 10), 0);
		cards.Add(card);

		PlaceCards();
	}

	private void PlaceCards() {
		int i = 0;
		foreach (CardInfo card in cards) {
			Vector3 newAngl = new Vector3(0, Random.Range(-10, 10), 0);
			Vector3 newPos = plasePos;
			newPos.x += i * SPASE - ((cards.Count - 1) * SPASE / 2);

			card.cardMovment.animator.MoveTo(newPos, newAngl, 0.2f);
			card.cardMovment.WriteNewPosition(newPos, newAngl);

			i++;
		}
	}

	public void ClearSide() {
		foreach (CardInfo card in cards)
			card.cardMovment.animator.CardToPile();
			//StartCoroutine(DestroyCard(card));
			//Destroy(card.gameObject);
		
		cards.Clear();
	}

	//private IEnumerator DestroyCard(CardInfo card) {
	//	float t = 0.5f;

	//	Vector3 velocity = Vector3.zero;
	//	Vector3 targetPosition = new Vector3(-15, 3, 0);
	//	Vector3 targetAngles = Vector3.zero;

	//	while (card.transform.position != targetPosition) {
	//		Vector3 newPosition = Vector3.SmoothDamp(card.transform.position, targetPosition, ref velocity, t);
	//		Vector3 newAngles = Vector3.SmoothDamp(card.transform.eulerAngles, targetAngles, ref velocity, t);

	//		card.transform.position = newPosition;
	//		card.transform.eulerAngles = newAngles;

	//		yield return new WaitForFixedUpdate();
	//	}

	//	Destroy(card.gameObject);
	//}
}
