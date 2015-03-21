Mindpixels may be exported in a format suiable for use with the [POP-11 language](http://en.wikipedia.org/wiki/POP-11).  This is an old skool logic system which doesn't have probabilistic weightings, so only mindpixels with >= 0.5 coherence values are exported.

Use the mpexport utility as follows:

```
    mpexport.exe -mindpixels /usr/bin/mindpixels.txt -pop pop-11.txt
```

Takes the mindpixels contained within a text file, parses them, then exports them to the text file pop-11.txt.