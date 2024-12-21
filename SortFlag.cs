using System;
using System.Collections;
using Decal.Adapter.Wrappers;
using System.Collections.Generic;

namespace mudsort
{
    public class SortFlag
    {
        const int FLAG_LONG = 0x06003353;
        const int FLAG_DOUBLE = 0x0600335C;
        const int FLAG_STRING = 0x0600335A;
        const int FLAG_BOOL = 0x06003356;
        const int FLAG_CUSTOM = 0x06003354;

        public static SortedList sortedFlagList = new SortedList(new AlphanumComparator());

        public static SortFlag OBJECT_CLASS = new SortFlag("ObjectClass", FLAG_CUSTOM, "OC", "OC");
        public static SortFlag CALCED_TOTAL_RATINGS = new SortFlag("CalcedTotalRatings", FLAG_CUSTOM, "TR", "TR");
        public static SortFlag BUFFED_WEAPON_DAMAGE = new SortFlag("BuffedWeaponDamage", FLAG_CUSTOM, "BW", "BW");
        public static SortFlag BUFFED_ELEMENTAL_DAMAGE = new SortFlag("BuffedElementalDamage", FLAG_CUSTOM, "BE", "BE");
        public static SortFlag BUFFED_MELEE_DEFENSE = new SortFlag("BuffedMeleeDefense", FLAG_CUSTOM, "BD", "BD");
        public static SortFlag BUFFED_ATTACK_BONUS = new SortFlag("BuffedAttackBonus", FLAG_CUSTOM, "BA", "BA");
        public static SortFlag BUFFED_ARMOR_LEVEL = new SortFlag("BuffedArmorLevel", FLAG_CUSTOM, "BP", "BP");
        public static SortFlag BUFFED_MANA_CONVERSION = new SortFlag("BuffedManaConversion", FLAG_CUSTOM, "BM", "BM");
        public static SortFlag TOTAL_MISSILE_DAMAGE = new SortFlag("TotalMissileDamage", FLAG_CUSTOM, "TM", "TM");
        public static SortFlag TOTAL_SUMMON_DAMAGE = new SortFlag("TotalSummonDamage", FLAG_CUSTOM, "TS", "TS");

        public String name;
        public String code;
        public Object key;
        public bool descending = false;
        public int keyIcon;
        public static ArrayList CommonFlags = new ArrayList();

