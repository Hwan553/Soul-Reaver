using BackEnd;
using System.Collections.Generic;
using UnityEngine;

// 뒤끝 서버의 우편(Post) 시스템을 관리하는 클래스
public class BackendMail : Singleton<BackendMail>
{
    [SerializeField] private List<Mail> _mailList = new List<Mail>();    // 현재 로컬에 저장된 우편 리스트

    public List<Mail> GetMailList()
    {
        return _mailList;
    }

    /// <summary>
    /// 받은 우편 아이템을 로컬 인벤토리에 저장
    /// </summary>
    /// <param name="item">postItems 데이터(JsonData)</param>
    public void SaveMailToLocal(LitJson.JsonData items)
    {
        foreach (LitJson.JsonData itemJson in items)
        {
            // 방어 코드: "item" 키가 없거나 null인 경우 건너뜀
            if (itemJson == null || !itemJson.ContainsKey("item") || itemJson["item"] == null)
            {
                Debug.LogWarning("itemJson 구조가 잘못되었습니다 (item 키 없음 또는 null)");
                continue;
            }

            // 아이템 형식이 유효한지 확인
            if (itemJson["item"].ContainsKey("itemType"))
            {
                // 뒤끝에서 내려온 아이템 정보 추출
                int itemId = int.Parse(itemJson["item"]["itemId"].ToString());
                string itemType = itemJson["item"]["itemType"].ToString();
                string itemName = itemJson["item"]["itemName"].ToString();
                int itemCount = int.Parse(itemJson["itemCount"].ToString());

                // 인벤토리 반영
                AddToInventory(itemName, itemCount);
            }
            else
            {
                // 관리자 우편 (차트 기반)
                if (itemJson["item"].IsObject)
                {
                    foreach (var key in itemJson["item"].Keys)
                    {
                        string itemName = key;
                        string rawCount = itemJson["item"][key].ToString();

                        if (int.TryParse(rawCount, out int itemCount))
                        {
                            AddToInventory(itemName, itemCount);
                        }
                        else
                        {
                            Debug.LogWarning($"차트 기반 아이템 파싱 실패: {itemName} = {rawCount}");
                        }
                    }
                }
                else
                {
                    Debug.LogError("itemJson[\"item\"] 구조가 예상과 다릅니다. Object가 아닙니다.");
                }
            }
        }
    }

    // 우편 리스트를 서버에서 받아와 로컬에 저장
    public void MailListGet(PostType mailType)
    {
        var bro = Backend.UPost.GetPostList(mailType);  // 우편 목록 요청

        string chartName = "MailRewardChart";   // 아이템 차트 이름 (콘솔에서 설정한 이름과 일치해야 함)

        if (!bro.IsSuccess())
        {
            Debug.LogError("우편 불러오기 중 에러가 발생했습니다.");
            return;
        }

        Debug.Log("우편 리스트 불러오기 요청에 성공했습니다. : " + bro);

        // 우편이 없는 경우
        if (bro.GetFlattenJSON()["postList"].Count <= 0)
        {
            Debug.LogWarning("받을 우편이 존재하지 않습니다.");
            return;
        }

        // 우편 하나씩 처리
        foreach (LitJson.JsonData mailListJson in bro.GetFlattenJSON()["postList"])
        {
            string inDate = mailListJson["inDate"].ToString();

            // 중복 방지 : inDate가 이미 있는지 확인
            bool alreadyExists = _mailList.Exists(p => p.InDate == inDate);

            if (alreadyExists)
            {
                continue; // 이미 추가된 공지라면 건너뜀
            }

            Mail mail = new Mail
            {
                Title = mailListJson["title"].ToString(),
                Content = mailListJson["content"].ToString(),
                InDate = mailListJson["inDate"].ToString()
            };

            // 유저 타입 우편 처리 (USER_DATA에서 직접 주는 경우)
            if (mailType == PostType.User)
            {
                if (mailListJson["itemLocation"]["tableName"].ToString() == "USER_DATA")
                {
                    if (mailListJson["itemLocation"]["column"].ToString() == "inventory")
                    {
                        foreach (string itemKey in mailListJson["item"].Keys)
                        {
                            mail.mailReward.Add(itemKey, int.Parse(mailListJson["item"][itemKey].ToString()));
                        }

                        mail.isCanReceive = true;
                    }
                    else
                    {
                        Debug.LogWarning("아직 지원되지 않는 컬럼 정보 입니다. : " +
                                         mailListJson["itemLocation"]["column"].ToString());
                    }
                }
                else
                {
                    Debug.LogWarning("아직 지원되지 않는 테이블 정보 입니다. : " +
                                     mailListJson["itemLocation"]["tableName"].ToString());
                }
            }
            // 관리자 우편 처리 (차트 기반)
            else
            {
                foreach (LitJson.JsonData itemJson in mailListJson["items"])
                {
                    Debug.Log(mailListJson.ToJson());

                    if (itemJson.ContainsKey("chartName") && itemJson["chartName"].ToString() == chartName)
                    {
                        // item 자체가 Dictionary<string, string> 형태로 되어 있음
                        foreach (var key in itemJson["item"].Keys)
                        {
                            string itemName = key;
                            string rawCount = itemJson["item"][key].ToString();

                            if (int.TryParse(rawCount, out int itemCount))
                            {
                                if (mail.mailReward.ContainsKey(itemName))
                                    mail.mailReward[itemName] += itemCount;
                                else
                                    mail.mailReward.Add(itemName, itemCount);
                            }
                            else
                            {
                                Debug.LogWarning($"[MailListGet] 숫자 파싱 실패: itemName={itemName}, rawValue={rawCount}");
                            }
                        }

                        mail.isCanReceive = true;
                    }
                    else
                    {
                        Debug.LogWarning("지원되지 않는 차트 정보입니다 : " +
                            (itemJson.ContainsKey("chartName") ? itemJson["chartName"].ToString() : "차트 키 없음"));

                        mail.isCanReceive = false;
                    }
                }
            }

            _mailList.Add(mail);    // 로컬 리스트에 추가
        }

        // 디버그 출력
        for (int i = 0; i < _mailList.Count; i++)
        {
            Debug.Log($"{i}번 째 우편\n" + _mailList[i].ToString());
        }
    }

