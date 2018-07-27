using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class HidenCard {
	public int closeId = 0;

	public DeckType deckType;
	public enum DeckType {
		DOOR,
		TREASURE,
		PERK
	}
}
[Serializable] public class Card: HidenCard {
	public int id;

	public CardType cardType;
	public enum CardType {
		MONSTER,
		CLASS,
		LVLUP,
		THING,
		EXPLOSIVE,
		TRAP
	}

	public string name;
	public string texName;
}
	
public static class CardManagerData {
	public static List<Card> allCards = new List<Card>();
}

public class CardManager : MonoBehaviour {
	
	void Awake() {
		try {
			CardManagerData.allCards.AddRange(JsonReader.ReadJson<Card>("MonsterCards"));
			CardManagerData.allCards.AddRange(JsonReader.ReadJson<Card>("ClassCards"));
			CardManagerData.allCards.AddRange(JsonReader.ReadJson<Card>("TrapCards"));
			CardManagerData.allCards.AddRange(JsonReader.ReadJson<Card>("LvlupCards"));
			CardManagerData.allCards.AddRange(JsonReader.ReadJson<Card>("ThingCards"));
			CardManagerData.allCards.AddRange(JsonReader.ReadJson<Card>("ExplosiveCards"));
		}
		catch(Exception e) {
			Debug.LogError(e.Message);
		}
	}
}