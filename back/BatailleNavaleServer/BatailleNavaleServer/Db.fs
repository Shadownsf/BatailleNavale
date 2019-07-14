﻿namespace SuaveRestApi.Rest
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
  let mutable playerIdTurn = 0
  let mutable playerAId = 0
  let mutable playerBId = 0

  //On a plusieurs utilisateurs mais on n'en a besoin que deux pour un jeu
  let private playerStorage = new Dictionary<int, Player>()
  //Pour récuper le jouer par son jeton
  let private tokenStorage = new Dictionary<string, Player>()
  //la liste de points ratés. le poit avec le bouléan étant true appartient au Jouer A
  let private missedPoints = new List<Position>()
 
  let AddToken (player : Player) =
    tokenStorage.Add(player.Token, player)
  
  let CreateNullPlayer = {
    Player.Id = 0;
    Name = "";
    Password = "";
    Token = "";
    Boats = new List<Boat>()
  }


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

  let GetPlayerByToken token =
    if tokenStorage.ContainsKey(token) then
      tokenStorage.[token]
    else
      CreateNullPlayer

  let GetPlayers () =
    playerStorage.Values |> Seq.map (fun p -> p)

  let GetToken (tokenRequest : TokenRequest) =
    let mutable r = {TokenResponse.Token = ""}
    if (playerStorage.ContainsKey(tokenRequest.Id)) then
      if (tokenRequest.Password = playerStorage.[tokenRequest.Id].Password) then
        r <- {TokenResponse.Token = playerStorage.[tokenRequest.Id].Token}
    r

  let CountBoatsPoints (boats : List<Boat>) =
    let nOfBoats = boats.Count
    let mutable nOfPoints = 0
    for b in boats do
      nOfPoints <- b.Positions.Count
    (nOfBoats, nOfPoints)
 
 
  let PutMap (request : MapPUTRequest) =
    let mutable nBoats = 0
    let mutable nPoints = 0
    if (tokenStorage.ContainsKey(request.Token)) then
      let requestionPlayer = tokenStorage.[request.Token]
      if (numberOfBoats, numberOfPoints) = (CountBoatsPoints request.Boats) then
        nBoats <- numberOfBoats
        nPoints <- numberOfPoints
        if (invitingPlayer = 0) then
          invitingPlayer <- requestionPlayer.Id
          playerAId <- requestionPlayer.Id
          playerIdTurn <- requestionPlayer.Id
        else
          playerBId <- requestionPlayer.Id
        if (playerAId > 0) && (playerBId > 0) then
          gameStatus <- 1

    {NumberOfBoats = nBoats; NumberOfPoints = nPoints}
 
  let GetMap (request : GenericRequest) =
    if tokenStorage.ContainsKey(request.Token) then
      let player = tokenStorage.[request.Token]
      {MapResponse.Jouer = player; MissedPoints = missedPoints}
    else
      {MapResponse.Jouer = CreateNullPlayer; MissedPoints = new List<Position>()}
  
  let Shoot (bullet : Bullet) =
    let mutable enemyBool = true
    let mutable ennemiId = playerBId
    let mutable getShot = -1
    let mutable r = -1
    let bX = bullet.X
    let bY = bullet.Y
    let player = GetPlayerByToken bullet.Token
    //Vérifier si le jeu est prêt
    if ((player.Id <> 0)
    && ((player.Id = playerAId) || (player.Id = playerBId))
    && (gameStatus = 1)
    && (playerIdTurn = player.Id)) then
      getShot <- 0
      //Chercher l'ennemi
      if (player.Id = playerBId) then
        ennemiId <- playerAId
        enemyBool <- false
      let ennemi = playerStorage.[ennemiId]

      let mutable i = 0
      let mutable j = 0
      while (i < ennemi.Boats.Count) do
        while (j < ennemi.Boats.[i].Positions.Count) do
          let (x, y, recu) = ennemi.Boats.[i].Positions.[j]
          if ((bX = x) && (bY = y)) then
            ennemi.Boats.[i].Positions.[j] <- (bX, bY, true)
            getShot <- 1
          j <- j + 1
        i <- i + 1
    if getShot <> -1 then
      playerIdTurn <- ennemiId
    elif getShot = 0 then
       missedPoints.Add((bX, bY, enemyBool))
    getShot

  let GetBoats (request : GenericRequest) =
    if tokenStorage.ContainsKey(request.Token) then
       let player = tokenStorage.[request.Token]
       {MapPUTRequest.Token = player.Token; Boats = player.Boats}
    else
       {MapPUTRequest.Token = ""; Boats = new List<Boat>()}
