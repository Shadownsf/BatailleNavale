namespace SuaveRestApi.Rest

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave.Successful
open Suave.RequestErrors
open Suave.Filters
open Suave
open Suave.Operators
open System.Text

[<AutoOpen>]
module RestFulService =
  //Convertir d'une chaîne JSON en objet
  let JsonToObj<'T> json =
    JsonConvert.DeserializeObject(json, typeof<'T>) :?> 'T
  
  let ObjToJson obj =
    let jsonSerializerSettings = new JsonSerializerSettings()
    jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
    JsonConvert.SerializeObject(obj, jsonSerializerSettings)
 
  //Convertir d'une HttpRequest en objet 
  let getResourceFromReq<'a> (req : HttpRequest) =
     let getString rawForm =
        Encoding.UTF8.GetString(rawForm)
     let r = req.rawForm |> getString |> JsonToObj<'a>
     printfn "%O" r
     r

  //Convertir d'un objet en chaîne JSON et ensuit créer un WebPart
  // 'a -> WebPart
  let JSON v =
    ObjToJson v |> OK
    >=> Writers.setMimeType "application/json; charset=utf-8"

  let handleResource requestError = function
    | Some r -> r |> JSON
    | _ -> requestError

  let resourceIdPath resourcePath =
     let path = resourcePath + "/%d"
     new PrintfFormat<(int -> string),unit,string,string,int>(path)

  let badRequest = BAD_REQUEST "Resource not found"

