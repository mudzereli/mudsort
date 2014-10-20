using System;
using System.Collections;

using Decal.Adapter;
using Decal.Adapter.Wrappers;
using VirindiViewService.Controls;
using System.Collections.Generic;

/*
 * Template created by Mag-nus. 8/19/2011, VVS added by Virindi-Inquisitor.
 * Plugin created by mudzereli 2012/12
*/

namespace mudsort
{

[WireUpBaseEvents]
[FriendlyName("mudsort")]
public class PluginCore : PluginBase
{

    const int ICON_ADD = 0x60011F9; // GREEN CIRCLE
    const int ICON_MOVE_DOWN = 0x60028FD; // RED DOWN ARROW
    const int ICON_MOVE_UP = 0x60028FC; // GREEN UP ARROW
    const int ICON_REMOVE = 0x60011F8; //RED CIRCLE SLASH
    private static PluginCore instance;
    public int containerDest = 0;
    public int containerDestSlot = 0;
    public int containerSource = 0;
    private State CURRENT_STATE = State.IDLE;
    private ArrayList sortFlags = new ArrayList();
    private ArrayList sortList = new ArrayList();
    private Queue sortQueue = new Queue();

    public static PluginCore getInstance()
    {
        return instance;
    }

    public void activate()
    {
        try
        {
            CURRENT_STATE = State.INITIATED;
            sortQueue.Clear();
            sortList.Clear();
            foreach (WorldObject worldObject in Core.WorldFilter.GetByContainer(containerSource))
            {
                if (worldObject.Values(LongValueKey.EquippedSlots, 0) == 0 && Core.WorldFilter[worldObject.Id].Values(LongValueKey.Slot) != -1 && !worldObject.ObjectClass.Equals(ObjectClass.Foci) && (MainView.cmbObjClassFilters.Current == 0 || worldObject.ObjectClass.ToString().Equals(((HudStaticText)MainView.cmbObjClassFilters[MainView.cmbObjClassFilters.Current]).Text)))
                {
                    addWorldObject(sortList, worldObject, false);
                }
            }
            Util.WriteToChat(sortList.Count + " items added to sort list...");
            CoreManager.Current.RenderFrame += new EventHandler<EventArgs>(Current_RenderFrame_Sort);
        }
        catch (Exception e) { Util.LogError(e); }
    }

    public void addWorldObject(System.Collections.IList toList, WorldObject worldObject, bool recursive)
    {
        if (!worldObject.HasIdData)
        {
            Core.IDQueue.AddToQueue(worldObject.Id);
        }
        if (worldObject.ObjectClass.Equals(ObjectClass.Container))
        {
            if (recursive)
            {
                foreach (WorldObject obj in Core.WorldFilter.GetByContainer(worldObject.Id))
                {
                    addWorldObject(toList, obj, recursive);
                }
            }
        }
        else
        {
            toList.Add(worldObject);
        }
    }

    public void cancel()
    {
        try
        {
            sortQueue.Clear();
            CURRENT_STATE = State.IDLE;
            CoreManager.Current.RenderFrame -= new EventHandler<EventArgs>(Current_RenderFrame_Sort);
            MainView.prgProgressBar.Position = 0;
            MainView.prgProgressBar.PreText = "";
        }
        catch (Exception ex) { Util.LogError(ex); }
    }

    [BaseEvent("LoginComplete", "CharacterFilter")]
    private void CharacterFilter_LoginComplete(object sender, EventArgs e)
    {
        try
        {
            Util.WriteToChat("Plugin online.");
            cancel();
            createSortFlagListFromString(MainView.edtSortString.Text);
            rebuildLstSortSettings();

            if (Properties.Settings.Default.IdentifyOnLogin)
            {
                foreach (WorldObject obj in Core.WorldFilter.GetByContainer(Core.CharacterFilter.Id))
                {
                    if (obj.ObjectClass.Equals(ObjectClass.Container))
                    {
                        foreach (WorldObject o in Core.WorldFilter.GetByContainer(obj.Id))
                        {
                            Core.IDQueue.AddToQueue(obj.Id);
                        }
                    }
                    Core.IDQueue.AddToQueue(obj.Id);
                }
            }

            MainView.edtSourceContainer.Text = Core.CharacterFilter.Id.ToString();
            containerSource = Core.CharacterFilter.Id;
            MainView.edtDestContainer.Text = Core.CharacterFilter.Id.ToString();
            containerDest = Core.CharacterFilter.Id;
        }
        catch (Exception ex) { Util.LogError(ex); }
    }
    
