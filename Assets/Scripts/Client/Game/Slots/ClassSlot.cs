using UnityEngine;

public class ClassSlot: MonoBehaviour {

	CardInfo card;

	public void AddCard(CardInfo newCard) {
		RemoveCard();

		card = newCard;
		newCard.transform.parent = transform;

		PlaceCard();
	}

	public void RemoveCard() {
		if (card != null) {
			Destroy(card.gameObject);
			card = null;
		}
	}

	void PlaceCard() {
		card.transform.position = transform.position;
		card.transform.eulerAngles = new Vector3(0f, 0, 0f);
		card.cardMovment.WriteNewPosition();
	}
}
