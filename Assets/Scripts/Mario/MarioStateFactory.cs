using System;
using Mario.MarioStates;

namespace Mario
{
    public static class MarioStateFactory
    {
        private static IMarioState _smallMarioState;
        private static IMarioState _bigMarioState;
        private static IMarioState _fireMarioState;
        private static IMarioState _starMarioState;
        private static IMarioState _iceMarioState;

        public static IMarioState GetState(MarioState stateType)
        {
            switch (stateType)
            {
                case MarioState.Small:
                    return _smallMarioState ??= new SmallMarioState();
                case MarioState.Big:
                    return _bigMarioState ??= new BigMarioState();
                case MarioState.Fire:
                    return _fireMarioState ??= new FireMarioState();
                case MarioState.Ice:
                    return _iceMarioState ??= new IceMarioState();
                case MarioState.Star:
                    return _starMarioState ??= new StarMarioState();
                default:
                    throw new ArgumentException($"State {stateType} not recognized in MarioStateFactory.");
            }
        }
    }
}