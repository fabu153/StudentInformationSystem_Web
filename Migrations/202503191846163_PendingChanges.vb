Imports System
Imports System.Data.Entity.Migrations
Imports Microsoft.VisualBasic

Namespace Migrations
    Public Partial Class PendingChanges
        Inherits DbMigration
    
        Public Overrides Sub Up()
            AlterColumn("dbo.AspNetUsers", "LockoutEndDateUtc", Function(c) c.DateTime())
        End Sub
        
        Public Overrides Sub Down()
            AlterColumn("dbo.AspNetUsers", "LockoutEndDateUtc", Function(c) c.DateTime())
        End Sub
    End Class
End Namespace
