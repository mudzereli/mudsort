using System;
using VirindiViewService;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using VirindiViewService.Controls;

namespace mudsort
{

class MainView : IDisposable
{

    public static HudButton btnActivate;
    public static HudCheckBox chkIdentifyOnLogin;
    public static HudCheckBox chkReverseSortList;
    public static HudCheckBox chkThinkWhenDone;
    public static HudCombo cmbObjClassFilters;
    public static HudCombo cmbSortListFilters;
    public static ControlGroup controls;
    public static HudTextBox edtDestContainer;
    public static HudTextBox edtInsertion;
    public static HudTextBox edtSavedSortString1;
    public static HudTextBox edtSavedSortString2;
    public static HudTextBox edtSavedSortString3;
    public static HudTextBox edtSortString;
    public static HudTextBox edtSourceContainer;
    public static HudList lstSortSettings;
    public static HudProgressBar prgProgressBar;
    public static ViewProperties properties;
    public static HudView View;

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public static void lstSortSettings_Selected(Object s, int row, int col) 
    {
        PluginCore.getInstance().lstSortSettings_Selected(s, row, col);
    }

    public static void ViewInit()
    {
        VirindiViewService.XMLParsers.Decal3XMLParser parser = new VirindiViewService.XMLParsers.Decal3XMLParser();
        parser.ParseFromResource("mudsort.mainView.xml", out properties, out controls);
        View = new VirindiViewService.HudView(properties, controls);

        // set up code for underlying controls
        edtSourceContainer  = View != null ? (HudTextBox)     View["edtSourceContainer"]    : new HudTextBox();
        edtDestContainer    = View != null ? (HudTextBox)     View["edtDestContainer"]      : new HudTextBox();
        edtInsertion        = View != null ? (HudTextBox)     View["edtInsertion"]          : new HudTextBox();
        cmbObjClassFilters  = View != null ? (HudCombo)       View["cmbObjClassFilters"]    : new HudCombo(new ControlGroup());
        edtSortString       = View != null ? (HudTextBox)     View["edtSortString"]         : new HudTextBox();
        prgProgressBar      = View != null ? (HudProgressBar) View["prgProgressBar"]        : new HudProgressBar();
        btnActivate         = View != null ? (HudButton)      View["btnActivate"]           : new HudButton();

        cmbSortListFilters  = View != null ? (HudCombo)View["cmbSortListFilters"] : new HudCombo(new ControlGroup());
        lstSortSettings     = View != null ? (HudList)View["lstSortSettings"]     : new HudList();

        chkIdentifyOnLogin  = View != null ? (HudCheckBox)View["chkIdentifyOnLogin"] : new HudCheckBox();
        chkThinkWhenDone    = View != null ? (HudCheckBox)View["chkThinkWhenDone"]   : new HudCheckBox();
        chkReverseSortList  = View != null ? (HudCheckBox)View["chkReverseSortList"] : new HudCheckBox();
        edtSavedSortString1 = View != null ? (HudTextBox)View["edtSavedSortString1"] : new HudTextBox();
        edtSavedSortString2 = View != null ? (HudTextBox)View["edtSavedSortString2"] : new HudTextBox();
        edtSavedSortString3 = View != null ? (HudTextBox)View["edtSavedSortString3"] : new HudTextBox();

        // associate tooltips with controls
        VirindiViewService.TooltipSystem.AssociateTooltip(View["btnSourceContainer"], "Sets the source Backpack/Person/Chest for sorting to your current Selection");
        VirindiViewService.TooltipSystem.AssociateTooltip(edtSourceContainer, "The Backpack/Person/Chest the items will move from when sorted (Default = Your Character ID)");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["btnDestContainer"], "Sets the destination Backpack/Person/Chest for sorting to your current Selection");
        VirindiViewService.TooltipSystem.AssociateTooltip(edtDestContainer, "The Backpack/Person/Chest the items will move to when sorted (Default = Your Character ID)");
        VirindiViewService.TooltipSystem.AssociateTooltip(edtInsertion, "The slot # you wish to start inserting at when sorting (Default = 0)");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["btnCopySortString"], "Copies the Sort String below to your clipboard");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["btnPasteSortString"], "Pastes the contents of your clipboard into the box below");
        VirindiViewService.TooltipSystem.AssociateTooltip(edtSortString, "The Sort String to use when sorting. (Use Build tab to create a new one)");
        VirindiViewService.TooltipSystem.AssociateTooltip(cmbObjClassFilters, "Limit sorting to specific types of items");
        VirindiViewService.TooltipSystem.AssociateTooltip(btnActivate, "Begins the sorting process. Press again to cancel.");

        VirindiViewService.TooltipSystem.AssociateTooltip(cmbSortListFilters, "Limit filter based on key type");

        VirindiViewService.TooltipSystem.AssociateTooltip(View["listHeaderCode"], "Sort Flag Code (Used in Sort String)");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["listHeaderName"], "Sort Flag Name");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["listHeaderUp"], "Increase Sort Flag Priority");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["listHeaderDown"], "Lower Sort Flag Priority");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["listHeaderAdd"], "Add/Remove Sort Flag");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["listHeaderOrder"], "Change Sort Flag Order (Trailing - in Sort String)");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["listHeaderKey"], "Sort Flag Key Type");

        VirindiViewService.TooltipSystem.AssociateTooltip(View["btnPropertyDump"], "Dump ALL properties of Selected Item to chat");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["edtSavedSortString1"], "Saved Sort String #1");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["btnCopySavedSortString1"], "Copy Saved Sort String #1 to clipboard");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["btnPasteSavedSortString1"], "Paste contents of Clipboard into Saved Sort String #1");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["edtSavedSortString2"], "Saved Sort String #2");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["btnCopySavedSortString2"], "Copy Saved Sort String #2 to clipboard");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["btnPasteSavedSortString2"], "Paste contents of Clipboard into Saved Sort String #2");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["edtSavedSortString3"], "Saved Sort String #3");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["btnCopySavedSortString3"], "Copy Saved Sort String #3 to clipboard");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["btnPasteSavedSortString3"], "Paste contents of Clipboard into Saved Sort String #3");
        VirindiViewService.TooltipSystem.AssociateTooltip(View["btnSaveSettings"], "Save all settings");

        if (View != null)
        {
            View.UserResizeable = true;

            chkIdentifyOnLogin.Checked = Properties.Settings.Default.IdentifyOnLogin;
            chkThinkWhenDone.Checked   = Properties.Settings.Default.ThinkWhenDone;
            chkReverseSortList.Checked = Properties.Settings.Default.ReverseSortList;
            edtSortString.Text         = Properties.Settings.Default.DefaultSortString;
            edtSavedSortString1.Text   = Properties.Settings.Default.SavedSortString1;
            edtSavedSortString2.Text   = Properties.Settings.Default.SavedSortString2;
            edtSavedSortString3.Text   = Properties.Settings.Default.SavedSortString3;

            ((HudCombo)View["cmbObjClassFilters"]).Change += (s, e) =>
            {
                //Util.WriteToChat("changing combo object class filter: " + cmbObjClassFilters[cmbObjClassFilters.Current].Name);
                if (((HudStaticText)cmbObjClassFilters[cmbObjClassFilters.Current]).Text.Equals("None"))
                {
                    PluginCore.getInstance().ocfilter = "";
                }
                if (!((HudStaticText)cmbObjClassFilters[cmbObjClassFilters.Current]).Text.Equals("Custom"))
                {
                    PluginCore.getInstance().ocfilter = ((HudStaticText)cmbObjClassFilters[cmbObjClassFilters.Current]).Text;
                }
            };

            View["btnSourceContainer"].Hit += (s, e) =>
            {
                PluginCore.getInstance().setSourceContainer();
            };

            View["btnDestContainer"].Hit += (s, e) =>
            {
                PluginCore.getInstance().setDestContainer();
            };

            View["edtInsertion"].KeyEvent += (s, e) =>
            {
                int slot = 0;
                try
                {
                    slot = Convert.ToInt32(((HudTextBox)View["edtInsertion"]).Text);
                }
                catch (Exception ex) { Util.LogError(ex); }
                PluginCore.getInstance().containerDestSlot = slot;
            };

            View["edtSortString"].KeyEvent += (s, e) =>
            {
                try
                {
                    PluginCore.getInstance().createSortFlagListFromString(((HudTextBox)View["edtSortString"]).Text);
                    PluginCore.getInstance().rebuildLstSortSettings();
                    Properties.Settings.Default.DefaultSortString = MainView.edtSortString.Text;
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex) { Util.LogError(ex); }
            };
            
            View["btnCopySortString"].Hit += (s, e) =>
            {
                try { System.Windows.Forms.Clipboard.SetText(edtSortString.Text); } catch (Exception ex) { Util.LogError(ex); }
            };
            
            View["btnPasteSortString"].Hit += (s, e) =>
            {
                edtSortString.Text = System.Windows.Forms.Clipboard.GetText();
                PluginCore.getInstance().createSortFlagListFromString(edtSortString.Text);
                PluginCore.getInstance().rebuildLstSortSettings();
                Properties.Settings.Default.DefaultSortString = edtSortString.Text;
                Properties.Settings.Default.Save();
            };

            View["btnActivate"].Hit += (s, e) =>
            {
                if (((HudButton) View["btnActivate"]).Text.Equals("Cancel"))
                {
                    ((HudButton)View["btnActivate"]).Text = "Activate";
                    PluginCore.getInstance().cancel();
                }
                else
                {
                    ((HudButton)View["btnActivate"]).Text = "Cancel";
                    PluginCore.getInstance().activate();
                }
            };

            ((HudCombo)View["cmbSortListFilters"]).Change += (s, e) =>
            {
                PluginCore.getInstance().createSortFlagListFromString(edtSortString.Text);
                PluginCore.getInstance().rebuildLstSortSettings();
                Properties.Settings.Default.DefaultSortString = edtSortString.Text;
                Properties.Settings.Default.Save();
            };

            ((HudList)View["lstSortSettings"]).Click += new HudList.delClickedControl(lstSortSettings_Selected);

            ((HudCheckBox)View["chkThinkWhenDone"]).Change += (s, e) =>
            {
                Properties.Settings.Default.ThinkWhenDone = ((HudCheckBox)View["chkThinkWhenDone"]).Checked;
                Properties.Settings.Default.Save();
            };

            ((HudCheckBox) View["chkIdentifyOnLogin"]).Change += (s, e) =>
            {
                Properties.Settings.Default.IdentifyOnLogin = ((HudCheckBox)View["chkIdentifyOnLogin"]).Checked;
                Properties.Settings.Default.Save();
            };

            ((HudCheckBox)View["chkReverseSortList"]).Change += (s, e) =>
            {
                Properties.Settings.Default.ReverseSortList = ((HudCheckBox)View["chkReverseSortList"]).Checked;
                Properties.Settings.Default.Save();
            };

            View["btnPropertyDump"].Hit += (s, e) =>
            {
                try
                {
                    foreach (SortFlag sf in SortFlag.sortedFlagList.Values)
                    {
                        sf.propertyDumpSelection();
                    }
                }
                catch (Exception ex) { Util.LogError(ex); }
            };

            edtSavedSortString1.Change += (s, e) =>
            {
                try
                {
                    Properties.Settings.Default.SavedSortString1 = edtSavedSortString1.Text;
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex) { Util.LogError(ex); }
            };

            edtSavedSortString2.Change += (s, e) =>
            {
                try
                {
                    Properties.Settings.Default.SavedSortString2 = edtSavedSortString2.Text;
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex) { Util.LogError(ex); }
            };

            edtSavedSortString3.Change += (s, e) =>
            {
                try
                {
                    Properties.Settings.Default.SavedSortString3 = edtSavedSortString3.Text;
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex) { Util.LogError(ex); }
            };
            
            View["btnCopySavedSortString1"].Hit += (s, e) =>
            {
                try 
                {
                    System.Windows.Forms.Clipboard.SetText(edtSavedSortString1.Text);
                }
                catch (Exception ex) { Util.LogError(ex); }
            };

            View["btnPasteSavedSortString1"].Hit += (s, e) =>
            {
                try 
                {
                    edtSavedSortString1.Text = System.Windows.Forms.Clipboard.GetText();
                    Properties.Settings.Default.SavedSortString1 = edtSavedSortString1.Text;
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex) { Util.LogError(ex); }
            };

            View["btnCopySavedSortString2"].Hit += (s, e) =>
            {
                try 
                {
                    System.Windows.Forms.Clipboard.SetText(edtSavedSortString2.Text);
                }
                catch (Exception ex) { Util.LogError(ex); }
            };

            View["btnPasteSavedSortString2"].Hit += (s, e) =>
            {
                try 
                {
                    edtSavedSortString2.Text = System.Windows.Forms.Clipboard.GetText();
                    Properties.Settings.Default.SavedSortString2 = edtSavedSortString2.Text;
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex) { Util.LogError(ex); }
            };

            View["btnCopySavedSortString3"].Hit += (s, e) =>
            {
                try 
                {
                    System.Windows.Forms.Clipboard.SetText(edtSavedSortString3.Text);
                }
                catch (Exception ex) { Util.LogError(ex); }
            };

            View["btnPasteSavedSortString3"].Hit += (s, e) =>
            {
                try
                {
                    edtSavedSortString3.Text = System.Windows.Forms.Clipboard.GetText();
                    Properties.Settings.Default.SavedSortString3 = edtSavedSortString3.Text;
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex) { Util.LogError(ex); }
            };

            View["btnSaveSettings"].Hit += (s, e) =>
            {
                try
                {
                    Properties.Settings.Default.Save();
                    Util.WriteToChat("Settings Saved!");
                }
                catch (Exception ex) { Util.LogError(ex); }
            };
        }
    }
}
}
