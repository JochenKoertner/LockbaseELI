# LbwELI-Driver Project

Entwicklungs Notizen zu der Treiber Entwicklung. Vorerst nur in Deutsch.

## Vorbereitung

In dem Projekt wird aktiv das Konzept der git Submodules verwendet. Damit 
ist es möglich 3rd Party Projekte in sein Workspace einzubinden, die aber nach wie vor außerhalb 
des eigenen Repository gehalten werden.   

Ein neues Submodule wird hierbei mit `git submodule add` hinzugefügt.

```
git submodule init
git submodule add git@github.com:zserge/jsmn.git driver/jsmn
```
## Konfiguration



Wenn ein Repository zum ersten Mal abgerufen wird oder sich an den Submodules irgendwas geändert 
hat ist es ratsam die beiden folgenden Kommandos abzusetzen damit man auf dem letzten Stand ist.
Im eigenen Repo wird lediglich ein Commit als Referenz gespeichert.

```
git submodule init  
git submodule update
```

## Konfiguration 

Der Treiber kann über eine JSON Datei mit dem Namen `config.json` konfiguriert werden. 
Derzeit werden dort die Einstellungen hinterlegt, wo sich der mqtt-Broker befindet und auf welchem 
Port er lauscht. Wird der Treiber z.B. in einer Virtuellen Windows Maschine (VirtualBox) betrieben 
der Broker befindet sich jedoch auf dem Hostsystem müsste der `host` auf `"10.10.10.2"` eingestellt 
werden.

```
{
  "host"  : "localhost",
  "port"  : 1883
}
```

## Source

### `utils.c`

Diverse Hilfsroutinen. 

* `session_id_to_string`  
    Wandelt eine Integer SessionId in einen Hex-String
* `string_to_session_id`  
    Wandelt einen HexString zurück in einen Integer
* `formatUrl`  
    Formatiert eine gültige Url aus den Angaben Protocol, Host und Port 
* `parse_payload`  
    Parsing eines JSON Payloads und Auslesen der SessionId und des Text Feld

### `driver.c`

Hier wird global in der Variable `driverInfo` folgende Sachen transient gehalten:
* `callBack` - Funktion die bei `ELICreate` als Callback übergeben wurde
* `client` - Die Client-Verbindung zum mqtt Broker
* `sessions` - Liste mit den geöffneten Sessions
* `config` - kompletter Inhalt einer vorgefundenen `config.json` Datei
* `host` - Hostname auf dem der mqtt-Broker läuft
* `port` - Port auf dem der mqtt-Broker lauscht (i.d.R. 1883)

Für das Anlegen bzw. Freigeben der Datenstruktur gibt es zwei Funktionen:
 
* `new_driver` - Wird von `ELICreate` aufgerufen
* `free_driver` -  Wird von `ELIDestroy` aufgerufen

### `session_list.c`

Beinhaltet eine einfache verkettete Liste in der die Sessions verwaltet werden. Eine Session besitzt folgende Eigenschaften: 
* `Id` - Identität der Session
* `sUserList` - Benutzer Liste
* `sSystem` - Name des Systems
* `sExtData` - Zusatz Angaben

Für die Liste gibt es folgende Funktionen : 
* `new_session` - Legt eine neue Session an
* `find_session` - Findet eine Session anhand seiner Identität
* `remove_session` - Entfernt eine Session aus der Liste

### `library.c`
 
Hier ist der eigentliche Treiber Code implementiert. 
