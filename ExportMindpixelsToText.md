The original GAC-80K is in the form of a plain text file, and this can be more useful than a database since it can be read on virtually any system.

To export the entire database:

```
    mpcreate.exe -save mp.txt
```

Saves all mindpixels to a text file called mp.txt.  The only requirement for mindpixel text files is that the phrase **"Mind Hack"** appears before the actual data begins.  This allows you to add comments or license info to the top of the file, and it will be ignored when being read by other utilities.