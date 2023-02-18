#

docker-compose -f avalanche-domain.yml up -d
docker-compose -f avalanche-authority.yml up -d
docker-compose -f avalanche-services.yml up -d
