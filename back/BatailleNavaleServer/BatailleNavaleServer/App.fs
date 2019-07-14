open SuaveRestApi.Rest
open Suave.Web
open GameInit
open System.Collections.Generic
(*

Je propose le déroulement du jeu en étapes:

1) Négotiation des plans
-> le jouer POST à /plan son plan
<- /plan confirme son plan (1 OK, 0 KO, on peut définir plus tard d'autres code)
-> le jouer GET à /plan son token pour savoir si l'autre jour est prêt
<- /plan confirme que l'autre jouer est prêt (1 OK, 0 KO, ...)

2) Jeu
-> le jouer POST à /play sa balle pour attaquer l'enemi
<- /play confirme la précision (1 réussite, 0 raté)

3) Statut du jeu
-> le jouer GET à /status pour demander le statut du jeu
<- /status répond avec tous les informations

4) Fin du jeu : si l'un des deux jouers a gagné
-> le jouer demande quoi que ce soit
<- l'api répond 100
-> le jouer GET à /status pour demander le statut du jeu
<- /status répond avec tous les informations y compris le plan de son enemi
-> le jouer GET à /end pour confirmer la fin du jeu pour qu'il puisse jouer encore
<- /end confirme la fin du jeu

Au niveau de conception, je propose
Prédéfinir des paramètres:
numberOfBoats : Le nombre total des bateau pour chaque jouer
numberOfPoints : le monbre de points pour construire les bateaux

la réponse du /status donne
- le plan complet du jouer
- les points d'attaque des deux jouers (soit endomagés ou non)
On a donc besoin :
- un champs booléan complémentaire dans le Position pour indiquer que ce point recoit une balle ou non: type Position = int * int * bool
- deux listes de points attaqués.

On doit se mettre d'accord sur les routes:
/plan
/play
/status
/end
avec des méthod GET, POST

*)
[<EntryPoint>]
let main argv =
  (*
 let playerResource = {
   GetPlayers = Db.GetPlayers
   CreatePlayer = Db.createPlayer
   UpdatePlayer = Db.updatePlayer
   GetPlayerById = Db.GetPlayerById
   DeletePlayer = Db.deletePlayer
   IsPlayerExists = Db.IsPlayerExists
 }
 *)
  let gameResource = {
    GetToken = Db.GetToken
    GetPlayers = Db.GetPlayers
    GetMap = Db.GetMap
    PutMap = Db.PutMap
    GetBoats = Db.GetBoats
    Shoot = Db.Shoot
  }
 
  //let playeWebPart = playerRest "player" playerResource
  let gampeWebPart = gameRest "game" gameResource
  createPlayers |> ignore
  startWebServer defaultConfig gampeWebPart
  //startWebServer defaultConfig (choose [playeWebPart; authWebPart])
  0