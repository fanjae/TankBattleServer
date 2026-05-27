# TankBattle Dedicated Server
<img width="620" height="420" alt="Image" src="https://github.com/user-attachments/assets/c3d72eef-0628-4cd9-aaa4-61b67081c865" />

> Unity 탱크 배틀 프로젝트의 멀티 플레이를 처리하기 위해 제작한 C# TCP 서버입니다.
> 클라이언트에서 전송한 입력 패킷을 서버에서 처리하고, 서버가 계산한 탱크 상태를 각 클라이언트에 전달하는 구조를 구현하였습니다.

## 프로젝트 개요

| 항목 | 내용 |
|---|---|
| 프로젝트명 | TankBattleServer |
| 개발 기간 | 2026.05.22 ~ 2026.05.25 |
| 개발 인원 | 1명 |
| 개발 환경 | C#, .NET |
| 실행 환경 | Windows Console |
| IDE | Visual Studio 2022 |

## 관련 저장소

| 구분 | 링크 |
|---|---|
| Unity Client | [TankBattle](https://github.com/fanjae/TankBattle) |
| C# TCP Server | [TankBattleServer](https://github.com/fanjae/TankBattleServer) |

## 구현 기능
<img width="1300" height="200" alt="Image" src="https://github.com/user-attachments/assets/bdee0fa7-faa0-40f5-89e2-dee2178ef56a" />
<img width="1300" height="200" alt="Image" src="https://github.com/user-attachments/assets/6f5f28ba-f2fb-4c55-a0b1-aed89fe1a4ff" />

- 비동기 TCP 서버 실행
- 클라이언트 접속 처리
- PlayerId 부여
- 클라이언트 입력 패킷 수신
- 서버 기준 탱크 상태 계산
- 탱크 위치, 회전, 터렛, 포신 상태 갱신
- 서버 상태 패킷 브로드캐스트
- Length-Prefix 기반 패킷 송수신
- JSON 기반 패킷 직렬화 / 역직렬화

## 구조 설계
- 서버는 클라이언트가 보낸 입력값을 기준으로 탱크 상태를 계산하도록 구성하였습니다.
- 클라이언트는 이동, 회전, 터렛, 포신, 발사 입력을 서버로 전송하고, 서버는 해당 입력을 기반으로 탱크의 위치와 회전 상태를 갱신합니다.
- TCP는 메시지 단위가 아니라 바이트 스트림 단위로 데이터를 주고받기 때문에, 패킷 경계를 구분하기 위해 Length-Prefix 방식을 사용하였습니다.

## 주요 클래스
- `Program` : 서버 실행 진입점
- `GameServer` : 클라이언트 접속 및 서버 루프 관리
- `ClientSession` : 클라이언트별 연결, 송수신 처리
- `ServerTank` : 서버에서 관리하는 탱크 상태와 이동 계산
- `InputPacket` : 클라이언트 입력 데이터
- `TankState` : 클라이언트에 전달할 탱크 상태 데이터

## 실행 방법
1. `TankBattleServer` 프로젝트를 실행합니다.
2. 서버가 클라이언트 연결 대기 상태인지 확인합니다.
3. Unity 클라이언트를 실행하여 서버에 접속합니다.

## 개선 예정 사항
- 서버 기준 포탄 생성 및 충돌 판정
- 체력 및 승패 처리
- 클라이언트 접속 종료 처리 보완
- 패킷 타입 관리 구조 개선
- 클라이언트 보간을 고려한 상태 전송 구조 개선

## 개발 기록
- [TankBattle 개발 과정 정리](https://fanjae.tistory.com/251)

