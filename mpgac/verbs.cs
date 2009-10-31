/*
    verb classification from VerbNet/PropBank
    Copyright (C) 2009 Bob Mottram
    fuzzgun@gmail.com

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;

namespace mpgac
{
	public class verbs
	{
static string[] verbclass = {
"FedEx","11.1-1","UPS","11.1-1","abandon","51.2","abas","45.4","abash","31.1","abat","45.4","abbreviat","45.4","abdicat","10.11","abduct","10.5","abet","72",
"abhor","31.2","abolish","10.1","abound","47.5.1-2","abridg","45.4","absolv","10.6 80","abstain","69","abstract","10.1","abus","33","abut","47.8","acced","95",
"accelerat","45.4","accept","13.5.2 29.2","acclaim","33","accommodat","26.9","accompany","51.7","accru","13.5.2","accumulat","13.5.2 47.5.2","accurs","33","accus","81","acetify","45.4",
"ach","40.8.2","acidify","45.4","acknowledg","37.10 29.9-1","acquaint","37.2","acquiesc","95","acquir","13.5.2-1 14","acquit","10.6","act","29.6","activat","45.4","adapt","26.9",
"add","25.3","addict","96","address","25.3","adher","22.5","adjoin","47.8-1","adjust","26.9","administer","13.2","admir","31.2","admit","29.5","admix","22.1-1",
"admonish","37.9","adopt","29.1","ador","31.2","adorn","9.8 25.3","advanc","45.4","advertis","35.2","advis","37.9-1","affect","31.1","affiliat","22.2","affirm","29.2",
"afflict","31.1","affront","31.1","ag","45.4","aggravat","31.1","aggregat","47.5.2","aggriev","31.1","agitat","31.1","agoni","31.3","agre","36.1","aid","72",
"aim","62","air","45.4","airmail","11.1-1","alarm","31.1","alert","37.9","alienat","31.1","align","31.1","allocat","13.3","allot","13.3","allow","29.5 65 64",
"allur","59","ally","71","alter","26.6 45.4","alternat","22.2","amalgamat","22.2 22.1-1-1","amass","47.5.2","amaz","31.1","ambl","51.3.2","ameliorat","45.4","americani","45.4",
"amplify","45.4","amputat","10.1","amus","31.1","analyz","34","anchor","22.4","anestheti","45.4","anger","31.1 31.3-8","angl","47.7","anglici","45.4","anguish","31.3",
"animat","45.4","annex","10.5","annihilat","44","annotat","25.1","announc","37.7","annoy","31.1 59-1","annul","106","anoint","9.8","antagoni","31.1","antiqu","13.7",
"appal","31.1","appall","31.1","appear","48.1.1","appeas","31.1","append","22.3-2","applaud","33","appliqu","25.1","apply","105","appoint","29.1","apportion","13.3",
"apprais","54.4","appreciat","45.6","apprehend","87.2-1","apprentic","29.7","approach","98 51.1-2","appropriat","13.5.2","approv","31.3 64","approximat","34.2 54.4","arch","40.3.2","archiv","9.10",
"argu","37.6 36.4-1","aris","48.1.1","arm","13.4.2","armor","13.4.2-1-1","arous","31.1","arrang","26.1","arriv","51.1-2","articulat","37.7","ascend","51.1-1","ascertain","84",
"ask","37.1-1","asphyxiat","40.7 42.2","assail","33","assassinat","42.1","assault","33","assembl","26.1","assert","48.1.2 29.5-2","assess","34 54.4","assign","13.3","assimilat","26.9",
"assist","72","associat","22.2","assuag","31.1","assum","93","assur","37.9 37.13","astonish","31.1","astound","31.1","atrophy","45.4","attach","22.3-2","attack","33",
"attain","13.5.1","attempt","61","attir","41.3.3","audit","34","augment","45.4","autograph","25.3","averag","108","avoid","52","awak","45.4 48.1.1","awaken","45.4 48.1.1",
"award","13.3","aw","31.1","baa","38","backbit","33","badger","58.1","baffl","31.1","bag","9.10","bail","10.4.1","bak","45.3 26.3-1","balanc","47.6 45.4",
"balk","40.5","balloon","51.4.1","bamboozl","59","ban","67","band","22.3-2 22.4","bandag","9.8","bang","43.2","banish","10.2","bank","9.10","bar","67",
"barbecu","26.1 26.3-1 45.3","bar","40.3.2","bargain","36.1 89","barg","11.5-1","bark","37.3","barricad","22.4","barter","13.6","bas","97.1","bash","18.1","bask","31.3",
"bat","40.3.2","bath","9.8 41.1.1","batter","18.1","battl","36.3 36.4-1","bawl","37.3","bay","38","beach","9.10","beam","43.1","bear","31.2","beat","18.1",
"beatify","31.2","beautify","45.4","beckon","40.3.1","bed","9.10","bedew","45.4","beep","43.2","befall","48.3","befit","54.3","befuddl","31.1","beg","58.2 58.1",
"beget","27","begin","55.1","beguil","31.1","behav","29.6","behead","10.7","behold","30.3","belch","40.1.1","believ","29.9-2 31.2","belittl","33","bellow","37.3 43.2",
"bench","9.10","bend","47.6 26.5 50 45.2","bequeath","13.3","bereav","10.6","berry","13.7","berth","9.10-1","beseech","60","bet","54.5 94","betroth","22.2","bewail","31.2",
"bewar","31.3","bewilder","31.4 22.2","bewitch","31.1","bias","96","bicker","36.1","bicycl","51.4.1","bid","37.7","bifurcat","45.4","bik","51.4.1 11.5","bilk","10.6",
"bill","54.5","billet","9.10","billow","47.2","bin","9.10","bind","9.8 22.3-2","birdnest","13.7","bitch","37.8","bit","18.2 40.8.3-2","blab","37.7","blabber","37.3",
"black","9.9","blackberry","13.7","blacken","45.4","blackmail","59","blam","33","blanket","9.8","blar","43.2","blasphem","33","blast","43.2","blat","38",
"blaz","43.1","bleach","45.4","bleat","38 37.3","bleed","40.1.2 43.4","blend","22.1-1 26.3-1","bless","33","blight","44","blind","45.4","blindfold","9.9","blink","40.3.1-1",
"blitz","44","block","9.8 16","blockad","67","bloody","45.4","bloom","45.5 47.2","blossom","47.2","blot","9.8","blow","23.2","blubber","40.2","bludgeon","18.3",
"bluff","59-1","blunt","45.4","blur","45.4","blurt","37.7","blush","40.1.1","bluster","37.3","board","46","boast","37.8","boat","51.4.1","bob","47.3",
"boggl","31.3","boil","45.3 26.3-2","bolt","51.3.2","bombard","9.8","bond","22.3","bon","10.7","book","13.5.1","boom","43.2","boost","102","border","47.8",
"bor","21.2-2","borrow","13.5.2","botch","45.4","bother","31.1","bottl","9.10","bounc","11.2-1 51.3.1 51.3.2","bound","51.3.2","bow","40.3.3","bowl","51.3.2","box","9.10",
"boycott","52","brag","37.8","brais","45.3","branch","23.4","brand","25.3 29.3","brav","98","bray","38 37.3","bread","9.9","break","23.2 45.1 40.8.3-1","breakfast","39.5",
"breath","47.2","brew","26.3-1","brib","59-1","bridg","47.8","brief","37.9","brighten","45.4","bring","11.3","bristl","47.2","broach","55.2","broadcast","37.4",
"broaden","45.4","broil","26.3-2 45.3","browbeat","31.1","brown","45.4, 45.3","bruis","21.2-1 40.8.3","brush","10.4.2","bubbl","43.2 43.4 ","buck","49","buckl","22.4","bud","45.5",
"buff","59","buffet","17.2","bug","31.1","build","26.1-1","bulg","47.5.3 47.2","bullet","17.1","bully","29.8","bump","18.1","bundl","22.3-2","bunk","9.10",
"burbl","37.3","burden","13.4.2","burgeon","45.5","burn","43.1 45.4","burst","48.1.1","bury","9.1","bustl","47.5.1","butcher","42.1","button","22.4","buttonhol","9.9",
"buy","13.5.1","buzz","38 43.2","cabbag","10.5","cackl","38","cadg","10.5 13.5.2","cag","9.10","cajol","59","calibrat","54.4","call","29.3","calm","31.1",
"calumniat","33","camouflag","29.6","camp","46","can","9.10","canoni","29.7","canvass","35.4","cap","9.9","capsi","45.4","captain","29.8-1","captivat","31.1",
"captur","10.5","carameli","45.4","carbonify","45.4","carboni","45.4","car","31.3","caress","20-1","carjack","10.5","carol","26.7-1 37.3 43.2","carpet","9.9","carry","11.4 54.2 54.3",
"cart","11.5","carv","21.2-2","cascad","47.7 47.2","cas","9.10","cash","13.5.1","cast","17.1","castigat","33","catapult","17.1","catch","13.5.1","categori","29.10 45.4",
"caus","27","caution","37.9-1","caw","38","ceas","55.1","ced","13.3","celebrat","33","cellar","9.10","cement","22.4","censur","33","center","87.1",
"centrali","45.4","certify","29.2","chagrin","31.1","chain","22.4","champion","29.8","chanc","94","chang","26.6 45.4","channel","9.3-2","chant","37.3 26.7-1","char","45.4",
"characteri","29.2","charg","54.5","charm","45.3","charter","13.5.1","chas","51.6","chasten","33","chastis","33","chat","37.6","chatter","37.6 37.3","chauffeur","29.8",
"cheapen","45.4","cheat","10.6","check","35.2","cheep","38","cheer","31.1","cherish","31.2","chew","39.2-1","chid","33","chill","45.4","chim","43.2",
"chip","21.1 21.2-1 40.8.3-1 45.1","chirp","38","chirrup","38","chisel","26.1-1","chitter","38","chok","40.7","choos","13.5.1 29.2","chop","21.2-2","choreograph","26.7-2","chortl","40.2",
"christen","29.3","chronicl","25.4","chuck","17.1","chuckl","40.2","chug","43.2","churn","26.1","cinch","22.4","circumvent","52","cit","29.2 37.1","claim","37.7",
"clam","13.7","clamber","51.3.2","clamp","22.4","clang","43.2","clank","43.2","clap","43.2 40.3.1","clash","36.1","clasp","15.1","class","29.2","classify","29.2",
"clatter","43.2","claw","21.1","clean","10.3-1 26.3-2 45.4","cleans","10.6","clear","10.3-1 26.3-1 45.4","cleav","21.2","clench","40.3.2","click","40.3.2","climb","51.1-1 51.3.2","cling","22.5",
"clink","43.2","clip","21.1","cloak","9.8 9.9","clobber","18.3","clock","54.1","clog","51.5","cloister","9.10","clomp","43.2","clos","40.3.2 45.4","cloth","41.1.1",
"club","18.3","cluck","38 37.3","cluster","22.3-2 47.5.2","clutch","15.1","clutter","9.8","co-exist","36.1 47.1-1","coach","29.8","coagulat","45.4","coalesc","22.1-1-1","coarsen","45.4",
"coast","51.3.2","coat","9.8","coax","59","cock","40.3.2","coddl","45.3","cod","29.10","coerc","59","cohabit","36.1","coher","89","coil","9.6",
"coin","26.4","coincid","22.2","collaborat","36.1","collaps","45.4","collat","22.3-2","collect","13.5.2 22.3-2 45.4","collid","18.4","collud","36.1","color","24","comb","35.2",
"combat","36.1","combin","22.1-1","com","51.1-2","comfort","31.1","commandeer","10.5 13.5.2","commenc","55.1","commend","33","comment","37.11-1-1","commerciali","45.4","commingl","22.1-1-1",
"commiserat","36.1 88.2","commission","59","commit","79","communicat","36.1","compar","22.2","compel","59","compensat","33","compet","36.1","compil","26.1","complain","37.8",
"complet","55.2","compliment","33","compos","26.4","compound","22.1-1-1","comprehend","87.2","compress","26.5","compromis","36.1","comput","26.4","con","10.6","conceal","16",
"conced","13.3","concentrat","87.1","concern","31.1","conciliat","45.4","conclud","97.2","concoct","26.4","concur","36.1","condemn","33","condens","45.4","condition","41.2.2",
"condon","33","conduct","51.7","confer","33","confess","37.3","confid","37.7","confin","92-1 76","confirm","29.2","confiscat","10.5","conflict","36.1","conform","26.9",
"confound","31.1","confront","98","confus","22.2 31.1","congratulat","33","congregat","47.5.2","conjectur","29.5-1","conjur","26.4-1","connect","22.1-2","conquer","42.3","consent","89",
"conserv","13.5.1","consider","29.1","consol","31.1","consolidat","22.2 22.1","consort","36.1","conspir","71","constitut","55.5","constrain","76","constrict","45.4","constring","45.4",
"construct","26.4 45.4","consult","36.3","consum","39.4 66","contain","54.3 47.8","contaminat","9.8","content","31.4","contextuali","45.4","continu","55.3 55.6","contract","45.4","contrast","22.2",
"contribut","13.2","conven","47.5.2","converg","87.1","convers","37.6","convert","26.6","convey","37.7 11.1","convinc","31.1 58.1","convuls","40.6","coo","38","cook","26.1 26.3-1 45.3",
"cool","45.4","coop","9.10","cooperat","36.1 73-1","cop","10.5","cop","83-1","copy","25.2 25.4","copyright","101","cork","45.3","corral","9.10","correct","45.4",
"correlat","22.2 86.1","correspond","36.1","corrugat","45.4","corrupt","45.4","cost","54.2","cough","40.1.2-1","counsel","37.9-1","count","29.6","coupl","22.2-1","court","36.2",
"cover","9.8","covet","32.1","cow","31.1","cower","40.5 40.6","cowrit","25.2","crab","13.7","crack","43.2","crackl","43.2","cram","9.7-2","cran","40.3.2",
"crash","18.4-1 45.1","crat","9.10","crav","32.1 32.2","crawl","47.7","creak","43.2","cream","22.1-1","creas","45.2","creat","26.4 27","credit","13.4.1-1","creep","51.3.2 47.3 47.5.1-1",
"cremat","45.4","crimp","41.2.2","crimson","45.4","cring","31.3 40.5","crippl","29.7","crisscross","22.2-2-1","critici","33","crook","40.3.2","croon","37.3","crop","21.2-2 41.2.2",
"cross","40.3.2 47.8","crouch","50 47.6","crow","37.3","crowd","9.7-2","crown","29.1 29.3","crucify","42.2","cruis","51.4.2","crumbl","45.4 21.2","crumpl","45.2","crunch","21.2-1 39.2-1",
"crush","21.2-1 45.1","cry","37.3","crystalli","45.4","cuckold","29.7","cuckoo","38","cuff","18.3","cull","10.1","cultivat","9.7-1 26.3-1","curb","42.3","curdl","45.4",
"cur","10.6 80-1","curl","9.6","curry","41.1.2","curs","33","curv","26.5","cut","21.1 23.2 26.1-1 40.8.3-2 41.2.2","cycl","51.4.1","dab","10.4.1-1 9.7","dally","53.1","damag","44",
"damn","33","dampen","45.4","danc","51.5 26.7-1","dangl","9.2 47.6","dar","59","darken","45.4","dart","51.3.2","dash","18.1","dat","25.3","daub","9.7-2",
"daunt","31.1","dawdl","53.1","dawn","48.1.1","daz","31.1","dazzl","31.1","deadlock","40.1.1 40.2","deafen","45.4","deal","83","debat","36.1 36.3-2","debug","10.8",
"decamp","51.1-1","decapitat","10.7","decay","47.2 45.5","deceiv","59-1","decelerat","45.4","decentrali","45.4","decimat","44","deck","9.8","declar","29.4 48.1.2","declin","45.6",
"decompos","45.4 47.2","decontaminat","10.8","decorat","9.8 25.3","decreas","45.4 45.6","decre","29.3","decry","33","dedicat","79","deduc","84 97.2","deduct","10.1 108-2","deem","29.1",
"deep-fry","45.3","deepen","45.4","deepfry","45.3","defam","33","defecat","40.1.2-1","defend","85","defin","29.2 48.1.2","deflat","45.4","deform","26.6.1","defraud","10.6",
"defrost","45.4 10.8","degenerat","45.4","degrad","45.4 45.5","deify","31.2","deject","31.1","delay","53.1","delet","10.1","deliberat","36.3-1","delight","31.1 31.3-5","deliver","11.1",
"delous","10.8","delud","59-1","delv","35.5","demagneti","45.4","demand","60-1","demobili","45.4","democrati","45.4","demolish","31.1 44","demonstrat","78-1 37.1.1","demorali","31.1",
"denigrat","33","denounc","33","dent","21.2","denud","10.6","deny","29.5","depart","51.1-1","depend","47.1-1 70","depict","29.2","deplet","10.6","deplor","31.2",
"depopulat","10.6","deport","10.2","depos","10.1","deposit","9.1","deprecat","33","depreciat","45.6","depress","31.1","depressuri","45.4","depriv","10.6","derid","33",
"deriv","26.4 48.1.1","desalinat","45.4","descend","51.1-1","describ","29.2","desecrat","44","desert","51.2","desiccat","45.4","design","26.4-1","designat","29.1","desir","32.1",
"desist","69","despair","31.3","despis","31.2","despoil","10.6","destabili","45.4","destroy","44","detach","23.3","detail","29.2","detect","30.1","deter","67",
"deteriorat","45.4 45.5","determin","84","detest","31.2","detonat","45.4","devalu","45.4","devastat","44","develop","48.1.1 48.3","deviat","23.4","devis","55.5-1","devot","79",
"devour","39.4","diagnos","29.2","dicker","36.1","dictat","37.1-1","di","48.2","differ","36.1","differentiat","23.1","diffus","45.4","dig","19 26.4-1","dilut","45.4",
"dim","45.4","diminish","45.4 45.6","din","39.5","dip","9.3","direct","26.7-1","disagre","36.1 36.4","disappear","48.2","disappoint","31.1","disapprov","31.3","disarm","10.6",
"disassembl","23.3","disassociat","23.1","disbeliev","31.2","disburs","13.2","discard","17.1","discern","30.1","discharg","10.1","disclos","37.7","discombobulat","31.1","discomfit","31.1",
"discompos","31.1","disconcert","31.1","disconnect","23.1 23.3","discontinu","55.2","discourag","67 77","discover","84 29.5-2","disdain","31.2","disembark","51.1-1","disenfranchis","42.3","disengag","10.1",
"disfigur","44","disgorg","10.1","disgrac","31.1","disgruntl","31.1","disguis","29.2","disgust","31.1","dishearten","31.1","disillusion","31.1","disintegrat","45.4","dislik","31.2",
"dislodg","10.1","dismantl","23.3","dismay","31.3","dismiss","10.1","dismount","23.3","disorgani","45.4","disparag","33","dispatch","11.1","dispers","45.4","dispirit","31.1",
"displeas","31.1","disput","36.1","disquiet","31.1","disrob","41.1.1","dissatisfy","31.1","dissembl","26.1","disseminat","13.2","dissent","36.1","dissipat","45.4","dissociat","23.1",
"dissolv","45.4","dissuad","67","distill","10.4.1","distinguish","23.1","distort","26.5","distract","31.1","distress","31.1","distribut","13.2","distrust","31.2","disturb","31.1",
"dither","53.1","div","35.2","diverg","23.4","diversify","45.4","divert","31.1","divest","10.6","divid","108-1","divorc","36.2","divulg","37.7","dock","9.10-1",
"doctor","29.8-1","dodg","52","doff","10.1 10.2","domesticat","45.4","dominat","47.8","don","41.3.1","donat","13.2","dot","9.8","double-cross","29.8","double-wrap","9.7-2",
"doubl","45.4","doublecross","29.8","doubt","29.5-1 33","dous","9.8","downlink","11.1","doz","40.4","drag","11.4 35.2","drain","10.3-1","drap","9.7-2","draw","25.2 26.7-2",
"drawl","37.3","dread","31.2","dream","62","dredg","35.2","drench","9.8","dress","41.1.1","dribbl","40.1.2","drift","47.3 51.3.1 51.3.2","drill","21.2-2","drink","39.1",
"drip","9.5","driv","51.4.2 11.5-1","dron","38 37.3","drool","40.1.2 43.4","droop","47.6","drop","9.4 45.6 51.3.1","drug","9.9","dry","45.4","drydock","9.10-1","dub","29.3",
"duck","52","duel","36.4-1","dull","45.4","dumbfound","31.1","dump","9.3","dup","59","duplicat","25.4","dust","9.7-1","dwell","46 47.1-1","dy","24 41.2.2",
"earn","13.5.1","eas","45.4","eat","39.1","echo","47.4","eject","10.1","elaborat","37.11-1-1","elat","31.1","elbow","18.3","elect","29.1","electrify","31.1",
"electrocut","42.2","elevat","9.4","eliminat","10.1","elucidat","37.1.1","elud","52","emaciat","45.4","emanat","48.1.1 43.4","emancipat","10.5","emasculat","45.4","embalm","45.4",
"embarrass","31.1","embattl","13.4.2","embed","9.1-1","embellish","9.8 25.3","embezzl","10.5","embitter","45.4","emblazon","9.8","embolden","31.1","emboss","25.1","embrocat","45.4",
"embroider","25.1","emerg","48.1.1","empathi","88.2","emplac","9.1","employ","29.2 13.5.3","empty","10.3 45.4","encircl","9.8 47.8","enclos","47.8","encompass","47.8","encourag","59 102",
"encrust","9.8","end","55.1","endow","9.8","endur","47.1-1","energi","45.4","enervat","31.1","enflam","31.1","enforc","63","engag","22.2 29.2 31.1","engender","27",
"engrav","25.1","engross","31.1","engulf","47.8","enhanc","45.4","enjoy","31.2","enlarg","45.4","enlighten","31.1","enlist","29.2","enliven","31.1","enrag","31.1",
"enraptur","31.1","enrich","9.8","enroll","29.2","enslav","45.4","ensnar","59","ensu","48.3","ensur","99","entangl","22.2","enter","51.1-1","entertain","31.1",
"enthrall","31.1","entic","31.1","entranc","31.1","entrap","59","entreat","58.1","entrust","13.4.1-1","entwin","22.2","envisag","29.2","envision","29.2","envy","31.2",
"equip","13.4.2","equivocat","53.1","eradicat","10.1","eras","10.4.1","erect","26.1","erod","45.5 47.2","erupt","48.1.1","escap","51.1-1","eschew","52","escort","51.7 29.8",
"establish","29.2 55.5-1","esteem","31.2","estimat","54.4","etch","25.1","eulogi","33","evacuat","10.2","evad","52","evaluat","34","evaporat","45.4","even","45.4",
"evict","10.1","eviscerat","10.8","evolv","26.2 48.1.1","exact","13.5.2","exalt","31.2","examin","30.2 35.4","exasperat","31.1","excavat","35.2","exceed","90","excel","90",
"excerpt","0","exchang","13.6","excis","10.1","excit","31.1","exclaim","37.7","excommunicat","10.1","excoriat","33","excus","33","execrat","31.2","execut","42.1",
"exercis","26.8","exhal","40.1.3-1","exhaust","31.1","exhibit","29.5 37.1-1","exhilarat","31.1","exil","36.4","exist","39.6 47.1-1","exit","51.1-1","exonerat","10.6","exorcis","10.5",
"expand","45.4","expect","62","expectorat","40.1.2-1","expel","10.1 10.2","experienc","30.2","expir","48.2","explain","37.1","explod","43.2","exploit","29.2","explor","35.4",
"expos","48.1.2","expound","37.1.1","express","48.1.2","expung","10.4.1","exterminat","44","extirpat","10.1 44","extol","33","extort","10.5","extract","10.1","extradit","10.2",
"extrapolat","108-2","extrud","10.1","exud","43.4 48.1.1","ey","30.2","fabricat","26.4","fac","98","fad","45.4","fail","74-2","faint","40.8.4","fall","45.6 51.1-2",
"falter","47.3","fam","33","famish","45.4","fanatici","45.4","fancy","29.4","fart","40.1.1","fascinat","31.1","fashion","26.1","fasten","22.3","fathom","87.2-1",
"fatigu","31.1","fatten","45.4","fault","33","favor","31.2","fawn","28","fax","37.4","faz","31.1","fear","31.2 31.3","feast","39.5","feather","9.9",
"federat","45.4","feed","13.1-2","feel","30.1 30.4","feign","55.5-1","felicitat","33","fenc","47.8 9.9","ferment","45.5 47.2","ferret","35.6","ferry","51.4.1 11.5","fess","37.3",
"fester","47.2","festoon","9.8","fetch","13.5.1","feud","36.1","fidget","49","fight","36.3","figur","29.5","filch","10.5","fil","9.10","fill","9.8 45.4",
"film","25.4","filter","10.4.1","find","13.5.1","fin","54.5","finish","55.1","fir","17.1","firm","45.4","fish","13.7 35.1","fit","54.3","fix","26.3-1",
"fizz","43.2 47.2","flabbergast","31.1","flam","43.1","flank","47.8","flap","47.3 40.3.2","flar","43.1","flash","40.3.2 43.1","flatten","45.4 21.2-1","flatter","31.1","flaunt","48.1.2",
"fleck","9.8","fle","51.1-1","fleec","10.6","flex","40.3.2","flick","40.3.2","flicker","43.1","flinch","40.5","fling","17.1","flip","17.1","flirt","36.1",
"float","11.2-1 47.3 51.3.1 51.3.2","flock","47.5.2","flog","18.3","flood","45.4 9.8","floor","31.1","flop","47.6 50","flourish","47.1-1","flow","47.2","flower","45.5 47.2","fluctuat","45.6",
"flush","10.4.1","fluster","31.1","flutter","40.3.2 47.3","fly","11.5-1 47.6 51.3.2 51.4.2","foam","43.4 47.2","focus","87.1","fog","57","fold","26.5","follow","47.8","fondl","20.1",
"fool","59-1","forbid","67","forc","59","forese","29.5-1","foretell","29.5-1","forfeit","13.2","forg","25.2 25.4","forgiv","33","fork","9.10","form","26.4",
"formulat","26.4 26.1","fortify","45.4","forward","11.1-1","fossili","45.4","found","55.5-1 97.1","fowl","13.7","fractur","45.1 40.8.3","fram","9.8 9.9","fray","45.4","fre","10.6",
"freez","26.5 45.4","freshen","45.4","fret","31.3","frighten","31.1","frock","41.1.1","frolic","51.3.2","frost","45.4","froth","47.2","frown","40.2","frustrat","31.1",
"fry","45.3 26.3-2","fuel","9.9","fumbl","35.5","fum","31.3","function","29.6","funnel","9.3","furnish","13.4.1","furrow","45.2","further","102","fus","22.1-1-1 22.3-1",
"gag","9.9 42.3","gain","45.6","gall","31.1","gallop","51.3.2","galvani","31.1","gambl","94 70","gap","30.3 40.2","garag","9.10-1","gas","9.9","gasify","45.4",
"gasp","40.2","gather","13.5.1 22.3-2 47.5.2","gaug","34.2","gaz","30.3","gear","26.9","gelatini","45.4","generat","27 26.2","germani","45.4","germinat","43.4 45.4 45.5, 47.2","gestur","40.3.1",
"get","13.5.1-1","gib","33","gift","13.2","giggl","40.2","giv","13.1-1","gladden","31.1, 31.3-1, 45.4","glanc","30.3","glar","40.2 30.3","glaz","24","gleam","43.1",
"glean","14","glid","51.3.2","glimps","30.2","glint","43.1","glisten","43.1","glitter","43.1","gloat","31.3","globetrot","51.3.2","glorify","37.12 33","glow","43.1",
"glower","40.2","glu","22.4","gnash","40.3.2","gnaw","39.2-1","go","47.7 51.1-2","gobbl","39.3","golf","51.3.2","gossip","37.6","goug","21.2-1","grab","10.5 13.5.2 15.1-1",
"grac","9.8","grad","29.10","graft","22.3-2","grant","13.3 29.5","grappl","83-1-1","grasp","15.1 87.2","gratify","31.1","gray","45.4","graz","20 39.5","green","45.4",
"greet","33","griev","31.1 31.3","grill","26.1 26.3-1 45.3","grimac","40.2","grin","40.2","grind","21.2-1 26.1 40.3.2","grip","15.1","grip","37.8","groan","37.3 40.2","groom","41.1.2",
"grop","35.5 20-1","ground","9.10 97.1","group","47.5.2 22.3-2 29.10","grous","37.8","grow","45.4 45.6","growl","37.3 40.2","grudg","31.2","grumbl","37.3 37.8","grunt","37.3","guarante","13.3 29.5 37.13",
"guard","29.8","guess","29.5","guid","51.7","gulp","39.3-2","gut","10.7","guzzl","39.3","gyrat","49","hack","21.1 26.1","haggl","36.1","hail","57",
"hallucinat","31.3-8","halt","45.4 55.1","hammer","9.3","hand-count","29.6","hand-deliver","11.1","hand-paint","25.1","hand","11.1-1","handcuff","22.4","handl","15.1","hang","9.2-1 9.7-1",
"hangar","9.10-1","happen","48.3","harass","31.1","harden","45.4","harm","31.1","harness","9.9 22.4","harry","59-1","hasten","45.4 51.3.2 53.2","hatch","26.1 26.2","hat","31.2",
"haul","11.4","haunt","31.1","hay","13.7","head","47.8","heal","45.4","heap","9.7","hear","30.1","hearten","31.1","heat","45.3 45.4","heav","12",
"heft","11.4","heighten","45.4","helicopter","51.4.1","help","72-1","herald","29.2 33","herd","47.5.2","hesitat","53.1","hew","21.1 23.2","hid","16","high-fiv","29.2 33 40.3.3",
"highjack","10.5","hijack","59-1","hik","51.3.2","hinder","67","hing","22.4","hint","37.7","hir","13.5.1 29.2","hiss","37.3","hit","18.1 18.4","hitch","22.4",
"hoard","15.2","hobbl","51.3.2","hock","13.1","ho","9.3","hoist","9.4","hold","15.1","holler","37.3","hollow","10.3 45.4","honk","43.2","honor","29.2 33",
"hoodwink","59-1","hook","22.4","hoot","38 37.3 43.2","hop","51.3.2 47.5.1-1","hop","32.2 62","hopscotch","51.3.2 47.5.1-1","horrify","31.1","host","29.8","hous","9.10 54.3","hover","47.3 47.6",
"howl","37.3 43.2 38 40.2","hug","36.2 47.8","hum","40.8.2 43.2 26.7","humbl","31.1","humiliat","31.1","hunch","40.3.2","hunt","35.1","hurl","17.1","hurry","53.2 51.3.2","hurt","31.1 40.8.3-2",
"hurtl","51.3.2","hush","45.4 42.3","hybridi","45.4","hydrogenat","45.4","hyperventilat","40.1.2","hypnoti","31.1","identify","29.2","idoli","31.2","ignit","45.4","illuminat","25.3",
"illustrat","25.3","imagin","29.1 29.2","imbu","9.8","imitat","25.4","immers","9.1","immuni","45.4","impair","31.1","impeach","33","imped","67","impel","59",
"impersonat","29.6","implant","9.1","implor","58.1","imply","78","impos","63","impound","10.5","imprecat","33","impregnat","9.8","impress","31.1","imprint","25.1",
"imprison","9.10","improv","45.4","improvis","26.7-1","incens","31.1","inch","51.3.2","incinerat","45.4","incis","25.1","incit","59","inclin","96","includ","107 65",
"incorporat","22.2","increas","45.4 45.6","incriminat","33","incubat","45.4","indicat","78-1","indict","33","induc","59","infect","9.8","infer","29.5-1 97.2","infest","9.8",
"inflam","31.1","inflat","45.4","inform","37.2 37.9","infuriat","31.1","ingest","39.4","ingrain","25.1","inhal","40.1.3-2","inherit","13.5.2","initial","25.3","initiat","55.2",
"inject","9.8","injur","40.8.3-2","inlay","9.8","innovat","55.5","inquir","37.1.2","inscrib","25.1","insert","9.1-1","insinuat","37.7","insist","37.7","inspect","30.2 35.4",
"inspir","31.1","install","9.1","institut","55.5-1","institutionali","92","instruct","37.9-1","insult","31.1 33","insur","99","integrat","22.2","intend","61 62","intensify","45.4",
"interact","36.1","interconnect","22.2","interest","31.1","interlac","22.2-2-1","interlink","22.2","intermarry","22.2","intermingl","36.1","intermix","22.1-1","interpret","29.2","interrogat","37.1.3",
"intersect","47.8","interspers","9.8 22.2","intertwin","22.2-2-1","interview","37.1.3","intimat","37.7","intimidat","31.1","inton","26.7-1","intoxicat","31.1","intrigu","31.1","introduc","22.2",
"intrud","48.1.2","inundat","9.8","invalidat","106","inveigl","59","invent","26.4","invert","45.4","invest","13.4.2","investigat","30.2 35.4","invigorat","31.1","involv","107",
"ioni","45.4","irk","31.1","iron","26.3-2 10.4.2","irritat","31.1","isolat","16","issu","13.3 13.4.1 48.1.1","itch","40.8.1","jab","18.1-1","jad","31.1","jail","9.10",
"jam","9.7-2","jangl","43.2","jar","31.1 31.4","jerk","12-1","jest","36.1","jet","51.4.1","jib","89","jiggl","47.3","jilt","29.2,13.6","jingl","43.2",
"jiv","51.5","jog","51.3.2","join","22.1-2","jok","36.1","jollify","31.1","jolt","31.1","jostl","12-1-1","journey","51.3.2","joust","36.4-1","judg","29.4 29.2 34.2",
"jug","9.10","jumbl","22.3","jump","45.6","justify","37.1.1","jut","47.6","keep","15.2 55.6","kennel","9.10","kick","11.4-1 17.1 18.1 23.2 40.3.2 49","kidnap","10.5","kill","42.1-1 42.3",
"kindl","45.4 26.3-1","kiss","36.2 20","knead","26.5","kneel","50 40.3.3 47.6","knif","18.3 42.2","knight","29.7","knit","26.1","knock","18.1","knot","22.4","know","29.5 29.9-1-1 29.2-1",
"label","9.9 25.3 29.3","lac","22.4","lambast","33","lament","31.2","laminat","45.4","lampoon","33","land","9.10","languish","47.1-1","laps","48.2","lash","22.4",
"last","54.2","latch","22.4","lather","41.2.2","laud","33","laugh","40.2","launch","55.5-1","lay","9.2","leach","10.4.1","lead","51.7","leak","43.4",
"lean","9.2-1 47.6 50","leap","51.3.2","learn","14","leas","13.5.1","leav","51.1-1 51.2","lectur","37.11-1","leer","30.3","lend","13.1-1","lengthen","45.4","lessen","45.4",
"level","45.4","levitat","45.4","liberat","10.5 80","licens","101","lick","39.2 20","li","47.6 50","lift","9.4","light","45.4","lighten","45.4","lignify","45.4",
"lik","31.2","lilt","37.3 43.2","limit","76","limp","51.3.2","lin","9.8","linger","47.1-1 53.1","link","22.1-2 22.4","liquefy","45.4","liquidat","42.1","liquify","45.4",
"listen","30.3 35.5","litter","9.8","liv","39.6 46 47.1-1","load","9.7-2","loaf","53.1","loan","13.1-1","loath","31.2","lobby","58.1","lock","22.4","lodg","46",
"loiter","53.1","long","32.2","look","30.3 30.4 35.5","loom","47.1-1 47.6","loop","9.6","loosen","45.4","lop","10.1","los","13.2","loung","47.6","lov","31.2",
"low","38","lower","9.4","lug","11.4-1","lull","31.1 42.3","lumber","51.3.2","lump","22.3-1 29.10","lurch","51.3.2","lur","59-1","lurk","47.1-1","macerat","45.4",
"madden","31.1, 31.3-1","magneti","45.4","magnify","45.4","mail","11.1-1","maim","44","maintain","29.5 55.6","mak","26.1-1","malign","33","manag","74-1-1 75-2","manifest","48.1.2",
"manipulat","59-1","manufactur","26.4","march","51.3.2","marinat","9.8","mark","29.1","marry","22.2 36.2","martyr","29.7","marvel","31.3","mash","21.2-1","mask","9.8",
"masquerad","29.6","mass-produc","26.4 26.7-2","mass","47.5.2","massacr","42.1","massag","20-1","mastermind","55.5","match","22.2-1","mat","22.2","materiali","48.1.1","matter","31.4 91",
"matur","26.2 45.4","mean","29.5","meander","51.3.2 47.7","measur","54.1","mechani","45.4","meditat","31.3-1","meld","22.1-1-1","mellow","45.4","melt","26.5 45.4","memori","14",
"menac","31.1","mention","37.7","meow","38","merg","22.1-1","mesmeri","31.1","metamorphos","26.6","mew","38","miff","31.1","milk","10.7","mill","21.2-2",
"minc","51.3.2","mind","31.3","min","10.9 35.1","minerali","45.4","mingl","36.1","mint","26.4-1","misappropriat","10.5","misconstru","87.2","misdiagnos","29.2","misguid","51.7",
"misinterpret","87.2","mislead","59","misread","30.1","miss","31.2","misspend","104","mistrust","13.4.1","misunderstand","87.2","mix","22.1-1","moan","37.3 40.2","mob","9.7",
"mobili","45.4","mock","33","model","26.4","moderat","45.4","moderni","45.4","modulat","45.4","moisten","45.4","moisturi","41.1.1","mold","26.1-1","molest","31.1",
"mollify","31.1","monitor","35.4","moo","38","moor","9.10-1","moot","29.1","mop","10.4.2","morph","26.6 45.4","mortify","31.1","motivat","59 58.1","motori","45.4",
"mount","9.1","mourn","31.2 31.3","mov","11.2 51.3.1","mow","21.2-2","muddl","31.1 22.2-2","muddy","45.4","muffl","42.3","multiply","45.4 108-1","mumbl","37.3","munch","39.2-1",
"murder","42.1","murmur","37.3","mus","31.3-8","mushroom","45.6","mutat","26.6.1","mut","45.4","mutilat","44","mutter","37.3","muzzl","9.9 22.4","mystify","31.1",
"nab","10.5","nail","22.4","nam","29.3","narrat","37.5","narrow","45.4","nasali","45.4","nationali","45.4","naturali","45.4","nauseat","31.1","navigat","51.4.2",
"neaten","45.4","necessitat","103","need","32.1 103","neglect","75-1-1","negotiat","36.1","neigh","38","nest","13.7","nestl","47.6","nettl","31.1","network","22.1-2-1",
"neutrali","45.4","nibbl","39.2","nick","21.2-1 40.8.3-2","nickel","9.9","nicknam","29.3","nitrify","45.4","nobbl","10.5","nod","40.3.1-1","nominat","29.1","normali","45.4",
"notch","21.2-1","not","30.2 37.7","notic","30.1","notify","37.2 37.9","nourish","42.2","nudg","17.1","nullify","106","numb","31.1","number","29.10","nurs","29.8",
"nut","13.7","nuzzl","36.2 47.8","object","37.8","obligat","44","obscur","45.4","observ","30.2 35.4","obsess","31.3","obtain","13.5.2-1","occupy","31.1","occur","48.3",
"offend","31.1","offer","13.3 29.2","officiat","29.6","offload","9.7-2","ogl","30.3","oil","9.9","oink","38","omit","10.1 75-1","ooz","43.4","open","40.3.2 45.4 47.6",
"operat","45.4","oppos","22.2","ordain","29.1","order","60-1","organi","26.4 55.5-1","ornament","9.9 9.8 25.3","orphan","29.7","ossify","45.4","ostraci","10.1","oust","10.1",
"outbid","90","outfit","13.4.2","outlaw","29.7","outliv","47.1-1","outnumber","90","outrag","31.1","overaw","31.1","overburden","13.4.2","overcharg","54.5","overestimat","54.4",
"overhang","47.8","overload","9.7-2","overrat","29.1 29.6 54.4","overstep","90","overstimulat","31.1 59-1","overturn","45.4","overwhelm","31.1","ow","13.3","oxidi","45.4 45.5","oyster","13.7",
"pacify","31.1","pack","9.7-2","packag","22.3-2","pad","9.8 51.3.2","pain","31.1 40.8.1","paint","25.1","pair","22.2 22.3-2 ","pal","45.4","pander","31.4","pant","40.1.1",
"parch","45.3","pardon","10.6","part","23.1","partition","23.3","party","36.3-1","past","22.4","pastur","9.10","patch","9.9","patrol","35.2","pav","9.8",
"paw","18.2","pay","13.1-1","peal","43.2","pearl","13.7","peck","39.2","pedal","51.4.2","peddl","13.1-1","pe","40.1.2","peek","30.3","peel","10.7",
"peep","38","peer","30.3","peev","31.1","penali","33","pepper","9.9","perceiv","30.2","perch","47.6 9.2 50","perform","26.7-1","perfum","9.9","perish","48.2",
"perplex","31.1","persecut","33","persist","47.1-1 55.1","perspir","40.1.3-1 43.4","perturb","31.1","phon","13.5.1 37.4","photograph","25.4","pick","13.5.1","picket","52","pickl","45.4",
"pickpocket","10.5","pigeonhol","29.2","pil","9.7-1","pilfer","10.5","pillory","9.10","pilot","29.8","pin","22.4","pinch","20","pin","32.2","ping","43.2",
"pioneer","29.8","piqu","55.1","pirat","10.5","pit","10.7","pitch","17.1","pity","31.2","plac","9.1","plagiari","10.5","plagu","31.4","plan","26.1",
"plant","9.7","plaster","9.7-1 9.9 22.4","plat","9.8","play","26.7-1","pleas","31.1","plow","10.4.2","pluck","10.4.1 13.5.1 41.2.2","plummet","45.6","plunder","10.6","plung","45.6",
"ply","13.4.2","poach","45.3 26.3","pocket","9.10","point","40.3.1-1","poison","42.2","pok","19","polari","45.4","polic","29.8","polish","9.9 10.4.1","pollut","9.8",
"polymeri","45.4","pool","22.1-2","pop","43.2","portray","29.2","pos","29.6","position","9.1","post","11.1","postmark","9.9","pot","9.10","potter","53.1",
"pound","18.1","pour","9.5 43.4","pranc","51.3.2","prawn","13.7","pray","32.2","preach","37.1","preced","47.8","preform","26.4","preoccupy","31.4","prepar","26.3-1",
"present","13.4.1","preserv","29.5","press","12-1","presum","29.4","pric","54.4","prick","19","prid","33","print","25.2","pri","31.4","prob","35.2",
"proceed","55.1","proclaim","37.3","procrastinat","53.1","procur","13.5.2-1","produc","26.4 26.7-2","profess","29.4","proffer","13.2","project","47.6","proliferat","45.4","promis","13.3",
"pronounc","29.3","propos","37.7","prospect","35.2","prov","29.4","provid","13.4.1","prowl","51.3.2","prun","21.2-2 10.4.1","puk","40.1.2","pulsat","47.3","pulveri","21.2-1 45.4",
"pummel","18.3","pump","9.7-1","punch","18.2","punctuat","23.1-1","purchas","13.5.2-1","purg","10.4.1 10.6","purloin","10.5","pursu","51.6","push","9.3 12-1 11.4-1 23.2","put","9.1",
"putter","53.1","puzzl","31.4","quack","38","quadrupl","45.4","qualify","29.2","quarrel","36.1","quench","31.1, 42.3","quibbl","36.1","quicken","45.4","quiet","45.4",
"quieten","45.4","quiz","35.4","quot","37.1-1","rabbit","13.7","rac","51.3.2","rack","35.2","radiat","43.4","radio","37.4","rag","37.3","raid","35.4",
"rain","57","rais","9.4 40.3.2","rak","9.3 10.4.2","ram","18.4-1","rambl","51.3.2","rank","29.2 29.6","rankl","31.1","ransack","35.4","rat","29.1 29.6 54.4","rattl","43.2",
"rav","31.3","ravish","31.1","re-creat","26.4 27","re-elect","29.1","re-emerg","48.1.1","re-fight","36.4-1","reach","13.5.1","react","31.3","read","14 37.1-1 54.1","realign","31.1",
"reallocat","13.3","reap","10.1","reapply","105","reassembl","26.1","reassert","48.1.2","reassign","13.3","reassur","31.1","rebuk","33","recall","10.2","recap","37.7-1",
"recaptur","10.5","reced","51.1-2","receiv","13.5.2","recirculat","47.3","recit","26.7-1 37.1","reckon","29.1","recogni","29.5 30.2","recoil","40.5","recollect","29.2","recombin","22.1-1",
"recommend","29.2 48.1.2","recompens","33","reconven","47.5.2","record","25.4","recount","37.7","recoup","10.5 13.5.2","recreat","26.4 27","recur","48.3","recus","10.1 10.5","redden","45.4",
"redecorat","9.8 25.3","redeem","10.5","redevelop","26.1 26.2","redoubl","45.4","reek","43.3","reel","40.8.2","reelect","29.1","reevaluat","34","refashion","26.1","refresh","31.3",
"refund","13.1-1","regain","10.5 13.5.2","regal","31.1","regard","29.2 30.2","register","54.1","regret","31.2","regulari","45.4","rehir","13.5.1 29.2","reimburs","13.2","reinscrib","25.1",
"reinstall","9.1","reinstat","29.2","reiterat","37.7","reject","29.2","rejoic","31.3","rejoin","22.1-2","rejuvenat","31.1","rekindl","45.4","relax","31.1","reliev","10.6",
"relinquish","13.2","relish","31.2","remain","47.1-1","remak","26.1-1","remark","37.7","remarry","22.2","remember","29.2","reminisc","62","remit","13.2","remov","10.1 10.2",
"renegotiat","36.1","rent","13.5.1","reopen","40.3.2 45.4 47.6","repackag","22.3-2","repaint","25.1","repay","13.1-1","repeat","37.7 55.1","repel","31.1","replay","26.7-1","replenish","9.8",
"report","29.1 37.7","repossess","10.5","represent","29.2","reprimand","33","reproach","33","reproduc","45.4","repuls","31.1","rescu","10.5","resent","31.2","reserv","13.5.1",
"resid","47.1-1","resonat","47.4","resound","47.4","rest","47.6 9.2-1","resubmit","13.2","result","48.1.1","resum","55.1","resupply","13.4.1","retail","13.1-1","retch","40.1.2-1",
"retrain","29.2","retriev","13.5.2","return","51.1-2","reus","29.2 54.3","revel","31.3","reverberat","47.4","rever","31.2","review","34","revil","33","revitali","31.1",
"revolv","47.3","reward","33","rhym","22.2","rid","10.6","ridden","9.8","riddl","9.8","rid","51.4.2","ridicul","33","ril","31.1","rim","47.8",
"ring","43.2","rins","10.4.1,42.2.2","rip","23.2 45.1","ripen","45.4","ris","45.6 47.6 48.1.1 50 51.1-2","roam","51.3.2","roar","37.3 43.2 38","roast","33","rob","10.6","rob","41.3.3",
"rock","47.3 49","roil","47.2","roll","9.6 11.2-1 22.3-2 23.2 26.1 40.3.2 43.2 51.3.1 51.3.2","romp","51.3.2","roost","47.6","rot","47.2 45.5","rotat","47.3","roust","10.1","rov","51.3.2","row","51.4.2 11.5-1",
"rub","9.7-1 10.4.1 40.3.2","ru","31.2","ruffl","31.1","rul","29.3","rummag","35.5","ruptur","40.8.3 45.4","rush","51.3.2","rust","45.5 47.2","sacrific","13.2","sadden","31.1 31.3",
"sag","47.6","sail","51.4.2","salivat","31.3","salt","9.9","salut","29.2 33 40.3.3","sap","10.6","saponify","45.4","satiat","31.1","satiri","33","satisfy","31.1",
"saturat","9.8","sav","13.5.1 54.5","savor","31.2","savvy","29.5 29.9-1-1 29.2-1","say","37.7","scald","45.3","scamper","51.3.2","scan","30.2 35.4","scandali","31.1","scar","25.1",
"scar","31.1","scatter","9.7-1","scaveng","35.2","schem","71","scoot","51.3.2","scorch","43.1 45.4","scorn","33","scout","35.2","scowl","40.2","scrabbl","35.5",
"scrap","21.1 10.4.1 9.3","scratch","21.1","scrawk","38","scream","37.3 38 43.2","scribbl","25.2","scriptwrit","25.2 26.7-2 37.1-1","scrutini","30.2 34 35.4","scuffl","36.4-1","sculpt","26.1","scurry","51.3.2",
"seal","22.4","search","35.2","season","9.8","seat","54.3","section","23.3","seculari","45.4","secur","13.5.1","sedat","42.3","se","29.2 30.1","seed","10.7",
"seek","35.6","seep","43.4","seesaw","45.6","segregat","23.1","sei","10.5 13.5.2","select","13.5.2","sell","13.1-1","send","11.1-1","sens","30.1","separat","10.1 23.1",
"sequester","16","serv","29.6","set","9.1","sever","10.1 23.1","sew","22.3-2","shackl","22.4","shak","31.1 40.3.2 40.6 47.3","shambl","51.3.2","sham","59-1","shap","26.1-1 27",
"shark","13.7","sharpen","45.4","shatter","45.1","shav","10.4.1","shear","10.4.2","sheath","9.10","shed","43.4","shell","10.7","shelter","46 9.10 47.1 16","shelv","9.10",
"shepherd","29.8","shift","11.1","shimmer","43.1","shin","43.1","ship","11.1-1","shit","40.1.2","shiver","40.6","shock","31.1","shoo","10.1 10.2","shoot","17.1",
"shop","35.2","shoplift","10.5","short-circuit","45.4","shortcircuit","45.4","shorten","45.4","shoulder","9.10","shout","37.3","shov","17.1 12-1 11.4-1 23.2","shovel","10.4.2-1","show","29.5 37.1-1 40.3.2",
"shower","9.7 17.2","shred","21.2-1","shrimp","13.7","shrink","40.5 45.4","shrivel","45.4","shroud","9.8","shrug","40.3.1-1","shuffl","51.5","shun","52","shunt","11.1",
"shut","45.4","shutter","9.9","shuttl","11.5-1","sicken","31.1","sidestep","52","sidl","51.3.2","sift","23.3","sight","30.2","sign","25.1","signal","37.4",
"silicify","45.4","silver","45.4","simmer","45.3","sing","26.7-1 37.3 43.2","sink","45.4","sip","39.2","siphon","9.3 10.4.2","sit","9.2-1 47.6 50","situat","9.1","sizzl","43.2",
"skat","51.4.1","sketch","25.2","skewer","9.10","ski","51.4.1","skin","10.7","skipper","29.8","skirt","47.8","skulk","51.3.2","skyrocket","45.6","slacken","45.4",
"slam","17.1","slander","33","slash","21.1","slather","9.7-2","slaughter","42.1","slay","42.1","sleep","40.4","slic","21.2-1","slim","45.4","sling","17.1",
"slit","21.2-1","slither","51.3.2","slobber","40.1.2-1","slog","51.3.2","slosh","9.5","slow","45.4","slumber","40.4","slump","47.6 50","slurp","39.2-2","smart","40.8.2",
"smarten","45.4","smash","45.1","smear","9.7","smell","30.1 30.4","smirk","40.2","smok","47.2","smooth","45.4","smother","9.8","smuggl","10.5 11.1","snack","39.5",
"snap","37.3 38","snar","9.9","snatch","13.5.2 10.5","sneak","51.3.2","snicker","40.2","sniff","30.3","snigger","40.2","snip","13.7","snitch","10.5 13.5.1","snoop","30.3 35.5",
"snor","40.1.1 40.2","snort","38 40.2","snub","33","soar","45.6","sob","40.2","sober","45.4","sock","18.3","soften","45.4","soil","9.8","solac","31.1",
"solidify","45.4","sooth","31.1","soulsearch","35.2","sound","30.4","sour","45.4","sovieti","45.4","sow","9.7-1","spac","9.1","span","47.8","spar","36.1",
"sparkl","43.1","spatter","9.7-1","spawn","27","speak","37.5","speed","51.3.2","spellbind","31.1","spill","9.5 43.4","spin","9.6 26.1 51.3.1","spindl","9.10","splash","9.7",
"splatter","9.7","splay","45.4","split","23.2 40.8.2 40.8.3-1 45.1","spong","13.7","sponsor","29.8","spook","31.1","spool","9.10","spoon-feed","39.7","spot","30.2","spout","43.4",
"sprain","45.1 40.8.3","sprawl","47.6 50","spray","9.7","spraypaint","24 25.2","spread","9.7-1","sprinkl","9.7-1","sprout","43.4 45.4 45.5 47.2","spurt","9.5","sputter","43.2","spy","30.2",
"squabbl","36.4","squat","47.6 50","squeak","37.3 38 43.2","squeez","9.3","squirm","49","squirt","9.7","stab","42.2 18.2","stabl","9.10-1","stack","9.7-1","staff","9.8",
"stagger","51.3.2","stagnat","45.5 47.2","stain","24 9.8","stamp","25.1","stand","9.2-1 47.6 50","stapl","22.4","star","29.8","starch","24 9.8","star","30.3","start","55.1",
"stash","9.1","stat","37.7","stay","47.1-1","steady","45.4","steal","10.5 13.5.1","steam","45.3","steamroller","59","steer","26.7-1","stem","48.1.1","stew","45.3",
"stick","9.7-1 19 22.3-2","stigmati","33","stimulat","31.1","sting","20 31.1 40.8.2","stink","43.3","stir","22.3-1","stitch","22.4","stock","9.7-1","ston","10.7","stop","55.1",
"stor","15.2","storm","57","stow","9.1","straddl","47.6 50 47.8","straighten","45.4","strain","40.8.3-2","strangl","42.2","strap","22.4","strategi","62","stratify","45.4",
"stray","51.3.2","streak","51.3.2","stream","43.4","strengthen","45.4","stretch","40.3.2 45.4","strew","9.7","strid","51.3.2","stridulat","38","strik","18.1","strip","10.4.1 10.6",
"strok","20","stroll","51.3.2","struggl","36.1","strut","51.3.2","stub","40.8.3","stud","9.8","study","14 30.2 34","stuff","9.7","stultify","33","stumbl","51.3.2",
"stump","31.1","stun","31.1","stupefy","31.1","styl","26.4","subjugat","42.3","submit","13.2","substitut","13.6","subtract","10.1","suck","10.4.1 39.2","suffer","31.3",
"suggest","37.7","sulfuri","45.4","sully","9.8","sunder","23.3","supply","13.4.1","support","31.2","suppos","29.4","surg","45.6 48.1.1","surmis","29.5-1","surmount","47.8",
"surpris","31.1","surrender","13.2","surround","9.8","surveil","30.2 35.4","survey","30.2 35.4","suspect","29.5","swab","59","swap","13.6","swath","9.8","sway","47.3",
"sweat","43.4","sweep","10.4.1 9.3","sweeten","45.4","swell","45.5","swim","51.3.2","swindl","10.6","swing","47.6","swoon","40.8.4","synthesi","26.4","tabulat","26.1",
"tack","22.4","tag","9.9 25.3","tail","51.6","tailor","29.8","taint","9.8","taiwani","45.4","tak","10.5 11.3","tam","45.4","tan","45.4","tango","51.5",
"tantali","31.1","tap","18.1","tap","22.4","tar","9.9","tarnish","45.5 47.2","tarry","53.1","tassel","9.10","tast","30.1","tattoo","29.1","tauten","45.4",
"tax","54.5","taxi","51.4.1 11.5","teach","37.1-1","team","22.2","tear","23.2 45.1","teas","35.6","teem","47.5.1-1","teeter","47.3","telephon","37.4","televis","25.4",
"tell","37.1-1 37.2","tens","45.4","terminat","55.1","terrify","31.1","terrori","31.1","tether","22.4","thank","33","thatch","9.9","thin","45.4","think","29.4",
"thrash","18.3","threaten","31.1","thrill","31.3 31.1","thriv","39.6 47.1-1","throw","17.1","thrust","12","thud","18.4, 43.2","thumb","35.5","thunder","37.3 43.2","tick","43.2",
"tickl","20.1","ti","22.4","tighten","45.4","til","9.8, 9.9","tilt","47.6 45.4","tin","9.10","ting","24","tipto","51.3.2","tir","31.1 45.4","titter","40.2",
"tolerat","31.2","toll","43.2","toot","43.2","top","10.7","torment","31.1","toss","17.1 40.3.2 26.3-1","total","54.1","totter","47.3","touch","20 31.1","toughen","45.4",
"tow","11.4","tower","47.6 47.1","trac","25.2","track","35.3 51.6","trad","13.1 13.6","trail","51.6","traips","51.3.2","tramp","51.3.2","tranquili","45.4","transcend","90",
"transfer","11.1 13.2","transfix","31.1","transform","26.6","transit","48.3","transition","26.6 45.4","transmit","11.1-1","transpir","48.3","transport","11.1","trap","9.10","traumati","31.1",
"travel","51.3.2","treasur","31.2","treat","29.2","tre","9.10","trek","51.3.2","trembl","47.3 40.6","trickl","47.2","trill","38 37.3","trim","10.4.1","tripl","45.4",
"trot","51.3.2","troubl","31.1","trounc","36.4","truck","11.5-1","trudg","51.3.2","trumpet","37.3","trundl","51.3.2","trust","13.4.1","tuck","9.3","tug","11.4",
"tumbl","45.6 47.7 51.1-2","tunnel","35.5","turn","26.6 40.3.2 40.8.3-1 51.3.1","tutor","29.8","tweet","38","tweez","10.5 13.5.2 15.1-1","twiddl","40.3.2","twist","9.6 26.5","twitch","40.3.2","twitter","38 37.3",
"typ","25.2 25.4","ululat","38","unburden","13.4.2","uncap","45.4","underinflat","45.4","underli","47.8","underpin","31.2","undress","41.1.1","undulat","47.3","unfold","45.4",
"unfurl","45.4","uniform","41.1.1","unify","22.2","unioni","45.4","unit","22.2","unnerv","31.1","unseal","23.3","unseat","10.10","unsettl","31.1","unshackl","23.3",
"unti","22.4","upbraid","33","uplift","9.4","uproot","10.1","upset","31.1","urbani","45.4","urinat","40.1.2-1","us","29.2 54.3","vacation","56","vacuum","10.4.2",
"valu","29.2 31.2 54.4","vanish","48.2","vapori","45.4","vault","51.3.2","veil","9.8 9.9","venerat","31.2","vex","31.1","vibrat","47.3 45.4","victimi","33","videotap","25.4",
"vi","36.1","view","30.2","vilify","33","visit","36.3","visuali","29.2","void","10.6","volatili","45.4","volunteer","29.8","vomit","40.1.2","wad","26.5 9.3",
"waddl","51.3.2","wad","51.3.2","waft","47.3","wag","40.3.1 40.3.2","wail","37.3","wait","47.1-1","waken","45.4","walk","51.3.2","wallow","31.3","wander","51.3.2 47.4",
"wangl","10.5","want","29.1 32.1","war","36.1","warbl","38 37.3","warehous","9.10","warm","45.4","warn","37.9-1","watch","30.2","water","9.9","wav","40.3.1-1 47.3",
"waver","47.3","wax","9.9","weaken","45.4","weaponi","13.4.2","wear","41.3.1","weary","31.1","wed","22.2","wedg","9.3","weep","40.2","weigh","54.1",
"westerni","45.4","whack","18.1","whal","13.7","wheel","11.5","wheez","37.3","whelk","13.7","whimper","38","whinny","38","whirl","51.3.1 9.6","whirr","43.2",
"whisk","10.4.1","whisper","37.3","whistl","26.7-1 37.3 40.2","whiten","45.4","whittl","26.1-1","whoop","37.3","whoosh","43.2","widen","45.4","widow","29.7","wield","15.1",
"will","13.3","wilt","45.4 47.2","win","13.5.1","wind","9.6 10.7 26.5 51.3.1","wing","11.5-1 47.6 51.3.2 51.4.2","wink","40.3.1","winkl","10.1","wip","9.3 10.4.1","wir","11.5 37.4","wish","32.2",
"withdraw","10.1 10.5","wither","47.2 45.5","witness","29.8 30.2","wolf","39.3","wonder","31.3","woof","38","worm","51.3.2 47.3 47.5.1-1","worry","31.1","worsen","45.4","worship","31.2",
"wound","31.1","wow","31.1","wrap","9.7-2","wrest","10.5","wrestl","36.1","wriggl","49","wring","40.3.2","wrinkl","45.2","writ","25.2 26.7-2 37.1-1","writh","47.3 40.6",
"x-ray","25.4","yank","12-1 23.2","yap","38 37.3","yell","37.7 38","yellow","45.4","yelp","37.3","yield","13.3","yip","38","yodel","37.3","yowl","38",
"zag","51.3.2","zig","51.3.2","zoom","51.3.2","give_in","95","defer","95","succumb","95","capitulat","95","dispos","96","predispos","96","woo","96",
"invit","96","slant","96","readjust","26.9","readapt","26.9","prefer","31.2","reaffirm","31.2","respect","31.2","exclud","65","permit","65","welcom","65",
"take_over","93","take_on","93","sanction","64","overlap","22.2","conjoin","22.2","confederat","22.2","identity","22.2","interchang","22.2","interlock","22.2","interweav","22.2",
"harmoni","22.2","interrelat","22.2","enchant","31.1","enthus","31.1","pester","31.1","placat","31.1","provok","31.1","recharg","31.1","revolt","31.1","startl","31.1",
"taunt","31.1","tempt","31.1","titillat","31.1","tortur","31.1","try","31.1","croak","38","hee-haw","38","pip","38","purr","38","squawk","38",
"squeal","38","rumbl","38","screech","38","snarl","38","whin","38","niggl","31.4","grat","31.4","appeal","31.4","gush","48.1.1","plop","48.1.1",
"pop_up","48.1.1","reappear","48.1.1","show_up","48.1.1","spring up","48.1.1","superven","48.1.1","take shap","48.1.1","coronat","29.1","upgrad","29.1","understand","77","disprefer","77",
"hunch_up","50","lie_down","50","sit_down","50","slouch","50","squat_down","50","stand_up","50","stoop","50","forgo","52","shanghai","10.2","constru","97.1",
"format","97.1","contend","36.4","brawl","36.4","skirmish","36.4","spat","36.4","tussl","36.4","wrangl","36.4","request","58.2","supplicat","58.2","importun","58.2",
"go_on","55.1","pledg","55.1","start_off","55.1","recommenc","55.1","undertak","55.1","clad","41.3.3","garb","41.3.3","crinkl","45.2","distend","45.2","round","45.2",
"rumpl","45.2","scrunch","45.2","unroll","45.2","unwind","45.2","warp","45.2","log","13.7","snail","13.7","amerc","54.5","invoic","54.5","mulct","54.5",
"scrimp","54.5","spar","54.5","tip","54.5","undercharg","54.5","wager","54.5","waggl","49","wiggl","49","wobbl","49","quak","40.6","quiver","40.6",
"shudder","40.6","braid","41.2.2","coldcream","41.2.2","gel","41.2.2","henna","41.2.2","highlight","41.2.2","manicur","41.2.2","perm","41.2.2","plait","41.2.2","powder","41.2.2",
"roug","41.2.2","shampoo","41.2.2","soap","41.2.2","talc","41.2.2","towel","41.2.2","break_apart","45.1","break_down","45.1","break_up","45.1","fragment","45.1","rend","45.1",
"riv","45.1","sliver","45.1","splinter","45.1","barf","40.1.2","piss","40.1.2","spit","40.1.2","throw_up","40.1.2","crochet","26.1","scultpur","26.1","weav","26.1",
"seeth","47.5.3","run","18.4","butt","18.4","rap","18.4","slap","18.4","smack","18.4","squash","18.4","thump","18.4","aromati","9.9","asphalt","9.9",
"bait","9.9","brick","9.9","bridl","9.9","bronz","9.9","butter","9.9","calk","9.9","caulk","9.9","chrom","9.9","clay","9.9","diaper","9.9",
"dop","9.9","flour","9.9","forest","9.9","garland","9.9","glov","9.9","graffiti","9.9","gravel","9.9","greas","9.9","groov","9.9","halter","9.9",
"heel","9.9","ink","9.9","leash","9.9","leaven","9.9","lipstick","9.9","lubricat","9.9","mantl","9.9","mulch","9.9","panel","9.9","paper","9.9",
"parquet","9.9","plank","9.9","pomad","9.9","poster","9.9","putty","9.9","roof","9.9","rosin","9.9","rut","9.9","saddl","9.9","salv","9.9",
"sand","9.9","sequin","9.9","shawl","9.9","shingl","9.9","sho","9.9","slat","9.9","slipcover","9.9","sod","9.9","sol","9.9","spic","9.9",
"stopper","9.9","stress","9.9","string","9.9","stucco","9.9","sugar","9.9","sulphur","9.9","tarmac","9.9","ticket","9.9","turf","9.9","varnish","9.9",
"veneer","9.9","wallpaper","9.9","whitewash","9.9","wreath","9.9","yok","9.9","zipcod","9.9","dwindl","45.6","rocket","45.6","vary","45.6","calv","28",
"cub","28","foal","28","kitten","28","lamb","28","pup","28","whelp","28","lawyer","29.8","midwif","29.8","prostitut","29.8","soldier","29.8",
"boss","29.8","butler","29.8","caddy","29.8","chaperon","29.8","clerk","29.8","cox","29.8","crew","29.8","emce","29.8","mother","29.8","partner","29.8",
"refere","29.8","umpir","29.8","understudy","29.8","usher","29.8","valet","29.8","pull","11.4","schlep","11.4","tot","11.4","cub","21.2","dic","21.2",
"fillet","21.2","flak","21.2","gash","21.2","mangl","21.2","perforat","21.2","squish","21.2","lanc","21.2","spear","21.2","tread","21.2","blanch","40.8.4",
"blench","40.8.4","peg","29.2","pictur","29.2","recast","29.2","redraw","29.2","specify","29.2","stereotyp","29.2","typecast","29.2","prais","29.2","conceiv","29.2",
"reveal","29.2","shadow","51.6","disabus","10.6","disencumber","10.6","dispossess","10.6","purify","10.6","render","10.6","rifl","10.6","wean","10.6","gull","10.6",
"burgl","10.6","chomp","39.2","masticat","39.2","teeth","39.2","lap","39.2","chitchat","37.6","gab","37.6","schmooz","37.6","yak","37.6","sort","29.10",
"reclassify","29.10","regroup","29.10","bracket","29.10","chunk","29.10","catalogu","29.10","sort_out","29.10","recod","29.10","swirl","9.6","twin","9.6","twirl","9.6",
"distemper","24","enamel","24","japan","24","lacquer","24","shellac","24","tint","24","bellyach","37.8","grouch","37.8","kvetch","37.8","accomplish","55.2",
"achiev","55.2","quit","55.2","misapprehend","87.2","curtain","16","quarantin","16","repress","16","screen","16","seclud","16","suppress","16","incarcerat","92",
"hospitali","92","detain","92","head_on","98","tackl","98","set_about","98","go_about","98","intercept","98","target","98","rival","98","counter","98",
"pre-empt","98","foreknow","29.5","prophesy","29.5","reali","29.5","reput","29.5","speculat","29.5","swear","29.5","vaticinat","29.5","attest","29.5","aver","29.5",
"avow","29.5","avouch","29.5","discriminat","71","lesgislat","71","protest","71","rebel","71","retaliat","71","sin","71","team_up","71","befriend","71",
"pass","66","spend","66","squander","66","wast","66","bestrid","47.8","circl","47.8","edg","47.8","envelop","47.8","neighbor","47.8","overcast","47.8",
"travers","47.8","meet","47.8","refer","13.2","resign","13.2","restor","13.2","hand_out","13.2","pass_out","13.2","shell_out","13.2","extend","13.2","come_around","26.6.2",
"get_around","26.6.2","get_down","26.6.2","go_back","26.6.2","resort","26.6.2","revert","26.6.2","settle_down","26.6.2","change_over","26.6.2","move_over","26.6.2","switch","26.6.2","switch_over","26.6.2",
"charbroil","45.3","charcoal-broil","45.3","crisp","45.3","french-fry","45.3","hardboil","45.3","microwav","45.3","oven-fry","45.3","oven-poach","45.3","overcook","45.3","overheat","45.3",
"pan-broil","45.3","pan-fry","45.3","parboil","45.3","percolat","45.3","perk","45.3","pot-roast","45.3","reheat","45.3","rissol","45.3","saut","45.3","scallop","45.3",
"sear","45.3","shirr","45.3","softboil","45.3","steam-bak","45.3","steep","45.3","stir-fry","45.3","toast","45.3","warm_up","45.3","participat","73","labour","73",
"work","73","get_by","83","succeed","83","forbear","83","flip-flop","86.1","co-occur","86.1","banter","36.1","coexist","36.1","confabulat","36.1","elop","36.1",
"hobnob","36.1","neck","36.1","parley","36.1","plot","36.1","rendezvous","36.1","reunit","36.1","spoon","36.1","net","54.2","drum","40.3.2","pucker","40.3.2",
"purs","40.3.2","contriv","26.4","rebuild","26.4","publish","26.4","rearrang","26.4","reconstitut","26.4","reorgani","26.4","schedul","26.4","stag","26.4","curtsey","40.3.3",
"genuflect","40.3.3","salaam","40.3.3","saw","21.1","snip","21.1","deaccent","10.8","debark","10.8","debon","10.8","debowel","10.8","debur","10.8","declaw","10.8",
"defang","10.8","defat","10.8","defeather","10.8","deflea","10.8","deflesh","10.8","deflower","10.8","defoam","10.8","defog","10.8","deforest","10.8","defuzz","10.8",
"degas","10.8","degerm","10.8","deglaz","10.8","degreas","10.8","degrit","10.8","degum","10.8","degut","10.8","dehair","10.8","dehead","10.8","dehorn","10.8",
"dehull","10.8","dehusk","10.8","deic","10.8","deink","10.8","delint","10.8","deluster","10.8","demast","10.8","derat","10.8","derib","10.8","derind","10.8",
"desalt","10.8","descal","10.8","desex","10.8","desprout","10.8","destarch","10.8","destress","10.8","detassel","10.8","detusk","10.8","devein","10.8","dewater","10.8",
"dewax","10.8","deworm","10.8","disembowel","10.8","reason","97.2","reason_out","97.2","disprov","97.2","rationali","97.2","protect","85","shad","85","insulat","85",
"effac","44","obliterat","44","ravag","44","raz","44","ruin","44","undo","44","unmak","44","wreck","44","down","39.4","hav","39.4",
"imbib","39.4","swill","39.4","banquet","39.5","brunch","39.5","lunch","39.5","luncheon","39.5","nosh","39.5","picnic","39.5","sup","39.5","par","23.3",
"unbolt","23.3","unbuckl","23.3","unbutton","23.3","unchain","23.3","unclamp","23.3","unclasp","23.3","unclip","23.3","unfasten","23.3","unfix","23.3","unglu","23.3",
"unhing","23.3","unhitch","23.3","unhook","23.3","unlac","23.3","unlatch","23.3","unlock","23.3","unleash","23.3","unpeg","23.3","unpin","23.3","unscrew","23.3",
"unstapl","23.3","unstitch","23.3","unzip","23.3","figure_out","84","solv","84","pick_up","84","find_out","84","preen","41.1.1","primp","41.1.1","sunbath","41.1.1",
"wash","41.1.1","doll","41.3.2","gussy","41.3.2","spruc","41.3.2","tog","41.3.2","trick","41.3.2","motor","11.5","paddl","11.5","bus","11.5","renam","29.3",
"term","29.3","vot","29.3","bapti","29.3","consecrat","29.3","sympathi","88.2","control","63","bring_about","27","instigat","27","set_off","27","sir","27",
"spark","27","blister","45.5","corrod","45.5","molder","45.5","molt","45.5","phosphoresc","45.5","putrefy","45.5","spoil","45.5","wan","45.5","effervesc","47.2",
"judder","47.2","moult","47.2","propagat","47.2","respir","47.2","rippl","47.2","smolder","47.2","suppurat","47.2","rearm","13.4.2","redress","13.4.2","encumber","13.4.2",
"weight","13.4.2","vacat","51.1","set_out","51.1","emigrat","51.1","make_it","51.1","retreat","51.1","skedaddl","51.1","vamoos","51.1","machinat","55.5","fak","55.5",
"implement","55.5","originat","55.5","simulat","55.5","strike_up","55.5","synthesis","55.5","open_up","55.5","quanti","34.2","guesstimat","34.2","outmatch","90","outrac","90",
"outsmart","90","outstrip","90","outweigh","90","outwit","90","overcom","90","overreach","90","overrun","90","surpass","90","outdo","90","outshin","90",
"better","90","replac","13.6","hemorrhag","40.1.3","regurgitat","40.1.3","overspread","47.1","?correspond","47.1","?depend","47.1","predominat","47.1","prevail","47.1","prosper","47.1",
"persever","47.1","surviv","47.1","bottlefeed","39.7","breastfeed","39.7","force-feed","39.7","handfeed","39.7","spoonfeed","39.7","suckl","39.7","nos","35.6","bestrew","9.8",
"dam","9.8","dappl","9.8","delug","9.8","dirty","9.8","garnish","9.8","gild","9.8","interlard","9.8","interleav","9.8","lard","9.8","mottl","9.8",
"plug","9.8","repopulat","9.8","soak","9.8","speckl","9.8","splotch","9.8","stippl","9.8","stop_up","9.8","suffus","9.8","swaddl","9.8","vein","9.8",
"sack","10.10","send_away","10.10","force_out","10.10","hibernat","54.3","quail","40.5","winc","40.5","floss","41.2.1","brood","87.1","hamper","67","inhibit","67",
"obstruct","67","preclud","67","prevent","67","prohibit","67","restrain","67","bullock","59","oblig","59","pressur","59","prod","59","prompt","59",
"rous","59","spur","59","sweet-talk","59","arm-twist","59","hustl","59","influenc","59","seduc","59","panic","59","talk","59","wheedl","59",
"alleviat","80","commut","80","releas","80","ladl","9.3","scoop","9.3","sop","9.3","portion","13.3","ration","13.3","scor","13.5.1","pawn","13.1",
"quaff","39.3","swallow","39.3","swig","39.3","gorg","39.6","subsist","39.6","succor","72","huddl","47.5.2","burp","40.1.1","eruct","40.1.1","hiccup","40.1.1",
"sneez","40.1.1","sniffl","40.1.1","snuff","40.1.1","snuffl","40.1.1","yawn","40.1.1","recruit","13.5.3","sign_on","13.5.3","sign_up","13.5.3","subcontract","13.5.3","train","13.5.3",
"tamp","18.1","thwack","18.1","pan","35.1","scroung","35.1","contus","40.8.3","sunburn","40.8.3","endors","25.3","letter","25.3","monogram","25.3","predict","78",
"denot","78","corroborat","78","verify","78","enquir","37.1.2","pry","37.1.2","cabl","37.4","e-mail","37.4","modem","37.4","netmail","37.4","relay","37.4",
"satellit","37.4","semaphor","37.4","telecast","37.4","telegraph","37.4","telex","37.4","wireless","37.4","question","37.1.3","frisk","35.4","riffl","35.4","test","35.4",
"relat","107","remunerat","33","prosecut","33","punish","33","reprov","33","scold","33","repudiat","33","contort","26.5","whip","26.5","morali","37.11",
"rant","37.11","testify","37.11","theori","37.11","fluoresc","43.1","glimmer","43.1","incandesc","43.1","scintillat","43.1","twinkl","43.1","reduc","76","restrict","76",
"paus","53.1","stall","53.1","bivouac","46","guest","46","overnight","46","quarter","46","room","46","settl","46","shack_up","46","sleep_over","46",
"stay_over","46","hanker","32.2","hunger","32.2","lust","32.2","thirst","32.2","yearn","32.2","babbl","37.3","gabbl","37.3","gibber","37.3","jabber","37.3",
"lisp","37.3","natter","37.3","prattl","37.3","quaver","37.3","rasp","37.3","shriek","37.3","simper","37.3","splutter","37.3","squall","37.3","stammer","37.3",
"stemmer","37.3","stutter","37.3","tisk","37.3","vociferat","37.3","yammer","37.3","gurgl","37.3","cuddl","36.2","embrac","36.2","pet","36.2","exult","31.3",
"moon","31.3","rhapsodi","31.3","glory","31.3","luxuriat","31.3","mop","31.3","reflect","31.3","ruminat","31.3","triumph","31.3","sorrow","31.3","crest","47.7",
"snak","47.7","straggl","47.7","rang","47.7","swerv","47.7","veer","47.7","zigzag","47.7","remonstrat","36.3","quarry","10.9","scrambl","22.1","eddy","47.3",
"joggl","47.3","oscillat","47.3","throb","47.3","vacillat","47.3","sum","108","tally","108","factor_out","108","inerpolat","108","euthani","42.1","immolat","42.1",
"lynch","42.1","off","42.1","forego","75","flub","75","oar","51.4.2","rally","51.4.2","voyag","51.4.2","gawk","40.2","goggl","40.2","guffaw","40.2",
"jeer","40.2","pout","40.2","scoff","40.2","sigh","40.2","smil","40.2","sneer","40.2","snivel","40.2","recover","13.5.2","eventuat","48.3","take plac","48.3",
"okay","60","summon","60","command","60","requir","60","beggar","29.7","pauper","29.7","abrad","45.4","africani","45.4","agglomerat","45.4","alkalify","45.4",
"apostati","45.4","atomi","45.4","attenuat","45.4","bisect","45.4","bolshevi","45.4","calcify","45.4","capacitat","45.4","castrat","45.4","catholici","45.4","cauteri","45.4",
"chlorinat","45.4","christiani","45.4","circumcis","45.4","civili","45.4","clouded","45.4","de-escalat","45.4","dehumidify","45.4","dehydrat","45.4","diffract","45.4","dilat","45.4",
"dislocat","45.4","effeminat","45.4","emulsify","45.4","equali","45.4","equilibrat","45.4","eternali","45.4","europeani","45.4","femini","45.4","fertili","45.4","fructify","45.4",
"gluteni","45.4","granulat","45.4","helleni","45.4","humidify","45.4","iodi","45.4","loos","45.4","objectify","45.4","paralyz","45.4","pasteuri","45.4","perfect","45.4",
"petrify","45.4","populari","45.4","publici","45.4","purpl","45.4","rarefy","45.4","reanimat","45.4","resuscitat","45.4","revers","45.4","reviv","45.4","roughen","45.4",
"short","45.4","shush","45.4","silenc","45.4","sing","45.4","slack","45.4","stabili","45.4","standardi","45.4","steepen","45.4","sterili","45.4","stiffen","45.4",
"submerg","45.4","subsid","45.4","taper","45.4","thaw","45.4","thicken","45.4","toppl","45.4","tousl","45.4","trebl","45.4","ulcerat","45.4","ventilat","45.4",
"vitrify","45.4","vulcani","45.4","wak","45.4","wet","45.4","overstat","37.12","overdraw","37.12","hyperboli","37.12","overemphasi","37.12","overstress","37.12","tout","37.12",
"own","100","possess","100","ail","40.8.1","patent","101","trademark","101","credential","101","evidenc","101","attend","30.3","eavesdrop","30.3","squint","30.3",
"pelt","17.2","enact","26.7","silkscreen","26.7","beard","10.7","burl","10.7","cor","10.7","gill","10.7","hull","10.7","husk","10.7","lint","10.7",
"lous","10.7","pinion","10.7","pip","10.7","pith","10.7","pod","10.7","poll","10.7","pulp","10.7","rind","10.7","scal","10.7","scalp","10.7",
"shuck","10.7","stalk","10.7","weed","10.7","zest","10.7","pen","9.10","drown","42.2","evicerat","42.2","garrott","42.2","impal","42.2","suffocat","42.2",
"throttl","42.2","pierc","19","slop","9.5","spew","9.5","cook_up","26.3","ready","26.3","barbequ","26.3","overbak","26.3","weld","26.3","auction","54.4",
"reapprais","54.4","promot","102","emphasi","102","station","9.1","superimpos","9.1","park","9.1","prop","9.2","suspend","9.2","refrain","69","tim","54.1",
"mistim","54.1","rehears","26.8","practic","26.8","walk_through","26.8","bear_on","86.2","touch_on","86.2","pertain","86.2","rely","70","take_a_chanc","70","retract","10.1",
"wrench","10.1","renounc","10.11","retir","10.11","ventur","94","risk","94","hazard","94","slid","51.3.1","spiral","51.3.1","burrow","35.5","forag","35.5",
"leaf","35.5","pag","35.5","root","35.5","backpack","51.3.2","canter","51.3.2","carom","51.3.2","cavort","51.3.2","clump","51.3.2","dodder","51.3.2","flit","51.3.2",
"gambol","51.3.2","goose_step","51.3.2","lollop","51.3.2","lop","51.3.2","mosey","51.3.2","nip","51.3.2","parad","51.3.2","perambulat","51.3.2","plod","51.3.2","promenad","51.3.2",
"sashay","51.3.2","saunter","51.3.2","scram","51.3.2","scud","51.3.2","scutter","51.3.2","scuttl","51.3.2","skip","51.3.2","skitter","51.3.2","sleepwalk","51.3.2","slink","51.3.2",
"somersault","51.3.2","step","51.3.2","stomp","51.3.2","swagger","51.3.2","toddl","51.3.2","toil","51.3.2","tour","51.3.2","troop","51.3.2","whiz","51.3.2","alleg","37.7",
"reply","37.7","respond","37.7","retort","37.7","promulgat","37.7","utter","37.7","voic","37.7","purpos","37.7","chalk","25.2","charcoal","25.2","crayon","25.2",
"doodl","25.2","misspell","25.2","pencil","25.2","scrawl","25.2","spell","25.2","stencil","25.2","plumb","35.2","scour","35.2","trawl","35.2","troll","35.2",
"seem","109","pass_on","11.1","port","11.1","slip","11.1","uncoil","23.1","decoupl","23.1","disentangl","23.1","squar","89","resolv","89","covenant","89",
"bast","22.3","concatenat","22.3","glom","22.3","splic","22.3","descry","30.2","espy","30.2","make_out","30.2","overhear","30.2","perus","30.2","scent","30.2",
"catnap","40.4","drows","40.4","nap","40.4","snooz","40.4","burr","43.2","chink","43.2","chir","43.2","clack","43.2","clunk","43.2","crepitat","43.2",
"ding","43.2","dong","43.2","fizzl","43.2","knell","43.2","patter","43.2","pink","43.2","plink","43.2","plonk","43.2","plunk","43.2","rustl","43.2",
"shrill","43.2","sough","43.2","sploosh","43.2","squelch","43.2","swish","43.2","swoosh","43.2","thrum","43.2","thunk","43.2","ting","43.2","tinkl","43.2",
"tootl","43.2","twang","43.2","vroom","43.2","whir","43.2","whish","43.2","whump","43.2","zing","43.2","bayonet","18.3","belt","18.3","biff","18.3",
"birch","18.3","bonk","18.3","brain","18.3","can","18.3","clout","18.3","conk","18.3","cosh","18.3","cudgel","18.3","flagellat","18.3","kne","18.3",
"k.o.","18.3","paddywhack","18.3","spank","18.3","stabb","18.3","truncheon","18.3","wallop","18.3","loll","47.6","protrud","47.6","reclin","47.6","repos","47.6",
"slop","47.6","drizzl","9.7","smudg","9.7","spritz","9.7","?wash","9.7","mound","9.7","knock_off","10.5","reclaim","10.5","salvag","10.5","snatch_away","10.5",
"swip","10.5","thiev","10.5","weasel_out","10.5","stamp_down","42.3","subdu","42.3","stifl","42.3","still","42.3","shut_up","42.3","hush_up","42.3","calm_down","42.3",
"quell","42.3","quash","42.3","keep_down","42.3","puff","43.4","starv","40.7","sustain","55.6","prolong","55.6","protract","55.6","keep_up","55.6","carry_on","55.6",
"swarm","47.5.1","throng","47.5.1","slug","18.2","swat","18.2","epoxy","22.4","fetter","22.4","lasso","22.4","manacl","22.4","padlock","22.4","rivet","22.4",
"rop","22.4","screw","22.4","solder","22.4","thread","22.4","thumbtack","22.4","trammel","22.4","truss","22.4","zip","22.4","gum","22.4","appris","37.2",
"updat","37.2","remind","37.2","bunt","17.1","lob","17.1","loft","17.1","punt","17.1","prickl","40.8.2","tingl","40.8.2","pat","20","tweak","20",
"document","25.4","microfilm","25.4","photocopy","25.4","tally_up","25.4","transcrib","25.4","explicat","37.1.1","translat","26.6.1","transmut","26.6.1","exhort","58.1","urg","58.1",
"persuad","58.1","utili","105","bobsled","51.4.1","cab","51.4.1","caravan","51.4.1","chariot","51.4.1","dogsled","51.4.1","gondola","51.4.1","jeep","51.4.1","moped","51.4.1",
"parachut","51.4.1","rickshaw","51.4.1","sledg","51.4.1","sleigh","51.4.1","tram","51.4.1","trolley","51.4.1","yacht","51.4.1","cano","51.4.1","kayak","51.4.1","motorbik","51.4.1",
"motorcycl","51.4.1","raft","51.4.1","skateboard","51.4.1","sled","51.4.1","toboggan","51.4.1","boogi","51.5","bop","51.5","cancan","51.5","conga","51.5","foxtrot","51.5",
"jig","51.5","jitterbug","51.5","pirouett","51.5","polka","51.5","quickstep","51.5","rumba","51.5","samba","51.5","squaredanc","51.5","tapdanc","51.5","waltz","51.5",
"gust","57","lightning","57","mist","57","mizzl","57","precipitat","57","sleet","57","snow","57","swelter","57","December","56","holiday","56",
"honeymoon","56","sojourn","56","summer","56","weekend","56","winter","56","hoover","10.4.2","plough","10.4.2","sandpaper","10.4.2","hos","10.4.2","expurgat","10.4.1",
"skim","10.4.1","unload","10.4.1","winnow","10.4.1","scrub","10.4.1","suction","10.4.1","plan","62","yen","62","back_out","82","chicken_out","82","get_out","82",
"wriggle_out","82","bow_out","82","pull_out","82","get_away","82","back_off","82","pull_away","82",
};
		
		List<string> verb;
		List<string> verb_class;
		
		public verbs()
		{
			verb = new List<string>();
			verb_class = new List<string>();
		}

		private static string TextOnly(
		    string text)
		{
			string result = "";
			char[] ch = text.ToCharArray();
			for (int i = 0; i < text.Length; i++)
			{
				if (((ch[i] >= 'a') &&
					(ch[i] <= 'z')) ||
					((ch[i] >= 'A') &&
					(ch[i] <= 'Z')) ||
	                ((ch[i] >= '0') &&
					(ch[i] <= '9')) ||
	                (ch[i] == ' '))
				result += ch[i];
			}
			return(result);
		}

		/// <summary>
		/// returns a sorted list of verb classes for the given proposition
		/// </summary>
		/// <param name="proposition"></param>
		/// <returns></returns>
		public static string GetVerbClasses(
		    string proposition)
		{
			List<string> result = new List<string>();
			string[] str = TextOnly(proposition.ToLower()).Split(' ');
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i].Length > 1)
				{
					for (int j = 0; j < verbclass.Length; j += 2)
					{
						if (str[i].StartsWith(verbclass[j]))
						{
							result.Add(verbclass[j+1]);
							break;
						}
					}
				}
			}
			result.Sort();
			string result_str = "";
			for (int i = 0; i < result.Count; i++)
			{
				result_str += result[i] + " ";
			}
			return(result_str.Trim());
		}
		
		public void SaveArray(
		    string filename)
		{
			StreamWriter oWrite = null;
			bool ok = false;
				
			try
			{
			    oWrite = File.CreateText(filename);
				ok = true;
			}
			catch
			{
			}
			
			if (ok)
			{
				string quote = "";
				quote += '"';
				int ctr = 0;
				oWrite.WriteLine("static string[] verbclass = {");
				for (int i = 0; i < verb.Count; i++)
				{
					if (verb[i].EndsWith("ize"))
					{
						verb[i] = verb[i].Substring(0,verb[i].Length-2);
					}
					else
					{
						if (verb[i].EndsWith("e"))
						{
							verb[i] = verb[i].Substring(0,verb[i].Length-1);
						}
					}
					
					oWrite.Write(quote + verb[i] + quote + "," + quote + verb_class[i] + quote + ",");
					ctr++;
					if (ctr >= 10)
					{
						ctr = 0;
						oWrite.WriteLine("");
					}
				}
				oWrite.WriteLine("");
				oWrite.WriteLine("};");
			}
			
			oWrite.Close();
		}
		
		public void LoadFromPropBank(
		    string directory)
		{
            if (Directory.Exists(directory))
            {
                string[] filename = Directory.GetFiles(directory, "*.xml");
                if (filename != null)
                {
					for (int i = 0; i < filename.Length; i++)
					{
						string verb_name = filename[i].Substring(0,filename[i].Length-4);
						if (verb_name.Contains("/"))
						{
							string[] str2 = verb_name.Split('/');
							verb_name = str2[str2.Length-1];
						}						
						
						bool class_found = false;
						StreamReader oRead = File.OpenText(filename[i]);
						while ((!class_found) &&
						       (!oRead.EndOfStream))
						{
							string str = oRead.ReadLine();
							int idx = str.IndexOf("vncls=");
							if (idx > -1)
							{
								idx += 7;
								int j = idx;
								string end_str = "";
								end_str += '"';
								string class_str = "";
								while ((j < str.Length) &&
								       (str.Substring(j,1) != end_str))
								{
									class_str += str.Substring(j,1);
									j++;
								}
								
								if (class_str != "")
								{
									class_found = true;
									if (class_str != "-")
									{										
										if (!verb.Contains(verb_name))
										{
											Console.WriteLine(verb_name + " " + class_str);
									        verb.Add(verb_name);
										    verb_class.Add(class_str);
										}
									}
								}
							}
						}
						oRead.Close();
					}
				}
			}
		}
		
		public void LoadFromVerbNet(
		    string directory)
		{
            if (Directory.Exists(directory))
            {
                string[] filename = Directory.GetFiles(directory, "*.xml");
                if (filename != null)
                {
					for (int i = 0; i < filename.Length; i++)
					{
						string class_str = "";
						StreamReader oRead = File.OpenText(filename[i]);
						while (!oRead.EndOfStream)
						{
							string str = oRead.ReadLine();
							if (class_str == "")
							{
								int idx = str.IndexOf("VNCLASS ID=");
								if (idx > -1)
								{
									idx += 12;
									int j = idx;
									string end_str = "";
									end_str += '"';
									while ((j < str.Length) &&
									       (str.Substring(j,1) != end_str))
									{
										class_str += str.Substring(j,1);
										j++;
									}									
								}
								if (class_str.Contains("-"))
								{
									int idx3 = class_str.IndexOf("-");
									class_str = class_str.Substring(idx3+1);
								}
							}
							
							int idx2 = str.IndexOf("MEMBER name=");
							if (idx2 > -1)
							{
								idx2 += 13;
								int j = idx2;
								string end_str = "";
								end_str += '"';
								string verb_name = "";
								while ((j < str.Length) &&
								       (str.Substring(j,1) != end_str))
								{
									verb_name += str.Substring(j,1);
									j++;
								}
								
								if (verb_name != "")
								{
									if (class_str != "-")
									{										
										if (!verb.Contains(verb_name))
										{
											Console.WriteLine(verb_name + " " + class_str);
									        verb.Add(verb_name);
										    verb_class.Add(class_str);
										}
									}
								}
							}
						}
						oRead.Close();
					}
				}
			}
		}
		
	}
}

