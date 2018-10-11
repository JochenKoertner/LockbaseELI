# Git-Flow Benutzung

Wir haben derzeit zwei Repositories auf Git-Hub am Start:

* `lockbase/LockbaseELI`
    Hier befindet sich der aktuelle Auslieferungsstand. Hier gibt es derzeit auch nur einen einzigen Branch mit dem Namen `master`. Die Weiterentwicklung findet in sogenannten Forks statt.
* `CapitanFuture/LockbaseELI`
    Ein Fork von `Lockbase/LockbaseELI` besteht derzeit aus zwei Remote Branches `master` & `develop`. Hier findet die aktuelle Entwicklung statt und in regelmäßigen Intervallen wird der der `master` per Pull-Request in den Upstream Branch `master` gesendet.

Die Entwicklung findet wird von gitflow prozeßmässig unterstützt. Das Vorgehen soll hier grob erläutert werden. 

Hierfür sind folgende Branches mit einer bestimmten Bedeutung hinterlegt.

* `master`  
    Hier befindet sich immer der ausgelieferte Code. Auf diesem Branch sollte immer mit Bedacht eingecheckt werden. In der Regel wenn eine Auslieferung gemacht wird oder ein Hotfix bereitgestellt wird.
* `develop`  
    In diesem Branch wird überwiegend gearbeitet. Das ist der Stand den sich ein Kunde jederzeit anschauen kann, um zu beurteilen ob dieses als neues Release dienen kann. Entwickelt sich `master` während der Entwicklung weiter, sollte dieser frühzeitig in `develop` gemergt werden. 
* `hotfix`  
    Muss ein BugFix ausgeliefert werden, so wird dieser Branch kurzfristig aufgemacht und sobald korrekt in den `master` gemergt und dann wieder gelöscht.
* `release`  
    Im Vorwege eines Releases wird unter Umständen ein temporärer Branch mit dem Namen `release/xyz` angelegt. Handelt es sich um größere Versionssprunge sollte dieses gemacht werden.
* `feature`  
    Dedizierte Branches um ein Feature zu entwickeln. Wird im Anschluß in `develop` zurückgeführt. Macht eigentlich nur Sinn wenn mit vielen Entwicklern auf einem Repository gearbeitet wird.