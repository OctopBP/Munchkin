using System.Collections;
using UnityEngine;
using TMPro;

public enum TurnStage {
	preparation,
	waiting,
	after_door,
	fight_player,
	fight_enemy,
	completion
}

public class ServerTurnController : MonoBehaviour {

	private int[] turnTime;
	private int turnNumber;
	private TurnStage currentTurnStage;

	// for debug
	public TextMeshProUGUI timeText;
	public TextMeshProUGUI stageText;
	public TextMeshProUGUI playerText;
	
	public int CurPlayerTurnNum { get { return turnNumber % 2; } }

	public void StatFirstTurn() {
		turnTime = new int[] { 20, 2, 20, 20, 5, 10, 5 };
		currentTurnStage = TurnStage.preparation;
		turnNumber = 0;

		Server.Instance.SendChangeTurn(currentTurnStage, CurPlayerTurnNum);

		StartCoroutine(TurnFunc());
	}

	IEnumerator TurnFunc() {
		int timeToEndTurn = turnTime[(int)currentTurnStage];
		stageText.text = currentTurnStage.ToString();
		playerText.text = "pNum: " + CurPlayerTurnNum;

		while (timeToEndTurn >= 0) {
			timeText.text = timeToEndTurn.ToString();
			yield return new WaitForSeconds(1);
			timeToEndTurn--;
		}

		ChangeTurn();
	}

	public void ChangeTurn() {
		StopAllCoroutines();

		switch (currentTurnStage) {
			case TurnStage.preparation:
				OpenDoor();
				break;

			case TurnStage.waiting:
				TakeCardFromWT();
				break;

			case TurnStage.after_door:
				currentTurnStage = TurnStage.completion;
				Server.Instance.SendChangeTurn(currentTurnStage, CurPlayerTurnNum);
				break;

			case TurnStage.fight_player:
				CheckWinAfterPlayerTurn();
				break;

			case TurnStage.fight_enemy:
				CheckWinAfterEnemyTurn();
				break;

			case TurnStage.completion:
				currentTurnStage = TurnStage.preparation;
				turnNumber++;
				Server.Instance.SendChangeTurn(currentTurnStage, CurPlayerTurnNum);
				break;
		}

		StartCoroutine(TurnFunc());
	}

	void OpenDoor() {
		bool isMonster;
		ServerGM.Instance.OpenDoor(out isMonster);

		if (isMonster)
			currentTurnStage = TurnStage.fight_player;
		else
			currentTurnStage = TurnStage.waiting;
	}

	void TakeCardFromWT() {
		ServerGM.Instance.warTable.PlaseCardToHand(CurPlayerTurnNum);
		Server.Instance.SendTakeCardFromWT();
		currentTurnStage = TurnStage.after_door;
	}

	void CheckWinAfterPlayerTurn() {
		if (ServerGM.Instance.warTable.PlayerCanWin) {
			currentTurnStage = TurnStage.fight_enemy;
		}
		else {
			currentTurnStage = TurnStage.completion;

			ServerGM.Instance.warTable.ClearTable();
			Server.Instance.SendEndFight(playerWin: false);
		}
		Server.Instance.SendChangeTurn(currentTurnStage, CurPlayerTurnNum);
	}

	void CheckWinAfterEnemyTurn() {
		if (ServerGM.Instance.warTable.PlayerCanWin) {
			ServerGM.Instance.OnPlayerWinFight();
			currentTurnStage = TurnStage.completion;

			Server.Instance.SendEndFight(playerWin: true);
		}
		else {
			currentTurnStage = TurnStage.fight_player;
		}

		Server.Instance.SendChangeTurn(currentTurnStage, CurPlayerTurnNum);
	}


	/*
	public void MonsterPlayed() {
		StopAllCoroutines();

		currentTurnStage = TurnStage.fight_player;

		StartCoroutine(TurnFunc());
	}

	void TakeDoor() {
		ServerGM.Instance.GiveCardToHand(ServerGM.Instance.doorDeck, ServerGM.Instance.GetCurPlayer().munchkin.hand);
		currentTurnStage = TurnStage.completion;
	}
	*/
}
