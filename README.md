# mindpix

This project is a set of command line utilities written in Mono/C# for manipulating, visualising and querying the mindpixel data set, creating new data sets in the same format as the original GAC-80K, and measuring the performance of language capable artificial intelligence systems. Mindpixels may be stored as plain text or within a MySql database, and can also be exported in a variety of formats for use with other systems such as OpenCog.

## Why are questions with yes or no answers useful?

### As a workaround for the limitations of NLP

Getting computers to understand ordinary written languages is a notoriously hard and still largely unsolved problem in artificial intelligence, known as Natural Language Processing (NLP). So there's a gap between the sorts of things which most humans can describe and the very formalised languages which computers can understand.

So how can you move knowledge from the human world into the computer world?

Unambiguous human languages, such as Lojban, have been proposed to bridge the gap, but for the average human learning to write (or speak) in Lojban is pretty hard and requires a significant investment of time and effort.

Computers are good at understanding things with a yes or no answer, because the fundamental unit of modern computers is the transistor which can have an on or off state. It turns out that questions with a yes or no answer are significantly easier to parse than solving the full NLP problem, because they typically contain separators which are relatively easy to identify. The other advantage of yes and no is that when many people vote one way or the other on a given question a coherence value can be calculated, similar to a probability.

If enough people vote and the database becomes large enough a sort of probabilistic map of human average psychology can be mapped out, including elaborate ontologies which in the past needed to be painstakingly crafted by experts versed in the black art of formal logic.

So something like mindpixel - a large set of yes or no questions upon which people vote - might be an interrim way of creating proto-AIs based upon human knowledge, prior to the natural language processing problem being decisively solved.

One criticism of the mindpixel approach is the lack of temporal associations. So for instance the mindpixel "Is George Bush the president of the US?" would have been mostly true in 2002, but mostly false in 2009. A way to get around this is also to retain the user information along with timestamps for each mindpixel vote. This would allow coherence values to be computed over different time spans, as the content of the collective conscious changes over time. Analysing such temporal change would itself be an interesting project, if a sufficiently large data set were available.

There were also more advanced and speculative ideas associated with the original mindpixel project, such as the ability to geometrically interpolate knowledge without the need for conventional reasoning, but such things are probably beyond the scope of this project.

### As a way of measuring humanness in artificial intelligences

"There was a minimum intelligent signal, and it was just one bit."

The classic intelligence test for AIs is the immitation game proposed by Turing. Turing's test involves human players making judgements as to whether they are dealing with another human or a machine, but this is a relatively crude test which makes it difficult to determine the degree of human-like responses in the machine - unless a large number of players are involved.

An alternative to Turing's proposal which gives a more fine grained measure of humanness is the Minimum Intelligent Signal Test (MIST). The mindpixel data can be used to carry out such tests, and provided that the machine has not cheated by obtaining direct access to the mindpixel data and assuming a sufficiently diverse set of mindpixels are available this should provide a fair method testing the machine.

## What is GAC-80K?

GAC-80K is a set of 80,000 propositions in plain english stored within a text file. It was originally released in 2005 as a subset of the mindpixel data gathered betwenen 2000-2005. The term "GAC" stands for General Artificial Consciousness.

The terms of use for GAC-80K specify that the data is only to be used for research or non-commercial uses, but since the original mindpixel project no longer exists this probably doesn't apply to contemporary users, unless some new ownership claim is forthcoming.

It is assumed that the original mindpixel database was lost or destroyed, after the death of its curator in 2006, but most of this data will have been of low quality or have consisted of unvalidated singular votes within the "long tail". However, a new version of the GAC has been created containing a total of approximately 360K propositions. This was achieved by converting ConceptNet? entries and their scores from the OpenMind project into the same text format as used by the original GAC-80K.

## Can't I obtain ontologies validated by multiple users from Wikipedia?

Yes, and indeed the mindpixels.txt file does contain some information mined from Wikipedia. However, Wikipedia only contains half of the world's knowledge. It contains information about what most humans believe to be true (or somewhat true), but it doesn't contain information about what the average person believes to be false. If anyone were to make such entries on Wikipedia they would quickly be removed. The original Mindpixel project explicitly attempted to capture information about what people belive to be false, such as:

    Are chairs edible?
    Is the moon made of cheese?
    Can the universe breakdance?
As various AI researchers have pointed out in the past, knowing what probably isn't true about the world is just as important as knowing what is.

Encyclopedias don't cover everything

The other problem is that encyclopedias only contain certain kinds of knowledge - what might commonly be refered to as "book smarts". What encyclopedias don't contain is the very common things which we all know and take for granted, such as that cups are often found on tables and that the sky is always above the ground. This knowledge comes from our shared embodied experience of the world, and it is at least in principle possible that such information could be acquired by a "child machine" inhabiting a simulated or robotic body.

