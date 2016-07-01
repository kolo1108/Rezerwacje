namespace Bookings.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class validateData5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Bookings", "CreateBook", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Bookings", "DateOf", c => c.DateTime());
            AlterColumn("dbo.Bookings", "DateTo", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Bookings", "DateTo", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Bookings", "DateOf", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Bookings", "CreateBook", c => c.DateTime());
        }
    }
}
