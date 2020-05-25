# Create Event Grid
az.cmd eventgrid topic create --name az204topic -l northeurope -g az204
# Query Topic Endpoint
endpoint=$(az.cmd eventgrid topic show --name az204topic -g az204 --query "endpoint" --output tsv)
echo $endpoint
# Query Topic SAS Key
key=$(az eventgrid topic key list --name az204topic -g az204 --query "key1" --output tsv)
echo $key
# create custom Event Object
event='[ {"id": "'"$RANDOM"'", "eventType": "recordInserted", "subject": "myapp/vehicles/motorcycles", "eventTime": "'`date +%Y-%m-%dT%H:%M:%S%z`'", "data":{ "make": "Ducati", "model": "Monster"},"dataVersion": "1.0"} ]'
# send with curl
curl -X POST -H "aeg-sas-key: $key" -d "$event" $endpoint

# Remove ressources
# az group delete --name az204