# ChangeResolution

ChangeResolution is a console application that allows you to change the screen resolution before launching a specified program or while waiting for it to start. This is particularly useful for applications that require specific display settings, such as games or multimedia applications.

## Features

- Change the screen resolution using command-line arguments.
- Optionally launch a specified program after changing the resolution.
- Wait for a specified program to start and change the resolution before it loads.
- Revert the screen resolution to its original settings after the program exits.
- Provides a user-friendly waiting message with an option to exit the waiting mode.

## Prerequisites

- Windows Operating System
- .NET Framework or .NET Core

## Usage

The application can be executed with the following command-line arguments:

ChangeResolution.exe -x <width> -y <height> [-p <program path>] [-w]


### Arguments

- `-x <width>`: Specifies the desired width of the screen resolution.
- `-y <height>`: Specifies the desired height of the screen resolution.
- `-p <program path>`: (Optional) Specifies the path to the program to launch or wait for.
- `-w`: (Optional) Indicates that the program should wait for the specified program to start. This cannot be used without specifying a program path with `-p`.

### Example for RetroArch

To change the resolution to **1920x1080** and wait for RetroArch to start, use the following command:

ChangeResolution.exe -x 1920 -y 1080 -p "C:\Path\To\RetroArch.exe" -w


If you want to change the resolution and immediately launch RetroArch, you can use:


ChangeResolution.exe -x 1920 -y 1080 -p "C:\Path\To\RetroArch.exe"



## Notes

- If the `-w` switch is specified, the application will continuously check for the specified program to start and change the resolution before it loads. You can exit the waiting mode by pressing `Ctrl+C`.
- If you provide a program path without the `-w` switch, the application will change the resolution and launch the program immediately.
- If the specified program is already running when using the `-w` switch, the application will wait until it exits before changing the resolution back to the original settings.


## Acknowledgments

Thank you for using ChangeResolution! If you have any feedback or issues, please feel free to reach out.


