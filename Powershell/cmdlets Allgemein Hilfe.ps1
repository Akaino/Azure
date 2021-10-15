# Version der PowerShell
$PSVersionTable

#Hilfe updaten
Update-Help -UICulture de-de -ea 0 # -ea 0 steht für ErrorAction SilentlyContinue, also keine Fehlerausgabe auf der Konsole

# Allgemeine Hilfe zu cmdlet abrufen
Get-Help Get-Module 
Get-Help Get-Module -Detailed    # Detailierte Ausgabe
Get-Help Get-Module -Full        # Gesamte Hilfe abrufen zum cmdlet
Get-Help Get-Module -ShowWindow  # Hilfe als Grafische Oberfläche ausgeben
Get-Help Get-Module -Online      # Online Artikel abrufen (nicht bei allen cmdlets verfügbar)
Get-Help Get-Module -Examples    # Beispiele 
Get-Help Get-Module -Parameter * # Hilfe zu den Parametern

#Übersichtlichere Hilfe zu den about_*-Files
Get-Help about_* | Out-GridView -PassThru | Get-Help -ShowWindow 

Get-Command ForEach-Object #Informationen über das cmdlet und das entsprechende Modul

Get-Alias   # Zeige alle derzeitigen Aliasse
Get-Alias % # wofür steht ein Alias ?
Get-Alias -Definition ForEach-Object #Alle bekannten Aliasse für ein cmdlet

# Auflisten aller installierter Module auf der aktuellen Maschine
Get-Module -ListAvailable
#Finden von verfügbaren Modulen, beginnend mit Az
Find-Module Az*
#Module installieren
Install-Module Az
#Updaten der Module
Update-Module Az

Get-Process | Get-Member #Inforamtionen über das abgerufene Objekt, kurzschreibweise: gm

# | Format-List | Out-String -Stream | Sort-Object <- Um Properties alphabetisch zu sortieren
Get-NetAdapter -Physical | select -First 1 -Property * | Format-List | Out-String -Stream | Sort-Object

#Alle dezeitigen Policies auflisten
Get-ExecutionPolicy -List

#Policy auf Unrestricted setzen
Set-ExecutionPolicy -ExecutionPolicy Unrestricted
# Bsp. zum Zurücksetzen: Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Restricted -Force
