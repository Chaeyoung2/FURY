#include <iostream>
#include <WinSock2.h>
#include <stdlib.h>
#include <stdio.h>
#include "protocol.h"

#pragma comment(lib, "ws2_32")

using namespace std;

struct SOCKETINFO {
	WSAOVERLAPPED overlapped;
	WSABUF wsabuf;
	SOCKET sock;
	char buf[BUFSIZE];
	char packet_buf[BUFSIZE];
	player_info playerinfo = {};
	bool connected = false;
	bool is_recv = true;
	int prev_size;
};

////////////////////////////////////////
SOCKETINFO clients[MAX_PLAYER];

///////////////////////////////////////
void CALLBACK recv_callback(DWORD Error, DWORD dataBytes, LPWSAOVERLAPPED overlapped, DWORD lnFlags);

///////////////////////////////////////
void send_packet(char, char*);
void do_recv(char id);

void err_quit(const char* msg);
void err_display(const char* msg);
void show_players();
void process_packet(int id);

//////////////////////////////////////
int main(int argc, char* argv[])
{
	int retval;

	WSADATA WSAData;
	if (WSAStartup(MAKEWORD(2, 2), &WSAData) != 0) {
		cout << ("Error - Can not load 'winsock.dll' file") << endl;
		return 1;
	}

	SOCKET listen_sock = WSASocket(AF_INET, SOCK_STREAM, 0, NULL, 0, WSA_FLAG_OVERLAPPED);
	if (listen_sock == INVALID_SOCKET)
		err_quit("[����] Invalid socket");

	SOCKADDR_IN serveraddr;
	ZeroMemory(&serveraddr, sizeof(serveraddr));
	serveraddr.sin_family = PF_INET;
	serveraddr.sin_addr.s_addr = htonl(INADDR_ANY);
	serveraddr.sin_port = htons(SERVERPORT);

	retval = bind(listen_sock, (SOCKADDR*)&serveraddr, sizeof(serveraddr));
	if (retval == SOCKET_ERROR) {
		closesocket(listen_sock);
		WSACleanup();
		err_quit("[����] bind()");
	}

	// ���� ��⿭ ����
	retval = listen(listen_sock, MAX_PLAYER);
	if (retval == SOCKET_ERROR) {
		closesocket(listen_sock);
		WSACleanup();
		err_quit("[����] listen()");
	}

	SOCKADDR_IN client_addr;
	int addr_length = sizeof(SOCKADDR_IN);
	memset(&client_addr, 0, addr_length);
	SOCKET client_sock;
	DWORD flags;

	int id{ -1 };

	while (1) {
		// accept 
		client_sock = accept(listen_sock, (struct sockaddr*) & client_addr, &addr_length);
		if (client_sock == INVALID_SOCKET) {
			err_display("[����] accept()");
			return 1;
		}

		// ���̵� �ο�.
		int new_id = -1;
		for (int i = 0; i < MAX_PLAYER; ++i) {
			if (false == clients[i].connected) {
				clients[i].connected = true;
				new_id = i;
				break;
			}
		}
		// ������ Ŭ���̾�Ʈ ���� ���
		cout << "[����] Ŭ���̾�Ʈ(" << new_id << ") : IP �ּ� (" << inet_ntoa(client_addr.sin_addr) << "), ��Ʈ ��ȣ (" << ntohs(client_addr.sin_port) << ")" << endl;
		// ������ �ɼ��� ����.
		bool NoDelay = TRUE;
		setsockopt(client_sock, IPPROTO_TCP, TCP_NODELAY, (const char FAR*) & NoDelay, sizeof(NoDelay));

		// Ŭ���̾�Ʈ ����ü�� ���� �Է�.
		{
			clients[new_id] = SOCKETINFO{};
			memset(&clients[new_id], 0x00, sizeof(SOCKETINFO));
			clients[new_id].playerinfo.id = new_id;  
			clients[new_id].sock = client_sock;
			clients[new_id].wsabuf.len = BUFSIZE;
			clients[new_id].wsabuf.buf = clients[new_id].buf;
			clients[new_id].overlapped.hEvent = (HANDLE)clients[new_id].sock;
			clients[new_id].connected = true;
			flags = 0;
		}



		// �α��� �Ǿ��ٴ� ��Ŷ�� ������.
		{
			char buf[BUFSIZE] = "";
			// (����)
			char b = '0';
			char s = '/';
			memcpy(buf, &b, sizeof(char));
			memcpy(buf + sizeof(char), &s, sizeof(char));
			// (����)
			char id = new_id + '0';
			memcpy(buf + sizeof(char) + sizeof(char), &(id), sizeof(char));
			// ����
			send_packet(new_id, buf);
		}

		// new_id�� ������ �ٸ� Ŭ���̾�Ʈ���� �˸��� ��Ŷ�� ������.
		for (int i = 0; i < MAX_PLAYER; ++i) {
			if (!clients[i].connected) continue;  // ����� Ŭ���̾�Ʈ���Ը� ������.
			if (i == new_id) continue;
			char buf[BUFSIZE] = "";
			// (����)
			char b = '1';
			char s = '/';
			memcpy(buf, &b, sizeof(char));
			memcpy(buf + sizeof(char), &s, sizeof(char));
			// (����)
			char id = new_id + '0';
			memcpy(buf + sizeof(char) + sizeof(char), &(id), sizeof(char)); // 1/1 ��Ŷ�� ���� Ŭ���̾�Ʈ�� 0��° Ŭ���̾�Ʈ�� ���̰�, ���ο� 1��° Ŭ���̾�Ʈ�� �����ߴٴ� ���� �˰� �� ���̴�.
			// ����
			send_packet(i, buf);

			////////////////// 0304 ������� ��! ������ Ŭ������ �ٸ� Ŭ�� ������ ���� ���� ���� ����غ�!! // �Ϸ�

		}

		// �ٸ� Ŭ���̾�Ʈ�� ���縦 new_id Ŭ���̾�Ʈ���� �˸��� ��Ŷ�� ������.
		for (int i = 0; i < MAX_PLAYER; ++i) {
			if (false == clients[i].connected) continue;
			if (i == new_id) continue; // �����Դ� ������ �ʴ´�.
			char buf[BUFSIZE] = "";
			// (����)
			char b = '1';
			char s = '/';
			memcpy(buf, &b, sizeof(char));
			memcpy(buf + sizeof(char), &s, sizeof(char));
			// (����)
			char id = i + '0';
			memcpy(buf + sizeof(char) + sizeof(char), &(id), sizeof(char)); // 1/1 ��Ŷ�� ���� Ŭ���̾�Ʈ�� 0��° Ŭ���̾�Ʈ�� ���̰�, ���ο� 1��° Ŭ���̾�Ʈ�� �����ߴٴ� ���� �˰� �� ���̴�.
			// ����
			send_packet(new_id, buf);
		}

		// new_id�� �ٽ� recv ó��
		do_recv(new_id);
		//show_allplayer();
	}
}



