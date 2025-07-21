using UnityEngine;

// 컨트롤러의 기본 동작을 정의하는 추상 클래스
public abstract class Base_Controller : MonoBehaviour
{
    // MonoBehaviour의 Awake()를 오버라이드하여 초기화 함수 호출
    private void Awake()
    {
        Initialized();
    }

    // 각 컨트롤러에서 반드시 구현해야 하는 초기화 함수
    protected abstract void Initialized();
}