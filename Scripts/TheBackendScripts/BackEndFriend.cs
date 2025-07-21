using UnityEngine;
using System;
using System.Collections.Generic;
using BackEnd;

public class BackEndFriend : Singleton<BackEndFriend>
{
    [SerializeField] private FriendSentRequestPage sentRequestPage;
    [SerializeField] private FriendReceivedRequestPage receivedRequestPage;
    [SerializeField] private FriendPage friendPage;

    private void Start()
    {
        if (friendPage == null)
            friendPage = FindAnyObjectByType<FriendPage>();

        if (sentRequestPage == null)
            sentRequestPage = FindAnyObjectByType<FriendSentRequestPage>();

        if (receivedRequestPage == null)
            receivedRequestPage = FindAnyObjectByType<FriendReceivedRequestPage>();
    }

    private string GetUserInfoBy(string nickname)
    {
        // 해당 닉네임의 유저가 존재하는지 여부는 동기로 진행
        var bro = Backend.Social.GetUserInfoByNickName(nickname);
        string inDate = string.Empty;

        if (!bro.IsSuccess())
        {
            Debug.LogError($"유저 검색 도중 에러가 발생했습니다. :{bro}");
            return inDate;
        }

        // JSON 데이터 피싱 성공
        try
        {
            LitJson.JsonData jsonData = bro.GetFlattenJSON()["row"];

            // 받아온 데이터의 개수가 0이면 데이터가 없는 것
            if (jsonData == null || jsonData.Count <= 0)
            {
                Debug.LogError("유저의 inDate 데이터가 없습니다.");
                return inDate;
            }

            inDate = jsonData["inDate"].ToString();

            Debug.Log($"{nickname}의 inDate 값은 {inDate} 입니다.");
        }
        // JSON 데이터 파싱 실패
        catch (Exception e)
        {
            // try-catch 에러 출력
            Debug.LogError(e);
        }

        return inDate;
    }

    public void SendRequestFriend(string nickname)
    {
        // RequestFriend() 메소드를 이용해 친구 추가 요청을 할 때 해당 친구의 inDate 정보가 필요
        string inDate = GetUserInfoBy(nickname);
        if (string.IsNullOrEmpty(inDate))
            return;

        // 먼저 친구 요청 대기 목록을 조회해서, 이미 보낸 요청이 있는지 중복 여부를 확인
        Backend.Friend.GetSentRequestList(sentCallback =>
        {
            // 1. 요청 결과가 실패일 경우 로그 출력하고 종료
            if (!sentCallback.IsSuccess())
            {
                Debug.LogError($"친구 요청 대기 목록 조회 도중 에러가 발생했습니다. : {sentCallback}");
                return;
            }

            // 2. 요청 결과에서 대기 중인 친구 목록(rows) 추출
            var rows = sentCallback.GetFlattenJSON()["rows"];

            // 3. 대기 중인 친구들의 inDate(식별자)를 저장할 집합 생성 (중복 방지용)
            var sentInDates = new HashSet<string>();

            // 4. 응답 데이터에서 각 친구의 inDate 값을 추출하여 HashSet에 추가
            foreach (LitJson.JsonData item in rows)
                sentInDates.Add(item["inDate"].ToString());

            // 5. 현재 요청하려는 inDate가 이미 존재하는지 검사 (중복 여부 판단)
            if (sentInDates.Contains(inDate))
            {
                Debug.LogWarning($"{nickname}님께 이미 친구 요청을 보냈습니다.");
                return; // 중복된 경우 요청하지 않고 종료
            }

            // 6. 중복되지 않은 경우, 실제로 친구 요청을 보냄
            Backend.Friend.RequestFriend(inDate, callback =>
            {
                // 6-1. 요청 결과가 실패한 경우 로그 출력
                if (!callback.IsSuccess())
                {
                    Debug.LogError($"{nickname} 친구 요청 도중 에러가 발생했습니다. : {callback}");
                    return;
                }

                // 6-2. 요청이 성공한 경우 성공 로그 출력
                Debug.Log($"친구 요청에 성공했습니다. : {callback}");

                // 6-3. 요청이 성공했으므로, UI 등을 갱신하기 위해 최신 친구 요청 대기 목록을 다시 불러옴
                GetSentRequestList();
            });
        });
    }

