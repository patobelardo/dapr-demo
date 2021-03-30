docker build -t patobelardo/v2webapi .
docker push patobelardo/v2webapi

k scale deploy webapiapp --replicas 0
k scale deploy webapiapp --replicas 1