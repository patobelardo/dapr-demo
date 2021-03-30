docker build -t patobelardo/v2frontend .
docker push patobelardo/v2frontend

k scale deploy blazorapp --replicas 0
k scale deploy blazorapp --replicas 1