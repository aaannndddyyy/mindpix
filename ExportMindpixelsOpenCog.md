[OpenCog](http://www.opencog.org/wiki/The_Open_Cognition_Project) currently uses the [Scheme language](http://en.wikipedia.org/wiki/Scheme_%28programming_language%29) as the recommended way of entering data into the system.  To create a Scheme formatted text file suitable for this purpose the **mpexport** utility can be used.

```
    mpexport.exe -mindpixels /usr/bin/mindpixels.txt -oc scheme.txt
```

This reads in the mindpixels contained within a text file, such as GAC-80K, parses them, then saves them as predicates to a file called scheme.txt in a format which [OpenCog](http://www.opencog.org/wiki/The_Open_Cognition_Project) can recognise.