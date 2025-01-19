// using UnityEngine;
//
// public class MarioAnimationController : MonoBehaviour
// {
//     private Animator _animator;
//     private MarioBaseState _baseState;
//
//     // Animator Parameter Hashes
//     private readonly int _speedHash = Animator.StringToHash("Speed");
//     private readonly int _isSlidingHash = Animator.StringToHash("IsSliding");
//     private readonly int _isJumpingHash = Animator.StringToHash("IsJumping");
//     private readonly int _isFallingHash = Animator.StringToHash("IsFalling");
//     
//     private readonly int _smallMarioTrigger = Animator.StringToHash("SmallMario");
//     private readonly int _bigMarioTrigger = Animator.StringToHash("BigMario");
//     private readonly int _fireMarioTrigger = Animator.StringToHash("FireMario");
//
//     private void Awake()
//     {
//         _animator = GetComponent<Animator>();
//         if (_animator == null)
//         {
//             Debug.LogError("Animator component not found on " + gameObject.name);
//         }
//
//         // Find the IMarioContext in the scene (assuming it's on the same GameObject)
//         _baseState = GetComponent<MarioBaseState>();
//         if (_baseState == null)
//         {
//             Debug.LogError("IMarioContext not found on " + gameObject.name);
//         }
//         else
//         {
//             // Subscribe to state change events
//             MarioBaseState.OnEnterSmallMario += TriggerSmallMario;
//             MarioBaseState.OnEnterBigMario += TriggerBigMario;
//             MarioBaseState.OnEnterFireMario += TriggerFireMario;
//         }
//     }
//
//     private void OnDestroy()
//     {
//         if (_baseState != null)
//         {
//             MarioBaseState.OnEnterSmallMario -= TriggerSmallMario;
//             MarioBaseState.OnEnterBigMario -= TriggerBigMario;
//             MarioBaseState.OnEnterFireMario -= TriggerFireMario;
//         }
//     }
//
//     /// <summary>
//     /// Updates the speed parameter in the Animator.
//     /// </summary>
//     /// <param name="speed">The current horizontal speed of Mario.</param>
//     public void UpdateSpeed(float speed)
//     {
//         _animator.SetFloat(_speedHash, Mathf.Abs(speed));
//     }
//
//     /// <summary>
//     /// Updates the sliding state in the Animator.
//     /// </summary>
//     /// <param name="isSliding">Whether Mario is sliding.</param>
//     public void UpdateSliding(bool isSliding)
//     {
//         _animator.SetBool(_isSlidingHash, isSliding);
//     }
//
//     /// <summary>
//     /// Updates the jumping state in the Animator.
//     /// </summary>
//     /// <param name="isJumping">Whether Mario is jumping.</param>
//     public void UpdateJumping(bool isJumping)
//     {
//         _animator.SetBool(_isJumpingHash, isJumping);
//     }
//
//     /// <summary>
//     /// Updates the falling state in the Animator.
//     /// </summary>
//     /// <param name="isFalling">Whether Mario is falling.</param>
//     public void UpdateFalling(bool isFalling)
//     {
//         _animator.SetBool(_isFallingHash, isFalling);
//     }
//
//     /// <summary>
//     /// Triggers the Small Mario animation.
//     /// </summary>
//     public void TriggerSmallMario()
//     {
//         _animator.SetTrigger(_smallMarioTrigger);
//     }
//
//     /// <summary>
//     /// Triggers the Big Mario animation.
//     /// </summary>
//     public void TriggerBigMario()
//     {
//         _animator.SetTrigger(_bigMarioTrigger);
//     }
//
//     /// <summary>
//     /// Triggers the Fire Mario animation.
//     /// </summary>
//     public void TriggerFireMario()
//     {
//         _animator.SetTrigger(_fireMarioTrigger);
//     }
//
//     /// <summary>
//     /// Resets all animation parameters to default states.
//     /// </summary>
//     public void ResetAnimations()
//     {
//         _animator.SetFloat(_speedHash, 0f);
//         _animator.SetBool(_isSlidingHash, false);
//         _animator.SetBool(_isJumpingHash, false);
//         _animator.SetBool(_isFallingHash, false);
//         
//         _animator.ResetTrigger(_smallMarioTrigger);
//         _animator.ResetTrigger(_bigMarioTrigger);
//         _animator.ResetTrigger(_fireMarioTrigger);
//     }
// }