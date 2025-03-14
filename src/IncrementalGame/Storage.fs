module IncrementalGame.Storage

open System.Runtime.CompilerServices
open Browser.Types



type StorageBuilder (storage: Storage) =
    
    member x.Save key value = storage.setItem (key,string value)
    member x.Load(key) func defaultVal =
        match storage.getItem key with
        | "" -> defaultVal
        | null -> defaultVal
        | x -> func x
    member x.Zero() = ()
    member x.Return(value) = value