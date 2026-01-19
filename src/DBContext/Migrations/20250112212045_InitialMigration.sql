CREATE TABLE IF NOT EXISTS main.Users
(
    Id       INTEGER PRIMARY KEY AUTOINCREMENT,
    Name     TEXT NOT NULL
        constraint Users_Name_uk
            unique,
    Email    TEXT NOT NULL,
    Password TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS main.Sessions
(
    key     TEXT PRIMARY KEY
        constraint Sessions_Key_uk
            unique,
    userId  INTEGER NOT NULL,
    expires TEXT    NOT NULL,
    FOREIGN KEY (userId) REFERENCES Users (Id)
);

CREATE TABLE IF NOT EXISTS main.Logs
(
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    Data      TEXT NOT NULL,
    UserId    INTEGER,
    Timestamp TEXT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users (Id)
);

create table if not exists main.Permissions
(
    Id   integer not null
        constraint Permissions_pk
            primary key autoincrement,
    Key  text    not null,
    Name TEXT    not null
);

create table if not exists main.UserPermissions
(
    UserId       integer not null,
    PermissionId integer not null,
    constraint UserPermissions_pk
        primary key (UserId, PermissionId),

    FOREIGN KEY (UserId) REFERENCES Users (Id),
    FOREIGN KEY (PermissionId) REFERENCES Permissions (Id)
);