void CALLBACK recv_callback(DWORD Error, DWORD dataBytes, LPWSAOVERLAPPED overlapped, DWORD lnFlags)
{
	// �������忡�� ������ �о�´�.
	SOCKETINFO* socketInfo = (struct SOCKETINFO*)overlapped;
	SOCKET client_sock = reinterpret_cast<int>(overlapped->hEvent);

	int id = socketInfo->playerinfo.id;

	if (id > MAX_PLAYER)
		return;

	if (dataBytes == 0) {
		// Ŭ���̾�Ʈ ���� ����.
		closesocket(clients[id].sock);
		clients[id].connected = false;
		return;
	}
	

	// ��ġ ��Ŷ �׽�Ʈ
	//object_pos pos = {};
	//memcpy(&pos, clients[id].buf, sizeof(clients[id].buf));
	//
	//float time = 0;
	//int index = 0;
	//unsigned char b[] = { clients[id].buf[index], clients[id].buf[index + 1], clients[id].buf[index + 2] , clients[id].buf[index + 3] };
	//memcpy(&time, &b, sizeof(float));
	//index += sizeof(float);

	// ��Ŷ ������
	int rest = dataBytes; // ������ ������ ��
	char* ptr = /*socketInfo->buf*/clients[id].buf;
	int packet_size = 0;
	if (0 < clients[id].prev_size) // ���� ������ �����̸�?
		packet_size = /*sizeof(packetinfo) + sizeof(playerinfo)*/BUFSIZE;
	while (0 < rest) { // ������ ������ �� rest�� 0�� �� ������
		if (0 == packet_size)
			packet_size = /*sizeof(packetinfo) + sizeof(playerinfo)*/BUFSIZE;
		int required = packet_size - clients[id].prev_size; // ������ �޾ƾ� �� �� ������
		if (required <= rest) { // �� ������ �����͸�ŭ �޾�����
			memcpy(clients[id].packet_buf + clients[id].prev_size, ptr, required);

			process_packet(id);

			rest -= required;
			ptr += required;
			packet_size = 0;
			clients[id].prev_size = 0;
		}
		else {
			memcpy(clients[id].packet_buf + clients[id].prev_size, ptr, rest);
			rest = 0;
			clients[id].prev_size += rest;
			//do_recv(id);
			//return;
		}

	}
	do_recv(id);
	// ---------------------------------------------
	// ��Ŷ �������� ���� �����ʹ� clients[id].packet_buffer�� ����.
	// ���� �����͸� ������ ���۴� clients[id].buf -> clients[id].packet_buf���� ��
	process_packet(id);

}

