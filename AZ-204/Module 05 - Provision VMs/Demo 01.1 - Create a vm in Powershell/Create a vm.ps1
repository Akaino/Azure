# Login if you haven't already
# Connect-AzAccount

# variables
$resourceGroup = ""
$vmName = ""
# To find location names
# Get-AzLocation | select location, displayname | Sort-Object location | ft
$location = "northeurope"

# 'code'
New-AzVM -ResourceGroupName $resourceGroup -Location $location -Name $vmName
