using PowerUps.PowerUps;
using UnityEngine;

public interface IFactory<T, U>
{
    T Spawn(U type);
    void Return(T instance);
}