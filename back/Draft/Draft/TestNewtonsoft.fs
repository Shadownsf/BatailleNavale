module TestNewtonsoft
open Newtonsoft.Json
open System.Collections.Generic

let checkJsonRoundTrip<'T when 'T : equality> (f:'T) description =
  let json = JsonConvert.SerializeObject f
  try
    let obj = JsonConvert.DeserializeObject<'T>(json)
    printfn "%s: %s -> %s" description json (if (f = obj) then "roundtrip success" else "deserialized object is not identical with source")
  with
    | _ -> printfn "%s: Failed with exception when trying to deserialize %s" description json

let testliterals =
  checkJsonRoundTrip "Australia" "Literal"
  checkJsonRoundTrip 93 "Literal"
  checkJsonRoundTrip 3.14M "Literal"

// single-level discriminated union
type Currency =
| LocalCurrency
| AUD | NZD | USD | EUR | JPY
let testSigleUnion = checkJsonRoundTrip AUD "Single-Level Discriminated Union"

// two-level discriminated union
type AccountClassification =
| AssetAccount of AssetAccountType
and AssetAccountType =
| Asset
| Bank
| AccountsReceivable

let testTwoLevelUnion = checkJsonRoundTrip (AssetAccount Bank) "Multi-Level Discriminated Union"

// class
type CFoo(integerValue : int) =
  member this.IntegerValue = integerValue
  override this.GetHashCode() = hash this.IntegerValue
  override this.Equals(other) =
    match other with
    | :? CFoo as _other -> this.IntegerValue = _other.IntegerValue
    | _ -> false

let testClass = checkJsonRoundTrip (new CFoo(23)) "Class"

// tuple
let testTuple = checkJsonRoundTrip ("Hello", 42) "Tuple"

// record with list property
type Foo = {
  Name : string
  Children : Foo list
}

let testRecordWithList = checkJsonRoundTrip {
  Name = "Root"; Children = [
    {Name = "1"; Children=[]};
    {Name = "2"; Children=[]};
  ]
}// "record with list property"

// record with sequence property
type Bar = {Name : string; Children : int seq}

let testNestedRecord = checkJsonRoundTrip {Name = "Daniel"; Children = [23;86]} "record with sequence property"

// record with set property
type FooSet = {
  Name : string
  Friends : string Set
}
let testSet = checkJsonRoundTrip {Name = "Daniel"; Friends = ([ "Joe";"Sally"; "Shawn"] |> Set.ofSeq)} "record with set property"

// record with map property
type FooMap = {
  Name : string
  FriendsAndNicknames : Map<string, string>
}
let testRecordWithMap = checkJsonRoundTrip {Name = "Daniel";  FriendsAndNicknames = ([("Joe", "Chumpy"); ("Sally", "Sal"); "Shawn", "Wally"]|> Map.ofSeq)} "record with map property"

// record with dict property
type FooDict = {
  Name : string
  FriendsAndNicknames : IDictionary<string, string>
}
let testRecordWithDict = checkJsonRoundTrip {Name = "Daniel";FriendsAndNicknames = ([ ("Joe", "Chumpy"); ("Sally", "Sal"); "Shawn", "Wally"] |> dict)} "record with dict property"

