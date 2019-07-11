import Browser
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)

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

type Action = NoOp | CheckCase

update : Action -> Model -> Model
update action model = 
    case action of
        NoOp -> model
        CheckCase (x, y)-> 
            {
                
            }


--  View

renderInteger : CaseBoard -> Html action
renderInteger caseBoard = 
    li[button (CheckCase (caseBoard.positionX, caseBoard.positionY))] [text caseBoard.value]

view : List CaseBoard -> Html action
view model = 
    div[]
    [
        ul[] (List.map renderInteger model)
    ]
