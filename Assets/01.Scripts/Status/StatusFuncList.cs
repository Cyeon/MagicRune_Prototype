using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusFuncList : MonoBehaviour
{
    public Status status;
    public Unit unit;

    public void AddGetDamage(float dmg)
    {
        status.unit.currentDmg += dmg;
    }

    public void AddAtkDamage(float dmg)
    {
        status.unit.currentDmg += dmg;
    }

    public void RemAtkDamagePercent(float percent)
    {
        status.unit.currentDmg -= status.unit.currentDmg * (percent * 0.01f);
    }

    public void StackDmg()
    {
        unit.TakeDamage(status.typeValue, true, status);
        UIManager.Instance.UpdateHealthbar(unit.IsPlayer);
        StatusManager.Instance.RemoveValue(unit, status, status.typeValue);
        UIManager.Instance.ReloadStatusPanel(unit, status.statusName, status.typeValue);
    }

    public void AddFire()
    {
        Status status = StatusManager.Instance.GetUnitHaveStauts(BattleManager.Instance.attackUnit, StatusName.Ice);
        if (status != null)
        {
            BattleManager.Instance.attackUnit.TakeDamage(status.typeValue * 2);
            status.typeValue = 0;
            UIManager.Instance.RemoveStatusPanel(status.unit, status.statusName);
        }
    }

    public void WoundGetDmg()
    {
        AddGetDamage(status.typeValue);
        status.typeValue = 0;
        UIManager.Instance.ReloadStatusPanel(unit, status.statusName, status.typeValue);
    }
}
