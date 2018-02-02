/* Network.cs */

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections.Generic;

enum packet_type {
	SUCCESS_CONNECTION,
	IDENTIFY,
	INIT,
	INIT_STRAT,
	START_GAME,
	CONQUER,
	MOVE,
	SEND_ROLE,
	SUCCESS_ROLE,
	TROOP_COUNT,
	PLANE,
	FREE_ROLE,
	MOVE_CURSOR,
	CURSOR_TEAM,
	FEEDBACK,
	MONEY,
	FINISH,
	NAME,
	INIT_CURSOR,
	SPARKLES,
	SCORES,
	RECONNECT,
	BUILD_BRIDGE,
	READY,
	COUNTDOWNSTART,
	COUNTDOWNABORT,
	RESTART,
	BRIDGE,
	NAMECHECK,
	DISPLAYSCORE,
	BRIDGECHECK
}

public enum team_id {
	DEFAULT,
	RED,
	BLUE,
	GREEN,
	YELLOW
}

public enum role_id {
	DEFAULT,
	COMMANDER,
	STRATEGIST
}

public enum client_type {
	PHONE,
	PROJECTOR
}

[Serializable]
struct packet_container {
	public packet_type type;
	public byte[]      payload;
	public int         payloadSize;

	public packet_container(packet_type t, byte[] pl) {
		type        = t;
		payload     = pl;
		payloadSize = pl.Length;
	}
}

[Serializable]
struct packet_identify {
	public client_type type;

	public packet_identify(client_type t) {
		type = t;
	}
}

[Serializable]
struct packet_plane {
	public team_id teamId;
	public int countryId;

	public packet_plane(int cId, team_id tId) {
		teamId = tId;
		countryId = cId;
	}
}

[Serializable]
struct packet_troop_count {
	public double[] troopCount;

	public packet_troop_count(double[] tc ) {
		troopCount = tc;
	}
}

[Serializable]
struct packet_success {
	public bool successful;

	public packet_success(bool s) {
		successful = s;
	}
}

[Serializable]
struct packet_init {
	public int[] countryIds;
	public float colorR;
	public float colorG;
	public float colorB;

	public packet_init(int[] ids, Color c) {
		countryIds = ids;
		colorR = c.r;
		colorG = c.g;
		colorB = c.b;
	}
}

[Serializable]
struct packet_init_strat {
	public float colorR;
	public float colorG;
	public float colorB;

	public packet_init_strat(Color c) {
		colorR = c.r;
		colorG = c.g;
		colorB = c.b;
	}
}

[Serializable]
public struct role_struct {
	public team_id team_id;
	public role_id role;

	public role_struct(team_id t, role_id r) {
		team_id = t;
		role = r;
	}
}

[Serializable]
public struct packet_conquer {
	public team_id teamId;
	public int countryId;
	public int remainingTroops;

	public packet_conquer(team_id tId, int cId, int troops) {
		teamId = tId;
		countryId = cId;
		remainingTroops = troops;
	}
}

[Serializable]
struct packet_move {
	public int[] moveFromIds;
	public int   moveToId;
	public team_id teamId;
	public int[] path;

	public packet_move(int[] mf, int mt, team_id id, int[] p) {
		moveFromIds = mf;
		moveToId    = mt;
		teamId = id;
		path = p;
	}
}

[Serializable]
struct packet_cursor {
	public int cursor;

	public packet_cursor(int c) {
		cursor = c;
	}
}

[Serializable]
public struct packet_feedback
{
    public bool valid;
    public int countryId;

    public packet_feedback(bool v, int cId)
    {
        valid = v;
        countryId = cId;
    }
}


[Serializable]
struct packet_money {
	public int money;

	public packet_money(int m) {
		money = m;
	}
}

[Serializable]
struct packet_displayScore
{
    public int score;
    public string teamName;

    public packet_displayScore(int m, string s)
    {
        score = m;
        teamName = s;

    }
}

[Serializable]
struct packet_finish {
	public bool won;
	public int score;
	public List<Finish.playerScore> topTen;

	public packet_finish(bool w, int s, List<Finish.playerScore> ps) {
		won = w;
		score = s;
		topTen = ps;
	}
}

public class Network {

	// Turn an object into a byte array
	public static byte[] Serialize<T>(T data) {
		BinaryFormatter formatter = new BinaryFormatter();
		MemoryStream    stream    = new MemoryStream();

		formatter.Serialize(stream, data);
		return stream.ToArray();
	}

	// Turn a byte array into an object
	public static T Deserialize<T>(byte[] array){
		MemoryStream    stream    = new MemoryStream(array);
		BinaryFormatter formatter = new BinaryFormatter();

		return (T)formatter.Deserialize(stream);
	}

}
