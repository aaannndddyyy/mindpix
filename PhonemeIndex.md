Indexing text based upon its phoneme content means that text containing similar phonemes tends to be grouped in sequence when sorted on the index.

To compute the phoneme index of a mindpixel:

  1. Remove common words from the mindpixel.  These are typically words such as "the", "is", "a", etc.
  1. Convert the text into a phoneme representation.  This is carried wout by the phoneme class.
  1. Extract N-grams from the phoneme representation.  These are typically groups of three phonemes in sequence.
  1. Sort the N-grams into alphabetical sequence, then concatenate them into a single string.
  1. Compute a hash value for the string.
  1. Sort all mindpixels by theor phoneme hash value.
  1. The record number of a mindpixel when sorted by phoneme hash value is the phoneme index, and can be used to plot the mindpixel as a point on an axis.