    [BaseEvent("Logoff", "CharacterFilter")]
    private void CharacterFilter_Logoff(object sender, EventArgs e)
    {
        try
        {
            cancel();
            Util.WriteToChat("Plugin offline.");
        }
        catch (Exception ex) { Util.LogError(ex); }
    }

    public void createSortFlagListFromString(String str)
    {
        sortFlags.Clear();

        foreach (SortFlag sf in SortFlag.sortedFlagList.Values)
        {
            sf.descending = false;
        }

        String[] sortKeys = str.Split(',');
        for (int i = 0; i < sortKeys.Length; i++)
        {
            try
            {
                SortFlag sf = SortFlag.decode(sortKeys[i]);
                sf.descending = sortKeys[i].Length == 3 && sortKeys[i].Substring(2, 1).Equals("-");
                sortFlags.Add(sf);
            }
            catch (Exception e) { Util.LogError(e); }
        }
    }

    private void Current_RenderFrame_Sort(object sender, EventArgs e)
    {
        try
        {
            if (CURRENT_STATE == State.IDLE)
            {
                CoreManager.Current.RenderFrame -= new EventHandler<EventArgs>(Current_RenderFrame_Sort);
            }
            else if (CURRENT_STATE == State.INITIATED)
            {
                bool identifying = false;
                MainView.prgProgressBar.Max = sortList.Count;
                MainView.prgProgressBar.PreText = "identifying...";
                foreach (WorldObject obj in sortList)
                {
                    if (!obj.HasIdData)
                    {
                        identifying = true;
                        MainView.prgProgressBar.Position = sortList.IndexOf(obj) + 1;
                        break;
                    }
                }
                if (!identifying)
                {
                    CURRENT_STATE = State.BUILDING_LIST;
                    MainView.prgProgressBar.Position = MainView.prgProgressBar.Max;
                }
            }
            else if (CURRENT_STATE == State.BUILDING_LIST)
            {
                String[] sortKeys = MainView.edtSortString.Text.Split(',');
                System.Collections.ArrayList sortValueList = new System.Collections.ArrayList();
                for (int i = sortKeys.Length - 1; i >= 0; i--)
                {
                    foreach (WorldObject worldObject in sortList)
                    {
                        SortFlag sf = SortFlag.decode(sortKeys[i]);
                        String sortMetric = sf.valueOf(worldObject);
                        if (!sortValueList.Contains(sortMetric))
                        {
                            sortValueList.Add(sortMetric);
                        }
                    }
                    sortValueList.Sort(new AlphanumComparator());
                    System.Collections.ArrayList newSortList = new System.Collections.ArrayList();
                    if (!(sortKeys[i].Length == 3 && sortKeys[i].Substring(2, 1).Equals("-")))
                    {
                        sortValueList.Reverse();
                    }
                    foreach (Object sortValue in sortValueList)
                    {
                        foreach (WorldObject worldObject in sortList)
                        {
                            String sortMetric = SortFlag.decode(sortKeys[i]).valueOf(worldObject);
                            if (sortMetric.Equals(sortValue))
                            {
                                newSortList.Add(worldObject);
                            }
                        }
                    }
                    sortList = newSortList;
                    if (i == 0)
                    {
                        if (Properties.Settings.Default.ReverseSortList)
                        {
                            sortList.Reverse();
                        }
                        foreach (WorldObject worldObject in sortList)
                        {
                            sortQueue.Enqueue(worldObject);
                        }
                    }
                }
                Util.WriteToChat(sortQueue.Count + " items in queue...");
                CURRENT_STATE = State.MOVING_ITEMS;
                MainView.prgProgressBar.PreText = "working...";
                MainView.prgProgressBar.Max = sortQueue.Count;
            }
            else if (CURRENT_STATE == State.MOVING_ITEMS)
            {   
                if (sortQueue.Count > 0)
                {
                    if (Core.Actions.BusyState == 0)
                    {
                        MainView.prgProgressBar.Position = MainView.prgProgressBar.Max - sortQueue.Count;
                        WorldObject obj = (WorldObject)sortQueue.Dequeue();
                        if (containerDest != Core.CharacterFilter.Id && Core.WorldFilter[containerDest].ObjectClass.Equals(ObjectClass.Player))
                        {
                            Globals.Host.Actions.GiveItem(obj.Id, containerDest);
                        }
                        else
                        {
                            Globals.Host.Actions.MoveItem(obj.Id, containerDest, containerDestSlot, true);
                        }
                    }
                }
                else
                {
                    CURRENT_STATE = State.IDLE;
                    MainView.prgProgressBar.PreText = "done!";
                    MainView.prgProgressBar.Position = MainView.prgProgressBar.Max;
                    MainView.btnActivate.Text = "Activate";
                    Util.WriteToChat("done sorting items!");
                }
            }
        }
        catch (Exception ex) { Util.LogError(ex); }
    }

