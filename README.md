# Steps for the POC

## AKS Cluster

You need to have installed an RBAC Enabled AKS cluster with linux node pool.

### Get Credentials

````bash
az aks list -o table

az aks get-credentials -n <clustername> -g <resourcegroup>
````

## Tools required

- Chocolatey - https://chocolatey.org/install
- helm
  ````
  choco install kubernetes-helm
  ````
- Dapr CLI - https://docs.dapr.io/getting-started/install-dapr-cli/
  ````
  powershell -Command "iwr -useb https://raw.githubusercontent.com/dapr/cli/master/install/install.ps1 | iex"
  ````

## Init Dapr - Locally

- Install Docker for Windows (Linux containers)
- To install dapr locally, following instructions  [here](https://docs.dapr.io/getting-started/install-dapr-selfhost/):

````
dapr init

dapr --version

docker ps
````

## Init Dapr - Kubernetes

````
> dapr init --kubernetes

Making the jump to hyperspace...
Note: To install Dapr using Helm, see here: https://docs.dapr.io/getting-started/install-dapr-kubernetes/#install-with-helm-advanced

Deploying the Dapr control plane to your cluster...
Success! Dapr has been installed to namespace dapr-system. To verify, run `dapr status -k' in your terminal. To get started, go here: https://aka.ms/dapr-getting-started

````
````
> dapr status -k
  NAME                   NAMESPACE    HEALTHY  STATUS   REPLICAS  VERSION  AGE  CREATED
  dapr-sentry            dapr-system  True     Running  1         1.0.1    46s  2021-03-30 11:48.09
  dapr-sidecar-injector  dapr-system  True     Running  1         1.0.1    46s  2021-03-30 11:48.09
  dapr-operator          dapr-system  True     Running  1         1.0.1    46s  2021-03-30 11:48.09
  dapr-dashboard         dapr-system  True     Running  1         0.6.0    46s  2021-03-30 11:48.09
  dapr-placement-server  dapr-system  True     Running  1         1.0.1    46s  2021-03-30 11:48.09
````

### Install Redis

````
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update
helm install redis bitnami/redis
````
You should wait until redis pods are running:
````
> k get pod
NAME             READY   STATUS    RESTARTS   AGE
redis-master-0   0/1     Running   0          21s
redis-slave-0    0/1     Running   0          21s
````

## Create State Store and PubSub (Dapr Components)
````
> kubectl apply -f deploy/_base-redis.yaml

component.dapr.io/statestore created
component.dapr.io/pubsub created
````

## Tracing
### Jaeger

Follow instructions [here](https://docs.dapr.io/operations/monitoring/tracing/supported-tracing-backends/jaeger/)

````
> kubectl apply -f deploy/tracing.yaml

> # Install Jaeger
> helm repo add jaegertracing https://jaegertracing.github.io/helm-charts
> helm install jaeger-operator jaegertracing/jaeger-operator
> kubectl apply -f jaeger-operator.yaml

> # Wait for Jaeger to be up and running
> kubectl wait deploy --selector app.kubernetes.io/name=jaeger --for=condition=available

````
To see traces:
````
kubectl port-forward svc/jaeger-query 16686
````
> Access to http://localhost:16686

### Prometheus - Grafana (optional)

Follow instructions [here](https://docs.dapr.io/operations/monitoring/metrics/prometheus/#setup-prometheus-on-kubernetes) and [here](https://docs.dapr.io/operations/monitoring/metrics/grafana/)

To get password:
````
# This will be encoded in base 64 (needs to be decoded to be used)

kubectl get secret --namespace dapr-monitoring grafana -o jsonpath="{.data.admin-password}"

# Decoded

[System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($(kubectl get secret --namespace dapr-monitoring grafana -o jsonpath="{.data.admin-password}")))
````

To access to grafana:
````
kubectl port-forward svc/grafana 8080:80 -n dapr-monitoring
````

- Grafana User: admin
- Grafana Password: The one you got from previous command

> To access http://localhost:8080

### Elastic Search and Kibana (optional)

Follow instructions [here](https://docs.dapr.io/operations/monitoring/logging/fluentd/#install-elastic-search-and-kibana)


To access to Kibana:
````
kubectl port-forward svc/kibana-kibana 5601 -n dapr-monitoring
````
> Then http://localhost:5601


## Deploy Sample App
````
> kubectl apply -f deploy/02-blazor-frontend.yaml
> kubectl apply -f deploy/03-webapi.yaml
````
Get Frontend IP:
````
> kubectl get svc -l app=blazor
NAME        TYPE           CLUSTER-IP    EXTERNAL-IP     PORT(S)        AGE
blazorapp   LoadBalancer   10.0.37.253   104.45.189.76   80:32292/TCP   20m

````

> Now you can access to your application using the EXTERNAL-IP address in a browser.

## References

- Dapr for .NET developers - https://docs.microsoft.com/en-us/dotnet/architecture/dapr-for-net-developers/publish-subscribe
- 6 Month Plan - https://github.com/dapr/dapr/issues/2882
- State Management Notes - https://docs.dapr.io/developing-applications/building-blocks/state-management/howto-share-state/
- SDK - https://docs.microsoft.com/en-us/dotnet/architecture/dapr-for-net-developers/getting-started

