InflightShipSave

=========================


Saves a ship that is in-flight, in case you launched it and forgot to save.

When you want to save a ship, visit it in-flight and press F6 (rebindable via the included .cfg). This will save the ship to a .craft file in your current game and will have the name "_Rescued" appended. 

===>  saves\[CurrentSaveName]\Ships\VAB\[NameOfShip]_Rescued.craft


NOTE: Still a work in progress. There are certain attributes about ships that, when active, cause the editor to throw NullRefs. Please let me know if you run into this, and provide a save and/or the offending .craft file.

Also note that this still doesn't properly handle craft that have been docked together in flight. It will save them, but with problems. A fix for this is planned.


Installation

============


Just place the InflightShipSave.dll in your GameData directory.




License

=======



Covered under the CC-BY-NC-SA license. See the license.txt for more details.

(https://creativecommons.org/licenses/by-nc-sa/4.0/)





Change Log

==========
v0.01.01 (15 Jan 15) - Does better job of handling more parts, and includes rebindable key for saving.
v0.01.00 (13 Jan 15) - Initial release