    public void lstSortSettings_Selected(object sender, int row, int col)
    {
        try
        {
            HudList.HudListRowAccessor acc = MainView.lstSortSettings[row];
            String code = ((HudStaticText) acc[0]).Text.ToString();
            SortFlag flag = SortFlag.decode(code);
            bool changed = false;
            if (col < 2) // DUMP PROPERTIES
            {
                flag.propertyDumpSelection();
            } else if (col == 2) // MOVE UP
            {
                if (sortFlags.Contains(flag))
                {
                    int index = sortFlags.IndexOf(flag);
                    if (index > 0)
                    {
                        SortFlag f = (SortFlag)sortFlags[index - 1];
                        sortFlags[index - 1] = sortFlags[index];
                        sortFlags[index] = f;
                        changed = true;
                    }
                }
            } else if (col == 3) // MOVE DOWN
            {
                if (sortFlags.Contains(flag))
                {
                    int index = sortFlags.IndexOf(flag);
                    if (index < sortFlags.Count - 1)
                    {
                        SortFlag f = (SortFlag)sortFlags[index + 1];
                        sortFlags[index + 1] = sortFlags[index];
                        sortFlags[index] = f;
                        changed = true;
                    }
                }
            }
            else if (col == 4) // REMOVE
            {
                if (sortFlags.Contains(flag))
                {
                    sortFlags.Remove(flag);
                    flag.descending = false;
                }
                else
                {
                    sortFlags.Add(flag);
                }
                changed = true;
            }
            else if (col == 5)
            {
                if (sortFlags.Contains(flag))
                {
                    flag.descending = !flag.descending;
                    changed = true;
                }
            }
            if (changed)
            {
                MainView.edtSortString.Text = sortFlagListToString();
                rebuildLstSortSettings();
            }
        }
        catch (Exception ex) { Util.LogError(ex); }
    }

