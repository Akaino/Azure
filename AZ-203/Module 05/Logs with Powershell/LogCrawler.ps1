$rg = Get-AzResourceGroup -name "AZ203" -Location "northeurope"

Get-AzLog -ResourceGroupName $rg.ResourceGroupName `
    -StartTime (Get-Date).AddDays(-1) `
    -EndTime (Get-Date) `
    -Status Succeeded