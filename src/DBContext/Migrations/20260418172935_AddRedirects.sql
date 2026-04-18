-- Migration: AddRedirects


create table if not exists main.Redirects
(
    Id   integer primary key autoincrement,
    Source       text not null,
    Target       text not null
);