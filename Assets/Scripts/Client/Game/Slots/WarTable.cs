using UnityEngine;

public class WarTable : MonoBehaviour {

	public WarTableSide playerSide;
	public WarTableSide monsterSide;

	public bool PlayerCanWin { get { return playerSide.dmg > monsterSide.dmg; } }

	public void AddCard(CardInfo card, bool playerDS) {
		if (playerDS)
			playerSide.AddCard(card);
		else
			monsterSide.AddCard(card);
	}

	public void StartFight(CardInfo monster) {
		AddCard(monster, false);
		//CalculateDmg();
	}

	public void PlaseCardToHand(int pNum) {
		ClientGM.Instance.GetMunchkin(pNum).hand.AddCard(playerSide.cards[0]);
		playerSide.cards.RemoveAt(0);
	}

	public void ClearTable() {
		playerSide.ClearSide();
		monsterSide.ClearSide();
	}

	/*
	public void PlayCard(CardInfo card, bool playerDS) {
		if (card.typeIs(Card.CardType.LVLUP)) {
			playerSide.AddCard(card);
			gameManager.player.LvlUp(1);
			Destroy(card.gameObject);
			return;
		}

		if (card.typeIs(Card.CardType.MONSTER) && gameManager.CurrentTS_Is(TurnStage.after_door)) {
			//gameManager.turnController.MonsterPlayed();
			AddCard(card, false);
		}

		if (card.typeIs(Card.CardType.EXPLOSIVE) && gameManager.CurrentTS_Is(TurnStage.fight_player)) {
			AddCard(card, playerDS);
			CalculateDmg();
		}
	}



	public void CalculateDmg() {
		monsterSide.dmg = 0;
		foreach (CardInfo card in monsterSide.cards) {
			if (card.typeIs(Card.CardType.MONSTER))
				monsterSide.dmg += (card.selfCard as MonsterCard).lvl;
			else
				monsterSide.dmg += (card.selfCard as ExplosiveCard).dmg;
		}

		playerSide.dmg = gameManager.player.damage;
		foreach (CardInfo card in playerSide.cards) {
			if (card.typeIs(Card.CardType.EXPLOSIVE))
				playerSide.dmg += (card.selfCard as ExplosiveCard).dmg;
		}

		monsterSide.dmgText.text = monsterSide.dmg.ToString();
		playerSide.dmgText.text = playerSide.dmg.ToString();
	}

	// TODO: Remove from Update()
	void Update() {
		if (gameManager.CurrentTS_Is(TurnStage.fight_enemy) || gameManager.CurrentTS_Is(TurnStage.fight_player)) {
			CalculateDmg();
		}
		else {
			monsterSide.dmgText.text = "0";
			playerSide.dmgText.text = "0";
		}
	}


	public void PlaseCardToHand() {
		gameManager.player.hand.AddCard(playerSide.cards[0]);
		playerSide.cards.RemoveAt(0);
	}

	public int GetNumberOfTreasure() {
		int number = 0;

		foreach (CardInfo card in monsterSide.cards)
			if (card.typeIs(Card.CardType.MONSTER))
				number += (card.selfCard as MonsterCard).numberOfTreasure;
		
		return number;
	}
	*/
}