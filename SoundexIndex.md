To compute the soundex index:

  1. Remove common words from the mindpixel, such as "a", "the", "is", etc
  1. Calculate a four character soundex value for each word
  1. Sort the soundex values alphabetically, then concatenate them into a single string.
  1. Compute a hash value for the string.
  1. Sort all mindpixels by the hash value.
  1. The soundex index is then the record number in the sorted table.  This can be used to plot the mindpixel as a point along an axis.