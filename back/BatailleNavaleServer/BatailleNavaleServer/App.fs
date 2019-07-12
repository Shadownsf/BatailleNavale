open SuaveRestApi.Rest
open Suave.Web

[<EntryPoint>]
let main argv =
  
  let restResource = {
    GetPlayers = Db.GetPlayers
    CreatePlayer = Db.createPlayer
    UpdatePlayer = Db.updatePlayer
    GetPlayerById = Db.GetPlayerById
    DeletePlayer = Db.deletePlayer
    IsPlayerExists = Db.IsPlayerExists
  }
 
  let playerWebPart = rest "people" restResource
 
  startWebServer defaultConfig playerWebPart
  //startWebServer defaultConfig (choose [playerWebPart;albumWebPart])
  0