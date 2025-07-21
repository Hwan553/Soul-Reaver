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
        // �ش� �г����� ������ �����ϴ��� ���δ� ����� ����
        var bro = Backend.Social.GetUserInfoByNickName(nickname);
        string inDate = string.Empty;

        if (!bro.IsSuccess())
        {
            Debug.LogError($"���� �˻� ���� ������ �߻��߽��ϴ�. :{bro}");
            return inDate;
        }

        // JSON ������ �ǽ� ����
        try
        {
            LitJson.JsonData jsonData = bro.GetFlattenJSON()["row"];

            // �޾ƿ� �������� ������ 0�̸� �����Ͱ� ���� ��
            if (jsonData == null || jsonData.Count <= 0)
            {
                Debug.LogError("������ inDate �����Ͱ� �����ϴ�.");
                return inDate;
            }

            inDate = jsonData["inDate"].ToString();

            Debug.Log($"{nickname}�� inDate ���� {inDate} �Դϴ�.");
        }
        // JSON ������ �Ľ� ����
        catch (Exception e)
        {
            // try-catch ���� ���
            Debug.LogError(e);
        }

        return inDate;
    }

    public void SendRequestFriend(string nickname)
    {
        // RequestFriend() �޼ҵ带 �̿��� ģ�� �߰� ��û�� �� �� �ش� ģ���� inDate ������ �ʿ�
        string inDate = GetUserInfoBy(nickname);
        if (string.IsNullOrEmpty(inDate))
            return;

        // ���� ģ�� ��û ��� ����� ��ȸ�ؼ�, �̹� ���� ��û�� �ִ��� �ߺ� ���θ� Ȯ��
        Backend.Friend.GetSentRequestList(sentCallback =>
        {
            // 1. ��û ����� ������ ��� �α� ����ϰ� ����
            if (!sentCallback.IsSuccess())
            {
                Debug.LogError($"ģ�� ��û ��� ��� ��ȸ ���� ������ �߻��߽��ϴ�. : {sentCallback}");
                return;
            }

            // 2. ��û ������� ��� ���� ģ�� ���(rows) ����
            var rows = sentCallback.GetFlattenJSON()["rows"];

            // 3. ��� ���� ģ������ inDate(�ĺ���)�� ������ ���� ���� (�ߺ� ������)
            var sentInDates = new HashSet<string>();

            // 4. ���� �����Ϳ��� �� ģ���� inDate ���� �����Ͽ� HashSet�� �߰�
            foreach (LitJson.JsonData item in rows)
                sentInDates.Add(item["inDate"].ToString());

            // 5. ���� ��û�Ϸ��� inDate�� �̹� �����ϴ��� �˻� (�ߺ� ���� �Ǵ�)
            if (sentInDates.Contains(inDate))
            {
                Debug.LogWarning($"{nickname}�Բ� �̹� ģ�� ��û�� ���½��ϴ�.");
                return; // �ߺ��� ��� ��û���� �ʰ� ����
            }

            // 6. �ߺ����� ���� ���, ������ ģ�� ��û�� ����
            Backend.Friend.RequestFriend(inDate, callback =>
            {
                // 6-1. ��û ����� ������ ��� �α� ���
                if (!callback.IsSuccess())
                {
                    Debug.LogError($"{nickname} ģ�� ��û ���� ������ �߻��߽��ϴ�. : {callback}");
                    return;
                }

                // 6-2. ��û�� ������ ��� ���� �α� ���
                Debug.Log($"ģ�� ��û�� �����߽��ϴ�. : {callback}");

                // 6-3. ��û�� ���������Ƿ�, UI ���� �����ϱ� ���� �ֽ� ģ�� ��û ��� ����� �ٽ� �ҷ���
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
                Debug.LogError($"ģ�� ��û ��� ��� ��ȸ ���� ������ �߻��߽��ϴ�. : {callback}");
                return;
            }

            // JSON ������ �Ľ� ����
            try
            {
                LitJson.JsonData jsonData = callback.GetFlattenJSON()["rows"];

                // �޾ƿ� �������� ������ 0�̸� �����Ͱ� ���� ��
                if (jsonData == null || jsonData.Count <= 0)
                {
                    Debug.LogWarning("ģ�� ��û ��� ��� �����Ͱ� �����ϴ�.");
                    return;
                }

                // ģ�� ��û ��� ��Ͽ� �ִ� ��� UI ��Ȱ��ȭ
                sentRequestPage.DeactivateAll();

                foreach (LitJson.JsonData item in jsonData) // JSON �迭 �����͸� ��ȸ
                {
                    FriendData friendData = new FriendData(); // �� ģ�� ������ ������ ��ü ����

                    // �г��� �׸��� �����ϸ� �� ����, ������ "NONAME"���� ��ü
                    friendData.nickname = item.ContainsKey("nickname") ? item["nickname"].ToString() : "NONAME";

                    // ���� ID �� (BackEnd���� �߱޵� inDate ��) ����
                    friendData.inDate = item["inDate"].ToString();

                    // ģ�� ��û ���� �ð� ���� (ISO8601 �Ǵ� yyyy-MM-ddTHH:mm:ss ���·� �����)
                    friendData.createdAt = item["createdAt"].ToString();

                    // ģ�� ��û �ð��� �����Ǿ����� Ȯ�� (���� �� �ڵ� ���)
                    if (IsExpirationDate(friendData.createdAt))
                    {
                        RevokeSentRequest(friendData.inDate); // ��û ��� ó�� (������ ����)
                        continue; // ���� UI ó�� �����ϰ� ���� ģ���� �Ѿ
                    }

                    // ģ�� ��û�� ��ȿ�� ���, �ش� ������ UI�� Ȱ��ȭ�Ͽ� ǥ��
                    sentRequestPage.Activate(friendData);
                }
            }
            // JSON ������ �Ľ� ����
            catch (Exception e)
            {
                // try - catch ���� ���
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
                Debug.LogError($"ģ�� ��û ��� ���� ������ �߻��߽��ϴ�. : {callback}");
                return;
            }

            Debug.Log($"ģ�� ��û ��ҿ� �����߽��ϴ�. : {callback}");

            // ģ�� ��û ��� �� ��� ����
            GetSentRequestList();
        });
    }

    public void GetReceivedRequestList()
    {
        Backend.Friend.GetReceivedRequestList(callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError($"ģ�� ���� ��� ��� ��ȸ ���� ������ �߻��߽��ϴ�. : {callback}");
                return;
            }

            // JSON ������ �Ľ� ����
            try
            {
                LitJson.JsonData jsonData = callback.GetFlattenJSON()["rows"];

                // �޾ƿ� �������� ������ 0�̸� �����Ͱ� ���°�
                if (jsonData == null || jsonData.Count <= 0)
                {
                    Debug.LogWarning("ģ�� ���� ��� ��� �����Ͱ� �����ϴ�.");
                    return;
                }

                receivedRequestPage.DeactivateAll();

                // jsonData�� BackEnd �������� ���� ģ�� ��û ��� (JsonData �迭)

                // jsonData�� �� �׸��� ��ȸ (LitJson.JsonData Ÿ�� ���)
                foreach (LitJson.JsonData item in jsonData)
                {
                    // ���ο� FriendData ��ü ���� (�г���, inDate, createdAt �� ������ ������ �뵵)
                    FriendData friendData = new FriendData();

                    // �г����� �����ϸ� �ش� ���� ����, ������ "NONAME"���� ��ü
                    friendData.nickname = item.ContainsKey("nickname") ? item["nickname"].ToString() : "NONAME";

                    // ��û�� ���� ������ ���� �ĺ��� (BackEnd���� inDate�� ���� ������)
                    friendData.inDate = item["inDate"].ToString();

                    // ģ�� ��û�� ������ �ð� (ISO 8601 ���� ���ڿ�, ��: "2025-07-19T14:52:00")
                    friendData.createdAt = item["createdAt"].ToString();

                    // ģ�� ��û�� ����Ǿ����� �Ǵ� (createdAt���κ��� 3�� �̻� ��������)
                    if (IsExpirationDate(friendData.createdAt))
                    {
                        // ����� ��û�� �ڵ����� ���� ó��
                        RejectFriend(friendData);

                        // ���� ģ�� ��û���� �Ѿ (�� ģ�� ��û�� UI�� ǥ������ ����)
                        continue;
                    }

                    // ��ȿ�� ģ�� ��û�� UI�� ǥ�� (ģ�� ��û ���� �ǿ� ���)
                    receivedRequestPage.Activate(friendData);
                }
            }
            // JSON ������ �Ľ� ����
            catch (Exception e)
            {
                // try-catch ���� ���
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
                Debug.LogError($"ģ�� ���� ���� ������ �߻��߽��ϴ�. : {callback}");
                return;
            }

            Debug.Log($"{friendData.nickname}�԰� ģ���� �Ǿ����ϴ�. : {callback}");

            // ���� �� ���� ��û ����
            GetReceivedRequestList();
            GetFriendList(); // ģ�� ��� ���ΰ�ħ
        });
    }

    public void RejectFriend(FriendData friendData)
    {
        Backend.Friend.RejectFriend(friendData.inDate, callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError($"ģ�� ���� ���� ������ �߻��߽��ϴ�. : {callback}");
                return;
            }

            Debug.Log($"{friendData.nickname}�� ģ�� ��û�� �����߽��ϴ�. : {callback}");

            // ���� �� ���� ��û ����
            GetReceivedRequestList();
        });
    }

    public void GetFriendList()
    {
        Backend.Friend.GetFriendList(callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError($"ģ�� ��� ��ȸ ���� ������ �߻��߽��ϴ�. : {callback}");
                return;
            }

            // JSON ������ �Ľ� ����
            try
            {
                LitJson.JsonData jsonData = callback.GetFlattenJSON()["rows"];

                // �޾ƿ� �������� ������ 0�̸� �����Ͱ� ���� ��
                if (jsonData == null || jsonData.Count <= 0)
                {
                    Debug.LogWarning("ģ�� ��� �����Ͱ� �����ϴ�.");
                    return;
                }

                // ģ�� ��Ͽ� �ִ� ��� UI ��Ȱ��ȭ
                friendPage.DeactivateAll();

                // ģ���� ���� �����͸� ��û�ϱ� ���� Ʈ����� ��û ����Ʈ ����
                List<TransactionValue> transactionList = new List<TransactionValue>();

                // �������� �޾ƿ� ģ�� ������ �Ͻ������� ������ ����Ʈ
                List<FriendData> friendDataList = new List<FriendData>();

                // ���� ����(jsonData)�� ��ȸ�ϸ� ģ�� ������ �Ľ�
                foreach (LitJson.JsonData item in jsonData)
                {
                    // FriendData ��ü ����
                    FriendData friendData = new FriendData();

                    // �г����� ������ ��� ����, ������ "NONAME"���� ó��
                    friendData.nickname = item.ContainsKey("nickname") ? item["nickname"].ToString() : "NONAME";

                    // ģ���� ���� �ĺ��� (���� inDate)
                    friendData.inDate = item["inDate"].ToString();

                    // ģ�� ��û�� ���� �ð�
                    friendData.createdAt = item["createdAt"].ToString();

                    // ģ���� ������ ���� �ð�
                    friendData.lastLogin = item["lastLogin"].ToString();

                    // ģ�� ����Ʈ�� �߰�
                    friendDataList.Add(friendData);

                    // �� ģ���� ���� �����͸� ��ȸ�ϱ� ���� ���� ���� (owner_inDate�� ��ȸ)
                    Where where = new Where();
                    where.Equal("owner_inDate", friendData.inDate);

                    // �ش� �������� ���� ������ ���̺��� ���� ��ȸ�ϴ� Ʈ����� ��û �߰�
                    transactionList.Add(TransactionValue.SetGet(Define.User_Data_Table, where));
                }

                // �ֽ� �ڳ����� 5.18.0 ���� TransactionReadV2 ���
                Backend.GameData.TransactionReadV2(transactionList, transactionCallback =>
                {
                    if (!transactionCallback.IsSuccess())
                    {
                        Debug.LogError($"Transaction Error : {transactionCallback}");
                        return;
                    }

                    // GetReturnValuetoJSON() ���: ���� JSON ����
                    LitJson.JsonData fullJson = transactionCallback.GetReturnValuetoJSON();

                    if (!fullJson.ContainsKey("Responses"))
                    {
                        Debug.LogWarning("Transaction ���信 'Responses' Ű�� �����ϴ�.");
                        return;
                    }

                    LitJson.JsonData responses = fullJson["Responses"];

                    // Ʈ����� ������ ��ȸ�ϸ� �� ģ���� ���� �����͸� friendDataList�� �ݿ�
                    for (int i = 0; i < friendDataList.Count; i++)
                    {
                        var resp = responses[i]; // Ʈ����� ���� �ϳ� ��������

                        // ���信 Get ��û ����� ���ų�, �ش� ���̺� �����Ͱ� ������ ��� ��� �� UI Ȱ��ȭ
                        if (!resp.ContainsKey("Get") || !resp["Get"].ContainsKey(Define.User_Data_Table))
                        {
                            Debug.LogWarning($"[{i}] ģ���� ���������Ͱ� �������� ����.");
                            friendPage.Activate(friendDataList[i]); // UI�� ���� ģ�� ������ ǥ��
                            continue;
                        }

                        // ���� �������� "rows"�� ������
                        var rows = resp["Get"][Define.User_Data_Table]["rows"];

                        // rows�� �����ϰ� �����Ͱ� �ִٸ�
                        if (rows != null && rows.Count > 0)
                        {
                            var row = rows[0]; // ù ��° row�� ���

                            // row�� nickname �ʵ尡 ������ friendDataList�� ����� (�г��� �ֽ�ȭ)
                            if (row.ContainsKey("nickname"))
                            {
                                friendDataList[i].nickname = row["nickname"].ToString();
                            }

                            // TODO: ģ���� ����, ������ �̹��� �� �߰� �ʵ尡 �ִٸ� ���⿡ �ݿ��� �� ����
                        }
                    }

                    // ��� ģ�� �����͸� UI�� �ϰ� �ݿ�
                    friendPage.ActivateAll(friendDataList);
                });
            }
            // JSON ������ �Ľ� ����
            catch (Exception e)
            {
                // try-catch ���� ���
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
                Debug.Log("ģ�� ���� ����");
                // ģ�� ����Ʈ �簻��
                GetFriendList();
            }
            else
            {
                Debug.LogError($"ģ�� ���� ���� ������ �߻��߽��ϴ�. : {callback}");

                // NotFound ���� ó�� (�̹� ������ ���� ��)
                if (callback.GetStatusCode() == "404" && callback.GetErrorCode() == "NotFoundException")
                {
                    Debug.LogWarning("�̹� ������ ģ���Դϴ�. UI���� ���Ÿ� ó���մϴ�.");
                    // UI���� ���Ÿ� ����
                    GetFriendList(); // ��ü ���� ��õ
                }
            }
        });
    }

    private bool IsExpirationDate(string createdAt)
    {
        // GetServerTime() - ���� �ð� �ҷ�����
        var bro = Backend.Utils.GetServerTime();

        if (!bro.IsSuccess())
        {
            Debug.LogError($"���� �ð� �ҷ����⿡ �����߽��ϴ�. : {bro}");
            return false;
        }

        // JSON ������ �Ľ� ����
        try
        {
            // createdAt �ð����κ��� 3�� ���� �ð�
            DateTime after3Days = DateTime.Parse(createdAt).AddDays(Define.Expiration_Days);

            // ���� ���� �ð�
            string serverTime = bro.GetFlattenJSON()["utcTime"]?.ToString();
            if (string.IsNullOrEmpty(serverTime))
            {
                Debug.LogError("���� �ð��� ��� �ֽ��ϴ�.");
                return false;
            }

            // ������� ���� �ð� = ���� �ð� - ���� ���� �ð�
            TimeSpan timeSpan = after3Days - DateTime.Parse(serverTime);

            if (timeSpan.TotalHours < 0)
            {
                return true;
            }
        }
        // JSON �Ľ� ����
        catch (Exception e)
        {
            // try-catch ���� ���
            Debug.LogError(e);
        }

        return false;
    }
}