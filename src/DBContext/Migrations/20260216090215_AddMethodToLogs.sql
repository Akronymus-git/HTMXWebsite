-- Migration: AddMethodToLogs
alter table main.Logs  ADD COLUMN Method    TEXT;