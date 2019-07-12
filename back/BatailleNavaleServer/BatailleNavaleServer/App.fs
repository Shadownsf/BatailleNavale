open SuaveRestApi.Rest
open SuaveJwt.Encoding
open SuaveRestApi.Db
open Suave.Http
open Suave.Web
open SuaveJwt

[<EntryPoint>]
let main argv =
    let base64Key =
        Base64String.fromString "Op5EqjC7aLS2dx3gIOzADPIZGX2As6UEWjA4oyBjMo"

    let jwtConfig = {
        Issuer = "http://localhost:8083/suave"
        ClientId = "7ff79ba3305c4e4f9d0ececeae70c78f"
        SecurityKey = KeyStore.securityKey base64Key
    }

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