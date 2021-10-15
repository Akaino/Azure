#Objekte zum Vergleichen holen
$processes = Get-Process
$p1 = $processes[15] 
$p2 = $processes[16] 

#Liste der zu vergleichenden Eigenschaften erstellen
$properties = ($p1 | Get-Member -MemberType Property | Select-Object -ExpandProperty Name)

#Per Foreach-Schleife die beiden Objekte mit den jeweiligen Eigenschaften einzeln vergleichen
foreach ($property in $properties) 
{
  Compare-Object $p1 $p2 -Property "$property" | Format-Table -AutoSize
}

Get-Help Compare-Object -Online