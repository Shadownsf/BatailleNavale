namespace SuaveRestApi.Rest

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

type GenericRequest = {
  Token : string
}

type MapResponse = {
  Jouer : Player;
  MissedPoints : list<Position>
}

type MapPUTRequest = {
  Token : string;
  Boats: list<Boat>
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

