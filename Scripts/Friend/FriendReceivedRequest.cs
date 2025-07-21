
public class FriendReceivedRequest : FriendBase
{
    public override void Setup(BackEndFriend friendSystem, FriendPageBase friendPage, FriendData friendData)
    {
        base.Setup(friendSystem, friendPage, friendData);
        base.SetExpirationDate();
    }

    // 친구 수락 버튼 클릭 시 호출되는 함수
    public void OnClickAcceptRequest()
    {
        // 친구 UI 오브젝트 삭제
        friendPage.Deactivate(gameObject);
        // 친구 요청 수락 (Backend Console)
        backendFriendSystem.AcceptFriend(friendData);

        SoundManager.Instance.PlaySFX("Button");
    }

    // 친구 거절 버튼 클릭 시 호출되는 함수
    public void OnClickRejectRequest()
    {
        // 친구 UI 오브젝트 삭제
        friendPage.Deactivate(gameObject);
        // 친구 거절(Backend Console)
        backendFriendSystem.RejectFriend(friendData);

        SoundManager.Instance.PlaySFX("Button");
    }
}