using UnityEngine;
using BackEnd;

// 리더보드에 등록하는 Manager
public class RankingManager : Singleton<RankingManager>
{
    public void UpdateUserLeaderboardScore(int stageWave)
    {
        string leaderboardName = "0197ce1b-cdd0-706f-b1be-c1c33b84a626";

        string tableName = "USER_STAGEWAVE";
        string rowInDate = string.Empty;

        Debug.Log("데이터 조회 시도");

        var bro = Backend.GameData.GetMyData(tableName, new Where());

        if (bro.IsSuccess() == false)
        {
            Debug.LogError("데이터 조회 중 문제가 발생했습니다. : " + bro);
            return;
        }

        Debug.Log("데이터 조회 성공" + bro);

        if (bro.FlattenRows().Count > 0)
        {
            rowInDate = bro.FlattenRows()[0]["inDate"].ToString();
        }
        else
        {
            Debug.Log("데이터가 존재하지 않습니다. 데이터 삽입을 시도합니다.");
            var bro2 = Backend.GameData.Insert(tableName);

            if (bro2.IsSuccess() == false)
            {
                Debug.LogError("데이터 삽입 중 문제가 발생했습니다. : " + bro2);
                return;
            }
            Debug.Log("데이터 삽입에 성공했습니다. : " + bro2);

            rowInDate = bro2.GetInDate();
        }
        Debug.Log("내 게임 정보의 rowInDate : " + rowInDate); // 추출된 rowIndate의 값은 다음과 같습니다.  

        Param param = new Param();
        param.Add("StatWave", stageWave);
        param.Add("score", stageWave);

        Debug.Log("랭킹(리더보드) 삽입 시도");
        var rankBro = Backend.Leaderboard.User.UpdateMyDataAndRefreshLeaderboard(leaderboardName, tableName, rowInDate, param);

        if (rankBro.IsSuccess() == false)
        {
            Debug.LogError("랭킹 등록 중 오류가 발생했습니다. : " + rankBro);
            return;
        }

        Debug.Log("랭킹 삽입에 성공했습니다. : " + rankBro);
    }
}