    public void rebuildLstSortSettings()
    {
        MainView.lstSortSettings.ClearRows();
        foreach (SortFlag iFlag in sortFlags)
        {
            HudList.HudListRowAccessor row = MainView.lstSortSettings.AddRow();
            ((HudStaticText)row[0]).Text = iFlag.code;
            ((HudStaticText)row[1]).Text = iFlag.name;
            ((HudPictureBox)row[2]).Image = ICON_MOVE_UP;
            ((HudPictureBox)row[3]).Image = ICON_MOVE_DOWN;
            ((HudPictureBox)row[4]).Image = ICON_REMOVE;
            ((HudPictureBox)row[5]).Image = iFlag.descending ? ICON_MOVE_DOWN : ICON_MOVE_UP;
            ((HudPictureBox)row[6]).Image = iFlag.keyIcon;
            VirindiViewService.TooltipSystem.AssociateTooltip((HudStaticText)row[0], "Click To Dump Selected Item's " + iFlag.key.ToString() + " To Chat");
            VirindiViewService.TooltipSystem.AssociateTooltip((HudStaticText)row[1], "Click To Dump Selected Item's " + iFlag.key.ToString() + " To Chat");
            VirindiViewService.TooltipSystem.AssociateTooltip((HudPictureBox)row[2], "Click To Increase Sort Priority Of " + iFlag.key.ToString());
            VirindiViewService.TooltipSystem.AssociateTooltip((HudPictureBox)row[3], "Click To Decrease Sort Priority Of " + iFlag.key.ToString());
            VirindiViewService.TooltipSystem.AssociateTooltip((HudPictureBox)row[4], "Click To Remove Sorting By " + iFlag.key.ToString());
            VirindiViewService.TooltipSystem.AssociateTooltip((HudPictureBox)row[5], "Click To Reverse Sort Order Of " + iFlag.key.ToString());
            VirindiViewService.TooltipSystem.AssociateTooltip((HudPictureBox)row[6], iFlag.key.GetType().Name);
        }
        foreach (SortFlag iFlag in SortFlag.sortedFlagList.Values)
        {
            int id = 6;
            if (iFlag.key is StringValueKey) {
                id = 2;
            } else if (iFlag.key is LongValueKey) {
                id = 3;
            } else if (iFlag.key is DoubleValueKey) {
                id = 4;
            } else if (iFlag.key is BoolValueKey) {
                id = 5;
            }
            bool common = false;
            if (SortFlag.CommonFlags.Contains(iFlag.name))
            {
                common = true;
            }
            if (!sortFlags.Contains(iFlag) && ((MainView.cmbSortListFilters.Current == 0 && common) || MainView.cmbSortListFilters.Current == 1 || MainView.cmbSortListFilters.Current == id))
            {
                HudList.HudListRowAccessor row = MainView.lstSortSettings.AddRow();
                ((HudStaticText)row[0]).Text = iFlag.code;
                ((HudStaticText)row[1]).Text = iFlag.name;
                ((HudPictureBox)row[2]).Image = null;
                ((HudPictureBox)row[3]).Image = null;
                ((HudPictureBox)row[4]).Image = ICON_ADD;
                ((HudPictureBox)row[5]).Image = null;
                ((HudPictureBox)row[6]).Image = iFlag.keyIcon;
                VirindiViewService.TooltipSystem.AssociateTooltip((HudStaticText)row[0], "Click To Dump Selected Item's " + iFlag.key.ToString() + " To Chat");
                VirindiViewService.TooltipSystem.AssociateTooltip((HudStaticText)row[1], "Click To Dump Selected Item's " + iFlag.key.ToString() + " To Chat");
                VirindiViewService.TooltipSystem.AssociateTooltip((HudPictureBox)row[4], "Click To Sort By " + iFlag.key.ToString());
                VirindiViewService.TooltipSystem.AssociateTooltip((HudPictureBox)row[6], iFlag.key.GetType().Name);
            }

        }
    }

    public void setDestContainer()
    {
        try
        {
            int selected = Core.Actions.CurrentSelection;
            if (selected != 0 && Core.WorldFilter[selected].Values(LongValueKey.ItemSlots) > 0)
            {
                containerDest = selected;
            }
            else
            {
                containerDest = 0;
            }
            MainView.edtDestContainer.Text = containerDest != 0 ? containerDest.ToString() : "invalid!";
        }
        catch (Exception ex) { Util.LogError(ex); }
    }

    public void setSourceContainer()
    {
        try
        {
            int selected = Core.Actions.CurrentSelection;
            if (selected != 0 && Core.WorldFilter[selected].Values(LongValueKey.ItemSlots) > 0)
            {
                containerSource = selected;
            }
            else
            {
                containerSource = 0;
            }
            MainView.edtSourceContainer.Text = containerSource != 0 ? containerSource.ToString() : "invalid!";
        }
        catch (Exception ex) { Util.LogError(ex); }
    }

    protected override void Shutdown(){}

    private String sortFlagListToString()
    {
        String s = "";
        foreach (SortFlag iFlag in sortFlags)
        {
            s = s + iFlag.code + (iFlag.descending ? "-" : "") + (sortFlags.IndexOf(iFlag) != sortFlags.Count - 1 ? "," : "");
        }
        return s;
    }

    protected override void Startup()
    {
        try
        {
            instance = this;
            Globals.Init("mudsort", Host, Core);
            MainView.ViewInit();
        }
        catch (Exception ex) { Util.LogError(ex); }
    }
}
}
