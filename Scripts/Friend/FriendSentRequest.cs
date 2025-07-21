
public class FriendSentRequest : FriendBase
{
    public override void Setup(BackEndFriend friendSystem, FriendPageBase friendPage, FriendData friendData)
    {
        base.Setup(friendSystem, friendPage, friendData);
        base.SetExpirationDate();
    }

    // ģ�� ��û ��� ��ư Ŭ���� ȣ��Ǵ� �Լ�
    public void OnClickCancelRequest()
    {
        // ģ�� UI ������Ʈ ��Ȱ��ȭ
        friendPage.Deactivate(gameObject);
        // ģ�� ��� ��û
        backendFriendSystem.RevokeSentRequest(friendData.inDate);

        SoundManager.Instance.PlaySFX("Button");
    }
}