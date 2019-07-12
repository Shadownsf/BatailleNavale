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
module RestFul =
  //record type representing a RESTful resource
  //
  type RestResource<'a> = {
    GetPlayers : unit -> 'a seq
    CreatePlayer : 'a -> 'a
    UpdatePlayer : 'a -> 'a option
    DeletePlayer : int -> unit
    GetPlayerById : int -> 'a option
    IsPlayerExists : int -> bool
  }

  //Convertir d'une chaîne JSON en objet
  let fromJson<'a> json =
    JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a
  
  //Convertir d'une HttpRequest en objet 
  let getResourceFromReq<'a> (req : HttpRequest) =
     let getString rawForm =
        Encoding.UTF8.GetString(rawForm)
     let r = req.rawForm |> getString |> fromJson<'a>
     printfn "%O" r
     r

  //Convertir d'un objet en chaîne JSON et ensuit créer un WebPart
  // 'a -> WebPart
  let JSON v =
    let jsonSerializerSettings = new JsonSerializerSettings()
    jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
    JsonConvert.SerializeObject(v, jsonSerializerSettings)
    |> OK
    >=> Writers.setMimeType "application/json; charset=utf-8"

 // string -> RestResource<'a> -> WebPart
 //resourceName : people
  let rest resourceName resource =
    //pour des mauvaises requêtes
    let badRequest = BAD_REQUEST "Resource not found"
    //chemain plain
    let resourcePath = "/" + resourceName

    let handleResource requestError = function
      | Some r -> r |> JSON
      | _ -> requestError
 
    //pour GET
    let getAll = warbler (fun _ -> resource.GetPlayers () |> JSON)
    //pour DELETE
    let resourceIdPath =
      let path = resourcePath + "/%d"
      new PrintfFormat<(int -> string),unit,string,string,int>(path)

    let getResourceById =
      resource.GetPlayerById >> handleResource (NOT_FOUND "Resource not found")
 
    //pour DELETE
    let deleteResourceById id =
      resource.DeletePlayer id
      NO_CONTENT

    let isResourceExists id =
      let isExits = resource.IsPlayerExists id
      printfn "resource %b" isExits
      if isExits then OK "" else NOT_FOUND ""

    choose [
      path resourcePath >=> choose [
      Filters.GET >=> getAll
      Filters.POST >=> request (getResourceFromReq >> resource.CreatePlayer >> JSON)
      Filters.PUT >=>
        request (getResourceFromReq >>
          resource.UpdatePlayer >> handleResource badRequest)
      ]
      Filters.DELETE >=> pathScan resourceIdPath deleteResourceById
      Filters.GET >=> pathScan resourceIdPath getResourceById
      Filters.HEAD >=> pathScan resourceIdPath isResourceExists
    ]

