using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

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
			//Debug.Log("Receiving: " + msg);

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
					OnNewStage(int.Parse(splitData[1]), (TurnStage)Enum.Parse(typeof(TurnStage), splitData[2]));
					break;

				case SendNames.dropallowed:
					//Debug.Log("Player " + splitData[1] + " drop " + splitData[2] + " card from pos " + splitData[3] + " to: " + splitData[4]);
					Drop(int.Parse(splitData[1]), int.Parse(splitData[2]), int.Parse(splitData[3]), splitData[4]);
					break;

				case SendNames.dropdisallowed:
					//Debug.Log("Drop for card whith id " + int.Parse(splitData[1]) + " disallowes beacose " + splitData[2]);
					DropDisallowed(int.Parse(splitData[1]), splitData[2]);
					break;

				case SendNames.removecard:
					RemoveCard(int.Parse(splitData[1]), splitData[2]);
					break;

				case SendNames.values:
					UpdateValues(int.Parse(splitData[1]), int.Parse(splitData[2]), int.Parse(splitData[3]), int.Parse(splitData[4]), int.Parse(splitData[5]), int.Parse(splitData[6]));
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
	private void RemoveCard(int pNum, string cardSlot) {
		GameManager.Instance.RemoveCard(pNum, cardSlot);
	}

	private void OnNewStage(int turnClientNumber, TurnStage turnStage) {
		// just for now
		// TODO: Remove
		if (turnStage != TurnStage.fight_enemy && turnStage != TurnStage.fight_player)
			GameManager.Instance.warTable.ClearTable();

		GameManager.Instance.turnController.ChangeTurn(turnStage, GameManager.Instance.player.info.number == turnClientNumber);
	}

	private void Drop(int pNum, int cardId, int closId, string targetSlot) {
		GameManager.Instance.Drop(pNum, cardId, closId, targetSlot);
	}
	private void DropDisallowed(int cardId, string reason) {
		GameManager.Instance.DropDisallowed(cardId, reason);
	}

	private void OpenDoor(string[] data) {
		int pNum = int.Parse(data[1]);
		int cardId = int.Parse(data[2]);
		bool isMonster = int.Parse(data[3]) == 1;

		if (isMonster)
			OnNewStage(pNum, TurnStage.fight_player);	
		else
			OnNewStage(pNum, TurnStage.waiting);

		GameManager.Instance.OpenDoor(cardId, isMonster);
	}

	private void OnEndFight(bool playerWin) {
		GameManager.Instance.warTable.ClearTable();
	}

	private void TakeCardFromWT(int pNum) {
		GameManager.Instance.warTable.PlaseCardToHand(pNum);
		GameManager.Instance.turnController.ChangeTurn(TurnStage.after_door, GameManager.Instance.player.info.number == pNum);
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

	// Send
	public void OnDrop(Card card, string targetSlot) {
		string msg = SendNames.trydropcard + "|" + GameManager.Instance.player.info.number + "|" + card.id + "|" + targetSlot;
		Send(msg);
	}
	public void EndTurn() {
		Send(SendNames.endturn + "|" + GameManager.Instance.player.info.number.ToString());
	}

	private void OnAskName(string[] data) {
		ourClientId = int.Parse(data[1]);
		Send(SendNames.nameis + "|" + GameManager.Instance.player.info.playerName);
	}

	private void Send(string message) {
		//Debug.Log("Sending: " + message);
		byte[] msg = Encoding.Unicode.GetBytes(message);
		NetworkTransport.Send(hostId, connectionId, reliableChannel, msg, message.Length * sizeof(char), out error);
	}
}
