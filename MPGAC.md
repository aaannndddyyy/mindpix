_"He who would design a good brain must first know how to make a bad one."_ - W. Ross Ashby

When scanning the skies how can we tell whether the radio signals detected are from an intelligent source, or are merely just background or sensor noise?

mpgac is a program created from the GAC-80K corpus capable of answering yes or no questions.  An _unintelligent system_ within this class would only be able to answer yes or no randomly, or perhaps with some small bias towards a yes answer.  A system with minimal human-like intelligence should be able to provide answers with a statistical profile measurably higher than chance would predict.

mpgac is a stand alone program, and doesn't require either the original text corpus or a database.  Instead it uses an image where each point represents a proposition plotted in a phonetic coordinate space.  The image could be regarded as a kind of compression of the original corpus - with the boolean nature of the propositions facilitating maximum compressibility.

To install mpgac:

```
    sudo dpkg -i mpgac.deb
```

Use of the program is extremely simple.

```
    mpgac.exe Do chairs have legs?
```

![http://groups.google.com/group/mindpix/web/mpgac.jpg](http://groups.google.com/group/mindpix/web/mpgac.jpg)

To a limited extent mpgac is able to interpolate propositions which did not exist in the original corpus, or were spelled differently.  This is due to the way in which the original pixels were indexed using a phoneme-based representation.  The basic philosophy behind this is that there may be some systematic relationship between word sounds and their meaning, and that people learn meanings for new words, at least initially as a default assumption, via a bootstrapping process based upon how similar they sound to words with which they are already familiar.

Obviously this should only be capable of producing a minimally intelligent signal when ordinary commonsense questions are asked, of the kind depicted above, using questions which would have made sense to an average human between the years 2000 and 2005.  On expert knowledge or current affairs related questions it should perform no better than chance.