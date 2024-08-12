CREATE TABLE IF NOT EXISTS Tokens (
    tokenId integer primary key, 
    token text,
    name text
);

create table if not exists Roles (
    roleId integer primary key autoincrement, --prevent reusing roles
    name text
);

create table if not exists TokenRoles (
    tokenId integer references Tokens (tokenId),
    roleId integer references  Roles (roleId),
    primary key (tokenId, roleId)
);


insert or ignore into Roles (roleId, name) values (0, 'admin');
insert or ignore into Roles (roleId, name) values (1, 'bingoEditor');

create table if not exists Bingos (
    bingoId integer primary key,
    name text,
    creatorId integer REFERENCES Tokens (tokenid)
);

create table if not exists BingoPeople (
    bingoPersonId integer primary key,
    bingoId integer references Bingos (bingoId),
    name text
);

create table if not exists BingoPeopleDeaths (
    bingoPersonDeathId integer primary key,
    bingoPersonId integer references BingoPeople (bingoId),
    info text,
    link text
);