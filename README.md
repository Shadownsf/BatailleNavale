# BatailleNavale

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

les requêtes

POST /game/token : pour obtenir le token d'un jouer en envoyant la requête et puis le réponse
{
  "id": 1,
  "password": "PlayerA",
}
response:
{
  "token": "PlayerA"
}

POST /game/map : pour obtenir le map d'un jouer en envoyant la requête et puis le réponse
{
  "token": "PlayerA"
}
response:
{
  "jouer": {
    "id": 1,
    "name": "PlayerA",
    "password": "PlayerA",
    "token": "PlayerA",
    "boats": []
  },
  "missedPoints": []
}
PUT /game/map : pour envoyer la liste des bateaux en attendant la réponse comme la confirmation
{
  "token": "PlayerA",
  "boats": [
    {
      "name": "BoatA",
      "positions": [
        {
          "item1": 2,
          "item2": 2,
          "item3": true
        },
        {
          "item1": 2,
          "item2": 3,
          "item3": true
        },
        {
          "item1": 2,
          "item2": 4,
          "item3": true
        }
      ]
    }
  ]
}
response
{
  "numberOfBoats": 1,
  "numberOfPoints": 2
}
PUT /game/shoot : pour tirer au ennemi
{
  "token": "PlayerA",
  "x": "2",
  "y": "1"
}
