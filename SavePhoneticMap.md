First ensure that mindpixels have been [imported into a MySql database](ImportingGACIntoMySQL.md), then use the command:

```
    mpcreate.exe -map myimage.jpg
```

Calculation of the map may take some time, depending upon the number of mindpixels loaded into the database.

The horizontal axis of the map corresponds to the [phoneme index](PhonemeIndex.md) of the mindpixel, and the vertical axis corresponds to the [standardised soundex index](SoundexIndex.md).  Hence moving across the map is moving through a topology of sound utterances.

Green pixels correspond to probably true propositions, having a coherence value greater than 0.5.  Blue and pink/red pixels correspond to probably false propositions, with a coherence value less than 0.5.  The more red the pixel the closer its value is to zero probability.  Black pixels are uncharted territory, where no mindpixels have been plotted.

The landscape looks something like this:

![http://mindpix.googlegroups.com/web/mindpixels.jpg](http://mindpix.googlegroups.com/web/mindpixels.jpg)