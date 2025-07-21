using UnityEngine;
using TMPro;

public class FriendSentRequestPage : FriendPageBase
{ 
    [Header("Send Request Friend")]
    [SerializeField] private TMP_InputField _inputFieldNickname;
    [SerializeField] private FadeEffect_TMP _textResult;

    private void OnEnable()
    {
        // [ģ�� ��û ���] ��� �ҷ�����
        BackEndFriend.Instance.GetSentRequestList();
    }

    private void OnDisable()
    {
        DeactivateAll();
    }

    // ģ�� ��û ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    public void OnClickRequestFriend()
    {
        // ��ư Ŭ�� ���� ���
        SoundManager.Instance.PlaySFX("Button");

        // �Է� �ʵ忡�� �г��� �ؽ�Ʈ�� ������
        string nickname = _inputFieldNickname.text;

        // �Է°��� ����ְų� ���鸸 ���� ��� ��� �޽��� ��� �� ����
        if (nickname.Trim().Equals(""))
        {
            _textResult.FadeOut("ģ�� ��û�� ���� �г����� �Է����ּ���.");
            return;
        }

        // �Է�â �ʱ�ȭ (�Է°� ����)
        _inputFieldNickname.text = "";

        // BackEnd API�� ���� ģ�� ��û�� ����
        BackEndFriend.Instance.SendRequestFriend(nickname);
    }
}