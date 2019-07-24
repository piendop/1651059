using UnityEngine;

class PatrolState : IEnemyState
{
    private float patrolTimer;
    private float patrolDuration;
    private Enemy enemy;
    public void Enter(Enemy enemy)
    {
        patrolDuration = UnityEngine.Random.Range(3, 10);
        this.enemy = enemy;
    }

    public void Execute()
    {
        Patrol();
        enemy.Move();
        if (enemy.Target != null)
        {
            enemy.ChangeState(new RangeState());
        }
    }

    public void Exit()
    {
    }

    public void OnTriggerEnter(Collider2D other)
    {
      
    }
    private void Patrol()
    {
        patrolTimer += Time.deltaTime;
        if (patrolTimer > patrolDuration)
            enemy.ChangeState(new IdleState());
    }
}