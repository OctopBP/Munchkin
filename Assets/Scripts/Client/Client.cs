using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Client : MonoBehaviour {

	public static Client Instance { get; set; }

	private const int MAX_CONNECTION = 2;
	private const string LOCALHOST = "127.0.0.1"; // my ip: "46.242.8.77", and this "192.168.31.1"

	private int port = 5701;

	private int hostId;
	//private int webHostId;

	private int ourClientId;
	private int connectionId;

	private int reliableChannel;
	//private int unreliableChannel;

	private float connectionTime;
	private bool isConnected = false;

	//private bool isStarted = false;
	private byte error;

	private void Awake() {
		Instance = this;

		ClientGM.Instance.cnnWaitGroup.SetActive(false);
	}

	public void Connect() {

		string pName = GameObject.Find("NameInput").GetComponent<TMP_InputField>().text;
		if (pName == "") {
			Debug.Log("Plesae, enter a name");
			return;
		}

		ClientGM.Instance.player.info.name = pName;
		ClientGM.Instance.cnnCnnGroup.SetActive(false);
		ClientGM.Instance.cnnWaitGroup.SetActive(true);

		NetworkTransport.Init();
		ConnectionConfig cc = new ConnectionConfig();

		reliableChannel = cc.AddChannel(QosType.Reliable);
		//unreliableChannel = cc.AddChannel(QosType.Unreliable);

		HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

		hostId = NetworkTransport.AddHost(topo, 0);
		connectionId = NetworkTransport.Connect(hostId, LOCALHOST, port, 0, out error);

		connectionTime = Time.time;
		isConnected = true;
	}

	public void Cancel() {
		ClientGM.Instance.cnnCnnGroup.SetActive(true);
		ClientGM.Instance.cnnWaitGroup.SetActive(false);

		NetworkTransport.Disconnect(hostId, connectionId, out error);

		isConnected = false;
	}

	private void Update() {
		if (!isConnected)
			return;

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
					Debug.Log("Add card " + splitData[3] + " to player " + splitData[1]);
					AddCard(int.Parse(splitData[1]), (Card.DeckType)Enum.Parse(typeof(Card.DeckType), splitData[2]), int.Parse(splitData[3]));
					break;

				case SendNames.opendoor:
					Debug.Log("Open door: " + splitData[1]);
					OpenDoor(splitData);
					break;

				case SendNames.endfigth:
					Debug.Log("End fight: " + splitData[1]);
					OnEndFight(int.Parse(splitData[1]) == 1);
					break;

				case SendNames.takecardfromwt:
					TakeCardFromWT(int.Parse(splitData[1]));
					break;

				case SendNames.newstage:
					Debug.Log("New Stage: " + splitData[2]);
					OnNewStage(int.Parse(splitData[1]), (TurnStage)Enum.Parse(typeof(TurnStage), splitData[2]));
					break;

				case SendNames.dropcard:
					Debug.Log("Player " + splitData[1] + " drop " + splitData[2] + " card from pos " + splitData[3] + " to: " + splitData[4]);
					Drop(int.Parse(splitData[1]), int.Parse(splitData[2]), int.Parse(splitData[3]), splitData[4]);
					break;

				default:
					Debug.Log("Invalid message: " + msg);
					break;
			}
		}
	}

	private void OnAskName(string[] data) {
		ourClientId = int.Parse(data[1]);
		Send(SendNames.nameis + "|" + ClientGM.Instance.player.info.name, reliableChannel);
	}

	private void StartGame(string[] data) {
		for (int i = 1; i <= 2; i++) {
			string[] d = data[i].Split('%');

			int cnnId = int.Parse(d[0]);
			int pNum = int.Parse(d[2]);

			PlayerInfo p = new PlayerInfo {
				name = d[1],
				number = pNum,
				connectionId = cnnId
			};

			if (cnnId == ourClientId) {
				ClientGM.Instance.player.info = p;
				ClientGM.Instance.playerName.text = p.name;
			}
			else {
				ClientGM.Instance.enemy.info = p;
				ClientGM.Instance.enemyName.text = p.name;

			}
		}

		ClientGM.Instance.cnnCanvas.SetActive(false);
	}
	private void PlayerDisconnected(int cnnId) {
		//players.Remove(players.Find(c => c.connectionId == cnnId));
	}

	public void OnDrop(Card card, string targetSlot) {
		string msg = SendNames.trydropcard +"|" + ClientGM.Instance.player.info.number + "|" + card.id + "|" + targetSlot;
		Send(msg, connectionId);
	}
	public void EndTurn() {
		Send(SendNames.endturn + "|" + ClientGM.Instance.player.info.number.ToString(), connectionId);
	}

	private void AddCard(int pNum, Card.DeckType deckType, int cardId) {
		CardInfo cardInfo;
		if (pNum == ClientGM.Instance.player.info.number)
			cardInfo = ClientGM.Instance.CreateCard(cardId);
		else
			cardInfo = ClientGM.Instance.CreateCard(deckType);

		ClientGM.Instance.GetMunchkin(pNum).hand.AddCard(cardInfo);
	}
	private void OnNewStage(int turnClientNumber, TurnStage turnStage) {
		ClientGM.Instance.turnController.ChangeTurn(turnStage, ClientGM.Instance.player.info.number == turnClientNumber);
	}
	private void Drop(int pNum, int cardId, int closId, string targetSlot) {
		ClientGM.Instance.Drop(pNum, cardId, closId, targetSlot);
	}
	private void OpenDoor(string[] data) {
		int pNum = int.Parse(data[1]);
		int cardId = int.Parse(data[2]);
		bool isMonster = int.Parse(data[3]) == 1;

		if (isMonster) {
			//int pDmg = int.Parse(data[4]);	
			//int mDmg = int.Parse(data[5]);	
			OnNewStage(pNum, TurnStage.fight_player);
		}
		else
			OnNewStage(pNum, TurnStage.waiting);
			

		ClientGM.Instance.OpenDoor(cardId, isMonster);
	}
	private void OnEndFight(bool playerWin) {
		ClientGM.Instance.warTable.ClearTable();
	}
	private void TakeCardFromWT(int pNum) {
		ClientGM.Instance.warTable.PlaseCardToHand(pNum);
		ClientGM.Instance.turnController.ChangeTurn(TurnStage.after_door, ClientGM.Instance.player.info.number == pNum);
	}

	private void Send(string message, int channelId) {
		Debug.Log("Sending: " + message);
		byte[] msg = Encoding.Unicode.GetBytes(message);
		NetworkTransport.Send(hostId, connectionId, channelId, msg, message.Length * sizeof(char), out error);
	}
}