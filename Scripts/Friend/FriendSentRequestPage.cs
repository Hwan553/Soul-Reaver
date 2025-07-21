using UnityEngine;
using TMPro;

public class FriendSentRequestPage : FriendPageBase
{ 
    [Header("Send Request Friend")]
    [SerializeField] private TMP_InputField _inputFieldNickname;
    [SerializeField] private FadeEffect_TMP _textResult;

    private void OnEnable()
    {
        // [친구 요청 대기] 목록 불러오기
        BackEndFriend.Instance.GetSentRequestList();
    }

    private void OnDisable()
    {
        DeactivateAll();
    }

    // 친구 요청 버튼 클릭 시 호출되는 함수
    public void OnClickRequestFriend()
    {
        // 버튼 클릭 사운드 재생
        SoundManager.Instance.PlaySFX("Button");

        // 입력 필드에서 닉네임 텍스트를 가져옴
        string nickname = _inputFieldNickname.text;

        // 입력값이 비어있거나 공백만 있을 경우 경고 메시지 출력 후 종료
        if (nickname.Trim().Equals(""))
        {
            _textResult.FadeOut("친구 요청을 보낼 닉네임을 입력해주세요.");
            return;
        }

        // 입력창 초기화 (입력값 비우기)
        _inputFieldNickname.text = "";

        // BackEnd API를 통해 친구 요청을 전송
        BackEndFriend.Instance.SendRequestFriend(nickname);
    }
}