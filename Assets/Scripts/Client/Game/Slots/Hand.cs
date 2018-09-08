using System.Collections.Generic;
using UnityEngine; 

public class Hand: DropSlot {

	private List<HandSlot> cardList;

    public Vector3 plasePosition;
    public float angle;
    public float radius;
    public float border;

	public const string SLOTID = "HA";

    void Awake() {
		cardList = new List<HandSlot>();
    }

    public void AddCard(CardInfo card) {
		string sId = "TEMP"; // SLOTID + cardList.Count;
		cardList.Add(new HandSlot(sId, card));

		card.transform.parent = transform;
		card.cardMovment.defaultParent = transform;

		PlaсeCards();
		SetNewslotIds();
    }

	// RemoveSlot() убирает слот и карту из руки
	// RemoveCard() сбрасивает карту в колоду сброса и затем убирает слот из руки

	/// <summary>
	/// Убирает слот с картой и вызывает PlaceCard().
	/// </summary>
	/// <param name="sId">Slot ID.</param>
	public void RemoveSlot(string sId) {
		cardList.Remove(GetSlot(sId));
		PlaсeCards();
    }
	public void RemoveCard(string sId) {
		GetSlot(sId).RemoveCard();
		//slotIdsToRemove.Add(sId);
		RemoveSlot(sId);
	}

	public void SetNewslotIds() {
		int i = 0;
		foreach (HandSlot slot in cardList) {
			string newSlotId = SLOTID + i;
			slot.slotId = newSlotId;
			slot.GetCard().selfCard.slotId = newSlotId;
			i++;
		}
	}

    public void PlaсeCards() {
		int i = 0;
		foreach(HandSlot slot in cardList) {
			float newAngle = (i - (float)(cardList.Count) / 2 + 0.5f) * angle;

			Vector3 newPos;
			newPos.x = radius * Mathf.Sin(newAngle);
			newPos.y = plasePosition.y + border * i;
			newPos.z = plasePosition.z - radius * (1 - Mathf.Cos(newAngle));

			Vector3 newAngl;
			if (GetComponent<DropSlot>().slotParent == SlotParent.ENEMY)
				newAngl = new Vector3(0f, (newAngle / Mathf.PI) * 180f, 180f);
			else
				newAngl = new Vector3(0f, (newAngle / Mathf.PI) * 180f, 0f);
			
			slot.GetCard().cardMovment.MoveTo(newPos, newAngl, 1f);
			
			i++;
		}
	}

	public HandSlot GetSlot(string slotId) {
		return cardList.Find(slot => slot.slotId == slotId);
	}
}

public class HandSlot {
	public string slotId;
	private readonly CardInfo card;

	public HandSlot(string sId, CardInfo c) {
		slotId = sId;
		card = c;
		card.selfCard.slotId = sId;
	}

	public void RemoveCard() {
		card.cardMovment.animator.CardToPile();
	}

	public CardInfo GetCard() {
		return card;
	}
}