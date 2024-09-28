using System;
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
        //public static SortFlag BITING_STRIKE = new SortFlag("BitingStrikeChance", 0x29CE, "BS", "BS");

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
                    + obj.Values((LongValueKey)MSLongValueKey.CritDamRating)
                    + obj.Values((LongValueKey)MSLongValueKey.CritDamResistRating)
                    + obj.Values((LongValueKey)MSLongValueKey.HealBoostRating)
                    + obj.Values((LongValueKey)MSLongValueKey.VitalityRating);
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
            return props;
        }
    }
}