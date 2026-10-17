namespace LGYTech_demo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrderStoredProcedure : DbMigration
    {
        public override void Up()
        {
            // Create the table-valued parameter type
            SqlResource("LGYTech_demo.Sql.OrderItemType.sql");

            // Create the order stored procedure
            SqlResource("LGYTech_demo.Sql.CreateOrder.sql");
        }

        public override void Down()
        {
            // Drop the procedure before its dependent table type
            Sql("DROP PROCEDURE dbo.CreateOrder;");

            Sql("DROP TYPE dbo.OrderItemType;");
        }
    }
}
