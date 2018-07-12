using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WarTableSide : MonoBehaviour {

	public WarTable warTable;
	public TextMeshPro dmgText;
	public Vector3 plasePos;
	public float SPASE;

	public List<CardInfo> cards;
	//public int dmg = 0;

	void Awake() {
		cards = new List<CardInfo>();
	}

	public void AddCard(CardInfo card) {
		card.transform.parent = transform;
		card.transform.eulerAngles = new Vector3(0, Random.Range(-10, 10), 0);
		cards.Add(card);

		PlaceCards();

		card.cardMovment.WriteNewPosition();
	}

	private void PlaceCards() {
		int i = 0;
		foreach (CardInfo card in cards) {
			Vector3 newPos = plasePos;
			newPos.x += i * SPASE - ((cards.Count - 1) * SPASE / 2);
			card.transform.position = newPos;
			i++;
		}
	}

	public void ClearSide() {
		foreach (CardInfo card in cards)
			Destroy(card.gameObject);
		
		cards.Clear();
	}

	//public void DestroyAllCards() {
	//	foreach (CardInfo card in cards)
	//		Destroy(card.gameObject);
	//}
}
