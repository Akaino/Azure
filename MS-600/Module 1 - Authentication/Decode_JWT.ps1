Param(
    [string] $token = ""
)

if((Get-InstalledModule -Name JWTDetails -ea 0).Count -eq 0){
    Install-Module -Name JWTDetails -Confirm: $False -Force 
}

Get-JWTDetails $token

Uninstall-Module -Name JWTDetails -Confirm:$false -Force