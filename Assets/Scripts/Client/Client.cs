using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections.Generic;

public class Client : MonoBehaviour {

	public static Client Instance { get; set; }

	private const int MAX_CONNECTION = 2;
	private const string LOCALHOST = "127.0.0.1";

	private int port = 5701;

	private int hostId;
	private int webHostId;

	private int ourClientId;
	private int connectionId;

	private int reliableChannel;

	private float connectionTime;
	private bool isConnected = false;

	private byte error;

	private void Awake() {
		Instance = this;

		GameManager.Instance.cnnWaitGroup.SetActive(false);
	}

	public void Connect() {
		string pName = GameObject.Find("NameInput").GetComponent<TMP_InputField>().text;
		if (pName == "") {
			Debug.Log("Plesae, enter a name");
			return;
		}

		GameManager.Instance.player.info.playerName = pName;
		GameManager.Instance.cnnCnnGroup.SetActive(false);
		GameManager.Instance.cnnWaitGroup.SetActive(true);

		NetworkTransport.Init();
		ConnectionConfig cc = new ConnectionConfig();

		reliableChannel = cc.AddChannel(QosType.Reliable);

		HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

		hostId = NetworkTransport.AddHost(topo, 0);
		connectionId = NetworkTransport.Connect(hostId, LOCALHOST, port, 0, out error);

		connectionTime = Time.time;
		isConnected = true;
	}
	public void Cancel() {
		GameManager.Instance.cnnCnnGroup.SetActive(true);
		GameManager.Instance.cnnWaitGroup.SetActive(false);

		NetworkTransport.Disconnect(hostId, connectionId, out error);

		isConnected = false;
	}

	private void Update() {
		if (!isConnected) {
			if (Input.GetKeyDown(KeyCode.Return))
				Connect();
			
			return;
		}

		int recHostId;
		int cnnId;
		int channelId;
		byte[] recBuffer = new byte[1024];
		int bufferSize = 1024;
		int dataSize;
		byte err;

		NetworkEventType recData = NetworkTransport.Receive(out recHostId, out cnnId, out channelId, recBuffer, bufferSize, out dataSize, out err);

		if (recData == NetworkEventType.DataEvent) {
			string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
			Debug.Log("Receiving: " + msg);

			string[] splitData = msg.Split('|');

			switch (splitData[0]) {
				case SendNames.askname:
					OnAskName(splitData);
					break;

				case SendNames.startgame:
					StartGame(splitData);
					break;

				case SendNames.disconnection:
					PlayerDisconnected(int.Parse(splitData[1]));
					break;

				case SendNames.cardtohand:
					//Debug.Log("Add card " + splitData[3] + " to player " + splitData[1]);
					AddCard(int.Parse(splitData[1]), (Card.DeckType)Enum.Parse(typeof(Card.DeckType), splitData[2]), int.Parse(splitData[3]));
					break;

				case SendNames.opendoor:
					//Debug.Log("Open door: " + splitData[1]);
					OpenDoor(splitData);
					break;

				case SendNames.endfigth:
					//Debug.Log("End fight: " + splitData[1]);
					OnEndFight(int.Parse(splitData[1]) == 1);
					break;

				case SendNames.takecardfromwt:
					TakeCardFromWT(int.Parse(splitData[1]));
					break;

				case SendNames.newstage:
					//Debug.Log("New Stage: " + splitData[2]);
					OnNewStage(int.Parse(splitData[1]), (TurnStage)Enum.Parse(typeof(TurnStage), splitData[2]), int.Parse(splitData[3]));
					break;

				case SendNames.dropallowed:
					//Debug.Log("Player " + splitData[1] + " drop " + splitData[2] + " card from pos " + splitData[3] + " to: " + splitData[4]);
					Drop(int.Parse(splitData[1]), splitData[2], splitData[3], int.Parse(splitData[4]));
					break;

				case SendNames.dropdisallowed:
					//Debug.Log("Drop for card whith id " + int.Parse(splitData[1]) + " disallowes beacose " + splitData[2]);
					DropDisallowed(splitData[1], splitData[2]);
					break;

				case SendNames.removecard:
					RemoveCard(int.Parse(splitData[1]), splitData[2]);
					break;

				case SendNames.values:
					UpdateValues(int.Parse(splitData[1]), int.Parse(splitData[2]), int.Parse(splitData[3]), int.Parse(splitData[4]), int.Parse(splitData[5]), int.Parse(splitData[6]));
					break;

				case SendNames.cardselectionstage:
					CardSelection(int.Parse(splitData[1]), splitData[2]);
					break;

				case SendNames.en_cardselectionstage:
					EmenyCardSelection(int.Parse(splitData[1]));
					break;

				case SendNames.hideweapon:
					HideWeapon(int.Parse(splitData[1]));
					break;

				case SendNames.showweapon:
					ShowWeapon(int.Parse(splitData[1]));
					break;

				default:
					Debug.Log("Invalid message: " + msg);
					break;
			}
		}
	}

	// Receive
	private void StartGame(string[] data) {
		for (int i = 1; i <= 2; i++) {
			string[] d = data[i].Split('%');

			int cnnId = int.Parse(d[0]);
			string pName = d[1];
			int pNum = int.Parse(d[2]);

			if (cnnId == ourClientId)
				GameManager.Instance.player.SetInfo(pName, pNum, cnnId);
			else
				GameManager.Instance.enemy.SetInfo(pName, pNum, cnnId);
		}

		GameManager.Instance.cnnCanvas.SetActive(false);
	}
	private void PlayerDisconnected(int cnnId) {
		//players.Remove(players.Find(c => c.connectionId == cnnId));
	}

