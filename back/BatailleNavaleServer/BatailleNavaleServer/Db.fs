namespace SuaveRestApi.Rest
open System.Collections.Generic
open System

module Db =
  let numberOfBoats = 1   //Le nombre total des bateau pour chaque jouer
  let numberOfPoints = 2  //le monbre de points pour construire les bateaux
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
  let private missedPoints = []

  let AddToken (player : Player) =
    tokenStorage.Add(player.Token, player)

  let createPlayer (player : Player) =
    let id = playerStorage.Values.Count + 1
    //let token = Guid.NewGuid().ToString()
    let token = player.Token
    let newPlayer : Player = {player with Id = id; Token = token}
    playerStorage.Add(id, newPlayer)
    tokenStorage.Add(token, newPlayer)
    newPlayer
  
  let updatePlayerById (playerId : int) (playerToBeUpdated : Player) =
    if playerStorage.ContainsKey(playerId) then
      let updatedPlayer = { playerToBeUpdated with Id = playerId }
      playerStorage.[playerId] <- updatedPlayer
      Some updatedPlayer
    else
      None

  let updatePlayer (playerToBeUpdated : Player) =
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

  let GetToken (tokenRequest : TokenRequest) =
    let mutable r = {TokenResponse.Token = ""}
    if (playerStorage.ContainsKey(tokenRequest.Id)) then
      if (tokenRequest.Password = playerStorage.[tokenRequest.Id].Password) then
        r <- {TokenResponse.Token = playerStorage.[tokenRequest.Id].Token}
    r

  let CreateNullPlayer = {
    Player.Id = 0;
    Name = "";
    Password = "";
    Token = "";
    Boats = []
  }
  let CountBoatsPoints (boats : list<Boat>) =
    let nOfBoats = boats.Length
    let mutable nOfPoints = 0
    for b in boats do
      nOfPoints <- b.Positions.Length
    (nOfBoats, nOfPoints)
 

  let PutMap (request : MapPUTRequest) =
    let mutable nBoats = 0
    let mutable nPoints = 0
    if (tokenStorage.ContainsKey(request.Token)) then
      let requestionPlayer = tokenStorage.[request.Token]
      if (numberOfBoats, numberOfPoints) = (CountBoatsPoints request.Boats) then
        if (invitingPlayer = 0) then
          invitingPlayer <- requestionPlayer.Id
          nBoats <- numberOfBoats
          nPoints <- numberOfPoints
    {NumberOfBoats = nBoats; NumberOfPoints = nPoints}
 
  let GetMap (request : GenericRequest) =
    if tokenStorage.ContainsKey(request.Token) then
      let player = tokenStorage.[request.Token]
      {MapResponse.Jouer = player; MissedPoints = missedPoints}
    else
      {MapResponse.Jouer = CreateNullPlayer; MissedPoints = []}
 
  let GetBoats (request : GenericRequest) =
    if tokenStorage.ContainsKey(request.Token) then
       let player = tokenStorage.[request.Token]
       {MapPUTRequest.Token = player.Token; Boats = player.Boats}
    else
       {MapPUTRequest.Token = ""; Boats = []}
