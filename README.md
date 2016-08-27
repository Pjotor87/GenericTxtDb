# GenericTxtDb
A tiny C# library for working with .txt files as a database. Useful for most lightweight windows apps.

This library can be used to interact with .txt files as a database.
The database this library interacts with is
- a directory somewhere on the filesystem.
- .txt files at the root level of that directory.
- 
Build this project in Visual Studio and add a reference to the .dll in the bin\Release folder.

How to
------
Create an instance of the "Db" class and use that object for doing all database operations.
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
