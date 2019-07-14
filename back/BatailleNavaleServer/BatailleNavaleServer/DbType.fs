namespace SuaveRestApi.Rest
open System.Collections.Generic

//x y et (subit une balle ou pas)
type Position = int * int * bool

type Bullet = {
  Token : string;
  X : int;
  Y : int
}

type Boat = {
  Name: string;
  Positions: List<Position>
}

type Player = {
  Id : int;
  Name : string;
  Password : string;
  Token : string
  Boats: List<Boat>
}

type GenericRequest = {
  Token : string
}

type MapResponse = {
  Jouer : Player;
  MissedPoints : List<Position>
}

type MapPUTRequest = {
  Token : string;
  Boats: List<Boat>
}

type MapPUTReponse = {
  NumberOfBoats : int;
  NumberOfPoints: int
}

type TokenRequest = {
  Id : int;
  Password : string
}

type TokenResponse = {
  Token : string
}
