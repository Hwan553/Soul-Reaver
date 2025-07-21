using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MySceneManager : Singleton<MySceneManager>
{
    public static MySceneManager Instance;

    [Header("Fade Panel")]
    [SerializeField] private Image _fadePanel;
    [SerializeField] private GameObject _loadingUI;

    public Image FadePanel => _fadePanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _fadePanel.gameObject.SetActive(false);
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeRoutine(sceneName));
    }

    private IEnumerator FadeRoutine(string sceneName)
    {
        // 1. Fade In
        _fadePanel.raycastTarget = true;
        yield return _fadePanel.DOFade(1f, 1f).WaitForCompletion(); // 1초 동안 어두워짐

        // 2. 로딩 UI 활성화
        _loadingUI.SetActive(true);

        // 3. 씬 비동기 로딩
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        // 4. 로딩 완료 대기
        while (async.progress < 0.9f)
        {
            yield return null;
        }

        // 5. 씬 활성화
        async.allowSceneActivation = true;

        // 6. 추가 대기 시간 (예: 2초)
        yield return new WaitForSeconds(1.5f);

        // 7. Fade Out
        yield return _fadePanel.DOFade(0.0f, 1.0f).WaitForCompletion();
        _fadePanel.raycastTarget = false;

        // 8. 로딩 UI 비활성화
        _loadingUI.SetActive(false);
        _fadePanel.gameObject.SetActive(false);
    }
}