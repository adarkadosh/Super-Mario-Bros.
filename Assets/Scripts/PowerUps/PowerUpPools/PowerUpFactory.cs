using PowerUps;
using UnityEngine;

public class PowerUpFactory : MonoSingleton<PowerUpFactory>
{
    [Header("Power-Up Pools")]
    [SerializeField] private OneUpMashroomPool oneUpMashroomPool;
    [SerializeField] private BlockCoinPool blockCoinPool;
    [SerializeField] private StarPool starPool;
    [SerializeField] private SuperMashroomPool superMashroomPool;
    
    public GenericPowerUp GetInstance(GenericBlockHit.PowerUpType powerUpType, Vector3 position)
    {
        switch (powerUpType)
        {
            case GenericBlockHit.PowerUpType.OneUpMashroom:
                return CreateOneUpMashroom(position);
            // case GenericBlockHit.PowerUpType.BlockCoin:
                // return CreateBlockCoin(position);
            case GenericBlockHit.PowerUpType.Star:
                return CreateStar(position);
            case GenericBlockHit.PowerUpType.SuperMashroom:
                return CreateSuperMashroom(position);
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
}