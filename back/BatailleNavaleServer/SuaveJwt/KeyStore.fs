﻿namespace SuaveJwt

[<AutoOpen>]
module KeyStore =
    open Microsoft.IdentityModel.Tokens
    
    let securityKey sharedKey:SecurityKey =
        let symmetricKey = sharedKey |> Base64String.decode
        new SymmetricSecurityKey (symmetricKey) :> SecurityKey

    let hmacSha256 secretKey =
        new SigningCredentials (secretKey,
            SecurityAlgorithms.HmacSha256Signature,
            SecurityAlgorithms.Sha256Digest)

