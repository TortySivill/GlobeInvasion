/* Server.cs */

/*
 * Game server implemented using the Unity Transport Layer API
 * https://docs.unity3d.com/Manual/UNetUsingTransport.html
 * https://docs.unity3d.com/ScriptReference/Networking.NetworkTransport.html
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Server {
    private       GameManager gm;

    private const int         BUF_SIZE  = 1024;

    private       int         channelId;
    private       int         hostId;
    private       int         nClients;
	private       bool        serverReadyToStart;
	private       int         readyClients;

	private List<int> unassignedClients = new List<int>();
	private List<int> disconnectedClients = new List<int>();

    public Server(int port, int nClients, GameManager gm) {
        this.nClients = nClients;
        this.gm       = gm;
		readyClients  = 0;
		serverReadyToStart = false;
        
        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();
        channelId = config.AddChannel(QosType.Reliable);

        HostTopology topology = new HostTopology(config, nClients);
        hostId = NetworkTransport.AddHost(topology, port);

        Debug.Log("[Server] Listening for " + nClients + " clients on port " + port);
    }

	public void addUnsassignedClient(int i) {
		if(!disconnectedClients.Contains(i))
			unassignedClients.Add (i);
	}

	public void setServerReady() {
		serverReadyToStart = true;
	}

	void addReadyClient() {
		readyClients++;	
		if (readyClients == gm.projectorConnectionIds.Count + gm.connectionMap.Count) {
			Debug.Log ("All clients ready");
			gm.startGameScene ();
		}
	}

    // Disconnect all clients and shut down the server
    public void close() {
        byte error;
        
        for (int cc = 0; cc < nClients; cc++) {
            NetworkTransport.Disconnect(hostId, cc, out error);
        }

        NetworkTransport.RemoveHost(hostId);
    }

	// Give connecting players a starting team
	public void initializeCommanderConnection(int connectionId, Team team) {
		List<Country> countries  = team.getCountries();
		int[]         countryIds = new int[countries.Count];

		for (int ii = 0; ii < countries.Count; ii++) {
			countryIds [ii] = countries [ii].getId ();
		}

		packet_init      init    = new packet_init(countryIds, team.getColor());
		byte[]           payload = Network.Serialize<packet_init>(init);
		packet_container packet  = new packet_container(packet_type.INIT, payload);

		send(connectionId, packet);
		Debug.Log ("Commander Sent");
	}

	public void initializeStrategistConnection(int connectionId, Team team) {
		packet_init_strat init_strat = new packet_init_strat (team.getColor());
		byte[] payload = Network.Serialize<packet_init_strat> (init_strat);
		packet_container packet = new packet_container (packet_type.INIT_STRAT, payload);
		send(connectionId, packet);
		Debug.Log ("Strategist Sent");
	}

	public bool chooseRole(role_struct t_str, int connection_id) {
		if (!gm.connectionMap.ContainsValue (t_str)) {
			gm.assignToTeam (t_str, connection_id);
			return true;
		}
		return false;
	}

	public void sendStartGame(int connection_id) {
		packet_container pack = new packet_container(packet_type.START_GAME, new byte[1]);
		send(connection_id, pack);
	}

	public void sendSuccessfulRole(bool b, int connection_id) {
		packet_success s_str = new packet_success(b);
		byte[] payload = Network.Serialize<packet_success> (s_str);
		packet_container pack = new packet_container (packet_type.SUCCESS_ROLE, payload);
		send(connection_id, pack);
	}

	public void sendSuccessfulConnection(bool b, int connectionId) {
		packet_success s_str = new packet_success(b);
		byte[] payload = Network.Serialize<packet_success> (s_str);
		packet_container pack = new packet_container (packet_type.SUCCESS_CONNECTION, payload);
		send(connectionId, pack);
	}

	public void initializeNewClient(int connectionId) {
		if(!gm.connectionMap.ContainsKey(connectionId))
			unassignedClients.Add (connectionId);
		Debug.Log ("Success sent");
	}

	public void sendNameCheck(int connection_id) {
		packet_container pack = new packet_container(packet_type.NAMECHECK, new byte[1]);
		send(connection_id, pack);
	}

	public void sendReconnectInformation(int connectionId) {
		role_struct rs = gm.connectionMap [connectionId];
		byte[] payload = Network.Serialize<role_struct> (rs);
		packet_container pack = new packet_container (packet_type.RECONNECT, payload);
		send(connectionId, pack);
	}

	public void sendTroopCounts(double[] troopCounts, int connection_id) {
		packet_troop_count p_troop = new packet_troop_count(troopCounts);
		byte[] payload = Network.Serialize<packet_troop_count>(p_troop);
		packet_container pack = new packet_container(packet_type.TROOP_COUNT, payload);
		send(connection_id, pack);
	}

    // Country 'countryId' is now owned by Team 'teamid' and has troop count 'remainingTroops'
    public void conquerCountry(int countryId, team_id teamId, int connectionId, int remainingTroops) {
        packet_conquer   conquer = new packet_conquer(teamId, countryId, remainingTroops);
        byte[]           payload = Network.Serialize<packet_conquer>(conquer);

        packet_container packet  = new packet_container(packet_type.CONQUER, payload);
        send(connectionId, packet);
    }

	public void sendBridgeToProjectors(team_id team, int countryId, int level) {
		Debug.Log("Sending bridge to projectors");
		packet_bridge b_str = new packet_bridge(team, countryId, level);
		byte[] payload = Network.Serialize<packet_bridge>(b_str);
		packet_container pack = new packet_container(packet_type.BRIDGE, payload);
		
		foreach (int i in gm.projectorConnectionIds)
			send(i, pack);
	}

	public void sendMovementToProjectors(int[] fromIds, int toId, team_id teamId, List<Country> path) {
		int[] pathIds = new int[path.Count];
		for ( int ii = 0; ii < path.Count; ii++) {
			pathIds [ii] = path [ii].getId ();
		}
		//Debug.Log ("Send Tank Movement to projectors");
		packet_move p_mov = new packet_move (fromIds, toId, teamId, pathIds);
		byte[]           payload = Network.Serialize<packet_move>(p_mov);
		packet_container packet  = new packet_container(packet_type.MOVE, payload);

		foreach (int i in gm.projectorConnectionIds) {
			send(i, packet);
		}
	}

	public void sendPlaneToProjectors(Country c, Team t) {
		packet_plane pl_pack = new packet_plane (c.id, t.team_col);
		byte[] payload = Network.Serialize<packet_plane> (pl_pack);
		packet_container packet  = new packet_container(packet_type.PLANE, payload);

		foreach (int i in gm.projectorConnectionIds) {
			send(i, packet);
		}
	}

	public void sendSparklesToProjectors(Country c) {
		packet_sparkles sp_pack = new packet_sparkles (c.getId ());
		byte[] payload = Network.Serialize<packet_sparkles> (sp_pack);
		packet_container packet = new packet_container (packet_type.SPARKLES, payload);

		foreach (int i in gm.projectorConnectionIds) {
			send(i, packet);
		}
	}

	public void initializeCursorOnProjectors(Team team, Country country) {
		packet_cursor_team cursorData = new packet_cursor_team(team.team_col, country.id);
		byte[] payload = Network.Serialize<packet_cursor_team>(cursorData);
		packet_container packet = new packet_container(packet_type.INIT_CURSOR, payload);

		foreach (int i in gm.projectorConnectionIds) {
			send(i, packet);
		}
	}

	public void sendCountdownStart() {
		Debug.Log("Sending countdown start to projectors.");
		packet_container pack = new packet_container(packet_type.COUNTDOWNSTART, new byte[1]);
		foreach (int i in gm.projectorConnectionIds)
			send(i, pack);
	}

	public void sendCountdownAbort() {
		Debug.Log("Sending count abort to projectors.");
		packet_container pack = new packet_container(packet_type.COUNTDOWNABORT, new byte[1]);
		foreach (int i in gm.projectorConnectionIds)
			send(i, pack);
	}

	public void sendCursorToProjectors(Team team, Country target) {
		packet_cursor_team cursorData = new packet_cursor_team(team.team_col, target.id);
		byte[] payload = Network.Serialize<packet_cursor_team>(cursorData);
		packet_container packet = new packet_container(packet_type.CURSOR_TEAM, payload);

		foreach (int i in gm.projectorConnectionIds) {
			send(i, packet);
		}
	}

	public void sendFreedRoleToProjectors(role_struct rs) {
		byte[] payload = Network.Serialize<role_struct> (rs);
		packet_container packet = new packet_container(packet_type.FREE_ROLE, payload);


		foreach (int i in gm.projectorConnectionIds) {
			send(i, packet);
		}
	}

	public void sendRoleReadyToProjectors(role_struct rs) {
		byte[] payload = Network.Serialize<role_struct> (rs);
		packet_container packet = new packet_container(packet_type.READY, payload);

		foreach (int i in gm.projectorConnectionIds) {
			send(i, packet);
		}
	}

	public void sendScoresToProjector(int[] scores, team_id winner, string[] names) {
		Debug.Log ("Send scores to Projectors");
		packet_scores sc = new packet_scores (scores, winner, names);
		byte[] payload = Network.Serialize<packet_scores> (sc);
		packet_container packet = new packet_container(packet_type.SCORES, payload);

		foreach (int i in gm.projectorConnectionIds) {
			send(i, packet);
		}
	}

	public void declineClient(int connection_id) {
		sendSuccessfulConnection (false, connection_id);
	}

	public void sendNotConnectedInTime() {
		foreach(int i in unassignedClients) {
			Debug.Log ("Client has not connected in time");
			declineClient(i);
		}
	}

	public void sendMoney(int connection_id, int money) {
		packet_money stratData = new packet_money (money);
		byte[] payload = Network.Serialize<packet_money> (stratData);
		packet_container packet = new packet_container (packet_type.MONEY, payload);
		send(connection_id, packet);
	}

    public void sendDisplayScore(int connection_id,int score, string teamName)
    {
        packet_displayScore display_score = new packet_displayScore(score, teamName);
        byte[] payload = Network.Serialize<packet_displayScore>(display_score);
        packet_container packet = new packet_container(packet_type.DISPLAYSCORE, payload);
        send(connection_id, packet);
    }



    public void sendCommanderFeedback(int connection_id, bool valid, int countryId)
    {
        packet_feedback pf = new packet_feedback(valid, countryId);
        byte[] payload = Network.Serialize<packet_feedback>(pf);
        packet_container pack = new packet_container(packet_type.FEEDBACK, payload);
        send(connection_id, pack);
    }

	public void sendInvalidStratMovement(int connection_id) {
        packet_feedback pf = new packet_feedback(false, -1);
        byte[] payload = Network.Serialize<packet_feedback>(pf);
        packet_container pack = new packet_container(packet_type.FEEDBACK, payload);
        send(connection_id, pack);
	}

	public void sendTopTen(Team team, bool won, int score, List<Finish.playerScore> topTen) {
		Debug.Log ("Send TopTen to " + team.team_col + " with score " + score);
		packet_finish fin_pack = new packet_finish (won, score, topTen);
		byte[] payload = Network.Serialize<packet_finish> (fin_pack);
		packet_container packet = new packet_container (packet_type.FINISH, payload);
		if(gm.getCommander(team) != -1)
			send(gm.getCommander(team), packet);
		if(gm.getStrategist(team) != -1)
			send(gm.getStrategist(team), packet);
	}

	public void sendBridgeCheck(int connectionId, bool possible) {
		byte[] payload = Network.Serialize<bool> (possible);
		packet_container pack = new packet_container(packet_type.BRIDGECHECK, payload);
		send(connectionId, pack);
	}

	public void sendRestartGame() {
		packet_container pack = new packet_container(packet_type.RESTART, new byte[1]);
		foreach (int i in gm.projectorConnectionIds) {
			send(i, pack);
		}
		foreach (int i in gm.connectionMap.Keys) {
			if (!disconnectedClients.Contains (i))
				send (i, pack);
		}
	}

	public void reset() {
		readyClients = 0;
		serverReadyToStart = false;
		unassignedClients.Clear ();
	}

    public void tick () {
        GameManager gm = GameManager.safeFind<GameManager> ();
        byte[] buffer = new byte[BUF_SIZE];

        int recvHostId, recvConnectionId, recvChannelId, recvDataSize;
        byte error;
        
        NetworkEventType recvEvent = NetworkTransport.Receive(out recvHostId, out recvConnectionId, out recvChannelId, buffer, BUF_SIZE, out recvDataSize, out error);

		if (recvEvent == NetworkEventType.Nothing) {

		} else if (recvEvent == NetworkEventType.ConnectEvent) {
			Debug.Log ("[Client " + recvConnectionId + "] $ Connected.");
			if (disconnectedClients.Contains (recvConnectionId))
				disconnectedClients.Remove (recvConnectionId);
			
			// If the game is not running, allow connection
			if (!gm.getGameRunning()) {
				if (!gm.getGameEnded ()) {
					sendSuccessfulConnection (true, recvConnectionId);
				} else {
					gm.reconnectAfterGameEnded (recvConnectionId);
				}
			} else {
				sendNameCheck(recvConnectionId);
			}
		}

        else if (recvEvent == NetworkEventType.DataEvent) {
            packet_container rcvMsg = Network.Deserialize<packet_container>(buffer);

            switch(rcvMsg.type) {

				// Client name after reconnect
				case packet_type.NAMECHECK:
					string name = Network.Deserialize<string>(rcvMsg.payload);
					gm.reconnectOnRunningGame(recvConnectionId, name);
					break;

				// Client identifies themself
				case packet_type.IDENTIFY:
					// Get identity
					packet_identify identity = Network.Deserialize<packet_identify>(rcvMsg.payload);
					Debug.Log(recvConnectionId + " identifies as a " + identity.type);


					if (identity.type == client_type.PHONE) {
						initializeNewClient (recvConnectionId);
					} else if (identity.type == client_type.PROJECTOR) {
						gm.projectorConnectionIds.Add(recvConnectionId);
					}

					break;

			case packet_type.MOVE:
				Debug.Log (gm.connectionMap [recvConnectionId].team_id + " wants to move troops!");
				packet_move move_str = Network.Deserialize<packet_move> (rcvMsg.payload);
				Team mov_team = gm.getTeamById (gm.connectionMap [recvConnectionId].team_id);
                mov_team.moveTroops(move_str.moveFromIds, move_str.moveToId);
                break;

				case packet_type.SEND_ROLE:
					Debug.Log ("Client sent a role");
					role_struct r_str = Network.Deserialize<role_struct> (rcvMsg.payload);
					if (chooseRole (r_str, recvConnectionId)) {
						unassignedClients.Remove (recvConnectionId);
						sendSuccessfulRole (true, recvConnectionId);
						foreach (int ii in gm.projectorConnectionIds) {
							send (ii, rcvMsg);
						}
					} else {
						sendSuccessfulRole (false, recvConnectionId);
					}
					break;

			case packet_type.START_GAME:
				if (!gm.getGameRunning ()) {
					Debug.Log ("Client " + recvConnectionId + " is ready to start");
					addReadyClient ();
				} else {
					Team team = gm.getTeamById (gm.connectionMap [recvConnectionId].team_id);
					role_id role = gm.connectionMap [recvConnectionId].role;
					if(role == role_id.COMMANDER)
						initializeCommanderConnection(recvConnectionId, team);
					if(role == role_id.STRATEGIST)
						initializeStrategistConnection(recvConnectionId, team);
				}
				break;

			case packet_type.FREE_ROLE:
				Debug.Log ("Client freed his role");
				gm.freeRoleOfClient (recvConnectionId);
				unassignedClients.Add (recvConnectionId);
				break;                
			
			case packet_type.MOVE_CURSOR:
				if (!gm.getGameRunning ())
					break;
				packet_cursor c_str = Network.Deserialize<packet_cursor> (rcvMsg.payload);
				Team cursor_team = gm.getTeamById (gm.connectionMap [recvConnectionId].team_id);
				cursor_team.moveCursor (c_str.cursor);
				break;

			case packet_type.BUILD_BRIDGE:
				Debug.Log ("build bridge called in network!");
				if (!gm.getGameRunning ())
					break;
                Debug.Log(gm.connectionMap[recvConnectionId].team_id + " is building a bridge ");
                Team bridge_team = gm.getTeamById(gm.connectionMap[recvConnectionId].team_id);
                bridge_team.buildBridge();
                break;

			case packet_type.NAME:
				string newName = Network.Deserialize<string> (rcvMsg.payload);
				gm.insertName (recvConnectionId, newName);
				Debug.Log ("Client " + recvConnectionId + " calls himself " + newName);
				break;

			case packet_type.READY:
				role_struct myRole = gm.connectionMap [recvConnectionId];
				sendRoleReadyToProjectors (myRole);
				gm.roleReady (myRole);
				break;
            }
        }

		else if (recvEvent == NetworkEventType.DisconnectEvent) {
			Debug.Log ("[Client " + recvConnectionId + "] $ Disconnected.");
			disconnectedClients.Add (recvConnectionId);
			if (!gm.getGameRunning() && !gm.getGameEnded()) {
				if(unassignedClients.Contains(recvConnectionId))
					unassignedClients.Remove (recvConnectionId);
				if (!gm.connectionMap.ContainsKey (recvConnectionId))
					return;
				if (serverReadyToStart) {
					Debug.Log ("Client disconnected while game loads - start anyway");
					addReadyClient ();
				} else {
					gm.freeRoleOfClient (recvConnectionId);
				}
			}
		}

        else {
            Debug.Log("[Client " + recvConnectionId + "] $ UNKNOWN EVENT - " + recvEvent);
        }
    }

	public int send<T>(int connectionId, T obj) 
		where T : struct
	{
		byte error;

		byte[] data = Network.Serialize<T>(obj);
		NetworkTransport.Send(hostId, connectionId, channelId, data, data.Length, out error);

		return 0;
	}
}
