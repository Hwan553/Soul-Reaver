using UnityEngine;

public class GameStartManager : MonoBehaviour
{
    public PopDown PopDown;
    public PopUp PopUp;

    private bool _setLoginPanel = false;

    [Header("GameManager UI")]
    public GameObject BackgroundPanel;
    public GameObject LoginPanel;

    private void Start()
    {
        SoundManager.Instance.PlayBGM("Geralt of Rivia");

        _ = MyBackendManager.Instance;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && _setLoginPanel == false)
        {
            _setLoginPanel = true;
            BackgroundPanel.SetActive(true);
            PopUp.PlayGrow(LoginPanel);
        }
    }
}