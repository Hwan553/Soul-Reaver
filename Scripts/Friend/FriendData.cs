
public class FriendData
{
    public string nickname; // 닉네임
    public string inDate; // 해당 유저의 inDate
    public string createdAt; // 친구 요청  보낸 시간/ 친구 요청 받은 시간 / 친구가 된 시간
    public string lastLogin; // 친구 마지막 접속 시간

    public override string ToString()
    {
        string result = string.Empty;
        result += $"닉네임 : {nickname}\n";
        result += $"inDate : {inDate}\n";
        result += $"createdAt : {createdAt}\n";
        result += $"lastLogin : {lastLogin}\n";

        return result;
    }
}