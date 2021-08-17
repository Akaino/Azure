$processes = Get-Process
$p1 = $processes[15] 
$p2 = $processes[16] 

$properties = ($p1 | Get-Member -MemberType Property | Select-Object -ExpandProperty Name)

foreach ($property in $properties) 
{
  Compare-Object $p1 $p2 -Property "$property" | Format-Table -AutoSize
}