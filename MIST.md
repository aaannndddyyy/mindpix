The [Minimum Intelligent Signal Test](http://en.wikipedia.org/wiki/Minimum_Intelligent_Signal_Test) is an alternative measure of intelligence to that proposed by Turing, operating under more restricted conditions and giving some indication of how similar the answers given by an AI system are to that of a typical human.  To perform better than chance on this test the AI must both understand the proposition and provide a statistically human-like reply.


**_"In behavioral research (consciouness is communicated through behaviour)
the significance level (alpha) is commonly .05 or %5. that means to be
[considered significant](http://en.wikipedia.org/wiki/Statistically_significant), a result must be 5% different than the null
hypothesis. as well the sample must large enough to allow for a normal
distribution as per the [central limit theorem](http://en.wikipedia.org/wiki/Central_limit_theorem)._**

**_A MIST test of 100 items can detect consciousness within accepted
statistical parameters. any system, conscious or otherwise, has a 1 in 2
to the 100th chance of guessing all the items correctly (that's a very
large number) and only a truely conscious system would be able to
respond 100% correctly, even once. a system that got 54 out of 100
correct would force us to accept the [null hypothesis](http://en.wikipedia.org/wiki/Null_hypothesis) (that the system is
random) as it is within 5% of random. A system that got 55 out of 100
would be considered minimally consciousness (it has the minimum
statistically significant score). Any improvement over 55 would indicate
an increase in consciousness." - K. Christopher McKinstry_**

Whether MIST really tests for _intelligence_ or _consciousness_ is a question open for debate by AI philosophers, and depends upon whether you believe that humans are conscious beings or not.  There is little doubt however that it is a test for _humanness_, or the degree of similarity to the typical responses/biases of an average human.  A test for human similarity seems to be in keeping with [Turing's original proposal](http://www.loebner.net/Prizef/TuringArticle.html).

Before commencing a MIST you must first have [created a MySQL mindpixel database](HowToCreateMySQLDatabase.md), and [imported GAC-80K](ImportingGACIntoMySQL.md) or another mindpixel data set into it.

To create a MIST use the _mpmist_ utility:

```
    mpmist.exe -create MIST.txt -samples 100
```

This creates a text file containing 100 questions.

Questions are selected in such a way that there is an equal chance of true and false propositions appearing within the test.  This avoids a situation where the system under test might have foreknowledge of the ratio of true and false propositions within the database (or generally), and be able to obtain a better score by biasing its choices accordingly.

The file should be passed to the AI system being tested, which should then append a yes or no response to the end of each row, with a space character separating the original question from the response.

For example:

```
    are women the fairer sex? no
    Do cats eat mice? yes
    Do salmon swim upstream to spawn? yes
    Are the means justified by their ends? no
    Does buying a beer cause getting drunk? yes
```

The test can then be evaluated like this:

```
    mpmist.exe -answers MIST.txt
```

or alternatively if you don't wish to create the MySQL database which mpmist relies upon you can also use the mpgac utility as follows:

```
    mpgac.exe -mist MIST.txt
```

mpgac can also deal with any arbitrary questions rather than being limited to only those appearing within the original corpus, although it will only be able to give human-like responses to commonsense questions which are non-specialist and not related to current affairs.

The system will provide an indication of whether each response passed or failed, with an overall MIST rating being given as a percentage.  The higher the percentage value the closer the system is judged to be to human-like response characteristics.

It is assumed that the AI system under test is a _general intelligence_ which **does not have direct access** to the mindpixel data and has instead acquired its knowledge in some other manner.