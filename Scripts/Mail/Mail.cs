using System.Collections.Generic;

// 개별 우편 정보를 담는 클래스
public class Mail
{
    public bool isCanReceive = false;   // 수령 가능한 우편인지 여부

    public string Title;    // 우편 제목
    public string Content;  // 우편 본문
    public string InDate;   // 우편 고유 ID (뒤끝에서 사용됨)
    public string Author;   // 우편 보낸 사람

    // 우편 보상 아이템 목록 (아이템명, 수량)
    public Dictionary<string, int> mailReward = new Dictionary<string, int>();

    // 디버그 출력을 위한 ToString 오버라이드
    public override string ToString()
    {
        string result = string.Empty;
        result += $"Title : {Title}\n";
        result += $"Content : {Content}\n";
        result += $"InDate : {InDate}\n";
        result += $"Author : {Author}\n";

        if (isCanReceive)
        {
            result += "우편 아이템\n";

            foreach (string itemKey in mailReward.Keys)
            {
                result += $"| {itemKey} : {mailReward[itemKey]}개\n";
            }
        }
        else
        {
            result += "지원하지 않는 우편 아이템입니다.";
        }

        return result;
    }
}