using Enemies;
using Enemies.Koopa;
using UnityEngine;

public enum EnemyType
{
    Goomba,
    Koopa,
    ShellKoopa
}

public class EnemyFactory : MonoSingleton<EnemyFactory>, IFactory<EnemyBehavior, EnemyType>
{
    [Header("Enemy Pools")]
    [SerializeField] private GoombaPool goombaPool;
    [SerializeField] private KoopaPool koopaPool;
    
    public EnemyBehavior Spawn(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Goomba:
                return goombaPool.Get();
            case EnemyType.Koopa:
                return koopaPool.Get();
            default:
                return null;
        }
    }

    public void Return(EnemyBehavior enemy)
    {
        switch (enemy)
        {
            case GoombaBehavior goomba:
                goombaPool.Return(goomba);
                break;
            case KoopaStateMachine koopa:
                koopaPool.Return(koopa);
                break;
        }
    }

}