### What it does
Force Resize makes any specified window of a program open and fill an area that is defined in the prefs file.

#### Example prefs file
```xml
<prefs
	width="3840"
	height="1080"
	posx="-1920"
	posy="0"
	application="C:\Windows\Notepad.exe"
	paramters=""
	windowname="">
</prefs>
```
This will open notepad and move the window to the left of the primary monitor by 1920 pixels, as well as make it span across 3840 pixels so it fills 2 1080p monitors both in width and height.

If you wanted to resize a specific window in the program, you would add in the windowname that you wish to resize.

The parameters attribute allows you to pass arguments to the program you are wanting to launch.