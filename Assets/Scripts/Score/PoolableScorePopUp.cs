using UnityEngine;


public class PoolableScorePopUp : MonoBehaviour, IPoolable
{
    private Sprite _textSprite;
    private SpriteRenderer _spriteRenderer;
    public float moveUpSpeed = 1f;
    public float lifetime = 1f;
    private float _timer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Move up slightly each frame
        transform.Translate(Vector3.up * (moveUpSpeed * Time.deltaTime));

        // Track lifetime, then destroy
        _timer += Time.deltaTime;
        if (_timer >= lifetime)
        {
            ScoreFactory.Instance.Return(this);
        }
    }

    // This gets set by ScoreManager if needed
    public void SetSprite(Sprite sprite)
    {
        // if (_textSprite != null)
        _textSprite = sprite;
        _spriteRenderer.sprite = sprite;
    }

    public void Reset()
    {
        _timer = 0f;
    }

    public void Kill()
    {
        ScoreFactory.Instance.Return(this);
    }
}