## How to install the mindpixel utilities

You can of course compile from source, with the Mono runtime and Monodevelop, but the easiest way to install is from pre-compiled packages.

Double click on the deb or rpm packages to install them, or within a terminal:

    sudo dpkg -i mpexport.deb
    sudo dpkg -i mpcreate.deb
    sudo dpkg -i mpmist.deb

## How to create a mindpixel database using MySQL

To install MySQL

    sudo apt-get install mysql-server

Run MySQL

    mysql -u root -p [enter password]

entering the root MySQL password which was given during installation. Or alternatively

    sudo mysql

Now enter

    mysql> create database mindpixel;

to create a database called mindpixel. The utilities assume that the database will have this name. Now create a default user.

    mysql> grant usage on *.* to testuser@localhost identified by 'password';
    mysql> grant all privileges on mindpixel.* to testuser@localhost;

And exit from MySQL

    mysql> exit

Now to check user was created

    mysql -u testuser -p'password' mindpixel

Some general tips on using MySQL

To show databases:

    mysql> show databases;

Select the mindpixel database

    mysql> use mindpixel;

To show tables

    mysql> show tables;

To show table structure

    mysql> show columns from mindpixels;

To empty tables prior to importing GAC-80K

    mysql> DELETE FROM mindpixels;
    mysql> DELETE FROM users;

## How to import GAC-360K into a MySQL database

In order to initially create the database tables you may initially need to add a test mindpixel, then empty the tables.

To import a text file containing mindpixels, use the mpcreate utility as follows:

    mpcreate.exe -load /usr/bin/mindpixels.txt

## Adding new mindpixels

To add new mindpixels use the mpcreate utility. The syntax is as follows:

    -q Question with a yes or no answer, which should be surrounded by quotes.
    -a Answer "yes" or "no"
    -username User entering the mindpixel
    -password User password

Here's an example:

    mpcreate.exe -q "Has Elvis left the building?" -a yes -username testuser -password password

## Grabbing a few random mindpixels

You might want to extract mindpixels at random to present them to users for validation. To do this use the mpcreate utility as follows:

    mpcreate.exe -random randpixel.txt

This will pick a random mindpixel from the database and save it into a text file called randpixel.txt

## Showing mindpixels entered by a particular user

To extract mindpixels created by a particular user and save them to a file, use the mpcreate utility as follows:

    mpcreate.exe -userpixels userpix.txt -username testuser -password password

## Performing the Minimum Intelligent Signal Test (MIST)

The Minimum Intelligent Signal Test is an alternative measure of intelligence to that proposed by Turing, operating under more restricted conditions and giving some indication of how similar the answers given by an AI system are to that of a typical human. To perform better than chance on this test the AI must both understand the proposition and provide a statistically human-like reply.

"In behavioral research (consciouness is communicated through behaviour) the significance level (alpha) is commonly .05 or %5. that means to be considered significant, a result must be 5% different than the null hypothesis. as well the sample must large enough to allow for a normal distribution as per the central limit theorem.

