using PowerUps.PowerUps;
using UnityEngine;


// Power-up Types for blocks (e.g. Super Mashroom, 1-Up Mashroom, Star, etc.)
public enum PowerUpType
{
    SuperMashroom,
    OneUpMashroom,
    Star,
    FireFlower,
    IceFlower,
    Nothing
}

public class PowerUpFactory : MonoSingleton<PowerUpFactory>, IFactory<GenericPowerUp, PowerUpType>
{
    [Header("Power-Up Pools")]
    [SerializeField] private OneUpMashroomPool oneUpMashroomPool;
    [SerializeField] private StarPool starPool;
    [SerializeField] private SuperMashroomPool superMashroomPool;
    [SerializeField] private FireFlowerPool fireFlowerPool;
    [SerializeField] private IceFlowerPool iceFlowerPool;
    [SerializeField] private BlockCoinPool blockCoinPool;
    
    private bool _marioIsBig;

    public GenericPowerUp Spawn(PowerUpType powerUpType)
    {
        switch (powerUpType)
        {
            case PowerUpType.OneUpMashroom:
                return oneUpMashroomPool.Get();
            case PowerUpType.Star:
                return starPool.Get();
            case PowerUpType.SuperMashroom:
                return _marioIsBig switch
                {
                    true => fireFlowerPool.Get(),
                    false => superMashroomPool.Get()
                };
            case PowerUpType.FireFlower:
                return fireFlowerPool.Get();
            case PowerUpType.IceFlower:
                return iceFlowerPool.Get();
            default:
                return null;
        }
    }

    public void Return(GenericPowerUp genericPowerUp)
    {
        switch (genericPowerUp)
        {
            case OneUpMashroom oneUpMashroom:
                oneUpMashroomPool.Return(oneUpMashroom);
                break;
            // case BlockCoin blockCoin:
            // blockCoinPool.Return(blockCoin);
            // break;
            case Star star:
                starPool.Return(star);
                break;
            case SuperMashroom superMashroom:
                superMashroomPool.Return(superMashroom);
                break;
            case FireFlower fireFlower:
                fireFlowerPool.Return(fireFlower);
                break;
            case IceFlower iceFlower:
                iceFlowerPool.Return(iceFlower);
                break;
            default:
                Debug.LogWarning($"Attempted to return unsupported PowerUp type: {genericPowerUp.GetType()}");
                break;
        }
    }
    
    public BlockCoin GetBlockCoin(Vector3 position)
    {
        var blockCoin = blockCoinPool.Get();
        blockCoin.transform.position = position;
        blockCoin.gameObject.SetActive(true);
        return blockCoin;
    }
    
    public void ReturnBlockCoin(BlockCoin blockCoin)
    {
        blockCoinPool.Return(blockCoin);
    }
    
    void OnEnable()
    {
        MarioEvents.OnMarioStateChange += SetMarioIsBig;
        // MarioEvents.OnEnterSmallMario += SetMarioIsSmall;
    }
    
    void OnDisable()
    {
        MarioEvents.OnMarioStateChange -= SetMarioIsBig;
        // MarioEvents.OnEnterSmallMario -= SetMarioIsSmall;
    }
    
    
    private void SetMarioIsBig(MarioState marioState)
    {
        _marioIsBig = marioState != MarioState.Small;
    }
}
