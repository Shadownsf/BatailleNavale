namespace SuaveRestApi.Rest

[<AutoOpen>]
module RestFul =
    open Suave
    open SuaveJwt
    open Successful
    open Suave.Filters
    open Suave.Operators
    open Newtonsoft.Json
    open Suave.RequestErrors
    open Newtonsoft.Json.Serialization
    open Microsoft.IdentityModel.Claims
    
    type RestResource<'T> = {
        GetAll:unit -> 'T seq
        Create:'T -> 'T
        Update:'T -> 'T option
        Delete:int -> unit
        GetById:int -> 'T option
        UpdateById:int -> 'T -> 'T option
        Exists:int -> bool
    }

    let fromJson<'T> json =
        JsonConvert.DeserializeObject(json, typeof<'T>) :?> 'T

    let toJson v =
        let settings = new JsonSerializerSettings()
        settings.ContractResolver <- new CamelCasePropertyNamesContractResolver()

        JsonConvert.SerializeObject(v, settings)
        |> OK >=> Writers.setMimeType "application/json; charset=utf-8" 

    let getResourceFromReq<'T> (req:HttpRequest) =
        let getString (rawForm:byte[]) = System.Text.Encoding.UTF8.GetString(rawForm)
        req.rawForm |> getString |> fromJson<'T>

    let rest resourceName resource =
        let resourcePath = "/" + resourceName
        let getAll = warbler (fun _ -> resource.GetAll () |> toJson)

        let badRequest = BAD_REQUEST "Resource not found"
        let handleResource requestError = function
            | Some r -> r |> toJson
            | _ -> requestError

        let resourceIdPath =
            let path = resourcePath + "/%d"
            new PrintfFormat<(int -> string),unit,string,string,int>(path)

        let deleteResourceById id =
            resource.Delete id
            NO_CONTENT

        let getResourceById =
            resource.GetById >> handleResource (NOT_FOUND "Resource not found")

        let updateResourceById id =
            request (getResourceFromReq >> (resource.UpdateById id) >> handleResource badRequest)

        let doesResourceExist id =
            if resource.Exists id then OK "" else NOT_FOUND ""

        let base64Key =
            Base64String.fromString "Op5EqjC7aLS2dx3gIOzADPIZGX2As6UEWjA4oyBjMo"

        let jwtConfig = {
            Issuer = "http://localhost:8083/suave"
            ClientId = "7ff79ba3305c4e4f9d0ececeae70c78f"
            SecurityKey = KeyStore.securityKey base64Key
        }

        let authorizeAdmin (claims:Claim seq) =
            let isAdmin (c:Claim) =
                c.ClaimType = ClaimTypes.Role && c.Value = "Admin"
            match claims |> Seq.tryFind isAdmin with
            | Some _ -> Authorized |> async.Return
            | None -> UnAuthorized "User is not an admin" |> async.Return

        choose [
            path resourcePath
                >=> jwtAuthenticate jwtConfig (OK "access granted")
                >=> jwtAuthorize jwtConfig authorizeAdmin (OK "rights granted")
                >=> choose [
                    GET >=> getAll
                    POST >=> request (getResourceFromReq >> resource.Create >> toJson)
                    PUT >=> request (getResourceFromReq >> resource.Update >> handleResource badRequest)
                ]
            DELETE >=> pathScan resourceIdPath deleteResourceById
            GET >=> pathScan resourceIdPath getResourceById
            PUT >=> pathScan resourceIdPath updateResourceById
            HEAD >=> pathScan resourceIdPath doesResourceExist
        ]
        


