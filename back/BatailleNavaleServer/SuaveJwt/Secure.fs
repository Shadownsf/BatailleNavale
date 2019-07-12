﻿namespace SuaveJwt

[<AutoOpen>]
module Secure =
    open Microsoft.IdentityModel.Tokens
    open Suave.RequestErrors
    open Suave.Http

    type JwtConfig = {
        Issuer: string
        SecurityKey: SecurityKey
        ClientId: string
    }

    let jwtAuthenticate jwtConfig webpart (ctx: HttpContext) =
        let updateContextWithClaims claims =
            { ctx with
                userState = ctx.userState.Remove("Claims").Add("Claims", claims)}

        match ctx.request.header "token" with
        | Choice1Of2 accessToken ->
            let tokenValidationRequest = {
                Issuer = jwtConfig.Issuer
                SecurityKey = jwtConfig.SecurityKey
                ClientId = jwtConfig.ClientId
                AccessToken = accessToken
            }

            let validationResult = validate tokenValidationRequest

            match validationResult with
            | Choice1Of2 claims -> webpart (updateContextWithClaims claims)
            | Choice2Of2 err -> FORBIDDEN err ctx
        | _ ->
            BAD_REQUEST "Invalid Request; Provide both clientId and token" ctx


