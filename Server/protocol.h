#pragma once

constexpr int MAX_USER = 10;
constexpr int SERVER_PORT = 3500;


constexpr int SC_LOGIN_OK = 1;
constexpr int SC_PUT_PLAYER = 2;
constexpr int SC_REMOVE_PLAYER = 3;
constexpr int SC_MOVE_PLAYER = 4;

struct sc_packet_login_ok {
	char size;
	char type;
	char id;
};

struct sc_packet_put_player {
	char size;
	char type;
	char id;
	char x, y, z;
};


struct sc_packet_remove_player {
	char size;
	char type;
	char id;
};

///////////////////////////////

struct Vec3 {
	char x;
	char y;
	char z;
};

