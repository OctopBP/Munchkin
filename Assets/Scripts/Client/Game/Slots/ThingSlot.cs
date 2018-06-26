using UnityEngine;

public class ThingSlot: MonoBehaviour {

	CardInfo card;
	public ThingCard.ThingType thingType;

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

	public int GetSlotBonus() {
		return card == null ? 0 : (card.selfCard as ThingCard).bonus;
	}
}
