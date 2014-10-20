#MudSort

Asheron's Call Inventory Management Decal Plugin

##What Can It Do?

![sorted-inventory](https://raw.githubusercontent.com/mudzereli/mudsort/master/docs/assets/beforeafter.jpg "Sorted Inventory Screenshot")

Mudsort is a VERY powerful sorting plugin for Decal. Some example uses:

1. Move all salvage from one character to another and sort it in the process
2. Move items from your backpack to a chest while sorting them in the process
3. Sort the contents of a chest
4. Grab all of a certain type of item from a chest while sorting them
5. And of course sort your inventory

##Requirements

1. [Asheron's Call](http://www.asheronscall.com/en)
2. [Decal 2.9.7.5](http://www.decaldev.com/)
3. [Virindi Views](http://virindi.net/plugins/) (Comes in Virindi Bundle)
4. [mudsort.dll](https://github.com/mudzereli/mudsort/raw/master/bin/Release/mudsort.dll)

##Installation

1. Download [mudsort.dll](https://github.com/mudzereli/mudsort/raw/master/bin/Release/mudsort.dll)
2. Open Decal
3. Click `Add` in Decal
4. Click `Browse` in Decal
5. Find and select `mudsort.dll`
6. Make sure you have `Virindi View Service Bootstrapper` running under `Services` in Decal
7. Start Asheron's Call and enjoy!

##Plugin Usage

The best way to find out how to use the plugin is log in and play with it (there are tooltips for everything, so it kind of explains as you go).
It will really save you a lot of time!

###Sort Tab

![plugin-sort-screenshot](https://raw.githubusercontent.com/mudzereli/mudsort/master/docs/assets/plugin-sort.png "Sort Tab Screenshot")

1. **Source Container** -- The container the items you want to sort are in (Can be either your Character, a Backpack, or an Opened Chest). Use the button to set it to your current selection.
2. **Destination Container** -- The container that you want your items to go into when sorting them (Can be your Character, a Backpack, an Opened Chest, or Another Player). Use the button to set it to your current selection.
3. **Insert Position** -- The `Slot ID` to start inserting at in the `Destination Container` (Default = 0)
4. **Sort String** -- The String to use when Sorting. Can be typed in manually or build on the `Build` tab
5. **Object Class Filter** -- Use this to only sort a particular type of item, like Armor/Notes/Gems/Salvage/etc
6. **Progress Bar** -- Items need to be identified before they are sorted, this progress bar tracks the identifying and sorting
7. **Activate Button** -- Begins the sorting process, or cancels it if it's already running.

###Build Tab

![plugin-build-screenshot](https://raw.githubusercontent.com/mudzereli/mudsort/master/docs/assets/plugin-build.png "Build Tab Screenshot")

1. **Sort List Filter** -- Filter Sort Flags by type (Commonly Used/Word/Number/etc)
2. **Added Sort Flags** -- Sort Flags that will be used to create the current Sort String in the Sort tab
3. **Remaining Sort Flags** -- The additional Sort Flags in the filtered list that can be added to Sort by

###Options Tab

![plugin-options-screenshot](https://raw.githubusercontent.com/mudzereli/mudsort/master/docs/assets/plugin-options.png "Options Tab Screenshot")

1. **Identify Inventory On Login** -- If checked, your inventory will be queued up for ID when you log in, so that when you go to sort them later, you won't have to wait for them to identify.
2. **Reverse Sort List** -- This reverses the sorting string completely
3. **Property Dump Selection** -- Select an item and then click this to view ALL of its properties
4. **Saved Sort Strings** -- Use this area to save your favorite Sort Strings (fancy I know)
5. **Save** -- Save all settings and Sort Strings