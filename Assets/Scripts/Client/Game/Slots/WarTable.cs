using UnityEngine;

public class WarTable : MonoBehaviour {

	public WarTableSide playerSide;
	public WarTableSide monsterSide;

	public void AddCard(CardInfo card, bool playerDS) {
		playerDS &= card.selfCard.cardType != Card.CardType.MONSTER;
		
		if (playerDS)
			playerSide.AddCard(card);
		else
			monsterSide.AddCard(card);
	}
	public void StartFight(CardInfo monster) {
		AddCard(monster, false);
	}

	public void PlaseCardToHand(int pNum) {
		GameManager.Instance.GetMunchkin(pNum).hand.AddCard(playerSide.cards[0]);
		playerSide.cards.RemoveAt(0);
	}

	public void ClearTable() {
		playerSide.ClearSide();
		monsterSide.ClearSide();
	}

	public void SetDmgText(int monDmg, int playerDmg) {
		playerSide.dmgText.text = playerDmg.ToString();
		monsterSide.dmgText.text = monDmg.ToString();
	}
}