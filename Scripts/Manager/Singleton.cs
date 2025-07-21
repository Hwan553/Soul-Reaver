using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // 싱글톤 인스턴스를 저장하는 정적 변수
    protected static T _instance = null;

    // 싱글톤 인스턴스를 반환하는 프로퍼티
    public static T Instance
    {
        get
        {
            // 인스턴스가 없으면 생성
            if (_instance == null)
            {
                // Managers 라는 이름의 게임 오브젝트 찾기
                GameObject manager = GameObject.Find("Managers");

                // 없으면 새롭게 생성하고, 씬이 변경되어도 삭제되지 않도록 설정
                if (manager == null)
                {
                    manager = new GameObject("Managers");
                    DontDestroyOnLoad(manager);
                }

                // 현재 씬에서 T 타입의 오브젝트를 찾아서 할당
                _instance = FindAnyObjectByType<T>();

                // 그래도 없으면 새 GameObject를 생성하여 T 타입 컴포넌트를 추가
                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    T component = obj.AddComponent<T>();

                    // 새로 만든 오브젝트를 Manager의 자식으로 설정
                    obj.transform.parent = manager.transform;

                    // 인스턴스 할당
                    _instance = component;
                }
            }
            return _instance;
        }
    }

    // MonoBehaviour의 Awake()를 오버라이드하여 초기화 함수 호출
    private void Awake()
    {
        Initialized();
    }

    // 싱글톤 초기화 함수 (상속 가능)
    protected virtual void Initialized()
    {

    }

    // 씬 변경 시 정리 작업을 수행하는 가상 함수 (상속하여 재정의 가능)
    protected virtual void Clear()
    {

    }
}