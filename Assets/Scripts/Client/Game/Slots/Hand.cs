using System.Collections.Generic;
using UnityEngine;

public class Hand: MonoBehaviour {

    List<CardInfo> cardList;

    public Vector3 plasePosition;
    public float angle;
    public float radius;
    public float border;

    void Awake() {
        cardList = new List<CardInfo>();
    }

    public void AddCard(CardInfo card) {
        cardList.Add(card);
		card.transform.parent = transform;
		card.cardMovment.defaultParent = transform;
        PlaсeCards();
    }

    public void RemoveCard(CardInfo card) {
        cardList.Remove(card);
        PlaсeCards();
    }

    public void PlaсeCards() {
		int i = 0;
		foreach(CardInfo card in cardList) {
			card.selfHidenCard.closeId = i;

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
			
			card.cardMovment.MoveTo(newPos, newAngl, 1f);
			
			i++;
		}
    }

	public CardInfo GetCard(int cloaseId) {
		return cardList.Find(card => card.selfHidenCard.closeId == cloaseId);
	}
}
