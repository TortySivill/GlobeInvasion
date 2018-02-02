/* Client.cs */

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;

public class Client
{
	private GameManager gm;

	private int         connectionId;
	private int         myReliableChannelId;
	private int         socketId;
	private string      hostName;
	private int 		port;
	private bool        disconnectAllowed;

	public Client(string hostName, int port, GameManager gm) {
		this.gm = gm;
		this.hostName = hostName;
		this.port = port;
		disconnectAllowed = false;

		NetworkTransport.Init();
		ConnectionConfig config = new ConnectionConfig();
		myReliableChannelId = config.AddChannel(QosType.Reliable);

		HostTopology topology = new HostTopology(config, 1);
		socketId = NetworkTransport.AddHost(topology, port);

		connectToServer ();
	}

	public void allowDisconnect() {
		disconnectAllowed = true;
	}

	public void identifyAsPhone() {
        packet_identify str_id = new packet_identify (client_type.PHONE);
        byte[] payload = Network.Serialize<packet_identify> (str_id);
        packet_container pack = new packet_container (packet_type.IDENTIFY, payload);
        send(pack);
    }

	public void connectToServer() {
		byte error;
		Debug.Log ("Host: " + hostName + " port: " + port);
		this.connectionId = NetworkTransport.Connect(socketId, hostName, port, 0, out error);
		NetworkError nwError = (NetworkError)error;
		Debug.Log ("Network error: " + nwError);
	}

	public void setServerInfo(string host, int newPort) {
		hostName = host;
		port = newPort;
	}

	//Disconnect from server
	public void disconnect() {
		byte error;
		NetworkTransport.Disconnect (socketId, connectionId, out error);
		NetworkError nwError = (NetworkError)error;
		Debug.Log ("Disconnect error: " + nwError);
	}

	public void sendReady() {
		packet_container pack = new packet_container(packet_type.READY, new byte[1]);
		send(pack);
		Debug.Log("Sent ready packet.");
	}

	public void sendStartGame() {
		packet_container pack = new packet_container(packet_type.START_GAME, new byte[1]);
        send(pack);
        Debug.Log("Sent start game packet.");
    }

	public void sendName() {
		byte[] payload = Network.Serialize<string> (gm.getName ());
		packet_container pack = new packet_container(packet_type.NAME, payload);
		send(pack);
		Debug.Log("Sent name packet.");
	}

	public void sendNameCheck() {
		byte[] payload = Network.Serialize<string>(gm.getName());
		packet_container pack = new packet_container(packet_type.NAMECHECK, payload);
		send(pack);
		Debug.Log("Sent name check packet.");
	}

	public void moveTroops(int[] moveFromIds, int moveToIds, team_id id) {
		packet_move      move    = new packet_move(moveFromIds, moveToIds, id, new int[0]);
		byte[]           payload = Network.Serialize<packet_move>(move);
		packet_container packet  = new packet_container(packet_type.MOVE, payload);

		send<packet_container>(packet);
	}

	public void sendRole(team_id team, role_id role) {
		role_struct r_str        = new role_struct (team, role);
		byte[] payload           = Network.Serialize<role_struct> (r_str);
		packet_container packet  = new packet_container(packet_type.SEND_ROLE, payload);

		send<packet_container>(packet);
	}

	public void freeRole() {
		byte[] payload           = new byte[] {};
		packet_container packet  = new packet_container(packet_type.FREE_ROLE, payload);

		send<packet_container>(packet);
	}

	public void moveCursor(int cursor) {
		packet_cursor c_str      = new packet_cursor (cursor);
		byte[] payload           = Network.Serialize<packet_cursor> (c_str);
		packet_container packet  = new packet_container(packet_type.MOVE_CURSOR, payload);

		send<packet_container>(packet);
	}

	public void buildBridge(bool buttonPressed)
    {
		byte[] payload = Network.Serialize<bool> (buttonPressed);
        packet_container packet = new packet_container(packet_type.BUILD_BRIDGE, payload);
        send<packet_container>(packet);
    }


	//Recieving messages 
	public void tick() {
		int recHostId, recConnectionId, recChannelId, bufferSize, dataSize;
		byte[] recBuffer;
		byte error;

		bufferSize = 1024;
		recBuffer = new byte[bufferSize];

		NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);

