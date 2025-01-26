public class AudioPool : MonoPool<PoolableAudio>
{
    void OnEnable()
    {
        ResetEvents.DestroyAllChickens += MinimizePool;
    }
    
    void OnDisable()
    {
        ResetEvents.DestroyAllChickens -= MinimizePool;
    }
    
    private void MinimizePool()
    {
        while (Available.Count > 0)
        {
            var chicken = Available.Pop();
            Destroy(chicken.gameObject);
        }
    }
}