        static SortFlag() {
            try
            {
                CommonFlags.Add("ObjectClass");
                CommonFlags.Add("EquipableSlots");
                CommonFlags.Add("Coverage");
                CommonFlags.Add("ArmorLevel");
                CommonFlags.Add("AttackBonus");
                CommonFlags.Add("DamageBonus");
                CommonFlags.Add("ElementalDmgBonus");
                CommonFlags.Add("MinLevelRestrict");
                CommonFlags.Add("Material");
                CommonFlags.Add("Name");
                CommonFlags.Add("ActivationReqSkillId");
                CommonFlags.Add("ArmorSet");
                CommonFlags.Add("Attuned");
                CommonFlags.Add("Bonded");
                CommonFlags.Add("Burden");
                CommonFlags.Add("CurrentMana");
                CommonFlags.Add("DamageType");
                CommonFlags.Add("Heritage");
                CommonFlags.Add("Icon");
                CommonFlags.Add("MaxDamage");
                CommonFlags.Add("RareId");
                CommonFlags.Add("SkilLevelReq");
                CommonFlags.Add("SlayerSpecies");
                CommonFlags.Add("StackCount");
                CommonFlags.Add("Value");
                CommonFlags.Add("WieldReqAttribute");
                CommonFlags.Add("WieldReqType");
                CommonFlags.Add("WieldReqValue");
                CommonFlags.Add("Workmanship");
                CommonFlags.Add("MeleeDefenseBonus");
                CommonFlags.Add("Variance");
                ArrayList codes = new ArrayList();
                codes.Add(OBJECT_CLASS.code);
                codes.Add(CALCED_TOTAL_RATINGS.code);
                codes.Add(BUFFED_WEAPON_DAMAGE.code);
                codes.Add(BUFFED_ELEMENTAL_DAMAGE.code);
                codes.Add(BUFFED_MELEE_DEFENSE.code);
                codes.Add(BUFFED_ATTACK_BONUS.code);
                codes.Add(BUFFED_ARMOR_LEVEL.code);
                codes.Add(BUFFED_MANA_CONVERSION.code);
                codes.Add(TOTAL_MISSILE_DAMAGE.code);
                codes.Add(TOTAL_SUMMON_DAMAGE.code);
                ArrayList enums = new ArrayList();
                enums.AddRange(Enum.GetValues(typeof(MSStringValueKey)));
                enums.AddRange(Enum.GetValues(typeof(MSLongValueKey)));
                enums.AddRange(Enum.GetValues(typeof(MSDoubleValueKey)));
                enums.AddRange(Enum.GetValues(typeof(MSBoolValueKey)));
                foreach (var key in enums)
                {
                    String name = key.ToString();
                    // set up code
                    String code = "";
                    foreach (Char c in name)
                    {
                        if ("ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(c.ToString()))
                        {
                            code = code + c;
                            if (code.Length == 2)
                            {
                                if (codes.Contains(code))
                                {
                                    code = code.Substring(0, 1);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (code.Length < 2)
                    {
                        foreach (Char c in name)
                        {
                            if ("bcdfghjklmnpqrstvwxyz".Contains(c.ToString()))
                            {
                                code = code + c;
                            }
                            if (code.Length == 2)
                            {
                                if (codes.Contains(code.ToUpper()))
                                {
                                    code = code.Substring(0, 1);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        code = code.ToUpper();
                    }
                    if (code.Length < 2)
                    {
                        code = name.Substring(0, 2);
                        code = code.ToUpper();
                    }
                    int keyIcon = SortFlag.FLAG_CUSTOM;
                    if (key is MSStringValueKey)
                    {
                        keyIcon = SortFlag.FLAG_STRING;
                    }
                    else if (key is MSLongValueKey)
                    {
                        keyIcon = SortFlag.FLAG_LONG;
                    }
                    else if (key is MSDoubleValueKey)
                    {
                        keyIcon = SortFlag.FLAG_DOUBLE;
                    }
                    else if (key is MSBoolValueKey)
                    {
                        keyIcon = SortFlag.FLAG_BOOL;
                    }
                    if (!codes.Contains(code) && !sortedFlagList.ContainsKey(keyIcon + name))
                    {
                        codes.Add(code);
                        new SortFlag(name, keyIcon, code, key);
                    }
                    else
                    {
                        Util.Log("duplicate code entry : " + code);
                    }
                }
            }
            catch (Exception e) { Util.LogError(e); }
        }

        public SortFlag(String name, int keyIcon, String code, Object key)
        {
            this.name = name;
            this.code = code;
            this.key = key;
            this.keyIcon = keyIcon;
            sortedFlagList.Add(keyIcon + name, this);
        }

        public static SortFlag decode(String decode)
        {
            //Util.WriteToChat("[MudSort] decoding string: " + decode);
            foreach (SortFlag flag in sortedFlagList.Values)
            {
                if (decode.Length >= 2 && flag.code.Equals(decode.Substring(0,2)))
                {
                    return flag;
                }
            }
            return null;
        }

        public String valueOf(WorldObject obj)
        {
            if (key is MSDoubleValueKey)
            {
                return (((int) ((Double) directValueOf(obj) * 10000)).ToString());
            }
            else if (this == BUFFED_ELEMENTAL_DAMAGE || this == BUFFED_ATTACK_BONUS || this == BUFFED_MANA_CONVERSION || this == BUFFED_MELEE_DEFENSE || this == TOTAL_SUMMON_DAMAGE)
            {
                return (((int) ((Double) directValueOf(obj) * 10000)).ToString());
            }
            else
            {
                return directValueOf(obj).ToString();
            }
        }

        public Object directValueOf(WorldObject obj)
        {
            if (this == OBJECT_CLASS)
            {
                return obj.ObjectClass;
            }
            else if (this == CALCED_TOTAL_RATINGS)
            {
                return obj.Values((LongValueKey)MSLongValueKey.DamRating)
                    + obj.Values((LongValueKey)MSLongValueKey.DamResistRating)
                    + obj.Values((LongValueKey)MSLongValueKey.CritRating)
                    + obj.Values((LongValueKey)MSLongValueKey.CritResist)
                    + obj.Values((LongValueKey)MSLongValueKey.CritDamRating)
                    + obj.Values((LongValueKey)MSLongValueKey.CritDamResistRating)
                    + obj.Values((LongValueKey)MSLongValueKey.HealBoostRating)
                    + obj.Values((LongValueKey)MSLongValueKey.VitalityRating);
            }
            else if (this == BUFFED_WEAPON_DAMAGE)
            {
                int val = obj.Values((LongValueKey)MSLongValueKey.MaxDamage);
                if (obj.SpellCount > 0)
                {
                    for (int i = 0; i < obj.SpellCount; i++)
                    {
                        int spellID = obj.Spell(i);
                        switch (spellID)
                        {
                            case 2453: val = val + 2; break;
                            case 2486: val = val + 2; break;
                            case 2487: val = val + 2; break;
                            case 2598: val = val + 2; break;
                            case 3828: val = val + 3; break;
                            case 2454: val = val + 4; break;
                            case 2586: val = val + 4; break;
                            case 2629: val = val + 5; break;
                            case 2452: val = val + 6; break;
                            case 4661: val = val + 7; break;
                            case 6089: val = val + 10; break;
                        }
                    }
                }
                return val;
            }
            else if (this == BUFFED_ARMOR_LEVEL)
            {
                int val = obj.Values((LongValueKey)MSLongValueKey.ArmorLevel);
                if (obj.SpellCount > 0)
                {
                    for (int i = 0; i < obj.SpellCount; i++)
                    {
                        int spellID = obj.Spell(i);
                        switch (spellID)
                        {
                            case 2604: val = val + 20; break;
                            case 2592: val = val + 40; break;
                            case 4667: val = val + 60; break;
                            case 2349: val = val + 170; break;
                            case 2948: val = val + 220; break;
                        }
                    }
                }
                return val;
            }
            else if (this == BUFFED_ELEMENTAL_DAMAGE)
            {
                double val = obj.Values((DoubleValueKey)MSDoubleValueKey.ElementalDamageVersusMonsters);
                if (obj.SpellCount > 0)
                {
                    for (int i = 0;i < obj.SpellCount;i++)
                    {
                        int spellID = obj.Spell(i);
                        switch (spellID)
                        {
                            case 3251: val = val + 0.01; break;
                            case 6035: val = val + 0.01; break;
                            case 2352: val = val + 0.02; break;
                            case 3250: val = val + 0.03; break;
                            case 4670: val = val + 0.05; break;
                            case 6098: val = val + 0.07; break;
                        }
                    }
                }
                return val;
            }
            else if (this == BUFFED_MELEE_DEFENSE)
            {
                double val = obj.Values((DoubleValueKey)MSDoubleValueKey.MeleeDefenseBonus);
                if (obj.SpellCount > 0)
                {
                    for (int i = 0; i < obj.SpellCount; i++)
                    {
                        int spellID = obj.Spell(i);
                        switch (spellID)
                        {
                            case 2600: val = val + 0.03; break;
                            case 3985: val = val + 0.04; break;
                            case 2588: val = val + 0.05; break;
                            case 4663: val = val + 0.07; break;
                            case 2488: val = val + 0.08; break;
                            case 6091: val = val + 0.09; break;
                        }
                    }
                }
                return val;
            }
            else if (this == BUFFED_ATTACK_BONUS)
            {
                double val = obj.Values((DoubleValueKey)MSDoubleValueKey.AttackBonus);
                if (obj.SpellCount > 0)
                {
                    for (int i = 0; i < obj.SpellCount; i++)
                    {
                        int spellID = obj.Spell(i);
                        switch (spellID)
                        {
                            case 2438: val = val + 0.01; break;
                            case 2439: val = val + 0.03; break;
                            case 2603: val = val + 0.03; break;
                            case 3984: val = val + 0.04; break;
                            case 2437: val = val + 0.05; break;
                            case 2591: val = val + 0.05; break;
                            case 2630: val = val + 0.06; break;
                            case 4666: val = val + 0.07; break;
                            case 6094: val = val + 0.09; break;
                        }
                    }
                }
                return val;
            }
            else if (this == BUFFED_MANA_CONVERSION)
            {
                double val = obj.Values((DoubleValueKey)MSDoubleValueKey.ManaCBonus);
                if (obj.SpellCount > 0)
                {
                    for (int i = 0; i < obj.SpellCount; i++)
                    {
                        int spellID = obj.Spell(i);
                        switch (spellID)
                        {
                            case 3201: val = val * 1.05; break;
                            case 3199: val = val * 1.1; break;
                            case 3202: val = val * 1.15; break;
                            case 3200: val = val * 1.2; break;
                            case 6086: val = val * 1.25; break;
                            case 6087: val = val * 1.3; break;
                        }
                    }
                }
                return val;
            }
            else if (this == TOTAL_MISSILE_DAMAGE)
            {
                double baseMod = obj.Values((DoubleValueKey)MSDoubleValueKey.DamageBonus);
                if (baseMod == 0)
                    return 0;
                int elementalDMG = obj.Values((LongValueKey)MSLongValueKey.ElementalDmgBonus);
                int cantripDMG = 0;
                int baseDMG = (int)((baseMod - 1) * 100 / 3);
                if (obj.SpellCount > 0)
                {
                    for (int i = 0; i < obj.SpellCount; i++)
                    {
                        int spellID = obj.Spell(i);
                        switch (spellID)
                        {
                            case 2453: cantripDMG = cantripDMG + 2; break;
                            case 2486: cantripDMG = cantripDMG + 2; break;
                            case 2487: cantripDMG = cantripDMG + 2; break;
                            case 2598: cantripDMG = cantripDMG + 2; break;
                            case 3828: cantripDMG = cantripDMG + 3; break;
                            case 2454: cantripDMG = cantripDMG + 4; break;
                            case 2586: cantripDMG = cantripDMG + 4; break;
                            case 2629: cantripDMG = cantripDMG + 5; break;
                            case 2452: cantripDMG = cantripDMG + 6; break;
                            case 4661: cantripDMG = cantripDMG + 7; break;
                            case 6089: cantripDMG = cantripDMG + 10; break;
                        }
                    }
                }
                return baseDMG + elementalDMG + cantripDMG;
            }
            else if (this == TOTAL_SUMMON_DAMAGE)
            {
                //((MaxDam + MinDam)/2 * DamageRating * (1 - Crit Rate)) + (MaxDam * 2 [CritMod] * TotalCritDamageRating * CritRate)
                //int maxDMG = 100;
                //int minDMG = 25;
                int ratingDMG = obj.Values((LongValueKey)MSLongValueKey.DamRating);
                int ratingCRIT = obj.Values((LongValueKey)MSLongValueKey.CritRating);
                int ratingCRITDAM = obj.Values((LongValueKey)MSLongValueKey.CritDamRating);
                //double avgDMG = (maxDMG + minDMG) / 2.0; // Fix integer division
                double DamageRating = 1 + (ratingDMG / 100.0); // Fix integer division
                double CritRating = (ratingCRIT + 10) / 100.0; // Fix integer division
                double CritMod = (100 + ratingDMG + ratingCRITDAM) / 100.0; // Fix integer division
                //double formulaCalc = (avgDMG * DamageRating * (1 - CritRating)) + (maxDMG * 2 * CritMod * CritRating);
                double formulaCalc = (0.625 * (1 + ratingDMG / 100.0) * (.9 - ratingCRIT / 100.0) + 2 * (1 + (ratingDMG + ratingCRITDAM) / 100.0) * (.1 + ratingCRIT / 100.0)) / 0.01365;
                return (double)formulaCalc;
                // ((MaxDam + MinDam)/2 * DamageRating * (1 - Crit Rate)) + (MaxDam * 2 [CritMod] * TotalCritDamageRating * CritRate)
                // (0.625 * (1 +$D / 100)*(.9 -$C / 100)+2 * (1 + ($D +$CD)/ 100)*(.1 +$C / 100))/ 0.01365
            }
            else if (key is MSStringValueKey)
            {
                return obj.Values((StringValueKey)key);
            }
            else if (key is MSLongValueKey)
            {
                return obj.Values((LongValueKey)key);
            }
            else if (key is MSDoubleValueKey)
            {
                return obj.Values((DoubleValueKey)key);
            }
            else if (key is MSBoolValueKey)
            {
                return obj.Values((BoolValueKey)key);
            }
            else
            {
                return obj;
            }
        }

        public String propertyDumpSelection()
        {
            WorldObject obj = Globals.Core.WorldFilter[Globals.Core.Actions.CurrentSelection];
            String props = obj.Values(StringValueKey.Name) + " : " + name.ToString() + " : " + directValueOf(obj);
            Util.WriteToChat(props);
            //Util.WriteToChat("============================================");
            //for (int i = 0;i <= obj.SpellCount;i++)
            //{
            //    Util.WriteToChat("Spell: "+obj.Spell(i).ToString());
            //}
            return props;
        }
    }
}