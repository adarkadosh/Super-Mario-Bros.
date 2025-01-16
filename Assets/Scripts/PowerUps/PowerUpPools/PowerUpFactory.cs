using PowerUps;
using PowerUps.PowerUps;
using UnityEngine;
using UnityEngine.Serialization;

// Power-up Types for blocks (e.g. Super Mashroom, 1-Up Mashroom, Star, etc.)
public enum PowerUpType
{
    SuperMashroom,
    OneUpMashroom,
    Star,
    FireFlower,
    Nothing
}

public class PowerUpFactory : MonoSingleton<PowerUpFactory>
{
    [Header("Power-Up Pools")] [SerializeField]
    private OneUpMashroomPool oneUpMashroomPool;

    [SerializeField] private BlockCoinPool blockCoinPool;
    [SerializeField] private StarPool starPool;
    [SerializeField] private SuperMashroomPool superMashroomPool;
    [SerializeField] private FireFlowerPool fireFlowerPool;
    private bool _marioIsBig;

    public GenericPowerUp GetInstance(PowerUpType powerUpType, Vector3 position)
    {
        switch (powerUpType)
        {
            case PowerUpType.OneUpMashroom:
                return CreateOneUpMashroom(position);
            // case GenericBlockHit.PowerUpType.BlockCoin:
            // return CreateBlockCoin(position);
            case PowerUpType.Star:
                return CreateStar(position);
            case PowerUpType.SuperMashroom:
                return _marioIsBig switch
                {
                    true => CreateFireFlower(position),
                    false => CreateSuperMashroom(position)
                };
            case PowerUpType.FireFlower:
                return CreateFireFlower(position);
            default:
                return null;
        }
    }

    public void ReturnPowerUp(GenericPowerUp genericPowerUp)
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
            default:
                Debug.LogWarning($"Attempted to return unsupported PowerUp type: {genericPowerUp.GetType()}");
                break;
        }
    }

    public BlockCoin GetBlockCoin(Vector3 position)
    {
        return CreateBlockCoin(position);
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
    


    private GenericPowerUp CreateOneUpMashroom(Vector3 position)
    {
        var oneUpMashroom = oneUpMashroomPool.Get();
        if (oneUpMashroom != null)
        {
            oneUpMashroom.transform.position = position;
            oneUpMashroom.gameObject.SetActive(true);
            Debug.Log("OneUpMashroom power-up spawned.");
        }
        else
        {
            Debug.LogError("Failed to spawn OneUpMashroom: Pool instance is null.");
        }

        return oneUpMashroom;
    }

    private BlockCoin CreateBlockCoin(Vector3 position)
    {
        var blockCoin = blockCoinPool.Get();
        blockCoin.transform.position = position;
        blockCoin.gameObject.SetActive(true);
        return blockCoin;
    }

    private Star CreateStar(Vector3 position)
    {
        var star = starPool.Get();
        if (star != null)
        {
            star.transform.position = position;
            star.gameObject.SetActive(true);
            Debug.Log("Star power-up spawned.");
        }
        else
        {
            Debug.LogError("Failed to spawn Star: Pool instance is null.");
        }

        return star;
    }

    private SuperMashroom CreateSuperMashroom(Vector3 position)
    {
        var superMashroom = superMashroomPool.Get();
        if (superMashroom != null)
        {
            superMashroom.transform.position = position;
            superMashroom.gameObject.SetActive(true);
            superMashroom.Trigger();
            Debug.Log("SuperMashroom power-up spawned.");
        }
        else
        {
            Debug.LogError("Failed to spawn SuperMashroom: Pool instance is null.");
        }

        return superMashroom;
    }

    private FireFlower CreateFireFlower(Vector3 position)
    {
        var fireFlower = fireFlowerPool.Get();
        if (fireFlower != null)
        {
            fireFlower.transform.position = position;
            fireFlower.gameObject.SetActive(true);
            fireFlower.Trigger();
            Debug.Log("FireFlower power-up spawned.");
        }
        else
        {
            Debug.LogError("Failed to spawn FireFlower: Pool instance is null.");
        }

        return fireFlower;
    }
}