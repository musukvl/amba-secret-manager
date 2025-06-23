# Project overview
The project is dotnet application which is cli tool to manage secrets. 
The main idea of this tool is to save secret files like .env into a user catalog. 
There are following terminology used:
- secret files - .env files, all files placed into .secrets directory
- profile - set of secret files and folders
The cli tool has the following main functions:
- save profile: traverse to all subfolders and searching .env files and .secrets folders. Copy the only secrets into user home folder. Keep folder structure of initial project.
- load profile: creates .env files or .secrets folders with their content in the file structure.

# Coding instructions:
- use the latest dotnet version 
- to test change use `dotnet build` command