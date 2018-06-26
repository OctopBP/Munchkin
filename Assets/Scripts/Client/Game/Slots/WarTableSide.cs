﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WarTableSide : MonoBehaviour {

	public WarTable warTable;
	public TextMeshPro dmgText;
	public Vector3 plasePos;

	public List<CardInfo> cards;
	public int dmg = 0;

	void Awake() {
		cards = new List<CardInfo>();
	}

	public void AddCard(CardInfo card) {
		card.transform.parent = transform;
		card.transform.position = transform.position;
		card.transform.eulerAngles = Vector3.zero;

		cards.Add(card);
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