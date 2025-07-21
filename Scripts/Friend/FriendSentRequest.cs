
public class FriendSentRequest : FriendBase
{
    public override void Setup(BackEndFriend friendSystem, FriendPageBase friendPage, FriendData friendData)
    {
        base.Setup(friendSystem, friendPage, friendData);
        base.SetExpirationDate();
    }

    // 친구 요청 취소 버튼 클릭시 호출되는 함수
    public void OnClickCancelRequest()
    {
        // 친구 UI 오브젝트 비활성화
        friendPage.Deactivate(gameObject);
        // 친구 취소 요청
        backendFriendSystem.RevokeSentRequest(friendData.inDate);

        SoundManager.Instance.PlaySFX("Button");
    }
}