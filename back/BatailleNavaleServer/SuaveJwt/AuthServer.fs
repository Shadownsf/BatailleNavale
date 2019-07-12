namespace SuaveJwt

[<AutoOpen>]
module AuthServer =
    open Suave
    open System
    open Suave.Filters
    open Suave.Operators
    open Suave.RequestErrors

    type AudienceCreateRequest = {
        Name:string
    }

    type AudienceCreateResponse = {
        ClientId:string
        Base64Secret:string
        Name:string
    }

    type Config = {
        AddAudienceUrlPath:string
        SaveAudience:Audience -> Async<Audience>
        CreateTokenUrlPath: string
        GetAudience: string -> Async<Audience option>
        Issuer: string
        TokenTimeSpan: TimeSpan
    }

    let audienceWebPart config identityStore =
        let toAudienceCreateResponse (audience:Audience) = {
            Base64Secret = audience.Secret.ToString()
            ClientId = audience.ClientId
            Name = audience.Name
        }

        let tryCreateAudience (ctx:HttpContext) =
            match mapJsonPayload<AudienceCreateRequest> ctx.request with
            | Some (audienceCreateRequest:AudienceCreateRequest) ->
                async {
                    let! audience =
                        audienceCreateRequest.Name
                        |> createAudience
                        |> config.SaveAudience

                    let audienceCreateResponse =
                        toAudienceCreateResponse audience
                    return! toJson audienceCreateResponse ctx
                }
            | None -> BAD_REQUEST "Invalid Audience Create Request" ctx

        let tryCreateToken (ctx: HttpContext) =
            match mapJsonPayload<TokenCreateCredentials> ctx.request with
            |Some tokenCreateCredentials ->
                async {
                    let! audience =
                        config.GetAudience tokenCreateCredentials.ClientId
                    match audience with
                    | Some audience ->
                        let tokenCreateRequest:TokenCreateRequest = {
                            Issuer = config.Issuer
                            UserName = tokenCreateCredentials.UserName
                            Password = tokenCreateCredentials.Password
                            TokenTimeSpan = config.TokenTimeSpan
                        }

                        let! token =
                            createToken tokenCreateRequest identityStore audience

                        match token with
                        | Some token -> return! toJson token ctx
                        | None -> return! BAD_REQUEST "Invalid Login Credentials" ctx
                    | None -> return! BAD_REQUEST "Invalid Client Id" ctx
                }
            | None -> BAD_REQUEST "Invalid Token Create Request" ctx

        choose [
            path config.AddAudienceUrlPath >=> POST >=> tryCreateAudience
            path config.CreateTokenUrlPath >=> POST >=> tryCreateToken
        ]
        
        


    