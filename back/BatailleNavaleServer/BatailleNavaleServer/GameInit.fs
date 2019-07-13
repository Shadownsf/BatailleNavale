namespace SuaveRestApi.Rest
module GameInit =
  let createPlayers =
    let pA = {
      Player.Id = 1;
      Name = "PlayerA";
      Password = "PlayerA";
      Token = "PlayerA";
      Boats = [
        {Boat.Name="BoatA";
          Positions = [
            (2,2,true);
            (2,3,true);
            (2,4,true);
          ]
        }
      ]
    }
    
    let pB = {
      Player.Id = 2;
      Name = "PlayerB";
      Password = "PlayerB";
      Token = "PlayerB";
      Boats = [
        {Boat.Name="BoatB";
          Positions = [
            (2,2,true);
            (2,3,true);
            (2,4,true);
          ]
        }
      ]
    }
    let newPlayerA = Db.createPlayer pA
    let newPlayerB = Db.createPlayer pB
    Db.AddToken newPlayerA
    Db.AddToken newPlayerB
    [newPlayerA; newPlayerB]
 