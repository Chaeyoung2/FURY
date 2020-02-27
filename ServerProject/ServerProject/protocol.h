#pragma once

#define BUFSIZE 128
#define MAX_PLAYER 3

#define SERVERIP   "127.0.0.1"
#define SERVERPORT 9000

#define SC_LOGIN_OK 0
#define SC_PLAYER_POS 1

//////////////////////////////////
enum packet_type {
	sc_login_ok, sc_put_player, sc_notify_playerinfo
};


//////////////////////////////////

struct player_info {
	int id = 999;
	float x;
	float y;
	float z;
};

struct packet_info {
	short size;
	packet_type type;
	DWORD id;
	SOCKET sock;
};

struct object_pos {
	float x;
	float y;
	float z;
};

//////////////////////////////////