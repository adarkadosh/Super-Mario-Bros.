﻿using Managers;
using PowerUps;
using UnityEngine;

namespace Mario.MarioStates
{
    public class IceMarioState : MarioBaseState
    {
        private static readonly int HitHash = Animator.StringToHash("GetSmaller");

        public override void EnterState(MarioStateMachine context)
        {
            MarioEvents.OnMarioStateChange?.Invoke(MarioState.Ice);
            Debug.Log("Entered Fire Mario State");
        }

        public override void GotHit(MarioStateMachine context)
        {
            // Revert to Big Mario
            SoundFXManager.Instance.PlaySound(context.PowerDownClip, context.transform);
            context.FlashTransparency?.StartFlashing();
            context.Animator.SetTrigger(HitHash);
            context.Invoke(nameof(context.StopFlashing), context.UntouchableDurationValue);
            context.StartCoroutine(context.UntouchableDurationCoroutine(context.UntouchableDurationValue));
            GameEvents.FreezeAllCharacters?.Invoke(1.1f);
            context.ChangeState(MarioState.Small);
        }
    
        public override void OnPickUpPowerUp(MarioStateMachine context, PowerUpType powerUpType)
        {
            if (powerUpType is PowerUpType.FireFlower)
            {
                context.PaletteSwapper.StartFlashing();
                context.Invoke(nameof(context.PaletteSwapper.StopFlashing), 1.2f);
                GameEvents.FreezeAllCharacters?.Invoke(1.2f);
            
                context.ChangeState(MarioState.Fire);
            } 
        }
    }
}