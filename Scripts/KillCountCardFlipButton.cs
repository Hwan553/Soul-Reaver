using UnityEngine;
using UnityEngine.UI;

public class KillCountCardFlipButton : MonoBehaviour
{
    public Image fillImage;  // UI Image (Fill 타입으로 설정)
    public int maxCount = 10; // 몇 마리 죽으면 꽉 차는지
    private int killCount = 0;

    public bool OpenButton = false;

    public void OnMonsterKilled()
    {
        killCount++;
        if (killCount > maxCount) killCount = maxCount;

        fillImage.fillAmount = (float)killCount / maxCount;

        if(killCount >= 10)
        {
            OpenButton = true;
        }
    }

    public void ResetCount()
    {
        OpenButton = false;
        killCount = 0;
        fillImage.fillAmount = 0f;
    }
}
