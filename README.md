# j0e_injector
This simple app for Arma: Cold War Assault *(also known as Operation Flashpoint)* allows you to add revive script by j0e to any mission. *(referred as "converting to revive" later)*

 [Download j0e_injector from here.](https://github.com/LappiLappland/j0e_injector/releases)

> Project manager - ***Grunt***.

## Requirements
This app was created for **Windows** and work ***ONLY*** on **Windows**.

It also requires **.NET Framework 4.7.2** (or higher)

(You probably already have it)

## How to use
This App is extremely easy to use!

> Note 1: It will be very difficult to remove revive from mission after it was converted.

> Note 2: You can update missions that already have j0e_revive.

 1. Select **folder with missions** or **mission folder itself** to convert. 
 You can do this by either inserting path by yourself, or by pressing *"Folder"* button to open folder using Windows Explorer.
 2. Change settings by pressing *"@"* button. (Optional)
 3. Read small instruction by pressing *"?"* button. (Optional)
 4. Convert missions by pressing *"Start Converstion!"* button.
 >  App does not show any progress bar, it will just freeze until conversion is finished.
 5. When conversion is finished, you will be notified.
 > Location of converted missions depends on settings.
 > !LOG.txt file is also created.
## Settings
This section explains everything you can find inside *"settings"* window.
You must press *"Accept Changes"* button to save settings.
### Conversion mode
This will define the way of saving.

 > *MISSION FOLDER* - unpacked mission. (Folder with mission.sqm)
 > *MISSION PBO* - packed mission. (File in PBO format)

 1. Convert one mission (*Unlisted*)
 
    This mode is **always** used if you select *ONE MISSION FOLDER* for conversion.
    Original folder will be converted, backup of mission will be created.

 2. Rewrite existing folders
 
    *MISSION FOLDERS* **themselves** will be converted to revive. 
(Basically, it overwrites original folder)
 
 3. Create new folders
 
    *MISSION FOLDERS* will be converted and placed inside *"!CONVERTED"* folder as *MISSION FOLDERS*.
 
 4. UNPBO and REPBO
 
    *MISSION PBOS* will be converted and placed inside *"!CONVERTED"* folder as *MISSION PBOS*
    
 5. Only UNPBO
 
    *MISSION PBOS* will be converted and placed inside *"!CONVERTED"* folder as *MISSION FOLDERS*

 6. Only REPBO
 
    *MISSION FOLDERS* will be converted and placed inside *"!CONVERTED"* folder as *MISSION PBOS*

> Any PBO manipulations  require PBO manager by Winseros! (Read section below)
### PBO manager (Optional)
Select *PBO manager console* file path.
You can do this by either inserting path by yourself, or by pressing *"PATH"* button to open file using Windows Explorer.

 [Download PBO manager from here.](https://github.com/winseros/pboman3)
### PVP mission
***DO NOT FORGET TO DISABLE THIS OPTION WHEN YOU CONVERT PVE MISSIONS!***

This option lets you make revive fit PVP missions better (TDM for example)
**[revg]** will be switched to **[revgp]** tag.
Players from different sides can't:
 1. Revive each other.
 2. Spectate each other.
 3. See markers of each other.
 4. See death messages about each other.
 5. Game Over after everyone dies is also disabled.

### Delete first [] in mission name
Delete first brackets from original mission name.

> Example: 
> [PvE-15][ABC]Assault.Eden
>  will be turned into
> [Revg-15][ABC]Assault.Eden
> 
> Note: only [] brackets, stuff like [} (] will not be deleted. (We assume they contain something important)
### Multithread
Use several threads for conversion.
This speeds up conversion, but only if there is a lot of files.
If there isn't that many missions, conversion will be slowed down.

***ONLY USE THIS FOR REALLY HUGE AMOUNT OF MISSIONS.***
***YOU DON'T NEED IT OTHERWISE.***


## j0e_revive modifications
This app uses modified version of j0e_revive.

Here are some changes:

 1. Revive body is created on the exact coordinates of death. (Including height)
 This also means your body can be spawned inside buildings.
 2. If revive body is created on the sea, it will be moved to the closest land.
 3. Bots are allowed, but they get deleted after death.
 4. Grenades from mods don't get deleted after death.
 5. Captive problem in some mods is fixed.
 6. Spectating is smoother.
 7. Spectating has new *"S"* option. It sets *smoothness* of camera.
 8. Spectating has bigger limits for camera options.
 9. Spectating has no delay when switching between players.
 10. Spectating shows player status. (Dead or Bot)
 11. Spectating shows player vehicle. (Class name)
10. Spectating shows player seat in vehicle. (Commander, Driver, Gunner, Cargo)
11. Spectating shows amount of players alive.
