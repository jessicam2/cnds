-- select all active existing users
/*
SELECT n.Name as Network, u.UserName, u.ID FROM Users u
JOIN NetworkEntities ne ON ne.ID = u.ID
JOIN Networks n ON ne.NetworkID = n.ID
WHERE u.Deleted = 0 AND u.Active = 1
*/

-- execute the following statement to generate a new ID for the security group
-- SELECT NEWID()


DECLARE @UserID uniqueidentifier
SET @UserID = ''--SET USER ID FROM CNDS HERE!!!!!!


DECLARE @GroupID uniqueidentifier
SET @GroupID = ''--SET SECURITY GROUP ID FROM CNDS HERE!!!!!!


INSERT INTO [dbo].[SecurityGroups]([ID],[Name])VALUES(@GroupID,'CNDS Users')

INSERT INTO SecurityGroupUsers(SecurityGroupID, UserID) VALUES (@GroupID, @UserID)

INSERT INTO AclGlobal(SecurityGroupID, PermissionID, Allowed)VALUES(@GroupID,'4EB90001-6F08-46E3-911D-A6BF012EBFB8',1)

INSERT INTO AclGlobal(SecurityGroupID, PermissionID, Allowed)VALUES(@GroupID,'E3410001-B6F4-4C51-B269-A6BF012EC64D',1)

INSERT INTO AclGlobal(SecurityGroupID, PermissionID, Allowed)VALUES(@GroupID,'E2A20001-1B7F-463E-8990-A6BF012ECC72',1)

INSERT INTO AclGlobal(SecurityGroupID, PermissionID, Allowed)VALUES(@GroupID,'10CF0001-451E-44ED-8388-A6BF012ED2D6',1)

INSERT INTO AclGlobal(SecurityGroupID, PermissionID, Allowed)VALUES(@GroupID,'25D50001-03BD-4EDE-9E6F-A6BF012ED91E',1)
