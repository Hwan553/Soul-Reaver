using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserRankingSlotManager : Singleton<UserRankingSlotManager>
{
    [SerializeField] private Transform rankingContentParent; // Content 오브젝트
    [SerializeField] private GameObject rankingSlotPrefab;   // 프리팹
    [SerializeField] private int rankCount = 2;              // 몇 명 출력할지
    [SerializeField] private Sprite[] rankIcons;             // 1~3위 이미지 등

    private string leaderboardUUID = "0197ce1b-cdd0-706f-b1be-c1c33b84a626";

    public void LoadRanking()
    {
        var bro = Backend.Leaderboard.User.GetLeaderboard(leaderboardUUID, rankCount);

        Backend.Leaderboard.User.GetLeaderboard("0197ce1b-cdd0-706f-b1be-c1c33b84a626");

        if (!bro.IsSuccess())
        {
            Debug.LogError("랭킹 불러오기 실패: " + bro);
            return;
        }

        Debug.Log("랭킹 불러오기 성공");

        var rows = bro.FlattenRows();
        Debug.Log("rows.Count: " + rows.Count);


        foreach (Transform child in rankingContentParent)
            Destroy(child.gameObject); // 이전에 있던 목록 삭제

        for(int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            string nickname = row["nickname"].ToString();
            int rank = int.Parse(row["rank"].ToString());
            int Wave = int.Parse(row["score"].ToString());

            GameObject slot = Instantiate(rankingSlotPrefab, rankingContentParent);
            Debug.Log("프리팹 생성됨: " + slot.name);

            // 컴포넌트 가져와서 설정
            TMP_Text rankText = slot.transform.Find("Ranking Obj/Ranking Text").GetComponent<TMP_Text>();
            TMP_Text nicknameText = slot.transform.Find("NickName Obj/NickName Text").GetComponent<TMP_Text>();
            TMP_Text WaveText = slot.transform.Find("Wave Obj/Wave Text").GetComponent<TMP_Text>();
            Image iconImage = slot.transform.Find("Ranking Obj/Ranking Image").GetComponent<Image>();
            Button friendButton = slot.transform.Find("Friend Buttom").GetComponent<Button>();

            rankText.text = $"{rank}";
            nicknameText.text = nickname;
            WaveText.text = $"{Wave}";

            // 아이콘 설정 (1~3위만)
            if (rank <= 3 && rankIcons.Length >= rank)
                iconImage.sprite = rankIcons[rank - 1];
            else
                iconImage.gameObject.SetActive(false); // 기본 비활성

            friendButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX("Button");

                string nickname = nicknameText.text;

                BackEndFriend.Instance.SendRequestFriend(nickname);
            });
        }
    }
}
