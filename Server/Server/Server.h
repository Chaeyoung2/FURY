#pragma once
#include <iostream>
#include <map>
#include <vector>
#include <thread>
#include <mutex>
#include <winsock2.h>
#include "../protocol.h"
using namespace std;

#pragma comment(lib, "Ws2_32.lib")

#define MAX_BUFFER 1024

struct OVER_EX {
	WSAOVERLAPPED over;
	WSABUF dataBuffer;
	char messageBuffer[MAX_BUFFER];
	bool is_recv;
};

class SOCKETINFO {
public:
	mutex access_lock;
	bool in_use;
	OVER_EX over_ex;
	SOCKET socket;
	char packet_buffer[MAX_BUFFER];
	int prev_size;
	float x, y, z;
	SOCKETINFO() {
		in_use = false;
		over_ex.dataBuffer.len = MAX_BUFFER;
		over_ex.dataBuffer.buf = over_ex.messageBuffer;
		over_ex.is_recv = true;
		x = y = z = 0;
	}
};

//////////////////////////////////////////

void worker_thread();
//////////////////////////////////////////

void disconnect_client(char id);

void process_packet(char client, char* packet);

//////////////////////////////////////////

void error_display(const char* mess, int err_no);

//////////////////////////////////////////

int do_accept();

void do_recv(char id);

//////////////////////////////////////////

void send_packet(char client, void* packet);

void send_put_player_packet(char client, char new_id);

void send_login_ok_packet(char new_id);
void send_remove_player_packet(char cl, char id);
