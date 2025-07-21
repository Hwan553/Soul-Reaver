using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : Singleton<BuffManager>
{
    private class ActiveBuff
    {
        public BuffType buffType;
        public float amount;
        public float endTime;
    }

    private List<ActiveBuff> activeBuffs = new List<ActiveBuff>();

    public void ApplyBuff(BuffData buff)
    {
        ActiveBuff newBuff = new ActiveBuff
        {
            buffType = buff.type,
            amount = buff.value,
            endTime = Time.time + buff.duration
        };

        activeBuffs.Add(newBuff);
        StartCoroutine(RemoveBuffAfterDuration(newBuff));
    }

    private IEnumerator RemoveBuffAfterDuration(ActiveBuff buff)
    {
        yield return new WaitForSeconds(buff.endTime - Time.time);
        activeBuffs.Remove(buff);
    }

    public float GetBuffBonus(BuffType type)
    {
        float total = 0f;
        foreach (var buff in activeBuffs)
        {
            if (buff.buffType == type)
                total += buff.amount;
        }
        return total;
    }
}
