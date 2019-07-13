namespace SuaveRestApi.Rest
open System.Collections.Generic
open System


//x y et (subit une balle ou pas)
type Position = int * int * bool

type Boat = {
  Name: string;
  Positions: list<Position>
}

type Player = {
  Id : int;
  Name : string;
  Password : string;
  Token : string
  Boats: list<Boat>
}

//record pour /status
type Plan = {
  Jouer : Player;
  PointsMissed : List<Position>
}

module Db =
  let numberOfBoats = 1   //Le nombre total des bateau pour chaque jouer
  let numberOfPoints = 3  //le monbre de points pour construire les bateaux
  let demension = (10, 10)  //largeur = 10, longeur = 10
  let mutable gameStatus = 0
  (*
  0 Négotiation des plans
  1 Prêt à jouer
  2 Fin du jeu
  *)
  let mutable invitingPlayer = 0
  let mutable playerTurn = 0

  //On a plusieurs utilisateurs mais on n'en a besoin que deux pour un jeu
  let private playerStorage = new Dictionary<int, Player>()
  //Pour récuper le jouer par son jeton
  let private tokenStorage = new Dictionary<string, Player>()
  //la liste de points ratés. le poit avec le bouléan étant true appartient au Jouer A
  let private pointsMissed = new List<Position>()

  let AddToken player =
    tokenStorage.Add(player.Token, player)


  let createPlayer player =
    let id = playerStorage.Values.Count + 1
    let token = Guid.NewGuid().ToString()
    let newPlayer = {player with Id = id; Token = token}
    playerStorage.Add(id, newPlayer)
    tokenStorage.Add(token, newPlayer)
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

  let deletePlayer playerId =
    playerStorage.Remove(playerId) |> ignore
  
  let IsPlayerExists = playerStorage.ContainsKey
  
  let GetPlayerById id =
    if playerStorage.ContainsKey(id) then
      Some playerStorage.[id]
    else
    None

  let GetPlayers () =
    playerStorage.Values |> Seq.map (fun p -> p)

  let GetToken playerId password =
    let mutable r = ""
    if (playerStorage.ContainsKey(playerId)) then
      if (password = playerStorage.[playerId].Password) then
        r <- playerStorage.[playerId].Token
    r
 