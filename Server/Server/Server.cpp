#include "Server.h"

HANDLE g_iocp;
SOCKETINFO clients[MAX_USER];

void worker_thread()
{
	while (true) {

		DWORD io_byte;
		ULONGLONG l_key; // 32bits일 때 안 되넴 !!
		// unsigned long l_key; // 왜 ULONGLONG은 안 되는 거지..?
		OVER_EX* over_ex;

		int is_error = GetQueuedCompletionStatus(g_iocp, &io_byte,
			&l_key, reinterpret_cast<LPWSAOVERLAPPED*>(&over_ex),
			INFINITE);


		char key = static_cast<char>(l_key);

		if (0 == is_error) { // 강제 접속 종료
			int err_no = WSAGetLastError();
			if (64 == err_no) {
				disconnect_client(key);
				continue;
			}
			else // 또다른 심각한 문제.. ?
				error_display("GQCS : ", err_no);
		}
		if (0 == io_byte) // 정상 접속 종료
		{
			disconnect_client(key);
			continue;
		}

		if (true == over_ex->is_recv) {
			// 패킷조립
			int rest = io_byte;
			char* ptr = over_ex->messageBuffer;
			char packet_size = 0;
			if (0 < clients[l_key].prev_size)
				packet_size = clients[key].packet_buffer[0];
			while (0 < rest) {
				if (0 == packet_size) packet_size = ptr[0];
				int required = packet_size - clients[key].prev_size;
				if (required <= rest) {
					memcpy(clients[key].packet_buffer + clients[key].prev_size,
						ptr, required);
					process_packet(key, clients[key].packet_buffer);
					rest -= required;
					ptr += required;
					packet_size = 0;
					clients[key].prev_size = 0;
				}
				else {
					memcpy(clients[key].packet_buffer + clients[key].prev_size,
						ptr, rest);
					rest = 0;
					clients[key].prev_size += rest;
				}
			}
			do_recv(key);
		}
		else {
			// SEND
			// recv 일 때 delete 하면 안 됨.
			if (false == over_ex->is_recv)
				delete over_ex;
		}
	}
}

//////////////////////////////////////////

int main()
{
	vector<thread> worker_threads;
	
	g_iocp = CreateIoCompletionPort(INVALID_HANDLE_VALUE, 0, 0, 0);

	for (int i = 0; i < 4; ++i) {
		worker_threads.emplace_back(thread{ worker_thread });
	}
	thread accept_thread{ do_accept };
	accept_thread.join();
	for (auto& th : worker_threads) th.join();
}

//////////////////////////////////////////

void disconnect_client(char id) {
	// 모든 유저에게 알린다.
	for (int i = 0; i < MAX_USER; ++i) {
		if (false == clients[i].in_use) continue;
		if (i == id) continue;
		send_remove_player_packet(i, id);
	}
	closesocket(clients[id].socket);
	clients[id].in_use = false;
}


void process_packet(char client, char* packet)
{
	//// 패킷이 날라왔다.
	//// 종류에 따라서 처리한다.
	//cs_packet_up* p = reinterpret_cast<cs_packet_up*>(packet);
	//int x = clients[client].x;
	//int y = clients[client].y;
	//switch (p->type) { // packet[0] : size, packet[1] : type
	//	// 여기에서 충돌 처리를 해야 한다.
	//case CS_UP:
	//	if (y > 0)
	//		y--;
	//	break;
	//case CS_DOWN:
	//	if (y < WORLD_HEIGHT - 1)
	//		y++;
	//	break;
	//case CS_LEFT:
	//	if (x > 0)
	//		x--;
	//	break;
	//case CS_RIGHT:
	//	if (x < (WORLD_WIDTH - 1))
	//		x++;
	//	break;
	//default:
	//	wcout << L"오류 : 정의 되지 않은 패킷 도착" << endl;
	//	while (true);
	//	break;
	//}

	//// x, y 업데이트.
	//clients[client].x = x;
	//clients[client].y = y;

	//// 다른 클라이언트에게도 보낸다.
	//for (int i = 0; i < MAX_USER; ++i) {
	//	if (true == clients[i].in_use)
	//		send_pos_packet(i, client);			// 위치가 바뀌었으니 해당 클라이언트에게 보낸다.
	//}

}

//////////////////////////////////////////

void error_display(const char* mess, int err_no) {
	WCHAR* lpMsgBuf; // 한글 에러로 출력하기 위해.
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER |
		FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, err_no, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	cout << mess;
	wcout << L"[에러 " << err_no << L"]" << lpMsgBuf << endl;
	while (true);
	LocalFree(lpMsgBuf);
}

//////////////////////////////////////////

