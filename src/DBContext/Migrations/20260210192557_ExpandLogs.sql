-- Migration: ExpandLogs

alter table main.Logs ADD COLUMN UserAgent  text;
alter table main.Logs  ADD COLUMN IP         text;
alter table main.Logs  ADD COLUMN StatusCode integer;
alter table main.Logs  ADD COLUMN Path       text;
alter table main.Logs  ADD COLUMN Success    TEXT;



