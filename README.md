# Batch uploader for Imagga.com API service

This program is Windows GUI application which allows you to select local folder with images and send them to tagging API (https://docs.imagga.com/#tags). Output is saved into two files: one for successfully tagged images (one line per tag value from API response) and one for errors (one line per image file).

## Prerequisites
You need .NET Core 3.1 installed to run this program. Dotnet runtime libraries can be downloaded and installed from the Microsoft website: https://dotnet.microsoft.com/download/dotnet/3.1/runtime. Also you will be redirected to download URL if you run this application without runtime installed.

## Installing
Installation is not needed, just copy the folder with all the files and run **ImaggaBatchUploader.exe** executable.

## Workflow

### Preparation
1. Select the folder with images by pressing button or dragging and dropping folder to the main window.
2. Program scans specified folder and counts all files with extensions that match *image extensions* settings.
3. Then it checks whether CSV with tags for that folder exists. In that case program displays counter in *processed images* label and skips these images when you start tagging.

### Tagging

Process begins when you press the **Start** button. Before actual tagging program performs some additional checks:

1. Is Imagga.com API available.
1. Are *API key* and *API secret* settings recognized by Imagga API.
1. Is there enough quota left to tag at least one image.
1. Can we open output files (tags and errors CSV) for writing. It's common mistake to have them left opened in Excel, which prevents another programs from writing.

If any of these checks fail, program displays message box and stops. Else it displays progress bar and procedes. File with old errors gets cleaned when tagging begins to avoid confusion: if error was temporary there's no need to persist it, in other case it will come up again.

### Inspecting output

Let's assume that we are tagging images inside folder named *ImagesFolder*, which resides on the Desktop. Then tags will be saved to the **Desktop\tags-ImagesFolder.csv**, and errors to the **Desktop\errors-ImagesFolder.csv**. You can open these files in Excel during tagging process, though it wil warn you about read-only mode.

Also you can read verbose info in *Intermediate output* field in case you need some more information about what's going on.

## Settings

You can open settings window by pressing cog icon on the main application screen. There are following settings available:
- *API Key / API secret* -- credentials used to access Imagga API. You can copy them from your Imagga User Dashboard.
- *API Endpoint* -- also available in Imagga User Dashboard, but unlikely that this one would ever change.
- *Image extensions* -- files with these extensions are considered *images* when you select folder. Other are considered *unrecognized files* and are not being sent to the tagging API. Items are delimited with spaces, and each should begin with single dot.

If you press **Save** button, settings are persisted in **settings.json** file within application folder.

### Settings override

**Settings.json** file can be copied to any folder with images. When you open folder with **settings.json** file in it, these settings will be loaded instead of default. You will get notification about override mode in intermediate output window. Also Settings window will Save button disabled with explanatory text.

You can edit **settings.json** file in any plain text editor like Notepad, just be careful not to break JSON markup.

### Hidden settings

Hidden settings are parameters in **settings.json** file that are not present in settings window. At this time there's only one such setting, called **TaggingThreshold** (integer type). This parameter limits lowest confidence level in received tags. You can read about it in https://docs.imagga.com/#tags and https://docs.imagga.com/?csharp#best-practices.
