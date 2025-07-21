// 뒤끝
using BackEnd;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackendLogin : Singleton<BackendLogin>
{
    public SignUpPanelMove SignUpPanelMove;

    [Header("Login Input Fields")]
    public TMP_InputField IDInputField;
    public TMP_InputField PwInputField;

    [Header("SingUp Input Fields")]
    public TMP_InputField SignupIDField;
    public TMP_InputField SignupPwField;
    public TMP_InputField SignupNewNickNameInputField;

    [Header("Login Buttons")]
    public Button loginButton;
    public Button SingnUpButton1;

    [Header("SingUp Buttons")]
    public Button SingnUpButton2;
    public Button SingnUpCloseButton;

    [Header("Login Erorr Text")]
    public TMP_Text IDorPwErorrText;

    [Header("Sing up Erorr Text")]
    public TMP_Text SignupIDorPwErorrText;
    public TMP_Text SignupNewNickNameErorrText;

    bool Singup = false;
    bool Nickname = false;

    private void Start()
    {
        loginButton.onClick.AddListener(CustomLogin);
        SingnUpButton1.onClick.AddListener(SignUpPanelMove.UpMove);
        SingnUpButton2.onClick.AddListener(OnSignUpButtonClicked);
        SingnUpCloseButton.onClick.AddListener(SignUpPanelMove.DownMove);
    }

    // 회원가입
    public void CustomSignUp()
    {
        string id = SignupIDField.text;
        string pw = SignupPwField.text;

        Debug.Log("회원가입을 요청합니다.");

        var bro = Backend.BMember.CustomSignUp(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("회원가입에 성공했습니다. :" + bro);
            SignupIDorPwErorrText.color = Color.green;
            SignupIDorPwErorrText.text = "회원가입에 성공햇습니다.";
            Singup = true;
        }
        else
        {
            Debug.LogError("회원가입에 실패했습니다. : " + bro);
            SignupIDorPwErorrText.text = "회원가입에 실패햇습니다.";
        }
    }

    // 로그인
    private void CustomLogin()
    {
        SoundManager.Instance.PlaySFX("Button");

        // 로그인 하기
        string id = IDInputField.text;
        string pw = PwInputField.text;

        Debug.Log("로그인을 요청합니다.");

        var bro = Backend.BMember.CustomLogin(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("로그인이 성공햇습니다. : " + bro);

            SoundManager.Instance.StopBGM();

            MySceneManager.Instance.FadePanel.gameObject.SetActive(true);

            MySceneManager.Instance.LoadSceneWithFade("Game");
        }
        else
        {
            Debug.LogError("로그인이 실패했습니다. : " + bro);
            IDorPwErorrText.text = "아이디 혹은 비밀번호가 맞지 않습니다.";
        }
    }

    // 닉네임
    public void UpdateNickname()
    {
        // 닉네임
        string nickname = SignupNewNickNameInputField.text;

        Debug.Log("닉네임을 요청합니다.");
        var bro = Backend.BMember.CreateNickname(nickname);

        if (bro.IsSuccess())
        {
            Debug.Log("닉네임 생성을 성공했습니다. : " + bro);
            SignupNewNickNameErorrText.color = Color.green;
            SignupNewNickNameErorrText.text = "닉네임 생성을 성공했습니다.";
            Nickname = true;
        }
        else
        {
            Debug.LogError("닉네임 생성을 실패했습니다. : "+ bro) ;
            SignupNewNickNameErorrText.text = "닉네임 생성을 실패했습니다.";
        }
    }

    private void NickName()
    {
        string name = SignupNewNickNameInputField.text;

        if (string.IsNullOrEmpty(name))
        {
            SignupNewNickNameErorrText.color = Color.red;
            SignupNewNickNameErorrText.text = "닉네임을 입력해주세요.";
            return;
        }

        if (name.Length > 10)
        {
            SignupNewNickNameErorrText.color = Color.red;
            SignupNewNickNameErorrText.text = "닉네임은 최대 10글자입니다.";
            return;
        }

        // 회원가입 시도
        CustomSignUp();

        // 닉네임 중복 검사
        UpdateNickname();

        // 결과 판단해서 성공 시에만 코루틴 시작
        if (Singup && Nickname)
        {
            StartCoroutine(InvokeSingObj());
        }
    }

    private IEnumerator InvokeSingObj()
    {
        yield return new WaitForSeconds(2f);

        SignupIDorPwErorrText.color = Color.red;
        SignupNewNickNameErorrText.color = Color.red;

        SignUpPanelMove.DownMove();
    }

    private void OnSignUpButtonClicked()
    {
        SoundManager.Instance.PlaySFX("Button");

        NickName();
    }
}