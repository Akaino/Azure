# Use kubectl to list nodes
kubectl get nodes

# Use kubectl to apply a yaml file
kubectl apply -f nginx.yaml

# Monitor deployment
kubectl get deployment nginx0-deployment --watch

# List pods from deployment
kubectl get pods -l app=nginx0

# List replicasets
kubectl get replicasets

# Expost deployment
kubectl expose deployment nginx0-deployment --type=LoadBalancer --name=my-service

# Show Service
kubectl get services my-service

# List pods again
kubectl get pods --output=wide

# Delete service and deployment
#kubectl delete services my-service
#kubectl delete deployment nginx0-deployment