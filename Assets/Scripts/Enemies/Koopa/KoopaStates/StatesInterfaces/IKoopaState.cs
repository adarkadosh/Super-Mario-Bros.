using UnityEngine;

namespace Enemies.Koopa.KoopaStates.StatesInterfaces
{
    public interface IKoopaState
    {
        void EnterState(KoopaStateMachine koopaState);
        void ExitState(KoopaStateMachine koopaState);
        void UpdateState(KoopaStateMachine koopaState);
        void OnTriggerEnter2D(KoopaStateMachine koopaState, Collider2D collider2D);
        void GotHit(KoopaStateMachine koopaState);
    }
}