void CALLBACK send_callback(DWORD Error, DWORD dataBytes, LPWSAOVERLAPPED overlapped, DWORD lnFlags)
{
	DWORD sendBytes = 0;
	DWORD receiveBytes = 0;
	DWORD flags = 0;

	SOCKETINFO* socketInfo = (struct SOCKETINFO*)overlapped;
	SOCKET client_s = reinterpret_cast<int>(overlapped->hEvent);
	int id = socketInfo->playerinfo.id;

	delete socketInfo;

	if (dataBytes == 0) {
		cout << "[���� ����] " << id << "��° ������ �ݽ��ϴ�." << endl;
		closesocket(clients[id].sock);
		clients[id].connected = false;
		return;
	}

}

void send_packet(char client, char* buf)
{
	SOCKETINFO* socketinfo = new SOCKETINFO;
	socketinfo->playerinfo = clients[client].playerinfo;
	socketinfo->wsabuf.len = BUFSIZE;
	socketinfo->wsabuf.buf = socketinfo->buf;
	memcpy(socketinfo->buf, buf, sizeof(socketinfo->buf));
	ZeroMemory(&(socketinfo->overlapped), sizeof(socketinfo->overlapped));
	DWORD recvBytes = 0;

	if (WSASend(clients[client].sock, &(socketinfo->wsabuf), 1, &recvBytes, 0, &(socketinfo->overlapped),
		send_callback) == SOCKET_ERROR)
	{
		if (WSAGetLastError() != WSA_IO_PENDING)
		{
			printf("Error - Fail WSASend(error_code : %d)\n", WSAGetLastError());
		}
	}
}

void process_packet(int id)
{
	player_info playerinfo;
	int packetId;

	// ���� ���� ��Ŷ.
	memcpy(&packetId, clients[id].packet_buf, sizeof(byte));


	// ���� ���� ��Ŷ.
	switch (packetId) {
	case 0:
		break;
	case 1:
		break;
	case 2:
		break;
	case 3:
		break;
	}

	// show_allplayer();

}


void do_recv(char id)
{
	DWORD flags = 0;

	clients[id].is_recv = true;
	if (WSARecv(clients[id].sock, &clients[id].wsabuf, 1,
		NULL, &flags, &(clients[id].overlapped), recv_callback))
	{
		if (WSAGetLastError() != WSA_IO_PENDING)
		{
			cout << "Error - IO pending Failure\n";
			while (true);
		}
	}
}


void show_players() {

}


// ���� �Լ� ���� ��� �� ����
void err_quit(const char* msg)
{
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	wchar_t text_msg[128];
	mbstowcs(text_msg, msg, strlen(msg) + 1);
	LPCWSTR test = text_msg;
	MessageBox(NULL, (LPCTSTR)lpMsgBuf, test, MB_ICONERROR);
	LocalFree(lpMsgBuf);
	exit(1);
}

// ���� �Լ� ���� ���
void err_display(const char* msg)
{
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	printf("[%s] %s", msg, (char*)lpMsgBuf);
	LocalFree(lpMsgBuf);
}

