using UnityEngine;

public interface IPowerUpPool 
{
    PowerUp Get();
    void Return(PowerUp powerUp);
}