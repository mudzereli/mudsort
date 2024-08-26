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

namespace mudsort {

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
    public string ocfilter = "";

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
                if (worldObject.Values(LongValueKey.EquippedSlots, 0) == 0 
                        && Core.WorldFilter[worldObject.Id].Values(LongValueKey.Slot) != -1 
                        && !worldObject.ObjectClass.Equals(ObjectClass.Foci) 
                        && (MainView.cmbObjClassFilters.Current == 0
                            || (MainView.cmbObjClassFilters.Current != 0 && worldObject.ObjectClass.ToString().ToLower().StartsWith(ocfilter.ToLower()))))
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
        //Util.WriteToChat("creating list from " + sortKeys.Length + " keys");
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
        //Util.WriteToChat("SortFlags Contains " + sortFlags.Count + " flags");
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
                System.Collections.ArrayList sortValueList = new System.Collections.ArrayList();
                for (int i = sortFlags.Count - 1; i >= 0; i--)
                {
                    SortFlag sf = (SortFlag)sortFlags[i];
                    foreach (WorldObject worldObject in sortList)
                    {
                        String sortMetric = sf.valueOf(worldObject);
                        if (!sortValueList.Contains(sortMetric))
                        {
                            sortValueList.Add(sortMetric);
                        }
                    }
                    sortValueList.Sort(new AlphanumComparator());
                    System.Collections.ArrayList newSortList = new System.Collections.ArrayList();
                    if (sf.descending)
                    {
                        sortValueList.Reverse();
                    }
                    foreach (Object sortValue in sortValueList)
                    {
                        foreach (WorldObject worldObject in sortList)
                        {
                            String sortMetric = sf.valueOf(worldObject);
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
                    if(Properties.Settings.Default.ThinkWhenDone)
                    {
                        Globals.Host.Actions.InvokeChatParser("/tell "+Core.CharacterFilter.Name+", done sorting!");
                    }
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
            if (iFlag.key is MSStringValueKey) {
                id = 2;
            } else if (iFlag.key is MSLongValueKey) {
                id = 3;
            } else if (iFlag.key is DoubleValueKey) {
                id = 4;
            } else if (iFlag.key is MSBoolValueKey) {
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
        //MainView.edtSortString.Text = sortFlagListToString();
    }

    public void setDestContainer()
    {
        setDestContainer(Core.Actions.CurrentSelection);
    }

    public void setDestContainer(int containerID)
    {
        try
        {
            if (containerID != 0 && Core.WorldFilter[containerID].Values(LongValueKey.ItemSlots) > 0)
            {
                containerDest = containerID;
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
        setSourceContainer(Core.Actions.CurrentSelection);
    }

    public void setSourceContainer(int containerID)
    {
        try
        {
            if (containerID != 0 && Core.WorldFilter[containerID].Values(LongValueKey.ItemSlots) > 0)
            {
                containerSource = containerID;
            }
            else
            {
                containerSource = 0;
            }
            MainView.edtSourceContainer.Text = containerSource != 0 ? containerSource.ToString() : "invalid!";
        }
        catch (Exception ex) { Util.LogError(ex); }
    }

    private String sortFlagListToString()
    {
        String s = "";
        foreach (SortFlag iFlag in sortFlags)
        {
            s = s + iFlag.code + (iFlag.descending ? "-" : "") + (sortFlags.IndexOf(iFlag) != sortFlags.Count - 1 ? "," : "");
        }
        return s;
    }

    void Current_CommandLineText(object sender, ChatParserInterceptEventArgs e)
    {
        try
        {
            if (e.Text == null)
                return;
            if (ProcessMSCommand(e.Text))
                e.Eat = true;
        }
        catch (Exception ex) { Util.LogError(ex); }
    }

    public bool ProcessMSCommand(string msCommand)
    {
        msCommand = msCommand.ToLower().Trim();

        if (msCommand.StartsWith("/ms help") || msCommand.Equals("/ms"))
        {
            Util.WriteToChat("listing help / commands...");
            Util.WriteToChat("/ms start - start sorting");
            Util.WriteToChat("/ms stop - cancel sorting");
            Util.WriteToChat("/ms set source (pack <1-8>|player|<containerID>) - set source container to: pack 1-8, player, given containerID, or selection (no argument)");
            Util.WriteToChat("/ms set dest (pack <1-8>|player|<containerID>) - set destination to: pack 1-8, player, given containerID, or selection (no argument)");
            Util.WriteToChat("/ms set flags (sort flag string) - sets the sort string to the given argument");
            Util.WriteToChat("/ms set ocfilter (object class string) - sets the object class filter to the given argument");
            Util.WriteToChat("/ms clear ocfilter - clears the object class filter");
            return true;
        }

        if (msCommand.Equals("/ms set ocfilter")|| msCommand.Equals("/ms clear ocfilter"))
        {
            msCommand = msCommand.Substring("/ms set ocfilter ".Length);
            Util.WriteToChat("Clearing ObjectClass Filter.");
            ocfilter = "";
            MainView.cmbObjClassFilters.Current = 0;
            return true;
        }

        if (msCommand.StartsWith("/ms set ocfilter "))
        {
            msCommand = msCommand.Substring("/ms set ocfilter ".Length);
            Util.WriteToChat("Setting ObjectClass Filter: " + msCommand);
            ocfilter = msCommand;
            MainView.cmbObjClassFilters.Current = MainView.cmbObjClassFilters.Count-1;
            return true;
        }

        if (msCommand.Equals("/ms set source"))
        {
            Util.WriteToChat("Setting Source Container to Selection...");
            setSourceContainer();
            return true;
        }

        if (msCommand.Equals("/ms set dest"))
        {
            Util.WriteToChat("Setting Destination Container to Selection...");
            setDestContainer();
            return true;
        }

        if (msCommand.StartsWith("/ms set source "))
        {
            msCommand = msCommand.Substring("/ms set source ".Length).ToLower();
            if (msCommand.StartsWith("player"))
            {
                Util.WriteToChat("Setting Source Container to Player ID: " + Core.CharacterFilter.Id);
                setSourceContainer(Core.CharacterFilter.Id);
                return true;
            }
            if (msCommand.StartsWith("pack"))
            {
                msCommand = msCommand.Substring("pack".Length);
                foreach (WorldObject worldObject in Core.WorldFilter.GetByContainer(Core.CharacterFilter.Id))
                {
                    if (worldObject.Values(LongValueKey.EquippedSlots, 0) == 0
                            && worldObject.ObjectClass == ObjectClass.Container
                            && worldObject.Values(LongValueKey.Slot) + 1 == Int32.Parse(msCommand)
                            && !worldObject.ObjectClass.Equals(ObjectClass.Foci))
                    {
                        Util.WriteToChat("Setting Source Container to: " + worldObject.Id + "...");
                        setSourceContainer(worldObject.Id);
                        return true;
                    }
                }
            }
            else
            {
                Util.WriteToChat("Setting Source Container to: " + msCommand + "...");
                setSourceContainer(Int32.Parse(msCommand));
                return true;
            }
            return true;
            }

        if (msCommand.StartsWith("/ms set dest "))
        {
            msCommand = msCommand.Substring("/ms set dest ".Length).ToLower();
            if (msCommand.StartsWith("player"))
            {
                Util.WriteToChat("Setting Destination Container to Player ID: " + Core.CharacterFilter.Id);
                setDestContainer(Core.CharacterFilter.Id);
                return true;
            }
            if (msCommand.StartsWith("pack"))
            {
                msCommand = msCommand.Substring("pack".Length);
                foreach (WorldObject worldObject in Core.WorldFilter.GetByContainer(Core.CharacterFilter.Id))
                {
                    if (worldObject.Values(LongValueKey.EquippedSlots, 0) == 0
                            && worldObject.ObjectClass == ObjectClass.Container
                            && worldObject.Values(LongValueKey.Slot) + 1 == Int32.Parse(msCommand)
                            && !worldObject.ObjectClass.Equals(ObjectClass.Foci))
                        {
                            Util.WriteToChat("Setting Destination Container to: " + worldObject.Id + "...");
                            setDestContainer(worldObject.Id);
                            return true;
                    }
                }
            } else
            {
                Util.WriteToChat("Setting Destination Container to: " + msCommand + "...");
                setDestContainer(Int32.Parse(msCommand));
                return true;
            }
            return true;
        }

        if (msCommand.StartsWith("/ms set flags "))
        {
            msCommand = msCommand.Substring("/ms set flags ".Length).ToUpper();
            Util.WriteToChat("Updated Sort Flags: "+msCommand);
            getInstance().createSortFlagListFromString(msCommand);
            getInstance().rebuildLstSortSettings();
            return true;
        }

        if (msCommand.StartsWith("/ms start"))
        {
            Util.WriteToChat("Starting Sort...");
            getInstance().activate();
            return true;
        }

        if (msCommand.StartsWith("/ms stop"))
        {
            Util.WriteToChat("Stopping Sort...");
            getInstance().cancel();
            return true;
        }
        return false;
    }

    protected override void Startup()
    {
        try
        {
            instance = this;
            Globals.Init("mudsort", Host, Core);
            MainView.ViewInit();
            CoreManager.Current.CommandLineText += new EventHandler<ChatParserInterceptEventArgs>(Current_CommandLineText);
        } catch (Exception ex) { Util.LogError(ex); }
    }

    protected override void Shutdown()
    {
        try
        {
            CoreManager.Current.CommandLineText -= new EventHandler<ChatParserInterceptEventArgs>(Current_CommandLineText);
        }
        catch (Exception ex) { Util.LogError(ex); }
    }
}}
