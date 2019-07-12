open SuaveRestApi.Rest
open SuaveRestApi.Db
open Suave.Http
open Suave.Web

[<EntryPoint>]
let main argv =
    let playerWebpart = rest "player" {
        GetAll = Db.getPlayers
        Create = Db.createPlayer
        Update = Db.updatePlayer
        Delete = Db.deletePlayer
        GetById = Db.getPlayerById
        UpdateById = Db.updatePlayerById
        Exists = Db.doesPlayerExist
    }

    let config =
        { defaultConfig
            with bindings = [HttpBinding.createSimple HTTP "127.0.0.1" 8080]}

    // startWebserver defaultConfig (choose [personWebPart;otherWebPart])
    startWebServer config playerWebpart

    0