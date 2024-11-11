#!/bin/bash
cd etc/docker 
docker-compose -f docker-compose.infrastructure.yml -f docker-compose.infrastructure.override.yml down
cd ../..