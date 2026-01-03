module Server.CapSim
open System.Net
open System.Net.Mail
open System.Threading
open Client
open MailKit.Security
open Microsoft.AspNetCore.Http
open MimeKit
open Saturn.Router
open Giraffe.Core
open Client.CapSim


let Router next ctx =
    let r =
        router {
            get "" (htmlView (CapSim.Index ctx))
            get "/" (htmlView (CapSim.Index ctx))
        }
    r next ctx