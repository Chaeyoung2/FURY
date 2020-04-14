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
		err_quit("[오류] Invalid socket");

	SOCKADDR_IN serveraddr;
	ZeroMemory(&serveraddr, sizeof(serveraddr));
	serveraddr.sin_family = PF_INET;
	serveraddr.sin_addr.s_addr = htonl(INADDR_ANY);
	serveraddr.sin_port = htons(SERVERPORT);

	retval = bind(listen_sock, (SOCKADDR*)&serveraddr, sizeof(serveraddr));
	if (retval == SOCKET_ERROR) {
		closesocket(listen_sock);
		WSACleanup();
		err_quit("[오류] bind()");
	}

	// 수신 대기열 생성
	retval = listen(listen_sock, MAX_PLAYER);
	if (retval == SOCKET_ERROR) {
		closesocket(listen_sock);
		WSACleanup();
		err_quit("[오류] listen()");
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
			err_display("[오류] accept()");
			return 1;
		}

		// 아이디 부여.
		int new_id = -1;
		for (int i = 0; i < MAX_PLAYER; ++i) {
			if (false == clients[i].connected) {
				clients[i].connected = true;
				new_id = i;
				break;
			}
		}
		// 접속한 클라이언트 정보 출력
		cout << "[접속] 클라이언트(" << new_id << ") : IP 주소 (" << inet_ntoa(client_addr.sin_addr) << "), 포트 번호 (" << ntohs(client_addr.sin_port) << ")" << endl;
		// 소켓의 옵션을 변경.
		bool NoDelay = TRUE;
		setsockopt(client_sock, IPPROTO_TCP, TCP_NODELAY, (const char FAR*) & NoDelay, sizeof(NoDelay));

		// 클라이언트 구조체의 정보 입력.
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



		// 로그인 되었다는 패킷을 보낸다.
		{
			char buf[BUFSIZE] = "";
			// (고정)
			char b = '0';
			char s = '/';
			memcpy(buf, &b, sizeof(char));
			memcpy(buf + sizeof(char), &s, sizeof(char));
			// (가변)
			char id = new_id + '0';
			memcpy(buf + sizeof(char) + sizeof(char), &(id), sizeof(char));
			// 전송
			send_packet(new_id, buf);
		}

		// new_id의 접속을 다른 클라이언트에게 알리는 패킷을 보낸다.
		for (int i = 0; i < MAX_PLAYER; ++i) {
			if (!clients[i].connected) continue;  // 연결된 클라이언트에게만 보낸다.
			if (i == new_id) continue;
			char buf[BUFSIZE] = "";
			// (고정)
			char b = '1';
			char s = '/';
			memcpy(buf, &b, sizeof(char));
			memcpy(buf + sizeof(char), &s, sizeof(char));
			// (가변)
			char id = new_id + '0';
			memcpy(buf + sizeof(char) + sizeof(char), &(id), sizeof(char)); // 1/1 패킷을 받은 클라이언트는 0번째 클라이언트일 것이고, 새로운 1번째 클라이언트가 접속했다는 것을 알게 될 것이다.
			// 전송
			send_packet(i, buf);

			////////////////// 0304 여기부터 해! 서버가 클라한테 다른 클라 정보를 어케 보낼 건지 고민해봐!! // 완료

		}

		// 다른 클라이언트의 존재를 new_id 클라이언트에게 알리는 패킷을 보낸다.
		for (int i = 0; i < MAX_PLAYER; ++i) {
			if (false == clients[i].connected) continue;
			if (i == new_id) continue; // 나에게는 보내지 않는다.
			char buf[BUFSIZE] = "";
			// (고정)
			char b = '1';
			char s = '/';
			memcpy(buf, &b, sizeof(char));
			memcpy(buf + sizeof(char), &s, sizeof(char));
			// (가변)
			char id = i + '0';
			memcpy(buf + sizeof(char) + sizeof(char), &(id), sizeof(char)); // 1/1 패킷을 받은 클라이언트는 0번째 클라이언트일 것이고, 새로운 1번째 클라이언트가 접속했다는 것을 알게 될 것이다.
			// 전송
			send_packet(new_id, buf);
		}

		// new_id는 다시 recv 처리
		do_recv(new_id);
		//show_allplayer();
	}
}



void CALLBACK recv_callback(DWORD Error, DWORD dataBytes, LPWSAOVERLAPPED overlapped, DWORD lnFlags)
{
	// 오버랩드에서 소켓을 읽어온다.
	SOCKETINFO* socketInfo = (struct SOCKETINFO*)overlapped;
	SOCKET client_sock = reinterpret_cast<int>(overlapped->hEvent);

	int id = socketInfo->playerinfo.id;

	if (id > MAX_PLAYER)
		return;

	if (dataBytes == 0) {
		// 클라이언트 접속 종료.
		closesocket(clients[id].sock);
		clients[id].connected = false;
		return;
	}
	

	// 위치 패킷 테스트
	//object_pos pos = {};
	//memcpy(&pos, clients[id].buf, sizeof(clients[id].buf));
	//
	//float time = 0;
	//int index = 0;
	//unsigned char b[] = { clients[id].buf[index], clients[id].buf[index + 1], clients[id].buf[index + 2] , clients[id].buf[index + 3] };
	//memcpy(&time, &b, sizeof(float));
	//index += sizeof(float);

	// 패킷 재조립
	int rest = dataBytes; // 도착한 데이터 양
	char* ptr = /*socketInfo->buf*/clients[id].buf;
	int packet_size = 0;
	if (0 < clients[id].prev_size) // 최초 데이터 도착이면?
		packet_size = /*sizeof(packetinfo) + sizeof(playerinfo)*/BUFSIZE;
	while (0 < rest) { // 도착한 데이터 양 rest가 0이 될 때까지
		if (0 == packet_size)
			packet_size = /*sizeof(packetinfo) + sizeof(playerinfo)*/BUFSIZE;
		int required = packet_size - clients[id].prev_size; // 앞으로 받아야 할 총 데이터
		if (required <= rest) { // 총 도착할 데이터만큼 받았으면
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
	// 패킷 재조립이 끝난 데이터는 clients[id].packet_buffer에 있음.
	// 따라서 데이터를 가져올 버퍼는 clients[id].buf -> clients[id].packet_buf여야 함
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
		cout << "[소켓 종료] " << id << "번째 소켓을 닫습니다." << endl;
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

	// 고정 길이 패킷.
	memcpy(&packetId, clients[id].packet_buf, sizeof(byte));


	// 가변 길이 패킷.
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


// 소켓 함수 오류 출력 후 종료
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

// 소켓 함수 오류 출력
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

