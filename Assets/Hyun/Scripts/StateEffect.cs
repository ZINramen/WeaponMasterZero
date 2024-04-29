using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateEffect : MonoBehaviour
{
    public enum EffectType { Heal, Damage, SpeedControl }
    public EffectType type;

    public Entity owner;
    public float value;
    public float delay;

    public bool wait = false;

    private void OnEnable()
    {
        wait = false;
    }

    private void Update()
    {
        if (!wait)
        {
            wait = true;
            StartCoroutine(StartEffect());
        }
    }
    IEnumerator StartEffect()
    {
        yield return new WaitForSeconds(delay);
        switch (type)
        {
            case EffectType.Heal:
                owner.SetHp(owner.GetHp() + value);
                break;
            case EffectType.Damage:
                owner.SetHp(owner.GetHp() - value);
                break;
            case EffectType.SpeedControl:
                owner.SetHp(owner.movement.speed + value);
                break;
        }
        wait = false;
    }
}
