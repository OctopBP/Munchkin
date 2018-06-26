using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HidenCard {
	public int closeId = 0;

	public DeckType deckType;
	public enum DeckType {
		DOOR,
		TREASURE,
		PERK
	}
}

[Serializable]
public class Card: HidenCard {
	public int id;

	public CardType cardType;
	public enum CardType {
		MONSTER,
		CLASS,
		LVLUP,
		THING,
		EXPLOSIVE
	}

	public string name;
	public string texName;
}

[Serializable]
public class MonsterCard: Card {
	public int lvl;
	public int numberOfTreasure = 1;
}

[Serializable]
public class ClassCard: Card {
}

[Serializable]
public class LvlupCard: Card {
}

[Serializable]
public class ThingCard: Card {
	public int bonus;

	public ThingType thingType;
	public enum ThingType {
		WEAPON,
		HEAD,
		ARMOR,
		SHOES
	}
}

[Serializable]
public class ExplosiveCard: Card {
	public int dmg;
}

public static class CardManagerData {
	public static List<Card> allDoorCards = new List<Card>();
    public static List<Card> allTreasureCards = new List<Card>();
	public static List<Card> allCard = new List<Card>();
}

public class CardManager : MonoBehaviour {
	
	void Awake() {
		try {
			CardManagerData.allDoorCards.AddRange(JsonReader.ReadJson<MonsterCard>("MonsterCards"));
			CardManagerData.allDoorCards.AddRange(JsonReader.ReadJson<ClassCard>("ClassCards"));

			CardManagerData.allTreasureCards.AddRange(JsonReader.ReadJson<LvlupCard>("LvlupCards"));
			CardManagerData.allTreasureCards.AddRange(JsonReader.ReadJson<ThingCard>("ThingCards"));
			CardManagerData.allTreasureCards.AddRange(JsonReader.ReadJson<ExplosiveCard>("ExplosiveCards"));
		}
		catch(Exception e) {
			Debug.LogError(e.Message);
		}

		CardManagerData.allCard.AddRange(CardManagerData.allDoorCards);
		CardManagerData.allCard.AddRange(CardManagerData.allTreasureCards);

		/*
		Card closeTreasureCard = new Card {
			texName = "TreasureClose",
			id = 0,
			cardType = Card.CardType.THING,
			deckType = HidenCard.DeckType.TREASURE,
			name = "Treasure"
		};
		CardManagerData.allTreasureCards.Add(closeTreasureCard);

		Card closeDoorCard = new Card {
			texName = "DoorClose",
			id = 0,
			cardType = Card.CardType.THING,
			deckType = HidenCard.DeckType.DOOR,
			name = "Door"
		};
		CardManagerData.allDoorCards.Add(closeDoorCard);

		ThingCard thingCard = new ThingCard {
			texName = "LincolnsHat",
			id = 1,
			bonus = 1,
			cardType = Card.CardType.THING,
			deckType = HidenCard.DeckType.TREASURE
		};
		thingCard.name = "LincolnsHat" + thingCard.id.ToString();
		CardManagerData.allTreasureCards.Add(thingCard);
		thingCard.name = "LincolnsHat" + (++thingCard.id).ToString();
		CardManagerData.allTreasureCards.Add(thingCard);
		thingCard.name = "LincolnsHat" + (++thingCard.id).ToString();
		CardManagerData.allTreasureCards.Add(thingCard);
		thingCard.name = "LincolnsHat" + (++thingCard.id).ToString();
		CardManagerData.allTreasureCards.Add(thingCard);
		thingCard.name = "LincolnsHat" + (++thingCard.id).ToString();
		CardManagerData.allTreasureCards.Add(thingCard);
		thingCard.name = "LincolnsHat" + (++thingCard.id).ToString();
		CardManagerData.allTreasureCards.Add(thingCard);
		thingCard.name = "LincolnsHat" + (++thingCard.id).ToString();
		CardManagerData.allTreasureCards.Add(thingCard);


		MonsterCard monsterCard = new MonsterCard {
			texName = "ViciousDog",
			id = 8,
			lvl = 1,
			cardType = Card.CardType.MONSTER,
			deckType = HidenCard.DeckType.DOOR
		};
		monsterCard.name = "ViciousDog" + monsterCard.id.ToString();
		CardManagerData.allDoorCards.Add(monsterCard);
		monsterCard.name = "ViciousDog" + (++monsterCard.id).ToString();
		CardManagerData.allDoorCards.Add(monsterCard);
		monsterCard.name = "ViciousDog" + (++monsterCard.id).ToString();
		CardManagerData.allDoorCards.Add(monsterCard);
		monsterCard.name = "ViciousDog" + (++monsterCard.id).ToString();
		CardManagerData.allDoorCards.Add(monsterCard);
		monsterCard.name = "ViciousDog" + (++monsterCard.id).ToString();
		CardManagerData.allDoorCards.Add(monsterCard);
		*/
	}
}