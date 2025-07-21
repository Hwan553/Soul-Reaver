
public class Friend : FriendBase
{
    public override void Setup(BackEndFriend friendSystem, FriendPageBase friendPage, FriendData friendData)
    {
        base.Setup(friendSystem, friendPage, friendData);
        textTime.text = System.DateTime.Parse(friendData.lastLogin).ToString();
    }

    // 친구 삭제 버튼 클릭 시 호출되는 함수
    public void OnClickDeleteFriend()
    {
        // 친구 UI 오브젝트 삭제
        friendPage.Deactivate(gameObject);
        // 친구 삭제 (Backend Console)
        backendFriendSystem.BreakFriend(friendData);

        SoundManager.Instance.PlaySFX("Button");
    }
}