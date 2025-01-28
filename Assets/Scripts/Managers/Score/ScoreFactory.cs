using System;
using UnityEngine;
using UnityEngine.Serialization;
public enum ScoresSet
{
    OneHundred = 100,
    TwoHundred = 200,
    FourHundred = 400,
    FiveHundred = 500,
    EightHundred = 800,
    OneThousand = 1000,
    TwoThousand = 2000,
    FourThousand = 4000,
    FiveThousand = 5000,
    EightThousand = 8000,
    OneUp = 1
    
    
}
public class ScoreFactory : MonoPool<PoolableScorePopUp>, IFactory<PoolableScorePopUp, int>
{
    [Serializable]
    private class SpriteHolder
    {
        // public string name;
        public ScoresSet score;
        public Sprite sprite;
    }

    [SerializeField] private SpriteHolder[] scoreSpriteCollection;

    public PoolableScorePopUp Spawn(int score)
    {
        var instance = Get();
        instance.SetSprite(GetSprite(score));
        return instance;
    }


    private Sprite GetSprite(int score)
    {
        foreach (var spriteHolder in scoreSpriteCollection)
        {
            if (spriteHolder.score == (ScoresSet)score)
            {
                return spriteHolder.sprite;
            }
        }
        return null;
    }
}