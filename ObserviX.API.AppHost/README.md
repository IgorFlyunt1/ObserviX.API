**Keycloak configuration**

Log in to Keycloak by going to Aspire Dashboard and clicking on the "Keycloak" link
(default link is configured to be [http://localhost:8080](http://localhost:8080)).

The default username is "admin" 
and the password is random and can be viewed in \
Aspire Dashboard > Keycloak entry details >
Environment variables > KEYCLOAK_ADMIN_PASSWORD.

Set up Keycloak with the following steps:
1. Create a new "gateway" realm in Keycloak;
2. Create a new "gateway-public-client" client in the "gateway" realm;
3. In Client Scopes, create a new scope named "gateway-scope";
4. In the "gateway-public-client" client, add the "gateway-scope" scope to the "Default Client Scopes";
5. In the "gateway-scope" mappers add new mapper "gateway-aud";
6. In the "gateway-aud" mapper IGNORE "Included Client Audience" and set "Included Custom Audience" to "observix.gateway";