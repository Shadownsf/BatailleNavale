module Bouillon
open System.Collections.Generic
open Newtonsoft.Json
open Newtonsoft.Json.Serialization

type Position = int * int * bool


type Boat = {
  Name: string
  Positions: List<Position>
}

type Player = {
  Id : int;
  Name : string;
  Password : string;
  Token : string
  Boats: List<Boat>
}

let p = {
  Player.Id = 1;
  Name = "Tom";
  Password = "Tom";
  Token = "Tom";
  Boats = new List<Boat>()
}

let JsonToObj<'T> json =
  JsonConvert.DeserializeObject(json, typeof<'T>) :?> 'T

let ObjToJson obj =
  let jsonSerializerSettings = new JsonSerializerSettings()
  jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
  JsonConvert.SerializeObject(obj, jsonSerializerSettings)
 