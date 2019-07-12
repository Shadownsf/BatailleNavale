namespace SuaveRestApi.Db

open System.Collections.Generic

type Position = {
    X: int
    Y: int
}

type Boat = {
    Name: string
    Positions: List<Position>
}

type Player = {
    Id: int
    Name: string
    Age: int
    Email: string
    Boats: List<Boat>
}

module Db =
    let private playerStorage = new Dictionary<int, Player>()
    let getPlayers () = playerStorage.Values :> seq<Player>

    let createPlayer player =
        let id = playerStorage.Values.Count + 1
        let newPlayer= {player with Id = id}
        playerStorage.Add(id, newPlayer)
        newPlayer

    let updatePlayerById playerId playerToBeUpdated =
        if playerStorage.ContainsKey(playerId) then
            let updatedPlayer = { playerToBeUpdated with Id = playerId }
            playerStorage.[playerId] <- updatedPlayer
            Some updatedPlayer
        else
            None

    let updatePlayer playerToBeUpdated =
        updatePlayerById playerToBeUpdated.Id playerToBeUpdated

    let deletePlayer personId =
        playerStorage.Remove(personId) |> ignore

    let getPlayerById id =
        if playerStorage.ContainsKey(id) then
            Some playerStorage.[id]
        else
            None

    let doesPlayerExist = playerStorage.ContainsKey
