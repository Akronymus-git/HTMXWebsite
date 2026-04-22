-- Migration: AddArchipelagoSessions

CREATE TABLE IF NOT EXISTS ArchipelagoSessions (
    gamename TEXT NOT NULL PRIMARY KEY,
    uri TEXT NOT NULL,
    game TEXT NOT NULL,
    name TEXT NOT NULL,
    password TEXT NULL
);