    public void GetSentRequestList()
    {
        Backend.Friend.GetSentRequestList(callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError($"친구 요청 대기 목록 조회 도중 에러가 발생했습니다. : {callback}");
                return;
            }

            // JSON 데이터 파싱 성공
            try
            {
                LitJson.JsonData jsonData = callback.GetFlattenJSON()["rows"];

                // 받아온 데이터의 개수가 0이면 데이터가 없는 것
                if (jsonData == null || jsonData.Count <= 0)
                {
                    Debug.LogWarning("친구 요청 대기 목록 데이터가 없습니다.");
                    return;
                }

                // 친구 요청 대기 목록에 있는 모든 UI 비활성화
                sentRequestPage.DeactivateAll();

                foreach (LitJson.JsonData item in jsonData) // JSON 배열 데이터를 순회
                {
                    FriendData friendData = new FriendData(); // 각 친구 정보를 저장할 객체 생성

                    // 닉네임 항목이 존재하면 값 저장, 없으면 "NONAME"으로 대체
                    friendData.nickname = item.ContainsKey("nickname") ? item["nickname"].ToString() : "NONAME";

                    // 고유 ID 값 (BackEnd에서 발급된 inDate 값) 저장
                    friendData.inDate = item["inDate"].ToString();

                    // 친구 요청 생성 시간 저장 (ISO8601 또는 yyyy-MM-ddTHH:mm:ss 형태로 저장됨)
                    friendData.createdAt = item["createdAt"].ToString();

                    // 친구 요청 시간이 오래되었는지 확인 (만료 시 자동 취소)
                    if (IsExpirationDate(friendData.createdAt))
                    {
                        RevokeSentRequest(friendData.inDate); // 요청 취소 처리 (서버로 전송)
                        continue; // 이후 UI 처리 생략하고 다음 친구로 넘어감
                    }

                    // 친구 요청이 유효할 경우, 해당 정보를 UI에 활성화하여 표시
                    sentRequestPage.Activate(friendData);
                }
            }
            // JSON 데이터 파싱 실패
            catch (Exception e)
            {
                // try - catch 에러 출력
                Debug.LogError(e);
            }
        });
    }

    public void RevokeSentRequest(string inDate)
    {
        Backend.Friend.RevokeSentRequest(inDate, callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError($"친구 요청 취소 도중 에러가 발생했습니다. : {callback}");
                return;
            }

            Debug.Log($"친구 요청 취소에 성공했습니다. : {callback}");

            // 친구 요청 취소 후 목록 갱신
            GetSentRequestList();
        });
    }

    public void GetReceivedRequestList()
    {
        Backend.Friend.GetReceivedRequestList(callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError($"친구 수락 대기 목록 조회 도중 에러가 발생했습니다. : {callback}");
                return;
            }

            // JSON 데이터 파싱 성공
            try
            {
                LitJson.JsonData jsonData = callback.GetFlattenJSON()["rows"];

                // 받아온 데이터의 개수가 0이면 데이터가 없는것
                if (jsonData == null || jsonData.Count <= 0)
                {
                    Debug.LogWarning("친구 수락 대기 목록 데이터가 없습니다.");
                    return;
                }

                receivedRequestPage.DeactivateAll();

                // jsonData는 BackEnd 서버에서 받은 친구 요청 목록 (JsonData 배열)

                // jsonData의 각 항목을 순회 (LitJson.JsonData 타입 사용)
                foreach (LitJson.JsonData item in jsonData)
                {
                    // 새로운 FriendData 객체 생성 (닉네임, inDate, createdAt 등 정보를 저장할 용도)
                    FriendData friendData = new FriendData();

                    // 닉네임이 존재하면 해당 값을 저장, 없으면 "NONAME"으로 대체
                    friendData.nickname = item.ContainsKey("nickname") ? item["nickname"].ToString() : "NONAME";

                    // 요청을 보낸 유저의 고유 식별값 (BackEnd에서 inDate는 유저 고유값)
                    friendData.inDate = item["inDate"].ToString();

                    // 친구 요청이 생성된 시간 (ISO 8601 형식 문자열, 예: "2025-07-19T14:52:00")
                    friendData.createdAt = item["createdAt"].ToString();

                    // 친구 요청이 만료되었는지 판단 (createdAt으로부터 3일 이상 지났는지)
                    if (IsExpirationDate(friendData.createdAt))
                    {
                        // 만료된 요청은 자동으로 거절 처리
                        RejectFriend(friendData);

                        // 다음 친구 요청으로 넘어감 (이 친구 요청은 UI에 표시하지 않음)
                        continue;
                    }

                    // 유효한 친구 요청은 UI에 표시 (친구 요청 받은 탭에 등록)
                    receivedRequestPage.Activate(friendData);
                }
            }
            // JSON 데이터 파싱 실패
            catch (Exception e)
            {
                // try-catch 에러 출력
                Debug.LogError(e);
            }
        });
    }

    public void AcceptFriend(FriendData friendData)
    {
        Backend.Friend.AcceptFriend(friendData.inDate, callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError($"친구 수락 도중 에러가 발생했습니다. : {callback}");
                return;
            }

            Debug.Log($"{friendData.nickname}님과 친구가 되었습니다. : {callback}");

            // 수락 후 받은 요청 갱신
            GetReceivedRequestList();
            GetFriendList(); // 친구 목록 새로고침
        });
    }

    public void RejectFriend(FriendData friendData)
    {
        Backend.Friend.RejectFriend(friendData.inDate, callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError($"친구 거절 도중 에러가 발생했습니다. : {callback}");
                return;
            }

            Debug.Log($"{friendData.nickname}님 친구 요청을 거절했습니다. : {callback}");

            // 거절 후 받은 요청 갱신
            GetReceivedRequestList();
        });
    }

    public void GetFriendList()
    {
        Backend.Friend.GetFriendList(callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError($"친구 목록 조회 도중 에러가 발생했습니다. : {callback}");
                return;
            }

            // JSON 데이터 파싱 성공
            try
            {
                LitJson.JsonData jsonData = callback.GetFlattenJSON()["rows"];

                // 받아온 데이터의 개수가 0이면 데이터가 없는 것
                if (jsonData == null || jsonData.Count <= 0)
                {
                    Debug.LogWarning("친구 목록 데이터가 없습니다.");
                    return;
                }

                // 친구 목록에 있는 모든 UI 비활성화
                friendPage.DeactivateAll();

                // 친구의 유저 데이터를 요청하기 위해 트랜잭션 요청 리스트 생성
                List<TransactionValue> transactionList = new List<TransactionValue>();

                // 서버에서 받아온 친구 정보를 일시적으로 저장할 리스트
                List<FriendData> friendDataList = new List<FriendData>();

                // 서버 응답(jsonData)을 순회하며 친구 정보를 파싱
                foreach (LitJson.JsonData item in jsonData)
                {
                    // FriendData 객체 생성
                    FriendData friendData = new FriendData();

                    // 닉네임이 존재할 경우 저장, 없으면 "NONAME"으로 처리
                    friendData.nickname = item.ContainsKey("nickname") ? item["nickname"].ToString() : "NONAME";

                    // 친구의 고유 식별자 (유저 inDate)
                    friendData.inDate = item["inDate"].ToString();

                    // 친구 요청을 보낸 시간
                    friendData.createdAt = item["createdAt"].ToString();

                    // 친구의 마지막 접속 시간
                    friendData.lastLogin = item["lastLogin"].ToString();

                    // 친구 리스트에 추가
                    friendDataList.Add(friendData);

                    // 각 친구의 유저 데이터를 조회하기 위한 조건 생성 (owner_inDate로 조회)
                    Where where = new Where();
                    where.Equal("owner_inDate", friendData.inDate);

                    // 해당 조건으로 유저 데이터 테이블의 값을 조회하는 트랜잭션 요청 추가
                    transactionList.Add(TransactionValue.SetGet(Define.User_Data_Table, where));
                }

                // 최신 뒤끝서버 5.18.0 기준 TransactionReadV2 사용
                Backend.GameData.TransactionReadV2(transactionList, transactionCallback =>
                {
                    if (!transactionCallback.IsSuccess())
                    {
                        Debug.LogError($"Transaction Error : {transactionCallback}");
                        return;
                    }

                    // GetReturnValuetoJSON() 사용: 원본 JSON 접근
                    LitJson.JsonData fullJson = transactionCallback.GetReturnValuetoJSON();

                    if (!fullJson.ContainsKey("Responses"))
                    {
                        Debug.LogWarning("Transaction 응답에 'Responses' 키가 없습니다.");
                        return;
                    }

                    LitJson.JsonData responses = fullJson["Responses"];

                    // 트랜잭션 응답을 순회하며 각 친구의 유저 데이터를 friendDataList에 반영
                    for (int i = 0; i < friendDataList.Count; i++)
                    {
                        var resp = responses[i]; // 트랜잭션 응답 하나 가져오기

                        // 응답에 Get 요청 결과가 없거나, 해당 테이블 데이터가 없으면 경고 출력 후 UI 활성화
                        if (!resp.ContainsKey("Get") || !resp["Get"].ContainsKey(Define.User_Data_Table))
                        {
                            Debug.LogWarning($"[{i}] 친구의 유저데이터가 존재하지 않음.");
                            friendPage.Activate(friendDataList[i]); // UI에 현재 친구 데이터 표시
                            continue;
                        }

                        // 유저 데이터의 "rows"를 가져옴
                        var rows = resp["Get"][Define.User_Data_Table]["rows"];

                        // rows가 존재하고 데이터가 있다면
                        if (rows != null && rows.Count > 0)
                        {
                            var row = rows[0]; // 첫 번째 row만 사용

                            // row에 nickname 필드가 있으면 friendDataList에 덮어쓰기 (닉네임 최신화)
                            if (row.ContainsKey("nickname"))
                            {
                                friendDataList[i].nickname = row["nickname"].ToString();
                            }

                            // TODO: 친구의 레벨, 프로필 이미지 등 추가 필드가 있다면 여기에 반영할 수 있음
                        }
                    }

                    // 모든 친구 데이터를 UI에 일괄 반영
                    friendPage.ActivateAll(friendDataList);
                });
            }
            // JSON 데이터 파싱 실패
            catch (Exception e)
            {
                // try-catch 에러 출력
                Debug.LogError(e);
            }
        });
    }

    public void BreakFriend(FriendData friendData)
    {
        Backend.Friend.BreakFriend(friendData.inDate, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("친구 삭제 성공");
                // 친구 리스트 재갱신
                GetFriendList();
            }
            else
            {
                Debug.LogError($"친구 삭제 도중 에러가 발생했습니다. : {callback}");

                // NotFound 예외 처리 (이미 삭제된 상태 등)
                if (callback.GetStatusCode() == "404" && callback.GetErrorCode() == "NotFoundException")
                {
                    Debug.LogWarning("이미 삭제된 친구입니다. UI에서 제거만 처리합니다.");
                    // UI에서 제거만 진행
                    GetFriendList(); // 전체 갱신 추천
                }
            }
        });
    }

    private bool IsExpirationDate(string createdAt)
    {
        // GetServerTime() - 서버 시간 불러오기
        var bro = Backend.Utils.GetServerTime();

        if (!bro.IsSuccess())
        {
            Debug.LogError($"서버 시간 불러오기에 실패했습니다. : {bro}");
            return false;
        }

        // JSON 데이터 파싱 성공
        try
        {
            // createdAt 시간으로부터 3일 뒤의 시간
            DateTime after3Days = DateTime.Parse(createdAt).AddDays(Define.Expiration_Days);

            // 현재 서버 시간
            string serverTime = bro.GetFlattenJSON()["utcTime"]?.ToString();
            if (string.IsNullOrEmpty(serverTime))
            {
                Debug.LogError("서버 시간이 비어 있습니다.");
                return false;
            }

            // 만료까지 남은 시간 = 만료 시간 - 현재 서버 시간
            TimeSpan timeSpan = after3Days - DateTime.Parse(serverTime);

            if (timeSpan.TotalHours < 0)
            {
                return true;
            }
        }
        // JSON 파싱 실패
        catch (Exception e)
        {
            // try-catch 에러 출력
            Debug.LogError(e);
        }

        return false;
    }
}