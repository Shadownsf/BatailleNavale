open TestNewtonsoft
open Bouillon
open Newtonsoft.Json

[<EntryPoint>]
let main argv =
  let pj = ObjToJson p
  let pO = JsonToObj pj
  printfn "%s" pO.Name
  printfn "%A" pO.Boats
  System.Console.ReadKey() |> ignore
  0 // return an integer exit code
