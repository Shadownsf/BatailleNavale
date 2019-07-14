//La partie de génération de données de test
namespace SuaveRestApi.Rest
open System.Collections.Generic
module GameInit =
  let createPlayers =

    let boatAPositions = new List<Position>()
    boatAPositions.Add((1,1,false))
    boatAPositions.Add((1,2,false))

    let boatsA = new List<Boat>()
    boatsA.Add({Boat.Name="BoatA"; Positions = boatAPositions})
 
    let playerA = {
      Player.Id = 1;
      Name = "PlayerA";
      Password = "PlayerA";
      Token = "PlayerA";
      Boats = boatsA
     }
    
    let boatBPositions = new List<Position>()
    boatBPositions.Add((2,1,false))
    boatBPositions.Add((2,2,false))
    
    let boatsB = new List<Boat>()
    boatsB.Add({Boat.Name="BoatB"; Positions = boatBPositions})

    let playerB = {
      Player.Id = 2;
      Name = "PlayerB";
      Password = "PlayerB";
      Token = "PlayerB";
      Boats = boatsB
    }

    let newPlayerA = Db.createPlayer playerA
    let newPlayerB = Db.createPlayer playerB
    Db.gameStatus <- 1
    Db.invitingPlayer <- 1
    Db.playerIdTurn <- 1
    Db.playerAId <- 1
    Db.playerBId <- 2
    [newPlayerA; newPlayerB]
 