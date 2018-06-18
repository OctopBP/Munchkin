using UnityEngine;

public class ClassSlot: MonoBehaviour {

	CardInfo card;

	public void AddCard(CardInfo card) {
		if (this.card != null) {
			Destroy(this.card.gameObject);
		}
		this.card = card;
		placeCard();
	}

	public void RemoveCard(CardInfo card) {
		//
	}

	void placeCard() {
		card.transform.position = transform.position;
		card.transform.eulerAngles = new Vector3(0f, 0, 0f);

	}
}