int do_accept()
{
	WSADATA WSAData;
	if (WSAStartup(MAKEWORD(2, 2), &WSAData) != 0) {
		cout << "Error - Can not load 'winsock.dll' file" << endl;
		return 1;
	}

	SOCKET listenSocket = WSASocket(AF_INET, SOCK_STREAM, 0, NULL, 0, WSA_FLAG_OVERLAPPED);

	if (listenSocket == INVALID_SOCKET) {
		cout << "Error - Invalid Socket" << endl;
		return 1;
	}

	SOCKADDR_IN serverAddr;
	memset(&serverAddr, 0, sizeof(SOCKADDR_IN));
	serverAddr.sin_family = PF_INET;
	serverAddr.sin_port = htons(SERVER_PORT);
	serverAddr.sin_addr.S_un.S_addr = htonl(INADDR_ANY);

	if (::bind(listenSocket, (struct sockaddr*) & serverAddr, sizeof(SOCKADDR_IN)) == SOCKET_ERROR) {
		cout << "Error - Fail bind" << endl;
		closesocket(listenSocket);
		WSACleanup();
		return 1;
	}

	if (listen(listenSocket, 5) == SOCKET_ERROR) {
		cout << "Error - Fail listen" << endl;
		closesocket(listenSocket);
		WSACleanup();
		return 1;
	}

	SOCKADDR_IN clientAddr;
	int addrLen = sizeof(SOCKADDR_IN);
	memset(&clientAddr, 0, addrLen);
	SOCKET clientSocket;
	DWORD flags;

	while (1) {
		clientSocket = accept(listenSocket, (struct sockaddr*) & clientAddr, &addrLen);
		if (clientSocket == INVALID_SOCKET) {
			cout << "Error - Accept Failure" << endl;
			return 1;
		}

		int new_id = -1;
		for (int i = 0; i < MAX_USER; ++i) {
			if (false == clients[i].in_use) {
				new_id = i;
				break;
			}
		}


		if (-1 == new_id) {
			cout << "MAX USER overflow" << endl;
			continue;
		}

		clients[new_id].socket = clientSocket;
		clients[new_id].prev_size = 0;
		clients[new_id].x = clients[new_id].y = clients[new_id].z = 0.f;
		ZeroMemory(&clients[new_id].over_ex.over, sizeof(clients[new_id].over_ex.over));
		flags = 0;

		CreateIoCompletionPort(reinterpret_cast<HANDLE>(clientSocket), g_iocp, new_id, 0);

		clients[new_id].in_use = true;

		send_login_ok_packet(new_id);

		for (int i = 0; i < MAX_USER; ++i) {
			if (true == clients[i].in_use) {
				send_put_player_packet(i, new_id);
			}
		}
		for (int i = 0; i < MAX_USER; ++i) {
			if (false == clients[i].in_use) continue;
			if (i == new_id) continue;
			send_put_player_packet(new_id, i);
		}
		do_recv(new_id);
	}

	closesocket(listenSocket);

	WSACleanup();

	return 0;
}

void do_recv(char id)
{
	DWORD flags = 0;

	if (WSARecv(clients[id].socket, &clients[id].over_ex.dataBuffer, 1,
		NULL, &flags, &(clients[id].over_ex.over), 0))
	{
		if (WSAGetLastError() != WSA_IO_PENDING)
		{
			cout << "Error - IO pending Failure\n";
			while (true);
		}
	}
	else {
		cout << "Non Overlapped Recv return.\n";
		while (true);
	}
}

//////////////////////////////////////////

void send_packet(char client, void* packet) // 패킷의 타입을 모르니까.
{
	char* p = reinterpret_cast<char*>(packet);
	OVER_EX* ov = new OVER_EX;
	ov->dataBuffer.len = p[0]; // 패킷의 사이즈.
	ov->dataBuffer.buf = ov->messageBuffer;
	ov->is_recv = false;
	memcpy(ov->messageBuffer, p, p[0]);
	ZeroMemory(&(ov->over), sizeof(ov->over)); // send 할때마다 new 하고, delete 해 주어야.
	int error = WSASend(clients[client].socket, &(ov->dataBuffer), 1, 0/*GQCS에서 콜백을 받아야 하므로*/, 0, &(ov->over), NULL);

	if (0 != error) {
		if (WSAGetLastError() != WSA_IO_PENDING)
		{
			cout << "Error - IO pending Failure\n";
			while (true);
		}
	}
	//else { // send가 overlapped로 도착하지 않았다.
	//	cout << "Non Overlapped Send return.\n";
	//	while (true);
	//}
}

void send_put_player_packet(char client, char new_id) {
	sc_packet_put_player packet;
	packet.id = new_id;
	packet.size = sizeof(packet);
	packet.type = SC_PUT_PLAYER;

	packet.x = clients[new_id].x;
	packet.y = clients[new_id].y;
	packet.z = clients[new_id].z;

	send_packet(client, &packet);
}

void send_login_ok_packet(char new_id) {
	sc_packet_login_ok packet;
	packet.id = new_id;
	packet.size = sizeof(packet);
	packet.type = SC_LOGIN_OK;

	send_packet(new_id, &packet);
}

void send_remove_player_packet(char cl, char id) {
	sc_packet_remove_player packet;
	packet.id = id;
	packet.size = sizeof(packet);
	packet.type = SC_REMOVE_PLAYER;

	send_packet(cl, &packet);
}

////////////////////////////////////////
