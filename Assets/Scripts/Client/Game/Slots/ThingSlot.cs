using UnityEngine;

public class ThingSlot: MonoBehaviour {

	CardInfo card;
	public ThingType thingType;
	public enum ThingType {
		WEAPON,
		HEAD,
		ARMOR,
		SHOES
	}
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
		Vector3 newPos = transform.position;
        Vector3 newAngl = new Vector3(0f, 0, 0f);

		card.cardMovment.animator.MoveTo(newPos, newAngl, 1f);
		card.cardMovment.WriteNewPosition(newPos, newAngl);
    }
}
