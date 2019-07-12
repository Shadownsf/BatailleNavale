let rule = fun q -> fun message -> fun n ->
  printf "%s %s %d\n" q message n
let result = rule "a" "b" 10
