# GenericTxtDb
A tiny C# library for working with .txt files as a database. Useful for most lightweight windows apps.

The database this library interacts with is:

- A directory somewhere on the filesystem
- with nothing but .txt files at the root level of that directory.

How to
------
1. Build my other project "FileTree" at: https://github.com/Pjotor87/FileTree and get the .dll from the bin\Release directory. You'll need a reference to that .dll in this project.
2. Add a reference to "FileTree" and build this project in Visual Studio. Again: get the .dll from the bin\Release directory.
3. Add the .dll to any project. Use it by creating an instance of the "Db" class. Use that object for doing all database operations.

___
Use the "ListFile" class when working with .txt files that looks like this:
```
One line in the file
Another line
A third line
```
___
___
Use the "KeyValuePairFile" class when working with files that looks like this:
```
First key=First value
Second key=Second value
Third key=Third value
```
___
___
Use the "TableFile" class when working with files that looks like this:
```
FirstRowFirstColumnValue|!|FirstRowSecondColumnValue|!|FirstRowThirdColumnValue
SecondRowFirstColumnValue|!|SecondRowSecondColumnValue|!|SecondRowThirdColumnValue
ThirdRowFirstColumnValue|!|ThirdRowSecondColumnValue|!|ThirdRowThirdColumnValue
```
___

Use Db.Commit() to write to the database.
