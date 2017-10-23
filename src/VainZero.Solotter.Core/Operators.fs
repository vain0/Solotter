namespace VainZero.Solotter

[<AutoOpen>]
module Operators =
  let tap f x =
    f x
    x
