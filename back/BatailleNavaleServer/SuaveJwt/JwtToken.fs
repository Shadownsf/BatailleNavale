namespace SuaveJwt

[<AutoOpen>]
module JwtToken =
    open System
    open System.Security.Cryptography
    open Microsoft.IdentityModel.Tokens
    open System.IdentityModel.Tokens.Jwt


    type Audience = {
        ClientId: string
        Secret: Base64String
        Name: string
    }

    type Token = {
        AccessToken: string
        ExpiresIn: float
    }

    type TokenCreateRequest = {
        Issuer: string
        UserName: string
        Password: string
        TokenTimeSpan: TimeSpan
    }

    type TokenValidationRequest = {
        Issuer: string
        SecurityKey: SecurityKey
        ClientId: string
        AccessToken: string
    }

    type TokenCreateCredentials = {
        UserName: string
        Password: string
        ClientId: string
    }

    type IdentityStore = {
        getClaims:string -> Async<Security.Claims.Claim seq>
        isValidCredentials:string -> string -> Async<bool>
        getSecurityKey:Base64String -> SecurityKey
        getSignInCredentials:SecurityKey -> SigningCredentials
    }

    let createAudience audienceName =
        let clientId = Guid.NewGuid().ToString()
        let data = Array.zeroCreate 32
        RNGCryptoServiceProvider.Create().GetBytes(data)
        let secret = data |> Base64String.create 
        {ClientId = clientId; Secret = secret; Name = audienceName}

    let createToken (request:TokenCreateRequest) identityStore audience =
        async {
            let userName = request.UserName

            let! isValidCredentials =
                identityStore.isValidCredentials userName request.Password

            if isValidCredentials then
                let signinCredentials =
                    audience.Secret
                    |> (identityStore.getSecurityKey >> identityStore.getSignInCredentials)
                
                let issuedOn = Nullable DateTime.UtcNow
                let expiresBy = Nullable (DateTime.UtcNow.Add (request.TokenTimeSpan))
                
                let! claims = identityStore.getClaims userName

                let jwtSecurityToken = 
                    new JwtSecurityToken (request.Issuer,
                        audience.ClientId, claims, issuedOn,
                        expiresBy, signinCredentials)

                let handler = new JwtSecurityTokenHandler ()
                let accessToken = handler.WriteToken (jwtSecurityToken)

                return Some {AccessToken = accessToken
                             ExpiresIn = request.TokenTimeSpan.TotalSeconds}
            else 
                return None
        }

    let validate (validationRequest:TokenValidationRequest) =
        let tokenValidationParameters =
            let validationParams = new TokenValidationParameters ()
            validationParams.ValidateAudience <- true
            validationParams.ValidateIssuer <- true
            validationParams.ValidateLifetime <- true
            validationParams.ValidateIssuerSigningKey <- true
            validationParams.IssuerSigningKey <- validationRequest.SecurityKey
            validationParams

        try 
            let handler = new JwtSecurityTokenHandler ()
            let principal =
                handler.ValidateToken(validationRequest.AccessToken,
                                        tokenValidationParameters, ref null)
            principal.Claims |> Choice1Of2
        with
            | ex -> ex.Message |> Choice2Of2