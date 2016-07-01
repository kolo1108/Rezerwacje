namespace Bookings.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class limitDescription2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Bookings", "Description", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Bookings", "Description", c => c.String(nullable: false, maxLength: 55));
        }
    }
}