		if (recNetworkEvent == NetworkEventType.Nothing) {

		} 

		else if (recNetworkEvent == NetworkEventType.DataEvent) {
			packet_container packet = Network.Deserialize<packet_container> (recBuffer);

			switch (packet.type) {

			case packet_type.SUCCESS_CONNECTION:
				identifyAsPhone ();
				sendName ();
				packet_success sc_str = Network.Deserialize<packet_success> (packet.payload);
				gm.connected (sc_str.successful);
				break;

			case packet_type.INIT:
				packet_init init = Network.Deserialize<packet_init> (packet.payload);
				int[] ids = init.countryIds;
				Color teamColor = new Color (init.colorR, init.colorG, init.colorB);
				gm.setStartingCountries (ids, teamColor);
				break;

			case packet_type.START_GAME:
				Debug.Log ("Game Start");
				gm.startGame ();
				break;


			case packet_type.INIT_STRAT:
				packet_init_strat init_strat = Network.Deserialize<packet_init_strat> (packet.payload);
				Color stratColor = new Color (init_strat.colorR, init_strat.colorG, init_strat.colorB);
				gm.setStratColor (stratColor);
				break;

			case packet_type.CONQUER:
				packet_conquer conquer = Network.Deserialize<packet_conquer> (packet.payload);
				gm.conquerCountry(conquer.countryId);
				break;

			case packet_type.SUCCESS_ROLE:
				packet_success success_str = Network.Deserialize<packet_success> (packet.payload);
				if (success_str.successful)
					ButtonTransitions.toLoad ();
				else
					ButtonTransitions.toStart ();
				break;

			case packet_type.NAMECHECK:
				sendNameCheck();
				break;

            case packet_type.DISPLAYSCORE:
                packet_displayScore display_info = Network.Deserialize<packet_displayScore> (packet.payload);
				Team team2 = GameManager.safeFind<Team> ();
				team2.updateTeamDetails(display_info.score,display_info.teamName);
				break;


			case packet_type.MONEY:
				packet_money money_info = Network.Deserialize<packet_money> (packet.payload);
				Team team = GameManager.safeFind<Team> ();
				team.updateMoney (money_info.money);
				break;

            case packet_type.FEEDBACK:
                    Debug.Log("Feedback packet received");
                    packet_feedback pfeed = Network.Deserialize<packet_feedback>(packet.payload);

                    // Strategist feedback
                    if (!pfeed.valid && pfeed.countryId == -1)
                    {
                        Handheld.Vibrate();
                    } else
                    {
                        gm.playerTeam.feedback(pfeed);
                    }
                    break;
			
			case packet_type.FINISH:
				packet_finish finish = Network.Deserialize<packet_finish> (packet.payload);
				Debug.Log ("Game has ended");
				gm.gameEnded (finish.won, finish.score, finish.topTen);
				break;

			case packet_type.RECONNECT:
				role_struct roleStruct = Network.Deserialize<role_struct>(packet.payload);
				Debug.Log("Received reconnect packet");
				gm.teamId = roleStruct.team_id;
				gm.roleId = roleStruct.role;
				gm.startGame();
				break;

			case packet_type.RESTART:
				Debug.Log("Received restart packet.");
				gm.reset();
				break;

			case packet_type.BRIDGECHECK:
				bool check = Network.Deserialize<bool> (packet.payload);
				GameManager.safeFind<UIMethods> ().setBridgeText (check);
				break;
			}

		} 

		else if (recNetworkEvent == NetworkEventType.DisconnectEvent) {
			if (disconnectAllowed) {
				Debug.Log ("Client Disconnected in an allowed state");
			} else {
				Debug.Log ("[Client " + recConnectionId + "] $ Disconnected.");
				gm.disconnected ();
			}
		}

		else {
			Debug.Log("[Client " + recConnectionId + "] $ UNKNOWN EVENT - " + recNetworkEvent);
		}
	}



	//Send message to server
	public void send<T>(T message)
		where T : struct
	{
		byte error;

		byte[] payload = Network.Serialize<T>(message);
		int    size    = payload.Length;

		NetworkTransport.Send(socketId, connectionId, myReliableChannelId, payload, size, out error);
	}


}
