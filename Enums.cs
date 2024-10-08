﻿using System;
using System.Collections.Generic;
using System.Text;

namespace mudsort
{
    public enum MSStringValueKey
    {
        SecondaryName = 184549376,
        Name = 1,
        //Title = 5,
        Inscription = 7,
        InscribedBy = 8,
        //FellowshipName = 10,
        //UsageInstructions = 14,
        SimpleDescription = 0xF,
        FullDescription = 0x10,
        //MonarchName = 21,
        OnlyActivatedBy = 25,
        //Patron = 35,
        PortalDestination = 38,
        LastTinkeredBy = 39,
        ImbuedBy = 40//,
        //DateBorn = 43,
        //MonarchyTitle = 47
    }
    public enum MSLongValueKey
    {
        Type = 218103808,
        Icon = 218103809,
        //Container = 218103810,
        //Landblock = 218103811,
        ItemSlots = 218103812,
        PackSlots = 218103813,
        StackCount = 218103814,
        StackMax = 218103815,
        AssociatedSpell = 218103816,
        SlotLegacy = 218103817,
        //Wielder = 218103818,
        WieldingSlot = 218103819,
        //Monarch = 218103820,
        Coverage = 218103821,
        EquipableSlots = 218103822,
        EquipType = 218103823,
        IconOutline = 218103824,
        MissileType = 218103825,
        //UsageMask = 218103826,
        //HouseOwner = 218103827,
        //HookMask = 218103828,
        //HookType = 218103829,
        Model = 218103830,
        //Flags = 218103831,
        //CreateFlags1 = 218103832,
        //CreateFlags2 = 218103833,
        Category = 218103834,
        //Behavior = 218103835,
        //MagicDef = 218103836,
        //SpecialProps = 218103837,
        SpellCount = 218103838,
        WeapSpeed = 218103839,
        EquipSkill = 218103840,
        DamageType = 218103841,
        MaxDamage = 218103842,
        //Unknown10 = 218103843,
        //Unknown100000 = 218103844,
        //Unknown800000 = 218103845,
        //Unknown8000000 = 218103846,
        //PhysicsDataFlags = 218103847,
        //ActiveSpellCount = 218103848,
        IconOverlay = 218103849,
        IconUnderlay = 218103850,
        //Species = 2,
        Burden = 5,
        EquippedSlots = 10,
        RareId = 17,
        Value = 19,
        //TotalValue = 20,
        //SkillCreditsAvail = 24,
        //CreatureLevel = 25,
        //RestrictedToToD = 26,
        ArmorLevel = 28,
        //Rank = 30,
        Bonded = 33,
        //NumberFollowers = 35,
        Unenchantable = 36,
        //LockpickDifficulty = 38,
        //Deaths = 43,
        WandElemDmgType = 45,
        MinLevelRestrict = 86,
        MaxLevelRestrict = 87,
        //LockpickSkillBonus = 88,
        //AffectsVitalId = 89,
        //AffectsVitalAmt = 90,
        HealKitSkillBonus = 90,
        UsesTotal = 91,
        UsesRemaining = 92,
        //DateOfBirth = 98,
        Workmanship = 105,
        Spellcraft = 106,
        CurrentMana = 107,
        MaximumMana = 108,
        LoreRequirement = 109,
        RankRequirement = 110,
        //PortalRestrictions = 111,
        //Gender = 113,
        Attuned = 114,
        SkillLevelReq = 115,
        //ManaCost = 117,
        //Age = 125,
        //XPForVPReduction = 129,
        Material = 131,
        WieldReqType = 158,
        WieldReqAttribute = 159,
        WieldReqValue = 160,
        SlayerSpecies = 166,
        //NumberItemsSalvagedFrom = 170,
        NumberTimesTinkered = 171,
        //DescriptionFormat = 172,
        //PagesUsed = 174,
        //PagesTotal = 175,
        ActivationReqSkillId = 176,
        //GemSettingQty = 177,
        //GemSettingType = 178,
        Imbued = 179,
        //Heritage = 188,
        //FishingSkill = 192,
        //KeysHeld = 193,
        ElementalDmgBonus = 204,
        //CleaveType = 263,
        ArmorSet = 265,
        Slot = 231735296,
        DamRating = 370,
        DamResistRating = 371,
        CritRating = 372,
        CritResist = 373,
        CritDamRating = 374,
        CritDamResistRating = 375,
        HealBoostRating = 376,
        VitalityRating = 379,
        SummoningMastery = 362
    }
    public enum MSBoolValueKey
    {
        //Lockable = 201326592,
        Inscribable = 201326593,
        //Open = 2,
        //Locked = 3,
        HookVisibility = 24,
        UnlimitedUses = 0x3F,
        CanBeSold = 69,
        Retained = 91,
        Ivoryable = 99,
        Dyeable = 100//,
        //AwayFromKeyboard = 110
    }

    public enum MSDoubleValueKey
    {
        SlashProt = 167772160,
        PierceProt = 167772161,
        BludgeonProt = 167772162,
        AcidProt = 167772163,
        LightningProt = 167772164,
        FireProt = 167772165,
        ColdProt = 167772166,
        Heading = 167772167,
        ApproachDistance = 167772168,
        SalvageWorkmanship = 167772169,
        Scale = 167772170,
        Variance = 167772171,
        AttackBonus = 167772172,
        Range = 167772173,
        DamageBonus = 167772174,
        ManaRateOfChange = 5,
        MeleeDefenseBonus = 29,
        ManaTransferEfficiency = 87,
        HealingKitRestoreBonus = 100,
        ManaStoneChanceDestruct = 137,
        ManaCBonus = 144,
        BitingStrikeChance = 147,
        MissileDBonus = 149,
        MagicDBonus = 150,
        ElementalDamageVersusMonsters = 152
    }
}