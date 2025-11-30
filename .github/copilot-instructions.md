# Project overview
The project is dotnet application which is cli tool to manage secrets. 
The main idea of this tool is to save secret files like .env into a user catalog. 
There are following terminology used:
- secret files - .env files, all files placed into .secrets directory
- profile - set of secret files and folders
The cli tool has the following main functions:
 
# Save function:
The `save "my-app"` does following actions:
Creates  ~/.secrets/my-app folder or clears it if it already exists.
Traverses to all subfolders from the current directory and searches for .env files and .secrets folders.
Copies found .env files and .secrets folders into ~/.secrets/my-app keeping the folder structure. 

# Load function
The `load "my-app"` does following actions:
Copies all content with all subfolders from ~/.secrets/my-app folder into the current directory.
By default `load` copies conetent overwrites existing files, but not affect other folders.
`--overwrite` removes all content from `.secrets` folder before copying new content.


# Coding instructions:
- use the latest dotnet version 
- to test change use `dotnet build` command