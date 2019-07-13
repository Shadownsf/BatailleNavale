Remove-Item -Path .\dist -Recurse -Force
New-Item -Path . -ItemType "directory" -Name "dist"
elm make .\src\Battleship.elm --output .\dist\index.html
Copy-Item -Path .\src\Img -Recurse -Destination .\dist