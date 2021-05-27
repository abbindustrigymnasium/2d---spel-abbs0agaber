# TopDownAtempt

## Introduktion

Detta är mitt första spel som heter TopDownAtempt. Ett extremt kreativt namn eftersom det är ett försök på ett TopDown spel.
Spelet går ut på att man ska döda alla terorister på kartan, men man kan endast se dem om de är inom ens Field of View.  (Ett visst område där man kan se alla typer objekt, utanför kan man inte se alla typer av objekt). Det gör att man måste kolla på där du tror att fienden kommer vara för att se dem. Man kan dock se deras mynningseld vilket är ett sybstitut eftersom man inte kan höra från vilket håll de skjuter.


## Projektbeskriving, tekniker och planeringen(Planner)

Spelet har utckelats i unity men eftersom det är första gången jag anväder unity har jag stött på fler problem.
Jag har använt raycasts för att sjuta. Till fiendens jakt läge har jag använt A star pathfinding som är lätt att implementera. För att få field of view effekten har jag skapat en mask vars form jag bestämmer igenom att skicka ut raycasts för att bstämma var kanterna av masken ska ligga. Sedan använde jag unitys Universal Pipeline Renderer för att projecera det en sekundär kamera såg till masken som spelaren ser. 
Jag använde mig av planner för att strukturera mitt arbete och dela upp det i mindre hanterbara delar. 


## Resultat och problem som man stötte på och hur man löst dem

De första stegen var enkla att gå igenom, skapa en spelare, playercontroller, karta, m.m. När det komm till att skjuta prövade jag två metoder.
Först försökte jag använda raycasts, men jag hade ingen visualisering på var de träffade eller vart de åkte. Så pga av in okunskap bytte jag till att instansiatea en prefab istellet. Då fick jag problem med colition vid höga hastigheter och skotten gick genom väggar så jag bytte tillbaka till raycasts. Efter att undersöka mer om hur de fungerade, fick jag de att fungera. Jag lade även till bullet trails med en line renderer, mellan pipan och träffpunkten. 
Det andra stora problemet jag hade var hur jag skulle få till Field of View effecten. Jag följde en gammal guide för hur man skulle kunna göra en sådan effekt, men den använde en gammal renderer. Jag löste det igenom att använda den masken från den gamla guiden och använde unitys nyare Universal pipline renderer för att endast visa fienderna där masken var. Det kunde göras igenom att använda en annan kamre och visa vad den såg endast där masken var.


## Avslutning

I helhet är jag relativt nöjd med projektet även om jag tycket att jag har haft mer problem än jag borde och att det har tagit längre tid än det borde. 
Som förbättring skulle flera nivåer kunna läggas till. En funktion för att ladda om och inte ha ett oändligt magasin skulle även vara en bra förbättring. 
De två största problemen med spelet nu är att fienden skjuter i början och att det inte finns något sätt att vinna.
