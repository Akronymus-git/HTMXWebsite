-- Migration: AddDefaultUserAndPermission

insert into main.Permissions (Key, Name) values ('admin', 'admin');

insert into main.Users (Name, Email, Password) values ('Akronymus', 'akronymus@akronymus.net', '$2a$11$L5G/.KMFyDZ.XfnHUlAq4OVNDuKcG26ud2GqF6MNT7Ip5I7jhvj9S');

insert into main.UserPermissions (UserId, PermissionId)
values (
           (select Id from main.Users where Email = 'akronymus@akronymus.net'),
           (select Id from main.Permissions where Key = 'admin')
       );
