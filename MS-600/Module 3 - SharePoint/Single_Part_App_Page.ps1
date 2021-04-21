$cred = Get-Credential -UserName "admin@devtobi.onmicrosoft.com" -Message "Enter Password"
Connect-PnPOnline -Url https://devtobi.sharepoint.com/sites/devtobi -Credentials $cred

$item2 = Get-PnPListItem -List "Site Pages" -Query "<View><Query><Where><Eq><FieldRef Name='FileLeafRef'/> <Value Type='Text'>Home.aspx</Value></Eq></Where></Query></View>" 

$item2["PageLayoutType"] = "Home" #Home #SingleWebPartAppPage
#Home ist ein fester wert im SharePoint (PowerShell), die Startseite kann geändert werden, jedoch bewirkt "Home" immer das default Layout

$item2.Update(); Invoke-PnPQuery 
