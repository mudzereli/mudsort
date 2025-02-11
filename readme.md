# MudSort

Asheron's Call Inventory Management Decal Plugin

## What Can It Do?

![sorted-inventory](https://raw.githubusercontent.com/mudzereli/mudsort/master/docs/assets/beforeafter.jpg "Sorted Inventory Screenshot")

Mudsort is a VERY powerful sorting plugin for Decal. Some example uses:

1. Move all salvage from one character to another and sort it in the process
2. Move items from your backpack to a chest while sorting them in the process
3. Sort the contents of a chest
4. Grab all of a certain type of item from a chest while sorting them
5. And of course sort your inventory

## Requirements

1. [Asheron's Call](http://www.asheronscall.com/en)
2. [Decal 2.9.7.5](http://www.decaldev.com/)
3. [Virindi Views](http://virindi.net/plugins/) (Comes in Virindi Bundle)
4. [mudsort](github.com/mudzereli/mudsort/releases/latest)

## Installation

1. Download [mudsort](github.com/mudzereli/mudsort/releases/latest).
2. **Run the Installer**
3. Open Decal.
4. Confirm that `mudsort` is in the plugin list. If not, restart Decal.
5. Make sure you have `Virindi View Service Bootstrapper` running under `Services` in Decal.
   - If you don't have this, you are missing **Step #3** from Requirements
6. Start Asheron's Call and enjoy!

## Plugin Usage

The best way to find out how to use the plugin is log in and play with it (there are tooltips for everything, so it kind of explains as you go).
It will really save you a lot of time!

### Sort Tab

![plugin-sort-screenshot](https://raw.githubusercontent.com/mudzereli/mudsort/master/docs/assets/plugin-sort.png "Sort Tab Screenshot")

<dl>
	<dt>1. Source Container</dt>
	<dd>The container the items you want to sort are in (Can be either your Character, a Backpack, or an Opened Chest). Use the button to set it to your current selection.</dd>
	<dt>2. Destination Container</dt>
	<dd>The container that you want your items to go into when sorting them (Can be your Character, a Backpack, an Opened Chest, or Another Player). Use the button to set it to your current selection.</dd>
	<dt>3. Insert Position</dt>
	<dd>The Slot ID to start inserting at in the Destination Container (Default = 0)</dd>
	<dt>4. Sort String</dt>
	<dd>The String to use when Sorting. Can be typed in manually or build on the `Build` tab</dd>
	<dt>5. Object Class Filter</dt>
	<dd>Use this to only sort a particular type of item, like Armor/Notes/Gems/Salvage/etc</dd>
	<dt>6. Progress Bar</dt>
	<dd>Items need to be identified before they are sorted, this progress bar tracks the identifying and sorting</dd>
	<dt>7. Activate Button</dt>
	<dd>Begins the sorting process, or cancels it if it's already running.</dd>
</dl>

### Build Tab

![plugin-build-screenshot](https://raw.githubusercontent.com/mudzereli/mudsort/master/docs/assets/plugin-build.png "Build Tab Screenshot")

<dl>
	<dt>1. Sort List Filter</dt>
	<dd>Filter Sort Flags by type (Commonly Used/Word/Number/etc)</dd>
	<dt>2. Added Sort Flags</dt>
	<dd>Sort Flags that will be used to create the current Sort String in the Sort tab</dd>
	<dt>3. Remaining Sort Flags</dt>
	<dd>The additional Sort Flags in the filtered list that can be added to Sort by</dd>
</dl>

### Options Tab

![plugin-options-screenshot](https://raw.githubusercontent.com/mudzereli/mudsort/master/docs/assets/plugin-options.png "Options Tab Screenshot")

<dl>
  <dt>1. Identify Inventory On Login</dt>
  <dd>If checked, your inventory will be queued up for ID when you log in, so that when you go to sort them later, you won't have to wait for them to identify.</dd>
  <dt>2. Reverse Sort List</dt>
  <dd>This reverses the sorting string completely.</dd>
  <dt>(new) Think to yourself when complete</dt>
  <dd>This will tell your character when you are done sorting (for meta usage/automation).</dd>
  <dt>3. Property Dump Selection</dt>
  <dd>Select an item and then click this to view ALL of its sortable properties.</dd>
  <dt>4. Saved Sort Strings</dt>
  <dd>This area can be used to save some of your customized sort strings.</dd>
  <dt>5. Save</dt>
  <dd>This saves all settings.</dd>
</dl>

## Commands
- `/ms help` or just `/ms` -- lists commands
- `/ms start` -- start sorting
- `/ms set source (pack <1-8>|player|<containerID>)` -- set source container to: pack 1-8, player, given containerID, or selection (no argument)
- `/ms set dest (pack <1-8>|player|<containerID>)` -- set destination container to: pack 1-8, player, given containerID, or selection (no argument)
- `/ms set flags (sort flag string)` -- sets the sort string to the given argument
- `/ms set ocfilter (object class string)` -- sets the object class filter to the given argument (accepts partial names, such as manast for ManaStone)
- `/ms clear ocfilter` -- clears the object class filter
