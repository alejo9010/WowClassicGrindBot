﻿using System;

namespace Core
{
    public class SpellInRange
    {
        private readonly long value;

        public SpellInRange(long value)
        {
            this.value = value;
        }

        public bool IsBitSet(int pos)
        {
            return (value & (1 << pos)) != 0;
        }

        // Warrior
        public bool Warrior_Charge => IsBitSet(0);
        public bool Warrior_Rend => IsBitSet(1);
        public bool Warrior_ShootGun => IsBitSet(2);

        // Rogue
        public bool Rogue_SinisterStrike => IsBitSet(0);
        public bool Rogue_Throw => IsBitSet(1);
        public bool Rogue_ShootGun => IsBitSet(2);

        // Priest
        public bool Priest_ShadowWordPain => IsBitSet(0);

        public bool Priest_MindBlast => IsBitSet(1);
        public bool Priest_MindFlay => IsBitSet(2);
        public bool Priest_Shoot => IsBitSet(3);

        // Druid
        public bool Druid_Wrath => IsBitSet(0);
        public bool Druid_Bash => IsBitSet(1);
        public bool Druid_Rip => IsBitSet(2);

        //Paladin
        public bool Paladin_Judgement => IsBitSet(0);

        //Mage
        public bool Mage_Fireball => IsBitSet(0);
        public bool Mage_Shoot => IsBitSet(1);
        public bool Mage_Pyroblast => IsBitSet(2);
        public bool Mage_Frostbolt => IsBitSet(3);
        public bool Mage_Fireblast => IsBitSet(4);

        //Hunter
        public bool Hunter_RaptorStrike => IsBitSet(0);
        public bool Hunter_AutoShoot => IsBitSet(1);
        public bool Hunter_SerpentSting => IsBitSet(2);

        // Warlock
        public bool Warlock_ShadowBolt => IsBitSet(0);
        public bool Warlock_Shoot => IsBitSet(1);

        // Shaman
        public bool Shaman_LightningBolt => IsBitSet(0);
        public bool Shaman_EarthShock => IsBitSet(1);

        public bool WithinPullRange(PlayerReader playerReader, PlayerClassEnum playerClass) => playerClass switch
        {
            PlayerClassEnum.Warrior => (playerReader.PlayerLevel >= 4 && Warrior_Charge) || playerReader.IsInMeleeRange,
            PlayerClassEnum.Rogue => Rogue_Throw,
            PlayerClassEnum.Priest => Priest_ShadowWordPain,
            PlayerClassEnum.Druid => playerReader.Druid_ShapeshiftForm == ShapeshiftForm.Druid_Bear ? Druid_Bash : playerReader.Druid_ShapeshiftForm == ShapeshiftForm.Druid_Cat ? Druid_Rip : Druid_Wrath,
            PlayerClassEnum.Paladin => (playerReader.PlayerLevel >= 4 && Paladin_Judgement) || playerReader.IsInMeleeRange,
            PlayerClassEnum.Mage => (playerReader.PlayerLevel >= 4 && Mage_Frostbolt) || Mage_Fireball,
            PlayerClassEnum.Hunter => (playerReader.PlayerLevel >=4 && Hunter_SerpentSting) || Hunter_AutoShoot,
            PlayerClassEnum.Warlock => Warlock_ShadowBolt,
            PlayerClassEnum.Shaman => (playerReader.PlayerLevel >= 4 && Shaman_EarthShock) || Shaman_LightningBolt,
            _ => true
        };

        public bool WithinCombatRange(PlayerReader playerReader, PlayerClassEnum playerClass) => playerClass switch
        {
            PlayerClassEnum.Warrior => (playerReader.PlayerLevel >= 4 && Warrior_Rend) || playerReader.IsInMeleeRange,
            PlayerClassEnum.Rogue => Rogue_SinisterStrike,
            PlayerClassEnum.Priest => Priest_Shoot,
            PlayerClassEnum.Druid => playerReader.Druid_ShapeshiftForm == ShapeshiftForm.Druid_Bear ? Druid_Bash : playerReader.Druid_ShapeshiftForm == ShapeshiftForm.Druid_Cat ? Druid_Rip : Druid_Wrath,
            PlayerClassEnum.Paladin => (playerReader.PlayerLevel >= 4 && Paladin_Judgement) || playerReader.IsInMeleeRange,
            PlayerClassEnum.Mage => Mage_Frostbolt || Mage_Fireball,
            PlayerClassEnum.Hunter => (playerReader.PlayerLevel >= 4 && Hunter_SerpentSting) || Hunter_AutoShoot || playerReader.IsInMeleeRange,
            PlayerClassEnum.Warlock => Warlock_ShadowBolt,
            PlayerClassEnum.Shaman => Shaman_LightningBolt,
            _ => true
        };
    }
}