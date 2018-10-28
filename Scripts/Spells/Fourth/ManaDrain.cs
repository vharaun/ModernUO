using System;
using System.Collections.Generic;
using Server.Targeting;

namespace Server.Spells.Fourth
{
  public class ManaDrainSpell : MagerySpell
  {
    private static SpellInfo m_Info = new SpellInfo(
      "Mana Drain", "Ort Rel",
      215,
      9031,
      Reagent.BlackPearl,
      Reagent.MandrakeRoot,
      Reagent.SpidersSilk
    );

    private static Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();

    public ManaDrainSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
    {
    }

    public override SpellCircle Circle => SpellCircle.Fourth;

    public override void OnCast()
    {
      Caster.Target = new InternalTarget(this);
    }

    private void AosDelay_Callback(Mobile m, int mana)
    {
      if (m.Alive && !m.IsDeadBondedPet)
      {
        m.Mana += mana;

        m.FixedEffect(0x3779, 10, 25);
        m.PlaySound(0x28E);
      }

      m_Table.Remove(m);
    }

    public void Target(Mobile m)
    {
      if (!Caster.CanSee(m))
      {
        Caster.SendLocalizedMessage(500237); // Target can not be seen.
      }
      else if (CheckHSequence(m))
      {
        SpellHelper.Turn(Caster, m);

        SpellHelper.CheckReflect((int)Circle, Caster, ref m);

        m.Spell?.OnCasterHurt();

        m.Paralyzed = false;

        if (Core.AOS)
        {
          int toDrain = 40 + (int)(GetDamageSkill(Caster) - GetResistSkill(m));

          if (toDrain < 0)
            toDrain = 0;
          else if (toDrain > m.Mana)
            toDrain = m.Mana;

          if (m_Table.ContainsKey(m))
            toDrain = 0;

          m.FixedParticles(0x3789, 10, 25, 5032, EffectLayer.Head);
          m.PlaySound(0x1F8);

          if (toDrain > 0)
          {
            m.Mana -= toDrain;

            m_Table[m] = Timer.DelayCall(TimeSpan.FromSeconds(5.0), () => AosDelay_Callback(m, toDrain));
          }
        }
        else
        {
          if (CheckResisted(m))
            m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
          else if (m.Mana >= 100)
            m.Mana -= Utility.Random(1, 100);
          else
            m.Mana -= Utility.Random(1, m.Mana);

          m.FixedParticles(0x374A, 10, 15, 5032, EffectLayer.Head);
          m.PlaySound(0x1F8);
        }

        HarmfulSpell(m);
      }

      FinishSequence();
    }

    public override double GetResistPercent(Mobile target)
    {
      return 99.0;
    }

    private class InternalTarget : Target
    {
      private ManaDrainSpell m_Owner;

      public InternalTarget(ManaDrainSpell owner) : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
      {
        m_Owner = owner;
      }

      protected override void OnTarget(Mobile from, object o)
      {
        if (o is Mobile mobile)
          m_Owner.Target(mobile);
      }

      protected override void OnTargetFinish(Mobile from)
      {
        m_Owner.FinishSequence();
      }
    }
  }
}