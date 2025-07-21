using System.Collections;
using UnityEngine;

public class ButtonClickEvent : MonoBehaviour
{
    private Coroutine _coroutine;
    private WaitForSeconds _delay = new WaitForSeconds(1.0f);

    private int _index;

    public void OnPointerDown(int index)
    {
        if (_coroutine == null)
        {
            _index = index;
            _coroutine = StartCoroutine(CoButtonAutoClickDelay());
        }
    }

    public void OnPointerUp()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator CoButtonAutoClickDelay()
    {
        while (true)
        {
            StatUpgradeManager.Instance.TryUpgradeStat(_index);

            yield return _delay;
        }
    }
}