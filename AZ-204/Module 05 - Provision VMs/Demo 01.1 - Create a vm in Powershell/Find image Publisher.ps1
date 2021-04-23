# Get a list of all publishers available in the East US region
Get-AzVMImagePublisher -Location eastus

# Get a list of all offers for the Canonical publisher
Get-AzVMImageOffer -Location eastus -PublisherName Canonical

# Get a list of SKUs for the UbuntuServer offer
Get-AzVMImageSku -Location eastus -PublisherName Canonical -Offer UbuntuServer

# Get a list of all images available for the 19.10-DAILY SKU
Get-AzVMImage -Location eastus -PublisherName Canonical -Offer UbuntuServer -Sku 19.10-DAILY

# Get the 19.10.201906230 version of the VM image
Get-AzVMImage -Location eastus -PublisherName Canonical -Offer UbuntuServer -Sku 19.10-DAILY -Version 19.10.201906230
