namespace SuaveRestApi.Rest
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave.Successful
open Suave.RequestErrors
open Suave.Filters
open Suave
open Suave.Operators
open System.Text

//record type representing a RESTful resource
 //
type PlayerResource<'a> = {
  GetPlayers : unit -> 'a seq
  CreatePlayer : 'a -> 'a
  UpdatePlayer : 'a -> 'a option
  DeletePlayer : int -> unit
  GetPlayerById : int -> 'a option
  IsPlayerExists : int -> bool
}

[<AutoOpen>]
module RestfulPlayer =

  let playerRest route resource =
    //chemain plain
    let resourcePath = "/" + route

    //pour GET
    let getAll = warbler (fun _ -> resource.GetPlayers () |> JSON)
    //pour DELETE
 
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
      Filters.DELETE >=> pathScan (resourceIdPath resourcePath) deleteResourceById
      Filters.GET >=> pathScan (resourceIdPath resourcePath) getResourceById
      Filters.HEAD >=> pathScan (resourceIdPath resourcePath) isResourceExists
    ]