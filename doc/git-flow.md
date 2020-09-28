# Git-Flow Benutzung

Wir haben derzeit zwei Repositories auf Git-Hub am Start:

* `lockbase/LockbaseELI`  
    Hier befindet sich der aktuelle Auslieferungsstand. Hier gibt es derzeit auch nur einen einzigen Branch mit dem Namen `master`. Die Weiterentwicklung findet in sogenannten Forks statt.

* `CapitanFuture/LockbaseELI`  
    Ein Fork von `Lockbase/LockbaseELI` besteht derzeit aus zwei Remote Branches `master` & `develop`. Hier findet die aktuelle Entwicklung statt und in regelmäßigen Intervallen wird der der `master` per Pull-Request in den Upstream Branch `master` gesendet.

Die Entwicklung wird von gitflow prozeßmässig unterstützt. Das Vorgehen soll hier grob erläutert werden.

Hierfür sind folgende Branches mit einer bestimmten Bedeutung hinterlegt.

* `master`  
    Hier befindet sich immer der ausgelieferte Code. Auf diesem Branch sollte immer mit Bedacht eingecheckt werden. In der Regel wenn eine Auslieferung gemacht wird oder ein Hotfix bereitgestellt wird.

* `develop`  
    In diesem Branch wird überwiegend gearbeitet. Das ist der Stand den sich ein Kunde jederzeit anschauen kann, um zu beurteilen ob, dieses als neues Release dienen kann. Entwickelt sich `master` während der Entwicklung weiter, sollte dieser frühzeitig in `develop` gemergt werden.
* `hotfix`  
    Muss ein BugFix ausgeliefert werden, so wird dieser Branch kurzfristig aufgemacht und sobald korrekt in den `master` gemergt und dann wieder gelöscht.
* `release`  
    Im Vorwege eines regulären Releases wird unter Umständen ein temporärer Branch mit dem Namen `release/xyz` angelegt. Bei einem größerem Versionssprung sollte dieses gemacht werden.
* `feature`  
    Dedizierte Branches um ein Feature zu entwickeln. Wird im Anschluß in `develop` zurückgeführt. Macht eigentlich nur Sinn wenn mit vielen Entwicklern auf oeinem Repository gearbeitet wird oder ein Versuch gestartet wird, bei dem im Vorwege noch nicht klar ist ob und wann dieses in dem `develop` Branch landen soll.

# Git Workflow's

## Start eines neuen Feature Branch

`master` und `develop` sollten clean sein. Am Anfang wird `gitflow start feature <name>` aufgerufen. 

## Start eines neuen Releases 

`master` und `develop` sollten clean sein. Am Anfang wird `gitflow start release <name>` aufgerufen. Am Ende wird dann 
`gitflow finish release <name>` das Release freigegeben und mit einem Tag versehen. Das Resultat ist das `master` und `develop` kurzfristig synchron sind.

Als Namensschema für die Releases neme ich folgende Notation `sprint-kw<ww>-<yyyy>`. Wobei `<ww>` die aktuelle Kalenderwoche und `<yyyy>` das aktuelle Jahr darstellen.

## Sync mit UpStream

Im regelmäßigen Abständen wird der Fork unter `CaptainFuture/LockbaseELI`mit dem Upstream syncronisiert. Diese Synchronisierung läuft immer im Branch `master`. Für diesen vorgang ist es eine Vorraussetzung das ein Remote Repository unter dem Namen `upstream` bekannt gegeben wird. 

Mit dem folgenden Kommando können die Remotes aufgelistet werden.

`git remote -v`

Ist noch kein Upstream vorhanden wird dieser mit dem folgenden Kommando bekanntgegeben. 

`git remote add upstream git@github.com:JochenKoertner/LockbaseELI.git`

Der eigentliche Vorgang ist dann der folgende Ablauf 

* `git fetch upstream`
    Das aktuelle Upstream repository wird einmal gefecht und somit auf Stand gebracht. 

* `git checkout master`
    Es wird auf dem `master` ausgecheckt. 

* `git merge upstream/master`
    Der eigentliche Sync zwischen upstream und master. Evtl. muß gemergt werden. 

* `git commit` & `git push`
    Der Fork wird auf Stand gebracht und es kann jetzt ein Pull-Request gestellt werden um den Upstream mit dem Downstreeam zu syncen. 

Im Anschluß kann dann der neue master wieder in einem Development Zweig gemergt werden und der Zyklus geht von vorne los. 

Merge Message mit vi 

press 'i'
press esc
write ':wq'
press enter  

