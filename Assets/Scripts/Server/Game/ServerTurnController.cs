﻿using System.Collections;
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
	public TurnStage currentTurnStage;

	// for debug
	public TextMeshProUGUI timeText;
	public TextMeshProUGUI stageText;
	public TextMeshProUGUI playerText;
	
	public int CurPlayerTurnNum { get { return turnNumber % 2; } }

	public void TryChangeTurn(int pNum) {
		bool isPlayerTurn = ServerGM.Instance.turnController.CurPlayerTurnNum == ServerGM.Instance.GetPlayerAt(pNum).info.number;
		bool canEndTurn = (isPlayerTurn ^ (currentTurnStage == TurnStage.fight_enemy)) && (currentTurnStage != TurnStage.waiting);

		if (canEndTurn)
			ChangeTurn();
	}
	public void StatFirstTurn() {
		turnTime = new int[] { 20, 2, 15, 15, 15, 10 };
		currentTurnStage = TurnStage.preparation;
		turnNumber = 0;

		Server.Instance.SendChangeTurn(currentTurnStage, CurPlayerTurnNum);

		StartCoroutine(TurnFunc());
	}
	public void MonsterPlayed() {
		StopAllCoroutines();

		currentTurnStage = TurnStage.fight_player;
		Server.Instance.SendChangeTurn(currentTurnStage, CurPlayerTurnNum);

		StartCoroutine(TurnFunc());
	}

	private IEnumerator TurnFunc() {
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

	private void OpenDoor() {
		bool isMonster;
		ServerGM.Instance.OpenDoor(out isMonster);

		if (isMonster)
			currentTurnStage = TurnStage.fight_player;
		else
			currentTurnStage = TurnStage.waiting;
	}
	private void TakeCardFromWT() {
		ServerGM.Instance.warTable.PlaseCardToHand(CurPlayerTurnNum);
		Server.Instance.SendTakeCardFromWT();
		currentTurnStage = TurnStage.after_door;
	}
	private void CheckWinAfterPlayerTurn() {
		if (ServerGM.Instance.warTable.PlayerCanWin) {
			currentTurnStage = TurnStage.fight_enemy;
		}
		else {
			// lose
			currentTurnStage = TurnStage.completion;

			ServerGM.Instance.warTable.ClearTable();
			Server.Instance.SendEndFight(playerWin: false);
		}
		Server.Instance.SendChangeTurn(currentTurnStage, CurPlayerTurnNum);
	}
	private void CheckWinAfterEnemyTurn() {
		if (ServerGM.Instance.warTable.PlayerCanWin) {
			// win
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