    // 특정 인덱스의 우편을 수령
    public void MailReceive(PostType mailType, int index)
    {
        if (_mailList.Count <= 0)
        {
            Debug.LogWarning("받을 수 있는 우편이 존재하지 않습니다. 혹은 우편 리스트 불러오기를 먼저 호출해주세요.");
            return;
        }

        if (index >= _mailList.Count)
        {
            Debug.LogError($"해당 우편은 존재하지 않습니다. : 요청 index{index} / 우편 최대 갯수 : {_mailList.Count}");
            return;
        }

        Debug.Log($"{mailType.ToString()}의 {_mailList[index].InDate} 우편수령을 요청합니다.");

        var bro = Backend.UPost.ReceivePostItem(mailType, _mailList[index].InDate);

        if (!bro.IsSuccess())
        {
            Debug.LogError($"{mailType.ToString()}의 {_mailList[index].InDate} 우편수령 중 에러가 발생했습니다. : " + bro);
            return;
        }

        Debug.Log($"{mailType.ToString()}의 {_mailList[index].InDate} 우편수령에 성공했습니다. : " + bro);

        _mailList.RemoveAt(index);  // 로컬 리스트에서 제거

        // 아이템이 존재할 경우 로컬 인벤토리에 저장
        if (bro.GetFlattenJSON()["postItems"].Count > 0)
        {
            SaveMailToLocal(bro.GetFlattenJSON()["postItems"]);
        }
        else
        {
            Debug.LogWarning("수령 가능한 우편 아이템이 존재하지 않습니다.");
        }

        BackendGameData.Instance.GameDataUpdate();  // 게임 데이터 업데이트 호출
    }

    // 모든 우편 일괄 수령
    public void MailReceiveAll(PostType mailType)
    {
        if (_mailList.Count <= 0)
        {
            Debug.LogWarning("받을 수 있는 우편이 존재하지 않습니다. 혹은 우편 리스트 불러오기를 먼저 호출해주세요.");
            return;
        }

        Debug.Log($"{mailType.ToString()} 우편 모두 수령을 요청합니다.");

        var bro = Backend.UPost.ReceivePostItemAll(mailType);

        if (!bro.IsSuccess())
        {
            Debug.LogError($"{mailType.ToString()} 우편 모두 수령 중 에러가 발생했습니다 : " + bro);
            return;
        }

        Debug.Log("우편 모두 수령에 성공했습니다. : " + bro);

        _mailList.Clear();  // 로컬 리스트 초기화

        foreach (LitJson.JsonData postItemsJson in bro.GetFlattenJSON()["postItems"])
        {
            SaveMailToLocal(postItemsJson); // 수령한 아이템 로컬 저장
        }

        BackendGameData.Instance.GameDataUpdate();  // 게임 데이터 갱신
    }

    private void AddToInventory(string itemName, int itemCount)
    {
        // 방어 코드: UserData가 null이면 초기화
        if (BackendGameData.UserData == null)
        {
            Debug.LogWarning("UserData가 null 상태여서 새로 생성합니다.");
            BackendGameData.UserData = new UserData();
        }
        // 방어 코드: UserData.Inventory가 null이면 초기화
        if (BackendGameData.UserData.Inventory == null)
        {
            Debug.LogWarning("Inventory가 null입니다. 새로 생성합니다.");
            BackendGameData.UserData.Inventory = new Dictionary<string, int>();
        }

        // 골드 또는 젬은 전용 변수에도 반영
        if (itemName == "Gold")
        {
            GameManager.Instance.GoldNum += itemCount;
        }
        else if (itemName == "Gem")
        {
            GameManager.Instance.GpNum += itemCount;
        }

        // 인벤토리 저장
        if (BackendGameData.UserData.Inventory.ContainsKey(itemName))
            BackendGameData.UserData.Inventory[itemName] += itemCount;
        else
            BackendGameData.UserData.Inventory.Add(itemName, itemCount);

        Debug.Log($"인벤토리에 추가됨: {itemName} - {itemCount}개");
    }
}