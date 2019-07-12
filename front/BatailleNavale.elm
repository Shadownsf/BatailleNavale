import Browser
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Http

--  Main

main = 
    Browser.sandbox { init = init, update = update, view = view}

--  Model

type alias CaseBoard = 
    {
        value : String
    ,   positionX : Int
    ,   positionY : Int
    }

type alias Model = List CaseBoard

init : Model
init = [    
            (CaseBoard "_" 0 0) 
        ,   (CaseBoard "_" 1 0) 
        ,   (CaseBoard "_" 2 0) 
        ,   (CaseBoard "_" 3 0) 
        ]

--  Update

type Action = NoOp | CheckCase (Int, Int)

update : Action -> Model -> Model
update action model = 
    case action of
        NoOp -> model
        CheckCase (x, y) -> List.map (updateCase (x,y) )model 

updateCase : (Int, Int) -> CaseBoard -> CaseBoard
updateCase (x, y) bCase = 
    if bCase.positionX == x && bCase.positionY == y then
    { bCase | value = "X"}
    else
    bCase

--  View

renderInteger : CaseBoard -> Html Action
renderInteger caseBoard = 
    li[][button[onClick (CheckCase (caseBoard.positionX, caseBoard.positionY))][text caseBoard.value]]

view : List CaseBoard -> Html Action
view model = 
    div[]
    [
        ul[] (List.map renderInteger model)
    ]
