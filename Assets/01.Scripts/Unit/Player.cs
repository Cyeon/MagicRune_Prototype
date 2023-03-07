using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public int cost = 10; // 마나
    public AudioClip attackSound = null;

    private void Awake()
    {
        _isPlayer = true;
    }

    public void Attack(float dmg)
    {
        currentDmg = dmg;

        InvokeStatus(StatusInvokeTime.Attack);

        AttackManager.Instance.enemy.TakeDamage(currentDmg);
        SoundManager.Instance.PlaySound(attackSound, SoundType.Effect);

    }
}
