using BladesOfTheFallen.Core;
using NUnit.Framework;

public sealed class PlayerCombatStateMachineTests
{
    [Test]
    public void Attack_AdvancesThroughExplicitPhases()
    {
        PlayerCombatStateMachine combat = new();

        Assert.That(combat.TryBeginAttack(), Is.True);
        Assert.That(combat.Phase, Is.EqualTo(PlayerCombatPhase.AttackStartup));
        Assert.That(combat.TryOpenAttackWindow(), Is.True);
        Assert.That(combat.Phase, Is.EqualTo(PlayerCombatPhase.AttackActive));
        Assert.That(combat.TryBeginRecovery(), Is.True);
        Assert.That(combat.TryBecomeReady(), Is.True);
        Assert.That(combat.CanAct, Is.True);
    }

    [Test]
    public void Damage_GrantsInvulnerabilityAndRejectsRepeatedHit()
    {
        PlayerCombatStateMachine combat = new();

        Assert.That(combat.TryTakeDamage(), Is.True);
        Assert.That(combat.TryTakeDamage(), Is.False);
        Assert.That(combat.IsDamageInvulnerable, Is.True);

        combat.TryRecoverFromHit();
        combat.TryBecomeReady();

        Assert.That(combat.TryTakeDamage(), Is.False);
        combat.EndDamageInvulnerability();
        Assert.That(combat.TryTakeDamage(), Is.True);
    }

    [Test]
    public void Damage_InterruptsAnAttack()
    {
        PlayerCombatStateMachine combat = new();
        combat.TryBeginAttack();

        Assert.That(combat.TryTakeDamage(), Is.True);
        Assert.That(combat.Phase, Is.EqualTo(PlayerCombatPhase.HitStunned));
        Assert.That(combat.TryOpenAttackWindow(), Is.False);
    }

    [Test]
    public void Attack_CanRestartAnExistingAttackOrRecovery()
    {
        PlayerCombatStateMachine combat = new();
        combat.TryBeginAttack();
        combat.TryOpenAttackWindow();

        Assert.That(combat.TryBeginAttack(), Is.True);
        Assert.That(combat.Phase, Is.EqualTo(PlayerCombatPhase.AttackStartup));

        combat.TryOpenAttackWindow();
        combat.TryBeginRecovery();

        Assert.That(combat.TryBeginAttack(), Is.True);
        Assert.That(combat.Phase, Is.EqualTo(PlayerCombatPhase.AttackStartup));
    }

    [Test]
    public void Attack_CannotCancelHitStunOrActiveParry()
    {
        PlayerCombatStateMachine combat = new();
        combat.TryTakeDamage();

        Assert.That(combat.TryBeginAttack(), Is.False);

        combat.Reset();
        combat.TryBeginParry();
        combat.TryOpenParryWindow();

        Assert.That(combat.TryBeginAttack(), Is.False);
    }

    [Test]
    public void MissRecovery_BlocksNewAttacksUntilPenaltyEnds()
    {
        PlayerCombatStateMachine combat = new();
        combat.TryBeginAttack();
        combat.TryOpenAttackWindow();

        Assert.That(combat.TryBeginMissRecovery(), Is.True);
        Assert.That(combat.Phase, Is.EqualTo(PlayerCombatPhase.MissRecovery));
        Assert.That(combat.TryBeginAttack(), Is.False);

        Assert.That(combat.TryBecomeReady(), Is.True);
        Assert.That(combat.TryBeginAttack(), Is.True);
    }

    [Test]
    public void Parry_HasStartupActiveAndRecoveryPhases()
    {
        PlayerCombatStateMachine combat = new();

        Assert.That(combat.TryBeginParry(), Is.True);
        Assert.That(combat.Phase, Is.EqualTo(PlayerCombatPhase.ParryStartup));
        Assert.That(combat.TryOpenParryWindow(), Is.True);
        Assert.That(combat.Phase, Is.EqualTo(PlayerCombatPhase.ParryActive));
        Assert.That(combat.TryBeginRecovery(), Is.True);
        Assert.That(combat.TryBecomeReady(), Is.True);
    }
}
