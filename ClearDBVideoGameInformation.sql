EXEC sp_MSForEachTable 'ALTER TABLE VideogameInformation NOCHECK CONSTRAINT ALL'
EXEC sp_MSForEachTable 'DELETE FROM VideoGameInformation'
EXEC sp_MSForEachTable 'ALTER TABLE VideogameInformation CHECK CONSTRAINT ALL'