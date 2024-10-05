﻿using System;
using System.Collections;
using Decal.Adapter.Wrappers;
using System.Collections.Generic;

namespace mudsort
{
    public class SortFlag
    {
        public static SortedList sortedFlagList = new SortedList(new AlphanumComparator());

        public static SortFlag OBJECT_CLASS = new SortFlag("ObjectClass",0x29D1,"OC", "OC");
        public static SortFlag CALCED_TOTAL_RATINGS = new SortFlag("CalcedTotalRatings", 0x29D1, "TR", "TR");
        public static SortFlag BUFFED_WEAPON_DAMAGE = new SortFlag("BuffedWeaponDamage", 0x29D1, "BW", "BW");
        public static SortFlag BUFFED_ELEMENTAL_DAMAGE = new SortFlag("BuffedElementalDamage", 0x29D1, "BE", "BE");
        public static SortFlag BUFFED_MELEE_DEFENSE = new SortFlag("BuffedMeleeDefense", 0x29D1, "BD", "BD");
        public static SortFlag BUFFED_ATTACK_BONUS = new SortFlag("BuffedAttackBonus", 0x29D1, "BA", "BA");
        public static SortFlag BUFFED_ARMOR_LEVEL = new SortFlag("BuffedArmorLevel", 0x29D1, "BP", "BP");
        public static SortFlag BUFFED_MANA_CONVERSION = new SortFlag("BuffedManaConversion", 0x29D1, "BM", "BM");

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
                    int keyIcon = 0x29D1;
                    if (key is MSStringValueKey)
                    {
                        keyIcon = 0x29CC;
                    }
                    else if (key is MSLongValueKey)
                    {
                        keyIcon = 0x29CD;
                    }
                    else if (key is MSDoubleValueKey)
                    {
                        keyIcon = 0x29CE;
                    }
                    else if (key is MSBoolValueKey)
                    {
                        keyIcon = 0x29CF;
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
                            case 4958: val = val + 10; break;
                            case 4959: val = val + 10; break;
                            case 4960: val = val + 10; break;
                            case 4961: val = val + 10; break;
                            case 4962: val = val + 10; break;
                            case 4963: val = val + 10; break;
                            case 4964: val = val + 10; break;
                            case 4965: val = val + 10; break;
                            case 4966: val = val + 10; break;
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
                            case 2680: val = val + 3; break;
                            case 2465: val = val + 4; break;
                            case 2456: val = val + 4; break;
                            case 2468: val = val + 4; break;
                            case 3222: val = val + 4; break;
                            case 2459: val = val + 4; break;
                            case 3228: val = val + 4; break;
                            case 2462: val = val + 4; break;
                            case 3219: val = val + 4; break;
                            case 3234: val = val + 4; break;
                            case 3225: val = val + 4; break;
                            case 3231: val = val + 4; break;
                            case 2560: val = val + 5; break;
                            case 2457: val = val + 8; break;
                            case 3229: val = val + 8; break;
                            case 3232: val = val + 8; break;
                            case 2463: val = val + 8; break;
                            case 5028: val = val + 8; break;
                            case 2466: val = val + 8; break;
                            case 2469: val = val + 8; break;
                            case 3217: val = val + 8; break;
                            case 2460: val = val + 8; break;
                            case 3220: val = val + 8; break;
                            case 3223: val = val + 8; break;
                            case 3226: val = val + 8; break;
                            case 3568: val = val + 10; break;
                            case 2691: val = val + 10; break;
                            case 2458: val = val + 12; break;
                            case 2461: val = val + 12; break;
                            case 2464: val = val + 12; break;
                            case 3218: val = val + 12; break;
                            case 3227: val = val + 12; break;
                            case 3221: val = val + 12; break;
                            case 2467: val = val + 12; break;
                            case 3224: val = val + 12; break;
                            case 3233: val = val + 12; break;
                            case 2455: val = val + 12; break;
                            case 3230: val = val + 12; break;
                            case 5029: val = val + 12; break;
                            case 2525: val = val + 15; break;
                            case 5027: val = val + 15; break;
                            case 4705: val = val + 25; break;
                            case 6064: val = val + 35; break;
                        }
                    }
                }
                return val;
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