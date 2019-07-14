//La base en mémoire et la logic du jeu
namespace SuaveRestApi.Rest
open System.Collections.Generic
open System

module Db =
  
  //la partie de configuration
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
 
  //La partie de guestion des joueurs
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

 
  //la partie du jeu
  //entrée : identifiant et mot de passe
  //sortie : le jeton ou une chaîne vide
  let GetToken (tokenRequest : TokenRequest) =
    let mutable r = {TokenResponse.Token = ""}
    if (playerStorage.ContainsKey(tokenRequest.Id)) then
      if (tokenRequest.Password = playerStorage.[tokenRequest.Id].Password) then
        r <- {TokenResponse.Token = playerStorage.[tokenRequest.Id].Token}
    r

  //Entrée : une liste de bateaux
  //Sortie : (nombre de bateaux, nombre de points)
  let CountBoatsPoints (boats : List<Boat>) =
    let nOfBoats = boats.Count
    let mutable nOfPoints = 0
    for b in boats do
      nOfPoints <- b.Positions.Count
    (nOfBoats, nOfPoints)
 
  //Entrée : un jeton pour identifier le joueur, une liste de bateaux
  //Opération : Joindre la liste de bateaux au joueur
  //Sortie : plusieurs problèmes divers si {NumberOfBoats = 0; NumberOfPoints = 0} 
  //| OK si {NumberOfBoats = x; NumberOfPoints = y} 
  let PutMap (request : MapPUTRequest) =
    let mutable nBoats = 0
    let mutable nPoints = 0
    if (tokenStorage.ContainsKey(request.Token)) then
      let requestingPlayer = tokenStorage.[request.Token]
      if (numberOfBoats, numberOfPoints) = (CountBoatsPoints request.Boats) then
        nBoats <- numberOfBoats
        nPoints <- numberOfPoints
        if (invitingPlayer = 0) then
          invitingPlayer <- requestingPlayer.Id
          playerAId <- requestingPlayer.Id
          playerIdTurn <- requestingPlayer.Id
        else
          playerBId <- requestingPlayer.Id
        if (playerAId > 0) && (playerBId > 0) then
          gameStatus <- 1

    {NumberOfBoats = nBoats; NumberOfPoints = nPoints}
 
  //Entrée : le jeton
  //Sortie : le plan du joueur
  let GetMap (request : GenericRequest) =
    if tokenStorage.ContainsKey(request.Token) then
      let player = tokenStorage.[request.Token]
      {MapResponse.Jouer = player; MissedPoints = missedPoints}
    else
      {MapResponse.Jouer = CreateNullPlayer; MissedPoints = new List<Position>()}
  //On tire une balle (position) à l'ennemi
  //Entrée : la position
  //Sortie : -1 : plusieurs problèmes | O hors de cible | 1 à cible
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
    //invalide
    if getShot <> -1 then
      playerIdTurn <- ennemiId
    //valide et hors de cible
    if getShot = 0 then
       missedPoints.Add((bX, bY, enemyBool))
    getShot

  //Entrée : le jeton
  //Sortie : la liste de bateaux
  let GetBoats (request : GenericRequest) =
    if tokenStorage.ContainsKey(request.Token) then
       let player = tokenStorage.[request.Token]
       {MapPUTRequest.Token = player.Token; Boats = player.Boats}
    else
       {MapPUTRequest.Token = ""; Boats = new List<Boat>()}
