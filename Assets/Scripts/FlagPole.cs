using System;
using System.Collections;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class FlagPole : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform flag;
    [SerializeField] private Transform poleBottom;
    [SerializeField] private Transform castleDoor;
    [SerializeField] private GameObject castleFlag;
    [SerializeField] private MarioMoveController marioController; // Reference to Mario's controller
    [SerializeField] private GameObject mario; // Reference to Mario's GameObject

    [Header("Settings")]
    [SerializeField] private float flagSpeed = 6f;
    [SerializeField] private float marioMoveSpeed = 2f;
    [SerializeField] private float delayBeforeCastle = 1f;

    private bool _flagTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _flagTriggered = true;
            Debug.Log("Player reached the flag pole");
            MarioEvents.OnMarioReachedFlagPole?.Invoke();
            GameEvents.OnEventTriggered?.Invoke(ScoresSet.FourHundred, transform.position + Vector3.right);
            StartCoroutine(HandleFlagPoleSequence());
        }
    }
    
    
    private IEnumerator HandleFlagPoleSequence()
    {
        // 1. Stop Player Input
        if (marioController != null)
        {
            marioController.DisableInput();
        }
        
        // 2. Get Mario's Rigidbody and Animator
        Rigidbody2D marioRigidbody = mario.GetComponent<Rigidbody2D>();
        Animator marioAnimator = mario.GetComponent<Animator>();
        
        // 2. Disable Mario's Gravity and Set to Kinematic
        if (marioRigidbody != null)
        {
            marioRigidbody.gravityScale = 0f;
            marioRigidbody.linearVelocity = Vector2.zero; // Reset velocity
            // marioRigidbody.bodyType = RigidbodyType2D.Kinematic;
        }

        // 3. Trigger Sliding Animation
        if (marioAnimator != null)
        {
            marioAnimator.SetBool("IsSlidingDownPole", true);
        }

        // 2. Move Mario to the Pole Bottom
        yield return StartCoroutine(MoveMarioToPoleBottom());


        // 3. Animate the Flag Lowering
        yield return StartCoroutine(LowerFlag());
        mario.gameObject.transform.position += new Vector3(1f, 0, 0);
        mario.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        
        // 4. Move Mario to the Castle
        yield return new WaitForSeconds(delayBeforeCastle);
        if (marioAnimator != null)
        {
            marioAnimator.SetBool("IsSlidingDownPole", false);
        }

        // 5. Move Mario to the Castle using his control movement
        if (castleDoor != null && marioController != null)
        {
            Vector3 castleEntrance = castleDoor.position;
            marioController.MoveToPosition(castleEntrance, marioMoveSpeed);
        }
    }
    
    private IEnumerator MoveMarioToPoleBottom()
    {
        Vector3 startPos = marioController.transform.position;
        Vector3 endPos = new Vector3(poleBottom.position.x, poleBottom.position.y, marioController.transform.position.z);

        float elapsed = 0f;
        float duration = Vector3.Distance(startPos, endPos) / marioMoveSpeed;

        while (elapsed < duration)
        {
            marioController.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        marioController.transform.position = endPos;
    }
    

    private IEnumerator LowerFlag()
    {
        Vector3 startFlagPos = flag.position;
        Vector3 endFlagPos = new Vector3(poleBottom.position.x, poleBottom.position.y, flag.position.z);

        float elapsed = 0f;
        float duration = Vector3.Distance(startFlagPos, endFlagPos) / flagSpeed;

        while (elapsed < duration)
        {
            flag.position = Vector3.Lerp(startFlagPos, endFlagPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        flag.position = endFlagPos;
    }

    private void OnEnable()
    {
        GameEvents.OnLevelCompleted += MoveCastleFlag;
    }
    
    private void OnDisable()
    {
        GameEvents.OnLevelCompleted -= MoveCastleFlag;
    }
    
    private void MoveCastleFlag()
    {
        castleFlag.gameObject.transform.DOMoveY(castleFlag.gameObject.transform.position.y + 1.5f, 0.25f)
            .SetEase(Ease.Linear);
    }
    
    

    // private IEnumerator MoveFlag()
    // {
    //     // move flag with do tween move y to 1.5
    //     gameObject.transform.DOMoveY(gameObject.transform.position.y + 1.5f, 0.25f)
    //         .SetEase(Ease.Linear);
    //     // yield return moveUp.WaitForCompletion();
    // }
    
}