A MIST test of 100 items can detect consciousness within accepted statistical parameters. any system, conscious or otherwise, has a 1 in 2 to the 100th chance of guessing all the items correctly (that's a very large number) and only a truely conscious system would be able to respond 100% correctly, even once. a system that got 54 out of 100 correct would force us to accept the null hypothesis (that the system is random) as it is within 5% of random. A system that got 55 out of 100 would be considered minimally consciousness (it has the minimum statistically significant score). Any improvement over 55 would indicate an increase in consciousness." - K. Christopher McKinstry?

Whether MIST really tests for intelligence or consciousness is a question open for debate by AI philosophers, and depends upon whether you believe that humans are conscious beings or not. There is little doubt however that it is a test for humanness, or the degree of similarity to the typical responses/biases of an average human. A test for human similarity seems to be in keeping with Turing's original proposal.

Before commencing a MIST you must first have created a MySQL mindpixel database, and imported GAC-80K or another mindpixel data set into it.

To create a MIST use the mpmist utility:

    mpmist.exe -create MIST.txt -samples 100

This creates a text file containing 100 questions.

Questions are selected in such a way that there is an equal chance of true and false propositions appearing within the test. This avoids a situation where the system under test might have foreknowledge of the ratio of true and false propositions within the database (or generally), and be able to obtain a better score by biasing its choices accordingly.

The file should be passed to the AI system being tested, which should then append a yes or no response to the end of each row, with a space character separating the original question from the response.

For example:

    are women the fairer sex? no
    Do cats eat mice? yes
    Do salmon swim upstream to spawn? yes
    Are the means justified by their ends? no
    Does buying a beer cause getting drunk? yes

The test can then be evaluated like this:

    mpmist.exe -answers MIST.txt

or alternatively if you don't wish to create the MySQL database which mpmist relies upon you can also use the mpgac utility as follows:

    mpgac.exe -mist MIST.txt

mpgac can also deal with any arbitrary questions rather than being limited to only those appearing within the original corpus, although it will only be able to give human-like responses to commonsense questions which are non-specialist and not related to current affairs.

The system will provide an indication of whether each response passed or failed, with an overall MIST rating being given as a percentage. The higher the percentage value the closer the system is judged to be to human-like response characteristics.

It is assumed that the AI system under test is a general intelligence which does not have direct access to the mindpixel data and has instead acquired its knowledge in some other manner.

## Exporting the mindpixel database as a text file

The original GAC-80K is in the form of a plain text file, and this can be more useful than a database since it can be read on virtually any system.

To export the entire database:

    mpcreate.exe -save mp.txt

Saves all mindpixels to a text file called mp.txt. The only requirement for mindpixel text files is that the phrase "Mind Hack" appears before the actual data begins. This allows you to add comments or license info to the top of the file, and it will be ignored when being read by other utilities.

## How to export mindpixels in OpenCog Scheme format

OpenCog currently uses the Scheme language as the recommended way of entering data into the system. To create a Scheme formatted text file suitable for this purpose the mpexport utility can be used.

    mpexport.exe -mindpixels /usr/bin/mindpixels.txt -oc scheme.txt

This reads in the mindpixels contained within a text file, such as GAC-80K, parses them, then saves them as predicates to a file called scheme.txt in a format which OpenCog can recognise.

## How to export mindpixels in POP format

Mindpixels may be exported in a format suiable for use with the POP-11 language. This is an old skool logic system which doesn't have probabilistic weightings, so only mindpixels with >= 0.5 coherence values are exported.

Use the mpexport utility as follows:

    mpexport.exe -mindpixels /usr/bin/mindpixels.txt -pop pop-11.txt

Takes the mindpixels contained within a text file, parses them, then exports them to the text file pop-11.txt.

## How to export mindpixels in NARS format

Mindpixels can be exported in a format suitable for reading into NARS as follows:

    mpexport.exe -mindpixels /usr/bin/minpixels.txt -nars nars.txt

This takes mindpixels stored within a text file, parses them, then saves them to a file called nars.txt.

## How to save a map of Mindpixels plotted in phonetic space

First ensure that mindpixels have been imported into a MySql database, then use the command:

    mpcreate.exe -map myimage.jpg

Calculation of the map may take some time, depending upon the number of mindpixels loaded into the database.

The horizontal axis of the map corresponds to the phoneme index of the mindpixel, and the vertical axis corresponds to the standardised soundex index. Hence moving across the map is moving through a topology of sound utterances.

Green pixels correspond to probably true propositions, having a coherence value greater than 0.5. Blue and pink/red pixels correspond to probably false propositions, with a coherence value less than 0.5. The more red the pixel the closer its value is to zero probability. Black pixels are uncharted territory, where no mindpixels have been plotted.

The landscape looks something like this:

## mpgac: A minimally intelligent AGI

"He who would design a good brain must first know how to make a bad one." - W. Ross Ashby

When scanning the skies how can we tell whether the radio signals detected are from an intelligent source, or are merely just background or sensor noise?

mpgac is a program created from the GAC-80K corpus capable of answering yes or no questions. An unintelligent system within this class would only be able to answer yes or no randomly, or perhaps with some small bias towards a yes answer. A system with minimal human-like intelligence should be able to provide answers with a statistical profile measurably higher than chance would predict.

mpgac is a stand alone program, and doesn't require either the original text corpus or a database. Instead it uses an image where each point represents a proposition plotted in a phonetic coordinate space. The image could be regarded as a kind of compression of the original corpus - with the boolean nature of the propositions facilitating maximum compressibility.

To install mpgac:

    sudo dpkg -i mpgac.deb

Use of the program is extremely simple.

    mpgac.exe Do chairs have legs?

To a limited extent mpgac is able to interpolate propositions which did not exist in the original corpus, or were spelled differently. This is due to the way in which the original pixels were indexed using a phoneme-based representation. The basic philosophy behind this is that there may be some systematic relationship between word sounds and their meaning, and that people learn meanings for new words, at least initially as a default assumption, via a bootstrapping process based upon how similar they sound to words with which they are already familiar.

Obviously this should only be capable of producing a minimally intelligent signal when ordinary commonsense questions are asked, of the kind depicted above, using questions which would have made sense to an average human between the years 2000 and 2005. On expert knowledge or current affairs related questions it should perform no better than chance.

## Background reading

The measurement of meaning, Charles E. Osgood, George J. Suci, Percy H. Tannenbaum

Some theory on relations between word sounds and their meaning by Margaret Magnus
http://www.trismegistos.com/MagicalLetterPage/Theory.html