	private void AddCard(int pNum, Card.DeckType deckType, int cardId) {
		CardInfo cardInfo;
		if (pNum == GameManager.Instance.player.info.number)
			cardInfo = GameManager.Instance.CreateCard(cardId);
		else
			cardInfo = GameManager.Instance.CreateCard(deckType);

		GameManager.Instance.GetMunchkin(pNum).hand.AddCard(cardInfo);
	}
	private void RemoveCard(int pNum, string slotId) {
		GameManager.Instance.RemoveCard(pNum, slotId);
	}

	private void OnNewStage(int turnClientNumber, TurnStage turnStage, int time) {
		GameManager.Instance.enemyDropCardsText.SetActive(false);


		// TODO: Remove

		// just for now
		if (turnStage != TurnStage.fight_enemy && turnStage != TurnStage.fight_player && turnStage != TurnStage.waiting && turnStage != TurnStage.after_door)
			GameManager.Instance.warTable.ClearTable();
		//


		GameManager.Instance.turnController.ChangeTurn(turnStage, GameManager.Instance.player.info.number == turnClientNumber, time);
	}

	private void Drop(int pNum, string sourseSlotId, string targetSlotId, int cardId) {
		GameManager.Instance.Drop(pNum, sourseSlotId, targetSlotId, cardId);
	}
	private void DropDisallowed(string slotId, string reason) {
		GameManager.Instance.DropDisallowed(slotId, reason);
	}

	private void OpenDoor(string[] data) {
		int pNum = int.Parse(data[1]);
		int cardId = int.Parse(data[2]);
		bool isMonster = int.Parse(data[3]) == 1;

		//if (isMonster)
		//	OnNewStage(pNum, TurnStage.fight_player);	
		//else
			//OnNewStage(pNum, TurnStage.waiting);

		GameManager.Instance.OpenDoor(cardId, isMonster);
	}

	private void OnEndFight(bool playerWin) {
		GameManager.Instance.warTable.ClearTable();
	}

	private void TakeCardFromWT(int pNum) {
		GameManager.Instance.warTable.PlaseCardToHand(pNum);
		//GameManager.Instance.turnController.ChangeTurn(TurnStage.after_door, GameManager.Instance.player.info.number == pNum);
	}

	private void CardSelection(int numberOfCards, string cardData) {
		string[] cards = cardData.Split('%');

		//Dictionary<int, string> cardsDictionary = new Dictionary<int, string>();
		List<string> cardsList = new List<string>();

		for (int i = 0; i < cards.Length; i++) {
			//string[] cardInfo = cards[i].Split('&');

			string slotName = cards[i];
			//int cardId = int.Parse(cardInfo[1]);

			cardsList.Add(slotName);
		}

		GameManager.Instance.SelectionCards(cardsList, numberOfCards);

		// for example
		//
		// cardData = "HAND & 32 % HAND & 21 % WEAPON1 & 43"	(with no spaces)
		// cards[0] = "HAND & 32" 								(with no spaces)
		// slotName = "HAND"
		// cardId = 32

	}
	private void EmenyCardSelection(int numberOfCards) {
		GameManager.Instance.enemyDropCardsText.SetActive(true);
		GameManager.Instance.enemyDropCardsText.GetComponent<TextMeshPro>().text = "Enemy select " + numberOfCards + " card to drop";
	}

	private void UpdateValues(int dmg_0, int lvl_0, int dmg_1, int lvl_1, int monDmg, int playerDmg) {
		if (GameManager.Instance.player.info.number == 0) {
			GameManager.Instance.player.SetDmgAndLvl(dmg_0, lvl_0);
			GameManager.Instance.enemy.SetDmgAndLvl(dmg_1, lvl_1);
		}
		else {
			GameManager.Instance.enemy.SetDmgAndLvl(dmg_0, lvl_0);
			GameManager.Instance.player.SetDmgAndLvl(dmg_1, lvl_1);
		}
		GameManager.Instance.warTable.SetDmgText(monDmg, playerDmg);
	}

	private void HideWeapon(int pNum) {
		GameManager.Instance.GetMunchkin(pNum).weapon2.gameObject.SetActive(false);
	}
	private void ShowWeapon(int pNum) {
		GameManager.Instance.GetMunchkin(pNum).weapon2.gameObject.SetActive(true);
	}

	// Send
	private void OnAskName(string[] data) {
		ourClientId = int.Parse(data[1]);
		Send(SendNames.nameis + "|" + GameManager.Instance.player.info.playerName);
	}

	public void OnDrop(string parentSlotId, string targetSlotId) {
		string msg = SendNames.trydropcard + "|" + GameManager.Instance.player.info.number + "|" + parentSlotId+ "|" + targetSlotId;
		Send(msg);
	}
	public void EndTurn() {
		Send(SendNames.endturn + "|" + GameManager.Instance.player.info.number.ToString());
	}

	public void SendCardToDrop(List<string> slotIdArr) {
		string msg = SendNames.cardtodrop + "|" + GameManager.Instance.player.info.number + "|";

		foreach (string slotId in slotIdArr) {
			msg += slotId + "%";
		}

		msg = msg.Trim('%');

		Send(msg);
	}

	private void Send(string message) {
		Debug.Log("Sending: " + message);
		byte[] msg = Encoding.Unicode.GetBytes(message);
		NetworkTransport.Send(hostId, connectionId, reliableChannel, msg, message.Length * sizeof(char), out error);
	}
}
