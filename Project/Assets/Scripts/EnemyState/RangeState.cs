using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeState : IEnemyState
{
    private Enemy enemy;
    
    void IEnemyState.Enter(Enemy enemy)
    {
        this.enemy = enemy;
    }
    void IEnemyState.Execute()
    {
        if (enemy.InMeleeRange)
        {
            enemy.ChangeState(new MeleeState());
        }
        else if (enemy.Target != null)
        {
            enemy.Move();
        }
        else
        {
            enemy.ChangeState(new IdleState());
        }
    }
    void IEnemyState.Exit()
    {
    }
    void IEnemyState.OnTriggerEnter(Collider2D other)
    {
    }
}
