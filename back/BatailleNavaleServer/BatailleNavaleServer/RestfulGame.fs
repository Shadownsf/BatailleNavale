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
type GameResource<'a> = {
  GetToken : TokenRequest -> TokenResponse
  GetMap : GenericRequest -> MapResponse
  PutMap : MapPUTRequest -> MapPUTReponse
  GetBoats : GenericRequest -> MapPUTRequest
  GetPlayers : unit -> 'a seq
}

[<AutoOpen>]
module RestfulGame =

  let gameRest pathBaseName resource =
    let pathBase = "/" + pathBaseName

    let getAll = warbler (fun _ -> resource.GetPlayers () |> JSON)
    choose [
      path (pathBase + "/token") >=> choose [
        Filters.POST >=> request (getResourceFromReq >> resource.GetToken >> JSON)
        Filters.PUT >=> request (getResourceFromReq >> resource.GetToken >> JSON)
      ]
      path (pathBase + "/map") >=> choose [
        Filters.POST >=> request (getResourceFromReq >> resource.GetMap >> JSON)
        Filters.PUT >=> request (getResourceFromReq >> resource.PutMap >> JSON)
      ]

      Filters.GET >=> getAll
    ]

