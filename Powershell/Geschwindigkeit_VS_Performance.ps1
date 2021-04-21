cls 
set-location C:\Windows\System32\drivers

#Welche Zeile ist schneller? Welche Performanter?
foreach($file in Get-ChildItem -Recurse) { $file.Fullname }

Get-ChildItem -Recurse | ForEach-Object { $_.FullName }


<#
	Hier wird Get-ChildItem ausgeführt und alle Objekte werden im Arbeitsspeicher abgelegt, bevor auch nur das Erste abgearbeitet wird.
	Dieses Vorgehen verbraucht mehr Arbeitsspeicher ist insgesamt aber für den Prozessor weniger Performancelastig
	Viele große Objekte könnten allerdings den Speicher überlasten
#>
$for = Measure-Command -Expression {
    foreach($file in Get-ChildItem -Recurse) { $file.Fullname }
}

<#
	Hier wird jedes Objekt das durch Get-ChildItem erzeugt wurde sofort auf die Pipe geschickt und dann einzeln abgearbeitet.
	Im Arbeitsspeicher existiert jeweils nur ein Objekt solange es verarbeitet wird. 
	Dieser Prozess ist zwar weniger RAM intensiv aber belastet den Prozessor insgesamt mehr.
#>
$pipe = Measure-Command -Expression {
    Get-ChildItem -Recurse | ForEach-Object { $_.FullName }
}

Write-Host "Ausgelesene Dateien: $((Get-ChildItem -Recurse).Length)" -ForegroundColor Green
Write-Host "Dauer (ms) durch ForEach-Schleife: $($for.TotalMilliseconds)" -ForegroundColor Red
Write-Host "Dauer (ms) durch Pipe-Anweisung: $($pipe.TotalMilliseconds)" -ForegroundColor Red