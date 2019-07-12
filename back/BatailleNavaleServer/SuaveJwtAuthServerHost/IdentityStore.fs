namespace SuaveJwt

[<AutoOpen>]
module IdentityStore =
    open System
    open Microsoft.IdentityModel.Claims

    let getClaims userName =
        seq {
            yield (ClaimTypes.Name, userName)
            if (userName = "Admin") then
                yield (ClaimTypes.Role, "Admin")
            if (userName = "Foo") then
                yield (ClaimTypes.Role, "SuperUser")
        } |> Seq.map (fun x -> new Security.Claims.Claim (fst x, snd x)) |> async.Return

    let isValidCredentials username password =
        username = password |> async.Return

