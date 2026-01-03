module Shared.Email

open System.Net
open System.Threading
open MailKit.Security
open MimeKit


let smtp = new MailKit.Net.Smtp.SmtpClient()
smtp.Connect ("smtp.protonmail.ch",587,SecureSocketOptions.StartTls, CancellationToken.None)
smtp.Authenticate(CredentialCache.DefaultNetworkCredentials)


let sendMessage text ``to`` displayName =
    let msg = new MimeMessage()
    msg.To.Add (MailboxAddress (displayName, ``to``) )
    msg.From.Add (MailboxAddress("no-reply","noreply@akronymus.net"))
    let tp = new TextPart ("plain")
    tp.Text <- text
    msg.Body <- tp
    smtp.Send(msg)