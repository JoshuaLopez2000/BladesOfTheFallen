namespace BladesOfTheFallen.Core
{
    /// <summary>
    /// Defines legal player-combat transitions independently from animation
    /// and Unity timing. The runtime adapter advances timed phases.
    /// </summary>
    public sealed class PlayerCombatStateMachine
    {
        public PlayerCombatPhase Phase { get; private set; } = PlayerCombatPhase.Ready;
        public bool IsDamageInvulnerable { get; private set; }
        public bool CanAct => Phase == PlayerCombatPhase.Ready;

        public bool TryBeginAttack()
        {
            if (Phase is PlayerCombatPhase.HitStunned
                or PlayerCombatPhase.ParryStartup
                or PlayerCombatPhase.ParryActive
                or PlayerCombatPhase.MissRecovery)
            {
                return false;
            }

            Phase = PlayerCombatPhase.AttackStartup;
            return true;
        }

        public bool TryBeginParry()
        {
            return TryTransitionFromReady(PlayerCombatPhase.ParryStartup);
        }

        public bool TryOpenAttackWindow()
        {
            if (Phase != PlayerCombatPhase.AttackStartup)
            {
                return false;
            }

            Phase = PlayerCombatPhase.AttackActive;
            return true;
        }

        public bool TryOpenParryWindow()
        {
            if (Phase != PlayerCombatPhase.ParryStartup)
            {
                return false;
            }

            Phase = PlayerCombatPhase.ParryActive;
            return true;
        }

        public bool TryBeginRecovery()
        {
            if (Phase is not (PlayerCombatPhase.AttackActive or PlayerCombatPhase.ParryActive))
            {
                return false;
            }

            Phase = PlayerCombatPhase.Recovery;
            return true;
        }

        public bool TryBeginMissRecovery()
        {
            if (Phase != PlayerCombatPhase.AttackActive)
            {
                return false;
            }

            Phase = PlayerCombatPhase.MissRecovery;
            return true;
        }

        public bool TryTakeDamage()
        {
            if (IsDamageInvulnerable || Phase == PlayerCombatPhase.HitStunned)
            {
                return false;
            }

            IsDamageInvulnerable = true;
            Phase = PlayerCombatPhase.HitStunned;
            return true;
        }

        public bool TryRecoverFromHit()
        {
            if (Phase != PlayerCombatPhase.HitStunned)
            {
                return false;
            }

            Phase = PlayerCombatPhase.Recovery;
            return true;
        }

        public bool TryBecomeReady()
        {
            if (Phase is not (PlayerCombatPhase.Recovery or PlayerCombatPhase.MissRecovery))
            {
                return false;
            }

            Phase = PlayerCombatPhase.Ready;
            return true;
        }

        public void EndDamageInvulnerability()
        {
            IsDamageInvulnerable = false;
        }

        public void Reset()
        {
            Phase = PlayerCombatPhase.Ready;
            IsDamageInvulnerable = false;
        }

        private bool TryTransitionFromReady(PlayerCombatPhase phase)
        {
            if (!CanAct)
            {
                return false;
            }

            Phase = phase;
            return true;
        }
    }

    public enum PlayerCombatPhase
    {
        Ready,
        AttackStartup,
        AttackActive,
        ParryStartup,
        ParryActive,
        Recovery,
        MissRecovery,
        HitStunned
    }
}
