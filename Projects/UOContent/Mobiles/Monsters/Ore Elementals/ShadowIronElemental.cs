using Server.Items;

namespace Server.Mobiles
{
    public class ShadowIronElemental : BaseCreature
    {
        [Constructible]
        public ShadowIronElemental(int oreAmount = 2) : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 111;
            BaseSoundID = 268;

            SetStr(226, 255);
            SetDex(126, 145);
            SetInt(71, 92);

            SetHits(136, 153);

            SetDamage(9, 16);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 50.1, 95.0);
            SetSkill(SkillName.Tactics, 60.1, 100.0);
            SetSkill(SkillName.Wrestling, 60.1, 100.0);

            Fame = 4500;
            Karma = -4500;

            VirtualArmor = 23;

            Item ore = new ShadowIronOre(oreAmount);
            ore.ItemID = 0x19B9;
            PackItem(ore);
        }

        public ShadowIronElemental(Serial serial) : base(serial)
        {
        }

        public override string CorpseName => "an ore elemental corpse";
        public override string DefaultName => "a shadow iron elemental";

        public override bool AutoDispel => true;
        public override bool BleedImmune => true;
        public override int TreasureMapLevel => 1;
        public override Poison PoisonImmune => Poison.Deadly;
        public override bool BreathImmune => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Gems, 2);
        }

        public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            if (from is BaseCreature bc && (bc.Controlled || bc.BardTarget == this))
                damage = 0; // Immune to pets and provoked creatures
        }

        public override void AlterDamageScalarFrom(Mobile caster, ref double scalar)
        {
            scalar = 0.0; // Immune to magic
        }

        public override void AlterSpellDamageFrom(Mobile from, ref int damage)
        {
            damage = 0;
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();
        }
    }
}