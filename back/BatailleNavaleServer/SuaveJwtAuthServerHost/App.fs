open SuaveJwt.AuthServer
open SuaveJwtAuthServerHost
open Suave.Web
open System
open SuaveJwt.JwtToken
open SuaveJwt
open Suave.Http

[<EntryPoint>]
let main argv =
    let authorizationServerConfig = {
        AddAudienceUrlPath = "/api/audience"
        CreateTokenUrlPath = "/oauth2/token"
        SaveAudience = AudienceStorage.saveAudience
        GetAudience = AudienceStorage.getAudience
        Issuer = "http://localhost:8080/suave"
        TokenTimeSpan = TimeSpan.FromMinutes(1.)
    }

    let identityStore = {
        getClaims = IdentityStore.getClaims
        isValidCredentials = IdentityStore.isValidCredentials
        getSecurityKey = KeyStore.securityKey
        getSignInCredentials = KeyStore.hmacSha256
    }
    
    let audienceWebPart' = audienceWebPart authorizationServerConfig identityStore
    
    let config =
        { defaultConfig
            with bindings = [HttpBinding.createSimple HTTP "127.0.0.1" 8083]}
    
    startWebServer config audienceWebPart'

    0
