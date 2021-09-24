using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


// 마스터(매치 메이킹) 서버와 룸 접속 담당
/// MonoBehaviourPunCallbacks : MonoBehaviour의 기능을 유지한 채,
/// 컴포넌트가 포톤 서비스에 의해 발생하는 콜백(이벤트 및 메시지)도 감지할 수 있게 함
/// MonoBehaviour가 Update()와 Start() 등의 메시지만 감지할 수 있다면,
/// MonoBehvaiourPunCallbacks는 OnConnectedToMaster() 등의 포톤 이벤트 감지 및 대응되는 메서드 자동 실행
/// (PUN 구현 한계상 포톤 전용 이벤트는 override를 사용하여 구현)
public class LobbyManager : MonoBehaviourPunCallbacks 
{
    // 게임 버전
    /// 같은 버전끼리 매칭할 때 사용
    private string gameVersion = "1";
    // 접속 정보
    public Text connectionInfoText;
    public Button joinButton;

    // Start is called before the first frame update
    // 게임 실행과 동시에 마스터 서버 접속
    void Start()
    {
        PhotonNetwork.GameVersion = gameVersion; // 접속에 필요한 게임 버전 정보 설정
        PhotonNetwork.ConnectUsingSettings(); // 설정한 정보로 마스터 서버 접속 시도
        joinButton.interactable = false; // 룸 접속 버튼 잠시 비활성화
        connectionInfoText.text = "마스터 서버에 접속 중..."; // 접속 시도 중임을 텍스트로 표시
    }

    // 마스터 서버 접속 성공 시 자동 실행
    public override void OnConnectedToMaster()
    {
        joinButton.interactable = true; // 룸 접속 버튼 활성화
        connectionInfoText.text = "온라인 : 마스터 서버와 연결됨";
        Debug.Log("OnConnectedToMaster 함수 호출");
    }

    // 마스터 서버 접속 실패 시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        joinButton.interactable = false; // 룸 접속 버튼 비활성화
        connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
        PhotonNetwork.ConnectUsingSettings(); // 서버 재접속 시도
    }


    // 룸 접속 시도
    public void Connect()
    {
        Debug.Log("Connect 함수 호출");
        joinButton.interactable = false; // 중복 접속 시도를 막기 위해 접속 버튼 잠시 비활성화
        if (PhotonNetwork.IsConnected) // 마스터 서버에 접속 중이라면
        {
            // 룸 접속 실행
            connectionInfoText.text = "룸에 접속...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
            PhotonNetwork.ConnectUsingSettings(); // 서버 재접속 시도
        }
    }

    // 빈 방이 없어서 랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "빈 방이 없음, 새로운 방 생성..."; // 접속 상태 표시
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 3 }); // 최대 3명 수용 가능한 빈 방 생성
        
    }

    // 룸 참가에 성공한 경우 자동 실행
    /// CreateRoom()을 사용하여 자신이 룸을 직접 생성하고 참가한 경우에도 실행됨
    public override void OnJoinedRoom()
    {
        connectionInfoText.text = "방 참가 성공";
        // LoadScene 대신 LoadLevel 쓴 이유
        /// LoadScene() : 이전 씬의 모든 게임 오브젝트를 삭제하고 다음 씬을 로드한다.
        /// 1. 로비 씬의 네트워크 정보가 유지되지 않는다. 
        /// 2. 플레이어들이 서로 동기화 없이 각자 "" 씬을 로드하므로 다른 사람의 캐릭터가 보이지 않는다.
        ///  LoadLevel() : 룸의 플레이어들이 '함께' 새로운 무대로 이동하는 메서드
        ///  1. 룸을 생성한 방장 플레이어가 해당 메서드를 실행하면 다른 플레이어들도 PhotonNetwork.LoadLevel()이 실행되어 방장과 같은 씬을 로드
        ///  1.1 방장 플레이어가 아닌 플레이어들이 각자 실행할 수 있지만 다른 플레이어들이 자동으로 같은 씬을 로드하지는 않아서 혼자 있는 상황 발생
        ///  2. 뒤늦게 입장한 플레이어를 위한 별도의 작업을 구현할 필요가 없음
        PhotonNetwork.LoadLevel("TestScene1"); // 모든 룸 참가자가 TestClient1 씬을 로드하게 함
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Connect();
        }
    }
}
