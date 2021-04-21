$toDelete = Get-ChildItem -Directory -Recurse -Depth 3 -Include "node_modules", "obj", "bin", "dist", "lib", "temp"  

$toDelete | remove-item -Confirm:$false -Recurse -WhatIf

#$toDelete | select -property * |  Out-GridView