namespace VainZero.Solotter

open System
open Prism.Commands

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module RaisableCommand =
  let create<'p> (canExecute: 'p -> bool) (execute: 'p -> unit) =
    DelegateCommand<'p>(Action<'p>(execute), Func<'p, bool>(canExecute))
