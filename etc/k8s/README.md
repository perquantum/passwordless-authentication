# Deployment Guide

This guide explains how to deploy your microservice template into local Kubernetes cluster.

 ## Pre-requirements

* [Docker for Desktop](https://www.docker.com/products/docker-desktop/) with Kubernetes enabled or [Minikube](https://minikube.sigs.k8s.io/docs/start/)
* [Helm](https://helm.sh/docs/intro/install/) for running helm charts
* [NGINX ingress](https://kubernetes.github.io/ingress-nginx/deploy/) for k8s

    OR

    NGINX ingress using helm
```powershell
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo update

helm upgrade --install --version=4.0.19 ingress-nginx ingress-nginx/ingress-nginx
```
## Configuring HTTPS for Local K8s 

You can run the solution on staging environment in your local Kubernetes cluster with HTTPS. There are various ways to create a self-signed certificate. 

### Installing mkcert
This guide will be using `mkcert` for creating self-signed certificates. Follow the [installation guide](https://github.com/FiloSottile/mkcert#installation) to install mkcert.

### Creating mkcert Root CA
Use the command to create root (local) certificate authority for your certificates:
```powershell
mkcert -install
```

> **Note:** All the certificates created by mkcert certificate creation will be trusted by the local machine

### Run mkcert

Use the `create-tls-secrets.ps1` powershell script to create self-signed certificates for your domains and the tls secret for Kubernetes.

> **Note:** mkcert must be installed and trusted root to use this script.

## Remove Helm Charts

Necessary helm charts are created under `etc/k8s/passwordlessauthentication` folder. Based on your back-office application UI, remove the other UI charts. Ex, if you are using **angular** UI, remove **web**, **blazor** and **blazor-server** charts.

## How to run?

* Add entries to the hosts file (in Windows: `C:\Windows\System32\drivers\etc\hosts`, in linux and macos: `/etc/hosts` ):

````powershell
127.0.0.1 web.passwordlessauthentication.dev
127.0.0.1 passwordlessauthentication.dev
127.0.0.1 authserver.passwordlessauthentication.dev
127.0.0.1 identity.passwordlessauthentication.dev
127.0.0.1 administration.passwordlessauthentication.dev
127.0.0.1 product.passwordlessauthentication.dev
127.0.0.1 saas.passwordlessauthentication.dev
127.0.0.1 gateway-web.passwordlessauthentication.dev
127.0.0.1 gateway-public.passwordlessauthentication.dev
````

* Run `build-images.ps1` or `build-images-locally.ps1` in the `build` directory.

  > **Note:** `build-images.ps1` uses multi-stage docker image building in containers, which is CI&CD friendly. `build-images-locally.ps1` requires `.NET SDK` but is faster than in-container image building.

* Run `deploy-staging.ps1` in the `helm-chart` directory. It is deployed with the `passwordlessauthentication` namespace.

* *You may wait ~30 seconds on first run for preparing the database*.

* Browse https://passwordlessauthentication-st-public-web for public and
   https://passwordlessauthentication-st-web for the back-office application
* Default username: `admin`, password: `1q2w3E*`.
