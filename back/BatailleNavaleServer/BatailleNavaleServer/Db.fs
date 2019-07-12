namespace SuaveRestApi.Rest
open System.Collections.Generic
open System

type Position = {
  X: int
  Y: int
}

type Boat = {
  Name: string
  Positions: List<Position>
}

type Player = {
  Id : int;
  Name : string;
  Password : string;
  Token : string
  //Boats: List<Boat>
}

module Db =
  //Créer un dictonaire de playernes
  let private playerStorage = new Dictionary<int, Player>()
  let private tokenStorage = new Dictionary<string, Player>()
 
  let updatePlayerById playerId playerToBeUpdated =
    if playerStorage.ContainsKey(playerId) then
      let updatedPlayer = { playerToBeUpdated with Id = playerId }
      playerStorage.[playerId] <- updatedPlayer
      Some updatedPlayer
    else
      None

  let updatePlayer playerToBeUpdated =
    updatePlayerById playerToBeUpdated.Id playerToBeUpdated

  let deletePlayer playerId =
    playerStorage.Remove(playerId) |> ignore
  
  let IsPlayerExists = playerStorage.ContainsKey
  
  let createPlayer player =
    let id = playerStorage.Values.Count + 1
    let token = Guid.NewGuid().ToString()
    let newPlayer = {player with Id = id; Token = token}
    playerStorage.Add(id, newPlayer)
    tokenStorage.Add(token, newPlayer)
    newPlayer
 
  let GetPlayerById id =
    if playerStorage.ContainsKey(id) then
      Some playerStorage.[id]
    else
    None

  let addPlayer nom password token =
    let id = playerStorage.Values.Count + 1
    let token = Guid.NewGuid().ToString()
    let player =
      {Player.Id = id;
      Player.Name = nom;
      Player.Password = password;
      Player.Token = token}
    playerStorage.Add(id, player)
    tokenStorage.Add(token, player)
    player
 
  let GetPlayers () =
    playerStorage.Values |> Seq.map (fun p -> p)

  let storePlayer filePath =
    System.IO.File.AppendAllText(filePath, sprintf "%s\n" filePath)

 