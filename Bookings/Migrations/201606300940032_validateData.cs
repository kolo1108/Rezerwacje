namespace Bookings.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class validateData : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Bookings", "DateOf", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Bookings", "DateTo", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Bookings", "DateTo", c => c.DateTime());
            AlterColumn("dbo.Bookings", "DateOf", c => c.DateTime());
        }
    }
}
