mkcert "passwordlessauthentication.dev" "*.passwordlessauthentication.dev" 
kubectl create namespace passwordlessauthentication
kubectl create secret tls -n passwordlessauthentication passwordlessauthentication-tls --cert=./passwordlessauthentication.dev+1.pem  --key=./passwordlessauthentication.dev+1-key.pem