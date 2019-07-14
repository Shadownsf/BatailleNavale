open SuaveRestApi.Rest
open Suave.Web
open GameInit
open System.Collections.Generic
open Suave

(*
Le déroulement du jeu en étapes:
0) Identification du joueur
-> le joueur envoie en POST au /game/token pour obtenir son jeton
   en fournissant son identifiant et son mot de passe.
   Il le joint dans toutes ses requêtes pour que le Rest API le connaisse
<- /game/token lui repond en lui renvoyant son jeton

1) Négotiation des plans
-> le joueur envoie en PUT à /game/map son plan
<- /game/map confirme son plan (1 OK | 0 KO)
-> le joueur envoie en POST à /game/map voir son plan

2) Jeu
-> le joueur envoie en POST à /game/status pour voir le stut du jeu
<- /game/status repond
  (-1 Problème de authentification, 0 le jeu n'est pas prêt, 1 il est prêt)
-> le jouer envoie en POST à /game/status sa balle (position) pour attaquer l'enemi
<- /play confirme la précision (-1 : problème | 1 réussite | 0 hors de cible)

4) Fin du jeu : si l'un des deux jouers a gagné
-> le jouer demande quoi que ce soit
<- l'api répond 100

Au niveau de conception :
Prédéfinir des paramètres:
numberOfBoats : Le nombre total des bateau pour chaque jouer
numberOfPoints : le monbre de points pour construire les bateaux

Toute la base se stocke en mémoire

*)
[<EntryPoint>]
let main argv =
 
  //Les resources de guestion de joueurs
  let playerResource = {
    GetPlayers = Db.GetPlayers
    CreatePlayer = Db.createPlayer
    UpdatePlayer = Db.updatePlayer
    GetPlayerById = Db.GetPlayerById
    DeletePlayer = Db.deletePlayer
    IsPlayerExists = Db.IsPlayerExists
  }

  //les resources du jeu
  let gameResource = {
    GetToken = Db.GetToken
    GetPlayers = Db.GetPlayers
    GetMap = Db.GetMap
    PutMap = Db.PutMap
    GetBoats = Db.GetBoats
    Shoot = Db.Shoot
  }
 
  let playerWebPart = playerRest "player" playerResource
  let gampeWebPart = gameRest "game" gameResource
 
  //Création deux joueurs pour les tests
  createPlayers |> ignore
 
  //On divise en deux parties associant deux chemain de base et deux resources
  startWebServer defaultConfig (choose [gampeWebPart; playerWebPart])
  0