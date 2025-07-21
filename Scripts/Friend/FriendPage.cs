

public class FriendPage : FriendPageBase
{
    private void OnEnable()
    {
        // [친구] 목록 불러오기
        BackEndFriend.Instance.GetFriendList();
    }

    private void OnDisable()
    {
        DeactivateAll();
    }
}