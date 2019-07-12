open SuaveRestApi.Rest
open SuaveRestApi.Db
open Suave.Http
open Suave.Web

[<EntryPoint>]
let main argv =
    let personWebPart = rest "people" {
        GetAll = Db.getPeople
        Create = Db.createPerson
        Update = Db.updatePerson
        Delete = Db.deletePerson
        GetById = Db.getPerson
        UpdateById = Db.updatePersonById
        Exists = Db.doesPersonExist
    }

    let config =
        { defaultConfig
            with bindings = [HttpBinding.createSimple HTTP "127.0.0.1" 8080]}

    // startWebserver defaultConfig (choose [personWebPart;otherWebPart])
    startWebServer config personWebPart

    0