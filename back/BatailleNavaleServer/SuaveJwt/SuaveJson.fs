namespace SuaveJwt

[<AutoOpen>]
module SuaveJson =
    open Suave
    open Suave.Operators
    open Newtonsoft.Json
    open Suave.Successful
    open Newtonsoft.Json.Serialization

    let toJson v =
        let settings = new JsonSerializerSettings()
        settings.ContractResolver
            <- new CamelCasePropertyNamesContractResolver()
        JsonConvert.SerializeObject (v, settings)
        |> OK
        >=> Writers.setMimeType "application/json, charset=utf-8"

    let mapJsonPayload<'T> (req:HttpRequest) =
        let fromJson json =
            try
                JsonConvert.DeserializeObject (json, typeof<'T>)
                :?> 'T
                |> Some
            with
            | _ -> None

        let getString (rawForm:byte[]) = 
            rawForm |> System.Text.Encoding.UTF8.GetString

        req.rawForm |> getString |> fromJson